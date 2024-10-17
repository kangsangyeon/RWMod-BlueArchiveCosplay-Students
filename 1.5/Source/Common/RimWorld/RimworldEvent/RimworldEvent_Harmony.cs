using HarmonyLib;
using RimWorld;
using UniRx;
using Verse;

namespace BA
{
    // https://harmony.pardeike.net/articles/patching-prefix.html

    [HarmonyPatch(typeof(Pawn_HealthTracker), "MakeDowned")]
    public static class Harmony_Pawn_HealthTracker_MakeDowned
    {
        public static Subject<(Pawn_HealthTracker instance, DamageInfo? dinfo, Hediff hediff)> OnPrefix = new();

        [HarmonyPrefix]
        public static void Prefix(Pawn_HealthTracker __instance, DamageInfo? dinfo, Hediff hediff)
        {
            if (__instance.Downed == false)
                OnPrefix?.OnNext((__instance, dinfo, hediff));
        }
    }

    [HarmonyPatch(typeof(Pawn), "DoKillSideEffects")]
    public static class Harmony_Pawn_DoKillSideEffects
    {
        public static Subject<(Pawn instance, DamageInfo? dinfo, Hediff exactCulprit, bool spawned)> OnPostfix = new();

        [HarmonyPostfix]
        public static void Postfix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit, bool spawned)
        {
            OnPostfix?.OnNext((__instance, dinfo, exactCulprit, spawned));
        }
    }

    [HarmonyPatch(typeof(Bullet), "Impact")]
    public static class Harmony_Bullet_Impact
    {
        public static Subject<(Bullet instance, Thing hitThing, bool blockedByShield)> OnPostfix = new();

        [HarmonyPostfix]
        public static void Postfix(Bullet __instance, Thing hitThing, bool blockedByShield = false)
        {
            OnPostfix.OnNext((__instance, hitThing, blockedByShield));
        }
    }
}