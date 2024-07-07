using HarmonyLib;
using Verse;


namespace BlueArchiveStudents;

/**
 * harmony 함수의 매개변수의 이름은 실제 RimWorld dll에서 정의한 매개변수의 이름과 일치해야 정상 작동함.
 * 따라서 우리의 컨벤션에 맞추려 함부로 변경해서는 안됌.
 */

[HarmonyPatch(typeof(PawnGenerator), "GenerateBodyType")]
public static class Patch_PawnGenerator_GenerateBodyType
{
    [HarmonyPostfix]
    public static void Postfix(Pawn pawn, PawnGenerationRequest request)
    {
        if (!(pawn.kindDef is BA_PawnKindDef _kindDef))
            return;

        switch (pawn.gender)
        {
            case Gender.Male:
                if (_kindDef.forcedBodyType != null)
                    pawn.story.bodyType = _kindDef.forcedBodyType;
                break;

            case Gender.Female:
                if (_kindDef.forcedBodyTypeFemale != null)
                    pawn.story.bodyType = _kindDef.forcedBodyTypeFemale;
                break;
        }
    }
}