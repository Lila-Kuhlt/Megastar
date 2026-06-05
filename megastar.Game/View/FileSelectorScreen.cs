using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Linguini.Shared.Types.Bundle;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using Realms;

namespace megastar.Game.View;

public partial class FileSelectorScreen : Screen
{
    private AdvancedDirectorySelector directorySelector = null!;
    private SpriteText selectedPathText = null!;

    [Resolved] private MegastarGameBase game { get; set; } = null!;
    [Resolved] private TrackLoader trackLoader { get; set; } = null!;

    [Resolved] private TrackRepository trackRepository { get; set; } = null!;

    private IDisposable? notificationToken;

    private SpriteText loadedTrackCounter = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Settings settings = Settings.GetSettings();
        string initialPath = settings.LastIndexPath.Value;

        InternalChildren =
        [
            // Background
            new Box
            {
                Colour = Color4.Violet,
                RelativeSizeAxes = Axes.Both,
            },

            new SpriteText
            {
                Text = Fluent.Translate("index-select-folder"),
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 20,
                Font = FontUsage.Default.With(size: 40)
            },

            // The Directory Selector
            directorySelector = new AdvancedDirectorySelector(initialPath)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(800, 500),
            },
            // Visual feedback to show what is currently selected
            selectedPathText = new SpriteText
            {
                Text = Fluent.Translate("index-folder-selected",
                    ("folderName", (FluentString)settings.LastIndexPath.Value)),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -80,
                Font = FontUsage.Default.With(size: 24),
            },

            // A button to confirm the selection
            new TextFitButton
            {
                Text = Fluent.Translate("index-select-current-folder"),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Height = 50,
                Y = -20,
                Action = () => Task.Run(confirmSelection)
            },

            loadedTrackCounter = new SpriteText
            {
                Text = Fluent.Translate("index-selection-successful",
                    ("songCount", (FluentNumber)0)),
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -110,
                Font = FontUsage.Default.With(size: 40),
            }
        ];
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        notificationToken = trackRepository.Run(realm => realm.All<MegastarTrackMetadata>()
            .SubscribeForNotifications((sender, _) =>
            {
                var count = sender.Count;
                Schedule(() =>
                {
                    Logger.Log($"Updated: {count} songs");
                    loadedTrackCounter.Text = Fluent.Translate("index-selection-successful",
                        ("songCount", (FluentNumber)count));
                });
            }));
    }

    private void confirmSelection()
    {
        var sw = Stopwatch.StartNew();


        // Start async task for indexing folder
        trackLoader.IndexFolder(directorySelector.CurrentPath.Value.FullName);

        Logger.Log($"Indexed songs in {sw.Elapsed}");
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        notificationToken?.Dispose();
    }
}
