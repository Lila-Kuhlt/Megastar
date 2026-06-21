using System.IO;
using megastar.Game.Track;
using osu.Framework.Allocation;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.Video;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osu.Framework.Timing;

namespace megastar.Game.Preset;

public sealed partial class DynamicTrackBackground : Container
{
    [Resolved] private GameHost host { get; set; } = null!;

    public DynamicTrackBackground()
    {
        RelativeSizeAxes = Axes.Both;
        Child = new Box
        {
            FillMode = FillMode.Fill,
            RelativeSizeAxes = Axes.Both,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Colour = StandardColours.BACKGROUND,
        };
    }

    public void LoadTrack(ITrackMetadata metadata, IClock? clock)
    {
        var videoStream = loadBackgroundVideo(metadata);

        if (videoStream != null)
        {
            var foClock = new FramedOffsetClock(clock) { Offset = metadata.VideoGap };

            Child = new Video(videoStream)
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Loop = false,
                Clock = foClock
            };
            return;
        }

        var bgImage = loadBackgroundImage(metadata);
        if (bgImage != null)
        {
            Child = new Sprite
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FillMode = FillMode.Fill,
                Texture = bgImage
            };
        }
    }

    private Texture? loadBackgroundImage(ITrackMetadata metadata)
    {
        if (metadata.BackgroundImageFile.IsNull()) return null;

        // Create clean virtual storage handles targeting the song's directory
        var textureStorage = new NativeStorage(metadata.DirPath, host);

        using var activeTextureResourceStore = new StorageBackedResourceStore(textureStorage);
        using var activeTextureStore = new TextureStore(host.Renderer,
            host.CreateTextureLoaderStore(activeTextureResourceStore));

        return activeTextureStore.Get(metadata.BackgroundImageFile);
    }

    private FileStream? loadBackgroundVideo(ITrackMetadata metadata)
    {
        if (metadata.BackgroundVideoFile.IsNull()) return null;

        string? videoPath = metadata.BackgroundVideoFilePath();

        return videoPath == null ? null : File.OpenRead(videoPath);
    }
}
