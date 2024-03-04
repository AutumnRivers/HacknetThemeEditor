using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Hacknet;
using Hacknet.Gui;
using Hacknet.Extensions;

using BepInEx;
using BepInEx.Hacknet;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pathfinder.Executable;
using HacknetThemeEditor.Patches;

namespace HacknetThemeEditor
{
    [BepInPlugin(ModGUID, ModName, ModVer)]
    public class ThemeEditorCore : HacknetPlugin
    {
        public const string ModGUID = "autumnrivers.themeeditor";
        public const string ModName = "HN Theme Editor Plugin";
        public const string ModVer = "1.0.2";

        public static readonly Dictionary<string, OSTheme> layoutNameToTheme = new()
        {
            { "blue", OSTheme.HacknetBlue },
            { "green", OSTheme.HackerGreen },
            { "white", OSTheme.HacknetWhite },
            { "mint", OSTheme.HacknetMint },
            { "greencompact", OSTheme.GreenCompact },
            { "riptide", OSTheme.Riptide },
            { "riptide2", OSTheme.Riptide2 },
            { "colamaeleon", OSTheme.Colamaeleon },
            { "purple", OSTheme.HacknetPurple }
        };

        public override bool Load()
        {
            HarmonyInstance.PatchAll(typeof(ThemeEditorCore).Assembly);

            ExecutableManager.RegisterExecutable<ExampleExecutable>("#EXAMPLE_EXEC#");

            return true;
        }
    }

    public class ThemeColors
    {
        public Color defaultHighlightColor;
        public Color defaultTopBarColor;
        public Color moduleColorSolidDefault;
        public Color moduleColorStrong;
        public Color moduleColorBacking;
        public Color exeModuleTopBar;
        public Color exeModuleTitleText;
        public Color warningColor;
        public Color subtleTextColor;
        public Color darkBackgroundColor;
        public Color indentBackgroundColor;
        public Color outlineColor;
        public Color lockedColor;
        public Color brightLockedColor;
        public Color brightUnlockedColor;
        public Color unlockedColor;
        public Color lightGray;
        public Color shellColor;
        public Color shellButtonColor;
        public Color semiTransText; // poggers
        public Color terminalTextColor;
        public Color topBarTextColor;
        public Color superLightWhite;
        public Color connectedNodeHighlight;
        public Color netmapToolTipColor;
        public Color netmapToolTipBackground;
        public Color topBarIconsColor;
        public Color thisComputerNode;
        public Color scanlinesColor;
    }
}
