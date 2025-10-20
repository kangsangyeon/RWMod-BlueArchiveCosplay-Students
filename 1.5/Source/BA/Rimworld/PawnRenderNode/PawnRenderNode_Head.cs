using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace BA
{
    [DefOf]
    public static class BodyPartDefOf
    {
        public static BodyPartDef Head;

        static BodyPartDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(BodyPartDefOf));
        }
    }

    [HarmonyPatch(typeof(PawnOverlayDrawer), nameof(PawnOverlayDrawer.RenderPawnOverlay))]
    public static class Harmony_PawnOverlayDrawer_RenderPawnOverlay
    {
        private static readonly FieldInfo PawnFieldInfo;
        private static List<Hediff> removedHediffs = new List<Hediff>();

        static Harmony_PawnOverlayDrawer_RenderPawnOverlay()
        {
            PawnFieldInfo =
                typeof(PawnOverlayDrawer).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        public static bool Prefix(
            PawnOverlayDrawer __instance,
            Matrix4x4 matrix,
            Mesh bodyMesh,
            PawnOverlayDrawer.OverlayLayer layer,
            PawnDrawParms parms,
            bool? overApparel = null)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn.kindDef is not BA.PawnKindDef)
                return true;

            // 오버레이가 상처인 경우 Head 상처만 제거하고 그리기
            if (__instance is PawnWoundDrawer)
            {
                removedHediffs.Clear();
                var hediffs = pawn.health.hediffSet.hediffs;
                for (int i = hediffs.Count - 1; i >= 0; i--)
                {
                    var h = hediffs[i];
                    if (h is Hediff_Injury && h.Part != null && h.Part.def == BodyPartDefOf.Head)
                    {
                        removedHediffs.Add(h);
                        hediffs.RemoveAt(i);
                    }
                }
                pawn.health.hediffSet.DirtyCache(); // 캐시 갱신
            }

            return true;
        }

        [HarmonyPostfix]
        public static void Postfix(PawnOverlayDrawer __instance)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn.kindDef is not BA.PawnKindDef)
                return;

            if (__instance is PawnWoundDrawer && removedHediffs.Count > 0)
            {
                pawn.health.hediffSet.hediffs.AddRange(removedHediffs);
                pawn.health.hediffSet.DirtyCache(); // 캐시 갱신
                removedHediffs.Clear();
            }
        }
    }

    [HarmonyPatch(typeof(PawnRenderNodeWorker), nameof(PawnRenderNodeWorker.CanDrawNow))]
    public static class Harmony_PawnRenderNodeWorker_CanDrawNow
    {
        [HarmonyPostfix]
        public static void Postfix(
            PawnRenderNodeWorker __instance,
            PawnRenderNode node,
            PawnDrawParms parms,
            ref bool __result)
        {
            if (!__result)
                return;
            if (parms.pawn.kindDef is not BA.PawnKindDef)
                return;
            // hediff eye는 그리지 않음.
            if (__instance is PawnRenderNodeWorker_HediffEye)
                __result = false;
        }
    }
}