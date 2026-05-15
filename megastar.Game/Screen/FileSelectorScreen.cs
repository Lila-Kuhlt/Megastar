using System;
using System.Collections.Generic;
using System.IO;
using megastar.Game.presets;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;

namespace megastar.Game.screens;

public partial class FileSelectorScreen : Screen
{
    private BasicDirectorySelector directorySelector = null!;
    private SpriteText selectedPathText = null!;

    [Resolved]
    private MegastarGameBase game { get; set; } = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
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
                Text = "Wähle den Song-Ordner aus:",
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                Y = 20,
                Font = FontUsage.Default.With(size: 40),
            },
            new BasicButton
            {
                Text = "Go Back",
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
                Size = new Vector2(100, 40),
                Position = new Vector2(10, 10),
                Action = this.Exit
            },

            // The Directory Selector
            directorySelector = new BasicDirectorySelector
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(800, 500),
            },

            // Visual feedback to show what is currently selected
            selectedPathText = new SpriteText
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -80,
                Font = FontUsage.Default.With(size: 24),
            },
            new BackButton(this.Exit, "Go Back"),
            selectedPathText = new SpriteText
            {
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -80,
                Font = FontUsage.Default.With(size: 24),
            },

            // A button to confirm the selection
            new BasicButton
            {
                Text = "Select Current Folder",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Size = new Vector2(200, 50),
                Y = -20,
                Action = confirmSelection
            }
        };

        // Track when the user clicks into different directories
        directorySelector.CurrentPath.BindValueChanged(pathChanged =>
        {
            selectedPathText.Text = $"Selected: {pathChanged.NewValue?.FullName}";
        }, true);
    }

    private void confirmSelection()
    {
        DirectoryInfo? currentDir = directorySelector.CurrentPath.Value;

        if (currentDir != null && currentDir.Exists)
        {
            string songsPath = currentDir.FullName;
            Console.WriteLine(currentDir.FullName);
            var fd = new DirectoryInfo(songsPath);
            FileInfo[] songFiles = currentDir.GetFiles("*.txt", SearchOption.AllDirectories);
            var tracks = new List<UsdxTrackMetadata>();

            game.LoadedSongs.Clear();

            foreach (FileInfo file in songFiles)
            {
                string content = File.ReadAllText(file.FullName);
                var metadata = Parser.ParseUsdxTrackMetadata(content);
                tracks.Add(metadata);
                game.LoadedSongs.Add(new UsdxTrack(metadata));
                Console.WriteLine(((Object)tracks[^1]).ToString());
            }


            AddInternal(
            new SpriteText()
            {
                Text = "Folder successfully selected.",
                Anchor = Anchor.BottomCentre,
                Origin = Anchor.BottomCentre,
                Y = -110,
                Font = FontUsage.Default.With(size: 40),
            }
            );
        }
    }
}
