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
        public const string ModVer = "1.0.0";

        public static ImGuiRenderer _imGuiRenderer;

        public static Texture2D imGuiFNA;
        public static IntPtr imGuiTexture;

        public static List<string> backgroundImages = new List<string>();

        public static readonly Dictionary<string, OSTheme> layoutNameToTheme = new()
        {
            { "blue", OSTheme.HacknetBlue },
            { "green", OSTheme.HackerGreen },
            { "white", OSTheme.HacknetWhite },
            { "mint", OSTheme.HacknetMint },
            { "greencompact", OSTheme.GreenCompact },
            { "riptide", OSTheme.Riptide },
            { "riptide2", OSTheme.Riptide2 },
            { "colamaeleon", OSTheme.Colamaeleon }
        };

        public override bool Load()
        {
            HarmonyInstance.PatchAll(typeof(ThemeEditorCore).Assembly);

            GraphicsDevice userGraphics = GuiData.spriteBatch.GraphicsDevice;

            ExecutableManager.RegisterExecutable<ExampleExecutable>("#EXAMPLE_EXEC#");

            _imGuiRenderer = new ImGuiRenderer(Game1.singleton);
            _imGuiRenderer.RebuildFontAtlas();

            imGuiFNA = CreateTexture(userGraphics, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            imGuiTexture = _imGuiRenderer.BindTexture(imGuiFNA);

            IllustratorFunctions.LoadBackgroundImageFiles();
            IllustratorFunctions.LoadExistingThemeFiles();

            return true;
        }

        public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
        {
            //initialize a texture
            var texture = new Texture2D(device, width, height);

            //the array holds the color for each pixel in the texture
            Color[] data = new Color[width * height];
            for (var pixel = 0; pixel < data.Length; pixel++)
            {
                //the function applies the color according to the specified pixel
                data[pixel] = paint(pixel);
            }

            //set the color
            texture.SetData(data);

            return texture;
        }
    }

    public class ThemeColors
    {
        public Color defaultHighlightColor;
        public Color defaultTopBarColor;
        public Color moduleColorSolidDefault;
        public Color moduleColorSolidStrong;
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
