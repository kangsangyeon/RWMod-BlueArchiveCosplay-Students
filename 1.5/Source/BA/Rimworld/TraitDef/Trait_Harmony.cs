using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

namespace BA
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
    public static class Patch_BlockDamage_PreApplyDamage
    {
        private static readonly FieldInfo PawnFieldInfo =
            typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        private const float BlockChance = 0.9f;

        static bool Prefix(ref DamageInfo dinfo, Pawn_HealthTracker __instance)
        {
            Pawn pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn == null || pawn.Dead)
                return true;

            TraitDef ignoreTrait = DefDatabase<TraitDef>.GetNamedSilentFail("Ignore");
            if (ignoreTrait == null)
                return true;

            if (pawn.story?.traits?.HasTrait(ignoreTrait) == true)
            {
                if (Rand.Value < BlockChance)
                {
                    MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, "Block!", Color.green);
                    return false; // 피해 차단 (회피)
                }
            }
            return true; // 피해 정상 진행
        }
    }


    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage")]
    public static class Patch_ModifyDamage_PreApplyDamage
    {
        private static readonly FieldInfo PawnFieldInfo = typeof(Pawn_HealthTracker)
            .GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        static bool Prefix(ref DamageInfo dinfo, Pawn_HealthTracker __instance)
        {
            Pawn pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn == null || pawn.Dead)
                return true;

            TraitDef emotionalBondTrait = DefDatabase<TraitDef>.GetNamedSilentFail("EmotionalBond");
            if (emotionalBondTrait == null)
                return true;

            if (pawn.story?.traits?.HasTrait(emotionalBondTrait) == true)
            {
                float mood = pawn.needs?.mood?.CurLevel ?? 1f;
                // 받는 피해량 보정 (기분 1 → 0.01배, 기분 0 → 0.05배)
                float damageTakenMultiplier = 0.05f - (mood * 0.04f);
                float originalDamage = dinfo.Amount;
                float adjustedDamage = Mathf.Max(originalDamage * damageTakenMultiplier, 1f);
                dinfo.SetAmount(adjustedDamage);

                Pawn instigator = dinfo.Instigator as Pawn;
                if (instigator != null && instigator.story?.traits?.HasTrait(emotionalBondTrait) == true)
                {
                    // 가하는 피해량 배율 (기분 1 → 1.3배, 기분 0 → 0.7배)
                    float damageDealtMultiplier = 0.7f + (mood * 0.6f);
                    float modifiedDamage = Mathf.Max(dinfo.Amount * damageDealtMultiplier, 1f);
                    dinfo.SetAmount(modifiedDamage);
                }
            }
            return true;
        }
    }
}
