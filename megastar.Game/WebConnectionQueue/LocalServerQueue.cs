using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using megastar.Game.Track;
using megastar.Game.Track.Usdx;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Logging;
using osu.Framework.Logging;
using osu.Framework.Logging;
using MegastarTrackMetadata = megastar.Game.Track.Megastar.MegastarTrackMetadata;

namespace megastar.Game.WebConnectionQueue;
// Adjust to your preferred namespace

/// <summary>
/// A Webserver, that manages the Queue that is initiated in <see cref="MegastarGameBase"/>.
/// </summary>
public partial class LocalQueueServer : Component
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private TrackRepository repository { get; set; }

    public List<MegastarTrackMetadata> LoadedSongs => repository.AllTracks().ToList();
    public List<MegastarTrackMetadata> QueuedSongs => repository.AllTracks().ToList(); // TODO

    private readonly HttpListener _listener = new();
    private readonly List<WebSocket> _activeSockets = [];
    private readonly string _port = "8080";

    private readonly object _listLock = new object();


    [BackgroundDependencyLoader]
    private void load()
    {
        _listener.Prefixes.Add($"http://+:{_port}/");
    }

    /// <summary>
    /// Starts the webserver. Will make it listen to http on the set port
    /// </summary>
    public void StartWebserver()
    {
        _listener.Start();
        _listener.Prefixes.Add($"http://+:{_port}/");

        Task.Run(StartServerLoopAsync);
    }

    /// <summary>
    /// Stops the webserver from listening on the port.
    /// This will actually not stop the server completly and can be undone with the startWebserver method
    /// To completely stop the server, use dispose()
    /// </summary>
    public void StopWebserver()
    {
        _listener.Prefixes.Clear();
        _listener.Stop();
        _activeSockets.Clear();
    }


    private async Task StartServerLoopAsync()
    {
        try
        {
            _listener.Start();
            Console.WriteLine($"[LocalQueueServer] Started listening on port {_port}");

            while (_listener.IsListening)
            {
                var context = await _listener.GetContextAsync();

                if (context.Request.IsWebSocketRequest)
                {
                    _ = ProcessWebSocketRequestAsync(context);
                }
                else
                {
                    ServeHtml(context);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[LocalQueueServer] Server loop ended: {ex.Message}");
        }
    }


    private void ServeHtml(HttpListenerContext context)
    {
        try
        {
            string html = File.ReadAllText(Path.Combine("Webapp", "USDXWebapp.html"));
            byte[] buffer = Encoding.UTF8.GetBytes(html);

            context.Response.ContentType = "text/html";
            context.Response.ContentLength64 = buffer.Length;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = 500;
            Console.WriteLine($"[LocalQueueServer] Error serving HTML: {ex.Message}");
        }
        finally
        {
            context.Response.OutputStream.Close();
        }
    }

    private async Task ProcessWebSocketRequestAsync(HttpListenerContext context)
    {
        var wsContext = await context.AcceptWebSocketAsync(null);
        WebSocket socket = wsContext.WebSocket;

        lock (_activeSockets)
        {
            _activeSockets.Add(socket);
        }

        await BroadcastStateAsync();

        var buffer = new byte[1024 * 4];
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                HandleClientMessage(message);
            }
        }
        catch (Exception)
        {
            /* Handle disconnects silently */
        }
        finally
        {
            lock (_activeSockets)
            {
                _activeSockets.Remove(socket);
            }

            socket.Dispose();
        }
    }

    private void HandleClientMessage(string jsonMessage)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(jsonMessage);
            string action = doc.RootElement.GetProperty("action").GetString();

            // Extract data before pushing
            int? songIndex = action == "ADD" ? doc.RootElement.GetProperty("songIndex").GetInt32() : null;
            int? queueIndex = (action is "REMOVE" or "MOVEUP" or "MOVEDOWN")
                ? doc.RootElement.GetProperty("queueIndex").GetInt32()
                : null;

            // Perform the update immediately using a lock
            lock (_listLock)
            {
                lock (_listLock)
                {
                    switch (action)
                    {
                        case "ADD" when songIndex is >= 0 && songIndex < LoadedSongs.Count:
                            game.QueueSong(LoadedSongs[songIndex.Value]);
                            break;

                        case "REMOVE" when queueIndex is >= 0 && queueIndex < QueuedSongs.Count:
                            QueuedSongs.RemoveAt(queueIndex.Value);
                            break;

                        case "MOVEUP" when queueIndex is > 0 && queueIndex < QueuedSongs.Count:
                            var trackUp = QueuedSongs[queueIndex.Value];
                            QueuedSongs.RemoveAt(queueIndex.Value);
                            QueuedSongs.Insert(queueIndex.Value - 1, trackUp);
                            break;

                        case "MOVEDOWN" when queueIndex is > 0 && queueIndex < QueuedSongs.Count - 1:
                            var trackDown = QueuedSongs[queueIndex.Value];
                            QueuedSongs.RemoveAt(queueIndex.Value);
                            QueuedSongs.Insert(queueIndex.Value + 1, trackDown);
                            break;
                    }
                }
            }

            // Notify all mobile devices immediately
            _ = BroadcastStateAsync();
        }
        catch (Exception ex)
        {
            Logger.GetLogger().Add(("[LocalQueueServer] Error: " + ex.Message), LogLevel.Verbose);
        }
    }

    /// <summary>
    /// This sends the current state of the queues to all applications and thus should be called on any modification
    /// </summary>
    public async Task BroadcastStateAsync()
    {
        // Clones lists with just the necessary information
        var state = new
        {
            Loaded = LoadedSongs.Select((song, index) => new
            {
                Index = index,
                Title = song.Title ?? "Unknown Title",
                Artist = song.Artist ?? "Unknown Artist"
            }).ToList(),

            Queued = QueuedSongs.Select((song, index) => new
            {
                Index = index,
                Title = song.Title ?? "Unknown Title",
                Artist = song.Artist ?? "Unknown Artist"
            }).ToList()
        };

        string json = JsonSerializer.Serialize(state);
        byte[] buffer = Encoding.UTF8.GetBytes(json);
        var segment = new ArraySegment<byte>(buffer);

        List<WebSocket> socketsToBroadcast;
        lock (_activeSockets)
        {
            socketsToBroadcast = _activeSockets.ToList();
        }

        foreach (var socket in socketsToBroadcast.Where(socket => socket.State == WebSocketState.Open))
        {
            try
            {
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Logger.GetLogger().Add($"Websocket send failed with message : {ex.Message}", LogLevel.Debug);
                /* Ignore failed sockets */
            }
        }
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        if (_listener.IsListening)
        {
            _listener.Stop();
            _listener.Close();
        }

        lock (_activeSockets)
        {
            foreach (var socket in _activeSockets.Where(socket => socket.State == WebSocketState.Open))
            {
                socket.Abort();
            }

            _activeSockets.Clear();
        }
    }
}
