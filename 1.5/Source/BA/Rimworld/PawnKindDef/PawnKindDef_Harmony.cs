﻿using System;
using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;

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
                PawnApparelGenerator.GenerateApparelOfDefFor(pawn, requiredButNotWornDefs[i]);
            }
        }
    }
}