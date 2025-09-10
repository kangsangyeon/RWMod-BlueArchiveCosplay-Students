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

    [HarmonyPatch(
        typeof(PawnGenerator),
        nameof(PawnGenerator.GeneratePawn),
        new Type[] { typeof(PawnGenerationRequest) })]
    public static class Harmony_PawnGenerator_GeneratePawn
    {
        [HarmonyPostfix]
        public static void Postfix(
            ref Pawn __result,
            PawnGenerationRequest request)
        {
            var pawn = __result;

            if (__result.kindDef is not BA.PawnKindDef kindDef)
                return;

            // PawnKindDef의 skinColorOverride를 실제 pawn에 강제로 적용함.
            if (kindDef.skinColorOverride.HasValue)
                __result.story.skinColorOverride = kindDef.skinColorOverride.Value;

            // PawnKindDef에서 apparelRequired로 지정한 옷이 아닌 옷을 입고 나오는 것을 강제로 막음.
            // (예: faction의 ideology의 선호 의상을 입고 나오기 등.)
            var notRequiredApparels =
                pawn.apparel.WornApparel.Where(a =>
                    !pawn.kindDef.apparelRequired.Exists(t => t == a.def)).ToArray();

            for (int i = 0; i < notRequiredApparels.Length; ++i)
            {
                Log.Message($"apparel not required. destroy.: {notRequiredApparels[i].def.defName}.");
                notRequiredApparels[i].Destroy();
            }

            // PawnKindDef에서 apparelRequired로 지정한 옷을 입고 나오지 않으면, 옷을 생성해서 강제로 입힘.
            var requiredButNotWornDefs = pawn.kindDef.apparelRequired.Where(r =>
                !pawn.apparel.WornApparel.Exists(a => a.def == r)).ToArray();

            for (int i = 0; i < requiredButNotWornDefs.Length; ++i)
            {
                Log.Message($"apparel required but not worn. generate.: {requiredButNotWornDefs[i].defName}.");
                var apparel = PawnApparelGenerator.GenerateApparelOfDefFor(pawn, requiredButNotWornDefs[i]);
                pawn.apparel.WornApparel.Add(apparel);
            }

            // PawnKindDef에서 fixedChildBackstories와 fixedAdultBackstories로 지정한 backstory를 강제로 적용함.
            if (kindDef.fixedChildBackstories.Count > 0)
                __result.story.Childhood = kindDef.fixedChildBackstories[0];
            if (kindDef.fixedAdultBackstories.Count > 0)
                __result.story.Adulthood = kindDef.fixedAdultBackstories[0];

            // 학생 나이로 설정함.
            var ageTick
                = GameResource.StudentTable[kindDef.studentId].Age * 3600000L;
            pawn.ageTracker.AgeBiologicalTicks = ageTick;
            pawn.ageTracker.AgeChronologicalTicks = ageTick;

            // 학생 이름으로 설정함.
            var name = GameResource.StudentTable[kindDef.studentId].Name;
            var firstName = BALocalizeKey.StudentFirstName(name).Translate();
            var lastName = BALocalizeKey.StudentLastName(name).Translate();
            // 일본식 이름으로 표기할것이기 때문에 이름과 성의 순서를 바꿈.
            __result.Name = new NameTriple(lastName, firstName, firstName);
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

    // [HarmonyPatch(
    //     typeof(Pawn),
    //     nameof(Pawn.PostApplyDamage))]
    // public static class Harmony_Pawn_PostApplyDamage
    // {
    //     [HarmonyPostfix]
    //     public static void Postfix(
    //         Pawn __instance,
    //         DamageInfo dinfo,
    //         float totalDamageDealt)
    //     {
    //         if (!__instance.Spawned)
    //             return;
    //         if (__instance.kindDef is not BA.PawnKindDef kindDef)
    //             return;
    //         if (__instance.health.summaryHealth.SummaryHealthPercent > 0.1f)
    //             return;
    //         // health 10% 이하면 despawn 대상임.
    //         // __instance.DeSpawn();
    //         Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(__instance);
    //     }
    // }

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
            // pawn.DeSpawn();
            Current.Game.GetComponent<GameComponent_DelayedPawnDestroy>().TryAdd(pawn);
        }
    }
}