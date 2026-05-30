using System;
using System.Collections.Generic;
using System.IO;
using Linguini.Shared.Types.Bundle;
using megastar.Game.Preset;
using megastar.Game.Track;
using megastar.Game.Translations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.View;

public partial class FileSelectorScreen : Screen
{
    private AdvancedDirectorySelector directorySelector = null!;
    private SpriteText selectedPathText = null!;

    [Resolved] private MegastarGameBase game { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Settings settings = Settings.GetSettings();
        string initialPath = settings.LastIndexPath.Value;

        InternalChildren = new Drawable[]
        {
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
                Font = FontUsage.Default.With(size: 40),
            },
            new BasicButton
            {
                Text = Fluent.Translate("common-back"),
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(100, 40),
                Position = new Vector2(10, 10),
                Action = this.Exit
            },

            // The Directory Selector
            directorySelector = new AdvancedDirectorySelector(initialPath)
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(800, 500),
            },
            new BackButton(this.Exit, Fluent.Translate("common-back")),
            // Visual feedback to show what is currently selected
            selectedPathText = new SpriteText
            {
                Text = Fluent.Translate("index-folder-selected", ("folderName", (FluentString) settings.LastIndexPath.Value)),
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
                Action = () => confirmSelection(settings)
            }
        };
    }

    private void confirmSelection(Settings settings)
    {
        DirectoryInfo? currentDir = directorySelector.CurrentPath.Value;

        if (currentDir != null && currentDir.Exists)
        {
            string songsPath = currentDir.FullName;
            // Update last selected directory
            settings.LastIndexPath.Value = songsPath;
            selectedPathText.Text = Fluent.Translate("index-folder-selected", ("folderName", (FluentString) songsPath));
            Console.WriteLine(currentDir.FullName);
            var directories = new List<DirectoryInfo>();
            directories.Add(currentDir);
            var songFiles = new List<FileInfo>();
            foreach (var folder in directories)
            {
                FileInfo[] files = folder.GetFiles("*.txt", SearchOption.AllDirectories);
                songFiles.AddRange(files);
            }

            var tracks = new List<UsdxTrackMetadata>();

            game.LoadedSongs.Clear();

            foreach (FileInfo file in songFiles)
            {
                string content = File.ReadAllText(file.FullName);
                try
                {
                    var metadata = Parser.ParseUsdxTrackMetadata(content);

                    metadata.Path = file.FullName;
                    metadata.DirPath = file.DirectoryName;
                    tracks.Add(metadata);
                    game.LoadedSongs.Add(new UsdxTrack(metadata));
                    game.LocalQueueServer.BroadcastStateAsync();
                }
                catch (InvalidDataException)
                {
                }
            }

            // TODO HIER IRGENDWIE Adden, dass man songs queuen kann
            if (game.LoadedSongs.Count != 0)
            {
                game.QueueSong(game.LoadedSongs[0]);
            }

            AddInternal(
                new SpriteText()
                {
                    Text = Fluent.Translate("index-selection-successful", ("songCount", (FluentNumber) game.LoadedSongs.Count)),
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Y = -110,
                    Font = FontUsage.Default.With(size: 40),
                }
            );
        }
    }
}
