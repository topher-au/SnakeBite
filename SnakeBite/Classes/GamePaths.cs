using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeBite
{
    static class GamePaths
    {
        internal static string GameDir { get { return Properties.Settings.Default.InstallPath; } }
        
        internal static string chunk0Path { get { return GameDir + "\\master\\chunk0.dat"; } }
        internal static string OnePath { get { return GameDir + "\\master\\0\\01.dat"; } }
        internal static string ZeroPath { get { return GameDir + "\\master\\0\\00.dat"; } }
        internal static string t7Path { get { return GameDir + "\\master\\a_texture7.dat"; } }
        internal static string c7Path { get { return GameDir + "\\master\\a_chunk7.dat"; } }
        internal static string SnakeBiteSettings { get { return GameDir + "\\snakebite.xml"; } }

        internal static string build_ext = ".SB_Build";
        internal static string original_ext = ".original";
        internal static string modded_ext = ".modded";

        internal static string NexusURLPath = "https://www.nexusmods.com/metalgearsolidvtpp";
        internal static string SBWMSearchURLPath = "https://www.nexusmods.com/metalgearsolidvtpp/search/?search_description=SBWM";
        internal static string SBWMBugURLPath = "https://www.nexusmods.com/metalgearsolidvtpp/mods/106?tab=bugs";
        internal static string WikiURLPath = "https://metalgearmodding.wikia.com/wiki/";
    }
}
