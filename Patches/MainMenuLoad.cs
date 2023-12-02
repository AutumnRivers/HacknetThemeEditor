using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Hacknet;
using Hacknet.Extensions;
using Hacknet.Screens;

using HarmonyLib;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Pathfinder.Replacements;

namespace HacknetThemeEditor.Patches
{
    [HarmonyPatch]
    public class MainMenuLoad
    {
        public static ImGuiRenderer _imGuiRenderer;

        public static Texture2D imGuiFNA;
        public static IntPtr imGuiTexture;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(MainMenu), "LoadContent")]
        public static void Postfix()
        {
            GraphicsDevice userGraphics = GuiData.spriteBatch.GraphicsDevice;

            _imGuiRenderer = new ImGuiRenderer(Game1.singleton);
            _imGuiRenderer.RebuildFontAtlas();

            imGuiFNA = CreateTexture(userGraphics, 300, 150, pixel =>
            {
                var red = (pixel % 300) / 2;
                return new Color(red, 1, 1);
            });

            imGuiTexture = _imGuiRenderer.BindTexture(imGuiFNA);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(ExtensionLoader),nameof(ExtensionLoader.LoadNewExtensionSession))]
        /*[HarmonyPatch(typeof(ExtensionInfoLoader))]
        [HarmonyPatch(new Type[] { })]
        [HarmonyPatch(MethodType.StaticConstructor)]*/
        public static void Postfix_LoadImagesAndThemes()
        {
            IllustratorFunctions.LoadBackgroundImageFiles();
            IllustratorFunctions.LoadExistingThemeFiles();
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
}
