﻿using System.Linq;
using UnityEngine;
using Verse;

namespace OpenTheWindows
{
    public class PlaceWorker_Window : PlaceWorker
    {
        public override void DrawGhost(ThingDef def, IntVec3 center, Rot4 rot, Color ghostCol, Thing thing = null)
        {
            if (thing == null)
            {
                Map currentMap = Find.CurrentMap;
                IntVec3 start = WindowUtility.FindEnd(center, rot, def.size, false);
                IntVec3 end = WindowUtility.FindEnd(center, rot, def.size, true);
                GenDraw.DrawFieldEdges(WindowUtility.GetWindowObfuscation(def.size, center, rot, currentMap, start, end).ToList());
            }
            else
            {
                var window = thing as Building_Window;
                GenDraw.DrawFieldEdges(window.EffectArea);
            }
        }
    }
}