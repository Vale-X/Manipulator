using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;

namespace ManipulatorMod.Modules
{
    internal static class ModCompatibility
    {
        internal static class BetterUICompat
        {
            public static bool? _betterUIInstalled;

            public static bool betterUIInstalled
            {
                get
                {
                    if (_betterUIInstalled == null)
                    {
                        _betterUIInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.xoxfaby.BetterUI");
                    }
                    return (bool)_betterUIInstalled;
                }
            }

            public static void RegisterBuffInfo(BuffDef def, string nameToken, string descToken)
            {
                BetterUI.Buffs.RegisterBuffInfo(def, nameToken, descToken);
            }
        }
    }
}
