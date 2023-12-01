using System.Linq;
using System.Reflection;
using System.IO;
using System.Xml;

using System.Text.RegularExpressions;
using System.Collections.Generic;

using HarmonyLib;

using Hacknet;
using Hacknet.Extensions;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using ImGuiNET;

using BepInEx;

using HacknetThemeEditor.Extensions;

using Num = System.Numerics;
using System;
using System.Text;

namespace HacknetThemeEditor.Patches
{
    [HarmonyPatch]
    public class Illustrator
    {
        private static readonly ImGuiRenderer _imGuiRenderer = ThemeEditorCore._imGuiRenderer;

        private static int selectedLayout = 0;
        private static int selectedBackground = 0;
        private static int selectedTheme = 0;

        private static int oldBackgroundSelection = -1;

        private static ThemeColors currentThemeColors;

        private static string infoMessage;

        private static readonly string[] themeLayoutNames = new string[]
        {
            "blue", "green", "white", "mint", "greencompact", "riptide", "riptide2", "colamaeleon"
        };

        private static readonly string extensionFolder = ExtensionLoader.ActiveExtensionInfo.FolderPath;
        private static readonly string backgroundsFolder = extensionFolder + "/Themes/Backgrounds/";

        public static List<string> backgroundImages = ThemeEditorCore.backgroundImages;
        public static List<string> existingThemes = new List<string>() { "-- NONE --" };

        public static Texture2D backgroundTexture;
        public static bool backgroundNeedsChanging = false;

