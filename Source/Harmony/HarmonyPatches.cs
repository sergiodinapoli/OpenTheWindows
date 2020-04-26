﻿using HarmonyLib;
using RimWorld;
using System;
using System.Linq;
using Verse;

namespace OpenTheWindows
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        public static readonly Type patchType = typeof(HarmonyPatches);
        public static bool ExpandedRoofing = false;
        public static bool DubsSkylights = false;
        public static Harmony instance = null;
        public static Harmony Instance
        {
            get
            {
                if (instance == null)
                    instance = new Harmony("JPT.OpenTheWindows");
                return instance;
            }
        }

        static HarmonyPatches()
        {
            Instance.PatchAll();

            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name == "Dubs Skylights"))
            {
                Log.Message("[OpenTheWindows] Dubs Skylights detected! Integrating...");
                DubsSkylights = true;
                Instance.Patch(AccessTools.Method("Dubs_Skylight.Patch_GameGlowAt:Postfix"), new HarmonyMethod(patchType, nameof(Patch_Inhibitor_Prefix)), null, null);
                Instance.Patch(AccessTools.Method("Dubs_Skylight.Patch_SectionLayer_LightingOverlay_Regenerate:Prefix"), new HarmonyMethod(patchType, nameof(Patch_Inhibitor_Prefix)), null, null);
                Instance.Patch(AccessTools.Method("Dubs_Skylight.Patch_SectionLayer_LightingOverlay_Regenerate:Postfix"), new HarmonyMethod(patchType, nameof(Patch_Inhibitor_Prefix)), null, null);
                Instance.Patch(AccessTools.Method("Dubs_Skylight.MapComp_Skylights:RegenGrid"), null, new HarmonyMethod(patchType, nameof(RegenGrid_Postfix)), null);
            }

            if (AccessTools.TypeByName("ExpandedRoofing.HarmonyPatches") is Type expandedRoofingType)
            {
                Log.Message("[OpenTheWindows] Expanded Roofing detected! Integrating...");
                ExpandedRoofing = true;
                Instance.Patch(AccessTools.Method("ExpandedRoofing.CompCustomRoof:PostSpawnSetup"), null, new HarmonyMethod(patchType, nameof(RegenGrid_Postfix)), null);
            }

            if (LoadedModManager.RunningModsListForReading.Any(x => x.Name.Contains("Nature is Beautiful") || x.Name.Contains("Beautiful Outdoors") || x.Name.Contains("Custom Natural Beauty")))
            {
                Log.Message("[OpenTheWindows] Landscape beautification mod detected! Integrating...");
                OpenTheWindowsSettings.IsBeautyOn = true;
                Instance.Patch(AccessTools.Method(typeof(Need_Beauty), "LevelFromBeauty"), null, new HarmonyMethod(typeof(NeedBeauty_LevelFromBeauty), nameof(NeedBeauty_LevelFromBeauty.LevelFromBeauty)), null);
            }
        }

        public static bool Patch_Inhibitor_Prefix()
        {
            return false;
        }

        public static void RegenGrid_Postfix()
        {
            Find.CurrentMap.GetComponent<MapComp_Windows>().RegenGrid();
        }

    }
}