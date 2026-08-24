using System;
using System.Collections.Generic;
using megastar.Game.Preset;
using megastar.Game.Track.Megastar;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;
using osuTK.Input;
using osu.Framework.Localisation;

namespace megastar.Game.View;

public partial class SearchScreen : Screen
{
    [Resolved] private MegastarGameBase game { get; set; } = null!;

    private FillFlowContainer<QueuedTrackItem> queueContainer = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        var searchBox = new BasicTextBox
        {
            PlaceholderText = "Search...",
            Height = 40,
            RelativeSizeAxes = Axes.X,
            Margin = new MarginPadding { Bottom = 10 }
        };

        var searchContainer = new SearchContainer<IndexedTrackItem>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5),
        };

        // Bind the text changes to the search
        searchBox.Current.BindValueChanged(change =>
        {
            searchContainer.SearchTerm = change.NewValue;
        }, true);

        // Populate Left Side (All Indexed Songs)
        foreach (var track in game.IndexedSongs)
        {
            searchContainer.Add(new IndexedTrackItem(track, addTrackToQueue));
        }


        queueContainer = new FillFlowContainer<QueuedTrackItem>
        {
            AutoSizeAxes = Axes.Y,
            RelativeSizeAxes = Axes.X,
            Direction = FillDirection.Vertical,
            Spacing = new Vector2(0, 5)
        };

        InternalChildren =
        [
            new Box
            {
                Colour = StandardColours.BACKGROUND,
                RelativeSizeAxes = Axes.Both,
            },
            new BackButton(this.Exit, Fluent.Translate("common-back")),

            // The Split Screen
            new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 80, Left = 20, Right = 20, Bottom = 20 },
                ColumnDimensions =
                [
                    new Dimension(GridSizeMode.Relative, 0.5f), // Left side 50%
                    new Dimension(GridSizeMode.Relative, 0.5f)  // Right side 50%
                ],
                RowDimensions = [new Dimension(GridSizeMode.Relative, 1f)],
                Content = new[]
                {
                    new Drawable[]
                    {
                        //Left side
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Right = 10 },
                            Children = new Drawable[]
                            {
                                searchBox,
                                new BasicScrollContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Top = 50 },
                                    Child = searchContainer
                                }
                            }
                        },

                        // Right Side
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = 10 },
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = "Current Queue",
                                    Font = FontUsage.Default.With(size: 30),
                                    Anchor = Anchor.TopCentre,
                                    Origin = Anchor.TopCentre,
                                },
                                new BasicScrollContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Padding = new MarginPadding { Top = 40 },
                                    Child = queueContainer
                                }
                            }
                        }
                    }
                }
            }
        ];

        // Initially draw the queue
        refreshQueueUi();
    }

    /// <summary>
    /// Adds a track from the catalog to the queue and refreshes the UI.
    /// </summary>
    private void addTrackToQueue(MegastarTrackMetadata track)
    {
        game.QueueSong(track);
        refreshQueueUi();
    }

    /// <summary>
    /// Moves a track's position in the queue.
    /// </summary>
    private void moveQueueItem(int oldIndex, int newIndex)
    {
        if (oldIndex < 0 || oldIndex >= game.QueuedSongs.Count || newIndex < 0 || newIndex >= game.QueuedSongs.Count)
            return;

        var track = game.QueuedSongs[oldIndex];
        game.QueuedSongs.RemoveAt(oldIndex);
        game.QueuedSongs.Insert(newIndex, track);

        // Push update to mobile devices
        _ = game.LocalQueueServer.BroadcastStateAsync();

        refreshQueueUi();
    }

    /// <summary>
    /// Removes a track from the queue.
    /// </summary>
    private void removeQueueItem(int index)
    {
        if (index < 0 || index >= game.QueuedSongs.Count) return;

        game.QueuedSongs.RemoveAt(index);
        _ = game.LocalQueueServer.BroadcastStateAsync();

        refreshQueueUi();
    }

    /// <summary>
    /// Rebuilds the queue list to reflect the current state of game.QueuedSongs.
    /// </summary>
    private void refreshQueueUi()
    {
        queueContainer.Clear();
        //This should hopefully not lead to bad performance as the queue should be rather short
        for (int i = 0; i < game.QueuedSongs.Count; i++)
        {
            int index = i;
            queueContainer.Add(new QueuedTrackItem(
                game.QueuedSongs[i],
                index,
                game.QueuedSongs.Count,
                onUp: () => moveQueueItem(index, index - 1),
                onDown: () => moveQueueItem(index, index + 1),
                onRemove: () => removeQueueItem(index)
            ));
        }
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            this.Exit();
        }
        return base.OnKeyDown(e);
    }
}

// --- UI Helper Classes ---

/// <summary>
/// Represents a track in the catalog that can be searched.
/// Must implement IFilterable for SearchContainer to work correctly.
/// </summary>
public partial class IndexedTrackItem : CompositeDrawable, IFilterable
{
    private readonly MegastarTrackMetadata metadata;

    // CHANGED: string -> LocalisableString
    public IEnumerable<LocalisableString> FilterTerms => [metadata.Title, metadata.Artist];

    public bool FilteringActive { get; set; }

    private bool matchingFilter = true;
    public bool MatchingFilter
    {
        get => matchingFilter;
        set
        {
            matchingFilter = value;
            if (value) Show(); else Hide(); // Automatically adjusts layout flow
        }
    }

    public IndexedTrackItem(MegastarTrackMetadata track, Action<MegastarTrackMetadata> onAdd)
    {
        metadata = track;
        RelativeSizeAxes = Axes.X;
        Height = 40;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions = [new Dimension(GridSizeMode.AutoSize), new Dimension(GridSizeMode.Distributed)],
            Content = new[]
            {
                new Drawable[]
                {
                    new BasicButton { Text = "+", Size = new Vector2(40), Action = () => onAdd(track) },
                    new SpriteText
                    {
                        Text = $"{track.Artist} - {track.Title}",
                        Margin = new MarginPadding { Left = 10 },
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            }
        };
    }
}

/// <summary>
/// Represents a track currently sitting in the active queue.
/// </summary>
public partial class QueuedTrackItem : CompositeDrawable
{
    public QueuedTrackItem(MegastarTrackMetadata track, int index, int totalCount, Action onUp, Action onDown, Action onRemove)
    {
        RelativeSizeAxes = Axes.X;
        Height = 40;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.Both,
            ColumnDimensions =
            [
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.Distributed)
            ],
            Content = new[]
            {
                new Drawable[]
                {
                    // Action buttons
                    new BasicButton { Text = "^", Size = new Vector2(40), Action = onUp, Enabled = { Value = index > 0 } },
                    new BasicButton { Text = "v", Size = new Vector2(40), Action = onDown, Enabled = { Value = index < totalCount - 1 } },
                    new BasicButton { Text = "X", Size = new Vector2(40), Action = onRemove },

                    // Track Information
                    new SpriteText
                    {
                        Text = $"{track.Artist} - {track.Title}",
                        Margin = new MarginPadding { Left = 10 },
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft
                    }
                }
            }
        };
    }
}