        public static string currentlyLoadedTheme = null;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(OS), "drawScanlines")]
        public static bool Prefix(OS __instance)
        {
            // Get game time for ImGUI
            GameTime gameTime = __instance.lastGameTime;

            if(oldBackgroundSelection != selectedBackground) {
                backgroundNeedsChanging = true;
                oldBackgroundSelection = selectedBackground;
            }

            // Assign theme colors
            #region assign theme colors
            currentThemeColors = new ThemeColors()
            {
                defaultHighlightColor = __instance.defaultHighlightColor,
                defaultTopBarColor = __instance.defaultTopBarColor,
                moduleColorSolidDefault = __instance.moduleColorSolidDefault,
                moduleColorSolidStrong = __instance.moduleColorStrong,
                moduleColorBacking = __instance.moduleColorBacking,
                exeModuleTopBar = __instance.exeModuleTopBar,
                exeModuleTitleText = __instance.exeModuleTitleText,
                warningColor = __instance.warningColor,
                subtleTextColor = __instance.subtleTextColor,
                darkBackgroundColor = __instance.darkBackgroundColor,
                indentBackgroundColor = __instance.indentBackgroundColor,
                outlineColor = __instance.outlineColor,
                lockedColor = __instance.lockedColor,
                brightLockedColor = __instance.brightLockedColor,
                brightUnlockedColor = __instance.brightUnlockedColor,
                unlockedColor = __instance.unlockedColor,
                lightGray = __instance.lightGray,
                shellColor = __instance.shellColor,
                shellButtonColor = __instance.shellButtonColor,
                semiTransText = __instance.semiTransText,
                terminalTextColor = __instance.terminalTextColor,
                topBarTextColor = __instance.topBarTextColor,
                superLightWhite = __instance.superLightWhite,
                connectedNodeHighlight = __instance.connectedNodeHighlight,
                netmapToolTipColor = __instance.netmapToolTipColor,
                netmapToolTipBackground = __instance.netmapToolTipBackground,
                topBarIconsColor = __instance.topBarIconsColor,
                thisComputerNode = __instance.thisComputerNode,
                scanlinesColor = __instance.scanlinesColor
            };
            #endregion assign theme colors

            _imGuiRenderer.BeforeLayout(gameTime);

            DrawImGuiLayout(currentThemeColors);

            _imGuiRenderer.AfterLayout();

            #region apply theme colors
            __instance.defaultHighlightColor = currentThemeColors.defaultHighlightColor;
            __instance.highlightColor = currentThemeColors.defaultHighlightColor;

            __instance.defaultTopBarColor = currentThemeColors.defaultTopBarColor;
            __instance.topBarColor = currentThemeColors.defaultTopBarColor;

            __instance.moduleColorSolidDefault = currentThemeColors.moduleColorSolidDefault;
            __instance.moduleColorSolid = currentThemeColors.moduleColorSolidDefault;

            __instance.moduleColorStrong = currentThemeColors.moduleColorSolidStrong;

            __instance.moduleColorBacking = currentThemeColors.moduleColorBacking;

            __instance.exeModuleTopBar = currentThemeColors.exeModuleTopBar;
            __instance.exeModuleTitleText = currentThemeColors.exeModuleTitleText;

            __instance.warningColor = currentThemeColors.warningColor;
            __instance.subtleTextColor = currentThemeColors.subtleTextColor;
            __instance.darkBackgroundColor = currentThemeColors.darkBackgroundColor;
            __instance.indentBackgroundColor = currentThemeColors.indentBackgroundColor;
            __instance.outlineColor = currentThemeColors.outlineColor;

            __instance.lockedColor = currentThemeColors.lockedColor;
            __instance.brightLockedColor = currentThemeColors.brightLockedColor;
            __instance.brightUnlockedColor = currentThemeColors.brightUnlockedColor;
            __instance.unlockedColor = currentThemeColors.unlockedColor;

            __instance.lightGray = currentThemeColors.lightGray;
            __instance.shellColor = currentThemeColors.shellColor;
            __instance.shellButtonColor = currentThemeColors.shellButtonColor;

            __instance.semiTransText = currentThemeColors.semiTransText;

            __instance.terminalTextColor = currentThemeColors.terminalTextColor;
            __instance.topBarTextColor = currentThemeColors.topBarTextColor;

            __instance.superLightWhite = currentThemeColors.superLightWhite;
            __instance.connectedNodeHighlight = currentThemeColors.connectedNodeHighlight;

            __instance.netmapToolTipColor = currentThemeColors.netmapToolTipColor;
            __instance.netmapToolTipBackground = currentThemeColors.netmapToolTipBackground;

            __instance.topBarIconsColor = currentThemeColors.topBarIconsColor;
            __instance.thisComputerNode = currentThemeColors.thisComputerNode;
            __instance.scanlinesColor = currentThemeColors.scanlinesColor;
            #endregion apply theme colors

            return true;
        }

        public static void DrawImGuiLayout(ThemeColors themeColors)
        {
            ImGui.Text("Hacknet Theme Editor -- v" + ThemeEditorCore.ModVer);

            if(ImGui.TreeNode("Existing Themes"))
            {
                if(existingThemes.Any())
                {
                    ImGui.ListBox("Theme Files", ref selectedTheme, existingThemes.ToArray<string>(), existingThemes.Count);
                }

                if(ImGui.Button("Refresh Themes List"))
                {
                    IllustratorFunctions.LoadExistingThemeFiles();
                }

                if(ImGui.Button("Apply Theme"))
                {
                    string themesFolder = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Themes/";
                    string themeSelection = existingThemes[selectedTheme];

                    string themePath = themesFolder + themeSelection;

                    OS.currentInstance.EffectsUpdater.StartThemeSwitch(
                        0.5f, OSTheme.Custom, OS.currentInstance,
                        themePath
                        );
                }

                ImGui.TreePop();
            }

            if(ImGui.TreeNode("Theme Basics"))
            {
                ImGui.ListBox("Theme Layout", ref selectedLayout, themeLayoutNames, themeLayoutNames.Count());

                if (ImGui.Button("Apply Layout"))
                {
                    ThemeManager.switchThemeLayout(OS.currentInstance, ThemeEditorCore.layoutNameToTheme[themeLayoutNames[selectedLayout]]);
                }

                if(backgroundImages.Any())
                {
                    ImGui.ListBox("Background Image", ref selectedBackground, backgroundImages.ToArray<string>(), backgroundImages.Count);

                    if(ImGui.Button("Refresh Backgrounds List"))
                    {
                        IllustratorFunctions.LoadBackgroundImageFiles();
                    }

                    if(ImGui.Button("Apply Background"))
                    {
                        if (backgroundImages.Any())
                        {
                            IllustratorFunctions.LoadBackgroundImage(backgroundsFolder + backgroundImages[selectedBackground]);
                        }

                        if (backgroundTexture != null)
                        {
                            ThemeManager.backgroundImage = backgroundTexture;
                        }
                    }
                }

                ImGui.TreePop();
            }

            if(ImGui.TreeNode("Theme Colors"))
            {
                FieldInfo[] themeColorProps = typeof(ThemeColors).GetFields();
                foreach (FieldInfo colorProp in themeColorProps)
                {
                    Color currentColor = (Color)themeColors.GetType().GetField(colorProp.Name).GetValue(themeColors);
                    if (currentColor == null) { continue; }

                    Num.Vector4 colorVec4 = IllustratorFunctions.ColorToNVec4(currentColor);

                    var r = new Regex(@"
                (?<=[A-Z])(?=[A-Z][a-z]) |
                 (?<=[^A-Z])(?=[A-Z]) |
                 (?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace);

                    string label = r.Replace(colorProp.Name.FirstCharToUpper(), " ");

                    ImGui.ColorEdit4(label, ref colorVec4);

                    Color vec4Color = IllustratorFunctions.NVec4ToColor(colorVec4);

                    themeColors.GetType().GetField(colorProp.Name).SetValue(themeColors, vec4Color);
                }

                currentThemeColors = themeColors;

                ImGui.TreePop();
            }
            
            if(ImGui.Button("Test Flash")) { IllustratorFunctions.TestFlash(); }
            if(ImGui.Button("Export to XML...")) {
                try
                {
                    WriteThemeXML();

                    infoMessage = "Success! Theme file created.";
                } catch(Exception e)
                {
                    Console.WriteLine("Error writing theme XML :: " + e);
                    infoMessage = "Error writing to theme file -- check game console for more information";
                }
            }

            if(!infoMessage.IsNullOrWhiteSpace())
            {
                ImGui.Text(infoMessage);
            }
        }

        public static bool WriteThemeXML()
        {
            string filename = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() + ".xml";
            string themesFolder = ExtensionLoader.ActiveExtensionInfo.FolderPath + "/Themes/";

            if(!File.Exists(themesFolder + filename))
            {
                File.Create(themesFolder + filename).Close();
            }

            XmlWriter xmlWriter = XmlWriter.Create(themesFolder + filename, new XmlWriterSettings()
            {
                Encoding = Encoding.UTF8,
                Indent = true
            });

            xmlWriter.WriteStartElement("CustomTheme");
            xmlWriter.WriteComment("///// Generated with HacknetThemeEditor");
            xmlWriter.WriteComment("//// Autumn Rivers (C) 2023");
            xmlWriter.WriteComment("/// https://github.com/AutumnRivers/HacknetThemeEditor");

            xmlWriter.WriteElementString("themeLayoutName", themeLayoutNames[selectedLayout]);
            xmlWriter.WriteElementString("backgroundImagePath", "Themes/Backgrounds/" + backgroundImages[selectedBackground]);

            #region write color elements
            FieldInfo[] themeColorProps = typeof(ThemeColors).GetFields();
            foreach (FieldInfo colorProp in themeColorProps)
            {
                Color currentColor = (Color)currentThemeColors.GetType().GetField(colorProp.Name).GetValue(currentThemeColors);
                if (currentColor == null) { continue; }

                string elemString = $"{currentColor.R},{currentColor.G},{currentColor.B},{currentColor.A}";

                xmlWriter.WriteElementString(colorProp.Name, elemString);
            }
            #endregion write color elements

            xmlWriter.WriteElementString("AFX_KeyboardMiddle", "255,255,255");
            xmlWriter.WriteElementString("AFX_KeyboardOuter", "255,255,255");
            xmlWriter.WriteElementString("AFX_WordLogo", "255,255,255");
            xmlWriter.WriteElementString("AFX_Other", "255,255,255");

            xmlWriter.WriteComment("// Your business is appreciated...");
            xmlWriter.WriteComment("/ - A.R.");

            xmlWriter.WriteEndElement();
            xmlWriter.Flush();
            xmlWriter.Close();

            return true;
        }
    }

    public class IllustratorFunctions
    {
        public static void TestFlash()
        {
            OS os = OS.currentInstance;

            os.warningFlash();
        }

        public static Num.Vector4 ColorToNVec4(Color color)
        {
            return color.ToVector4().ToNumVector4();
        }

        public static Color NVec4ToColor(Num.Vector4 vec4)
        {
            return new Color(vec4.ToXNAVector4());
        }

        public static void LoadBackgroundImage(string path)
        {
            if(path.IsNullOrWhiteSpace() || !File.Exists(path)) { return; }
            if(!Illustrator.backgroundNeedsChanging && Illustrator.backgroundTexture != null) { return; }

            FileStream backgroundStream = File.OpenRead(path);
            Illustrator.backgroundTexture = Texture2D.FromStream(GuiData.spriteBatch.GraphicsDevice, backgroundStream);
            backgroundStream.Dispose();

            Illustrator.backgroundNeedsChanging = false;
        }

        public static void LoadBackgroundImageFiles()
        {
            Illustrator.backgroundImages.Clear();

            ExtensionInfo activeExtInfo = ExtensionLoader.ActiveExtensionInfo;

            string backgroundsPath = activeExtInfo.FolderPath + "/Themes/Backgrounds/";

            if (Directory.Exists(backgroundsPath))
            {
                foreach (string backgroundImagePath in Directory.GetFiles(backgroundsPath))
                {
                    string backgroundFileName = Path.GetFileName(backgroundImagePath);

                    Illustrator.backgroundImages.Add(backgroundFileName);
                }
            }
        }

        public static void LoadExistingThemeFiles()
        {
            Illustrator.existingThemes.Clear();

            ExtensionInfo activeExtInfo = ExtensionLoader.ActiveExtensionInfo;
            string themesPath = activeExtInfo.FolderPath + "/Themes/";

            if (Directory.Exists(themesPath))
            {
                foreach (string themeFilePath in Directory.GetFiles(themesPath))
                {
                    if (!themeFilePath.EndsWith(".xml")) { continue; }

                    Illustrator.existingThemes.Add(Path.GetFileName(themeFilePath));
                }
            }
        }
    }
}
