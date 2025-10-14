using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BA
{
    [HarmonyPatch(
        typeof(Thing),
        nameof(Thing.SetFactionDirect))]
    public static class Harmony_Thing_SetFactionDirect
    {
        [HarmonyPostfix]
        public static void Postfix(
            Thing __instance,
            Faction newFaction)
        {
            if (__instance is not Pawn pawn)
                return;
            if (!__instance.Spawned)
                return;
            if (pawn.kindDef is not BA.PawnKindDef kindDef)
                return;
            if (newFaction != null && newFaction.IsPlayer)
                return;

            // Despawn 트리거 시 Letter 출력
            Utils.SendDespawnLetter(pawn, "faction change.");

            // 플레이어 faction이 아니면 despawn 대상임.
            Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(pawn);
        }
    }
    [HarmonyPatch(
        typeof(Pawn),
        nameof(Pawn.SetFaction))]
    public static class Harmony_Pawn_SetFaction
    {
        [HarmonyPostfix]
        public static void Postfix(
            Pawn __instance,
            Faction newFaction)
        {
            if (!__instance.Spawned)
                return;
            if (__instance.kindDef is not BA.PawnKindDef kindDef)
                return;
            // CharacterEditor에서 faction 변경하면 임시 faction으로 잠시 변경되는데, 이 때 null exception 안나도록 확인함.
            if (newFaction != null && newFaction.IsPlayer)
                return;

            // Despawn 트리거 시 Letter 출력 (중복 방지 위해 TryAdd 전에 호출)
            Utils.SendDespawnLetter(__instance, "faction change.");

            // 플레이어 faction이 아니면 despawn 대상임.
            // CharacterEditor에서 faction 변경하면 SetFaction이 2번 호출됌. 한 번만 추가해야 함.
            Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(__instance);
        }
    }
    [HarmonyPatch(
        typeof(PawnCapacitiesHandler),
        nameof(PawnCapacitiesHandler.GetLevel))]
    public static class Harmony_PawnCapacitiesHandler_GetLevel
    {
        private static readonly FieldInfo PawnFieldInfo;
        static Harmony_PawnCapacitiesHandler_GetLevel()
        {
            PawnFieldInfo =
                typeof(PawnCapacitiesHandler).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        [HarmonyPostfix]
        public static void Postfix(
            PawnCapacitiesHandler __instance,
            PawnCapacityDef capacity,
            ref float __result)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (!pawn.Spawned)
                return;
            if (pawn.kindDef is not BA.PawnKindDef kindDef)
                return;
            if (capacity != PawnCapacityDefOf.Consciousness)
                return;
            if (__result > 0.1f)
                return;

            // 의식(consciousness)이 10% 이하면 despawn 대상임.
            Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(pawn);
        }
    }
    [HarmonyPatch(typeof(Thing))]
    [HarmonyPatch("SpawnSetup")]
    class Patch_Corpse_SpawnSetup
    {
        static void Postfix(Thing __instance)
        {
            // Thing이 시체이고, InnerPawn이 존재하며
            if (__instance is Corpse corpse && corpse.InnerPawn != null)
            {
                // 해당 시체 폰이 BA.PawnKindDef 인지 체크
                if (corpse.InnerPawn.kindDef.defName == "BA_PawnKindDefName") // 실제 이름으로 교체
                {
                    // 시체가 스폰되자마자 삭제 (Despawn)
                    corpse.DeSpawn(DestroyMode.Vanish);
                }
            }
        }
    }
    // 폰 사망시 Despawn 처리 (시체 없이 바로 제거)
    [HarmonyPatch(typeof(Pawn), nameof(Pawn.Kill))]
    public static class Patch_Pawn_Kill_Prefix
    {
        [HarmonyPrefix]
        public static bool Prefix(Pawn __instance, DamageInfo? dinfo, Hediff exactCulprit)
        {
            if (__instance.Dead) return false; // 이미 사망했으면 중복 처리 방지
            if (__instance.kindDef is BA.PawnKindDef)
            {
                // 폰이 스폰 되어 있고 월드에 존재하면서 즉시 제거 가능하면 Despawn 실시 후 원본 Kill 실행 차단
                if (__instance.Spawned && __instance.Map != null)
                {
                    // Despawn 전에 Letter 출력
                    Utils.SendDespawnLetter(__instance, BALocalizeKey.DespawnReasonDeath.Translate());

                    __instance.DeSpawn(DestroyMode.Vanish);
                    return false;
                }
            }
            return true; // BA 외 폰은 원래 로직 실행
        }
    }

    // 기본 Death Letter 억제 (BA Pawn 사망 시 NotifyPlayerOfKilled 스킵)
    [HarmonyPatch(typeof(Pawn_HealthTracker), nameof(Pawn_HealthTracker.NotifyPlayerOfKilled))]
    public static class Harmony_Pawn_HealthTracker_NotifyPlayerOfKilled
    {
        private static readonly FieldInfo PawnFieldInfo;
        static Harmony_Pawn_HealthTracker_NotifyPlayerOfKilled()
        {
            PawnFieldInfo = typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        [HarmonyPrefix]
        public static bool Prefix(Pawn_HealthTracker __instance)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn.kindDef is BA.PawnKindDef)
            {
                // BA Pawn일 때 기본 Death Letter 스킵 (커스텀 Letter만 출력)
                return false;
            }
            return true; // 다른 Pawn은 기본 Letter 출력
        }
    }

    // DespawnPatches.cs 내 Utils 클래스 수정 예 (전체 파일 이전 응답 유지, 이 부분만 업데이트)
    public static class Utils
    {
        public static void SendDespawnLetter(Pawn pawn, string reason)
        {
            if (pawn == null || !pawn.Spawned) return;

            string title = "BA_PawnDespawnAlert".Translate();
            string message = BALocalizeKey.DespawnMessage.Translate(new NamedArgument(pawn.Name, "NAME"), new NamedArgument(pawn.kindDef.label, "LABEL"), new NamedArgument(reason, "REASON"));  // NamedArgument로 오버로드 지정, 모호함 방지

            Find.LetterStack.ReceiveLetter(title, message, LetterDefOf.ThreatSmall, pawn);
        }
    }
}