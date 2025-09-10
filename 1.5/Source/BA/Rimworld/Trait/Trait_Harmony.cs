using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace BA.Traits
{
    public static class Trait_Harmony
    {
        // 지정한 Pawn과 TraitDef 이름으로 설명을 받아 정착민 이름 치환 후 반환
        public static string GetFormattedTraitDescription(Pawn pawn, string traitDefName)
        {
            TraitDef traitDef = DefDatabase<TraitDef>.GetNamed(traitDefName, false);
            if (traitDef == null)
                return string.Empty;

            string description = traitDef.description ?? string.Empty;

            string pawnName = pawn?.Name?.ToStringFull ?? "Unknown";

            // {0} 플레이스홀더를 정착민 이름으로 치환
            return string.Format(description, pawnName);
        }
    }

    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage")]
    public static class Patch_Pawn_HealthTracker_PreApplyDamage
    {
        private static readonly FieldInfo PawnFieldInfo = typeof(Pawn_HealthTracker)
            .GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        static void Prefix(ref DamageInfo dinfo, Pawn_HealthTracker __instance)
        {
            var pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn == null || pawn.Dead)
                return;

            if (pawn.story?.traits?.HasTrait(DefDatabase<TraitDef>.GetNamed("HalosPassion")) == true)
            {
                float mood = pawn.needs?.mood?.CurLevel ?? 1f;

                float moodOffset = mood - 0.5f;
                float maxChange = 0.3f;

                // 받는 피해량 배율 (30%)
                float damageTakenMultiplier = 1f + (0.3f - moodOffset * maxChange * 2);

                // 주는 피해량 배율은 기존과 동일 (mood 0: 0.7, mood 1: 1.3)
                float damageDealtMultiplier = 1f - (0.3f - moodOffset * maxChange * 2);

                dinfo.SetAmount(dinfo.Amount * damageTakenMultiplier);

                var instigator = dinfo.Instigator as Pawn;
                if (instigator != null && instigator.story?.traits?.HasTrait(DefDatabase<TraitDef>.GetNamed("HalosPassion")) == true)
                {
                    dinfo.SetAmount(dinfo.Amount * damageDealtMultiplier);
                }
            }
        }
    }


}
