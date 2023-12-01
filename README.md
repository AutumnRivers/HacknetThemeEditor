# HacknetThemeEditor
Look, you try thinking of a more creative name.

---

Self-explanatory. In-game tool for editing Hacknet themes in real time. Made specifically for extensions.

---

# INSTALLATION (READ THIS!!!!!!!!!!!)
1. Download the [latest release](https://github.com/AutumnRivers/HacknetThemeEditor/releases)
2. Unzip the file
3. Place all DLLs in the `/global` folder to Hacknet's global plug-in directory. (`<HacknetInstall>/BepInEx/plugins`)
4. Place the main DLL (`HacknetThemeEditor.dll`) in your target extension's `/Plugins` folder.
5. Profit!

# "Gotchas"
* No sub-directory support for theme or background files yet
* No editing AlienwareFX colors - you'll have to do that manually
* Due to a bug with text input, the theme file name will always be the current unix time in seconds. You can manually rename the file after you export it