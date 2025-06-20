using System;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BA
{
    // [HarmonyPatch(
    //     typeof(PawnRenderNode_Apparel),
    //     MethodType.Constructor,
    //     [typeof(Pawn), typeof(PawnRenderNodeProperties), typeof(PawnRenderTree), typeof(Apparel)])]
    // public class Harmony_PawnRenderNode_Apparel_Constructor
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(
    //         PawnRenderNode_Apparel __instance,
    //         Pawn pawn,
    //         PawnRenderNodeProperties props,
    //         PawnRenderTree tree,
    //         Apparel apparel)
    //     {
    //         if (pawn.kindDef is not BA.PawnKindDef)
    //             return;
    //         if (apparel.def.apparel.LastLayer == ApparelLayerDefOf.Shell)
    //             props.oppositeFacingLayerWhenFlipped = false;
    //     }
    // }
}