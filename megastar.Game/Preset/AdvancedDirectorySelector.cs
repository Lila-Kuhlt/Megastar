
// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

#nullable disable
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using System.IO;
using megastar.Game.Translations;
using osuTK;

namespace megastar.Game.Preset;

public partial class AdvancedDirectorySelector(string initialPath = null) : DirectorySelector(initialPath)
{

    protected override DirectorySelectorBreadcrumbDisplay CreateBreadcrumb() => new BasicDirectorySelectorBreadcrumbDisplay();

    protected override Drawable CreateHiddenToggleButton() => new BasicButton
    {
        Size = new Vector2(200, 25),
        Text = Fluent.Translate("dir-select-toggle-hidden"),
        Action = ShowHiddenItems.Toggle,
        Masking = true,
        CornerRadius = 20,
        BackgroundColour = StandardColours.MAIN,
    };

    protected override DirectorySelectorDirectory CreateDirectoryItem(DirectoryInfo directory, LocalisableString? displayName = null) => new BasicDirectorySelectorDirectory(directory, displayName);

    protected override DirectorySelectorDirectory CreateParentDirectoryItem(DirectoryInfo directory) => new BasicDirectorySelectorParentDirectory(directory);

    protected override ScrollContainer<Drawable> CreateScrollContainer() => new BasicScrollContainer();

    protected override void NotifySelectionError()
    {
        this.FlashColour(Colour4.Red, 300);
    }
}

