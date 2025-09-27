using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
namespace BA
{
    /**
     * harmony 함수의 매개변수의 이름은 실제 RimWorld dll에서 정의한 매개변수의 이름과 일치해야 정상 작동함.
     * 따라서 우리의 컨벤션에 맞추려 함부로 변경해서는 안됌.
     */
    [HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
    public static class Harmony_PawnGenerator_GenerateBodyType
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, PawnGenerationRequest request)
        {
            if (!(pawn.kindDef is BA.PawnKindDef kindDef))
                return;
            switch (pawn.gender)
            {
                case Gender.Male:
                    if (kindDef.forcedBodyType != null)
                        pawn.story.bodyType = kindDef.forcedBodyType;
                    if (kindDef.forcedHeadType != null)
                        pawn.story.headType = kindDef.forcedHeadType;
                    break;
                case Gender.Female:
                    if (kindDef.forcedBodyTypeFemale != null)
                        pawn.story.bodyType = kindDef.forcedBodyTypeFemale;
                    if (kindDef.forcedHeadTypeFemale != null)
                        pawn.story.headType = kindDef.forcedHeadTypeFemale;
                    break;
            }
        }
    }
    [HarmonyPatch(typeof(PawnGenerator), nameof(PawnGenerator.GeneratePawn), new Type[] { typeof(PawnGenerationRequest) })]
    public static class Harmony_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(ref Pawn __result, PawnGenerationRequest request)
        {
            if (__result == null)
                return;
            // 최초에 한번만 PawnKindDef를 안전하게 캐스팅
            if (__result.kindDef is not BA.PawnKindDef kindDef)
                return;

            var pawn = __result;

            // 1) skinColorOverride 강제 적용
            if (kindDef.skinColorOverride.HasValue)
                pawn.story.skinColorOverride = kindDef.skinColorOverride.Value;

            // 2) apparelRequired 아닌 옷 제거
            var notRequiredApparels = pawn.apparel.WornApparel.Where(a =>
                !pawn.kindDef.apparelRequired.Exists(t => t == a.def)).ToArray();
            foreach (var apparel in notRequiredApparels)
            {
                apparel.Destroy();
            }
            // 3) apparelRequired에 있지만 입지 않은 옷 생성 및 착용 강제
            var requiredButNotWornDefs = pawn.kindDef.apparelRequired.Where(r =>
                !pawn.apparel.WornApparel.Any(a => a.def == r)).ToArray();
            foreach (var def in requiredButNotWornDefs)
            {
                var apparel = PawnApparelGenerator.GenerateApparelOfDefFor(pawn, def);
                pawn.apparel.WornApparel.Add(apparel);
            }
            // 4) fixedChildBackstories와 fixedAdultBackstories 강제 적용
            if (kindDef.fixedChildBackstories.Count > 0)
                pawn.story.Childhood = kindDef.fixedChildBackstories[0];
            if (kindDef.fixedAdultBackstories.Count > 0)
                pawn.story.Adulthood = kindDef.fixedAdultBackstories[0];
            // 5) 학생 나이 세팅
            long ageTick = GameResource.StudentTable[kindDef.studentId].Age * 3600000L;
            pawn.ageTracker.AgeBiologicalTicks = ageTick;
            pawn.ageTracker.AgeChronologicalTicks = ageTick;
            // 6) 학생 이름 세팅 (일본식 성, 이름 순서)
            var name = GameResource.StudentTable[kindDef.studentId].Name;
            var firstName = BALocalizeKey.StudentFirstName(name).Translate();
            var lastName = BALocalizeKey.StudentLastName(name).Translate();
            pawn.Name = new NameTriple(lastName, firstName, firstName);

            // 7) 학생 체력 세팅 
            float GetTotalHealth(Pawn p)
            {
                float total = 0f;
                foreach (var part in p.health.hediffSet.GetNotMissingParts())
                {
                    total += p.health.hediffSet.GetPartHealth(part);
                }
                return total;
            }

            float baseScale = pawn.RaceProps.baseHealthScale;
            float finalScale = baseScale * kindDef.healthScale;
            pawn.RaceProps.baseHealthScale = finalScale;

            pawn.health.Reset();
        }
    }
    [HarmonyPatch(
        typeof(Pawn_AgeTracker),
        nameof(Pawn_AgeTracker.AgeTick))]
    public static class Harmony_Pawn_AgeTracker_AgeTick
    {
        private static readonly FieldInfo PawnFieldInfo;
        static Harmony_Pawn_AgeTracker_AgeTick()
        {
            PawnFieldInfo =
                typeof(Pawn_AgeTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        [HarmonyPrefix]
        public static bool Prefix(Pawn_AgeTracker __instance)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn.kindDef is not BA.PawnKindDef kindDef)
                return true;
            // BA pawn이면 나이를 먹지 않도록 강제함.
            return false;
        }
    }
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
            // 플레이어 faction이 아니면 despawn 대상임.
            // CharacterEditor에서 faction 변경하면 SetFaction이 2번 호출됌. 한 번만 추가해야 함.
            Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(__instance);
        }
    }
    [HarmonyPatch(typeof(PawnGenerator), "GenerateTraits")]
    public static class Patch_PawnGenerator_GenerateTraits
    {
        public static bool Prefix(Pawn pawn)
        {
            if (pawn.kindDef is BA.PawnKindDef kindDef && kindDef.traits != null && kindDef.traits.Count > 0)
            {
                foreach (TraitDef traitDef in kindDef.traits)
                {
                    // 중복 Trait 방지
                    if (!pawn.story.traits.HasTrait(traitDef))
                    {
                        pawn.story.traits.GainTrait(new Trait(traitDef));
                    }
                }
                return false; // 기존 GenerateTraits 로직 건너뜀
            }
            return true; // 기본 로직 실행
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
        public static bool Prefix(Pawn __instance, Verse.DamageInfo? dinfo, Verse.Hediff exactCulprit)
        {
            if (__instance.Dead) return false; // 이미 사망했으면 중복 처리 방지
            if (__instance.kindDef is BA.PawnKindDef)
            {
                // 폰이 스폰 되어 있고 월드에 존재하면서 즉시 제거 가능하면 Despawn 실시 후 원본 Kill 실행 차단
                if (__instance.Spawned && __instance.Map != null)
                {
                    __instance.DeSpawn(DestroyMode.Vanish);
                    return false;
                }
            }
            return true; // BA 외 폰은 원래 로직 실행
        }
    }
}
