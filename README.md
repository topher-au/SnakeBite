# SnakeBite
SnakeBite is a mod manager/launcher for Metal Gear Solid V (PC/Steam).

## Getting started with SnakeBite
When you first run the application, you will be greeted by the setup wizard. This will walk you through the steps required to run SnakeBite.

During the setup process you will be prompted to

1. Select your MGSV installation directory
2. Create a backup of the game data
3. Modify game data to prepare for modding
 
Some of the game data needs to be modified to allow for easier installation of mods. During setup, any MGSV system data contained in 00.dat will be moved into 01.dat. Afterwards, any files installed by SnakeBite can be installed into 00.dat with minimal risk of conflicting with the game files.

## Mods and Mod Files
Mods can be installed and uninstalled by selecting **MODS** from the main window. Additionally, you can temporarily disable/enable all mods by clicking the toggle switch next to **MODS**.

If the mod you wish to install is available as a .MGSV file, it is the recommended way to install mods using SnakeBite. If not, SnakeBite provides basic functionality to install unsupported mods.

### If your mod is a **.MGSV**:

Click "Install .MGSV" from the mod manager window and select the mod you wish to install.

### If your mod is a **.ZIP**:

**Please note that this functionality is not guaranteed and may cause bugs or glitches with other mods.**

If the mod you downloaded is a zip archive, and that archive does not contain a .MGSV file, you can attempt to install the mod by clicking "Install .ZIP" from the mod manager window.

You will need to manually specify a name, and can optionally save the file as a .MGSV for later use.

# Developers

Use MakeBite (included) to create .MGSV mod files compatible with SnakeBite

For information regarding using makebite to create mod files, please see here: https://github.com/topher-au/SnakeBite/wiki/Using-MakeBite

# Found a bug?

Please submit a bug report to GitHub with as much detail as possible. If you are using version 0.8.0.0, please include log.txt, which is accessed by double clicking the version in the main window, or found in your SnakeBite install directory (default %LocalAppData%\SnakeBite). Warning: the logfile is reset every time you launch SnakeBite so please save it immediately after the application crashes.
