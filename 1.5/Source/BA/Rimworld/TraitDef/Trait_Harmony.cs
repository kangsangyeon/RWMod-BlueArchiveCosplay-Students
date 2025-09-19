using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;
using UnityEngine;

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

        private const float EvadeChance = 0.5f; // 50% 회피 확률

        static bool Prefix(ref DamageInfo dinfo, Pawn_HealthTracker __instance)
        {
            Pawn pawn = (Pawn)PawnFieldInfo.GetValue(__instance);
            if (pawn == null || pawn.Dead)
            {
                Log.Message("[EmotionalBond] Pawn is null or dead, skipping damage adjustment.");
                return true;
            }

            TraitDef emotionalBondTrait = DefDatabase<TraitDef>.GetNamedSilentFail("EmotionalBond");
            if (emotionalBondTrait == null)
            {
                Log.Error("[EmotionalBond] TraitDef 'EmotionalBond' not found in DefDatabase!");
                return true;
            }

            if (pawn.story?.traits?.HasTrait(emotionalBondTrait) == true)
            {
                // 피해 회피 판정
                if (Rand.Value < EvadeChance)
                {
                    MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, "Block!", Color.green);
                    Log.Message($"[EmotionalBond] {pawn.Name} evaded the damage!");
                    dinfo.SetAmount(0f);
                    return true;
                }

                float mood = pawn.needs?.mood?.CurLevel ?? 1f;
                Log.Message($"[EmotionalBond] Pawn mood level: {mood:F2}");

                // 받는 피해량 보정 (기분 1 → 0.01배, 기분 0 → 0.05배)
                float damageTakenMultiplier = 0.05f - (mood * 0.04f);
                Log.Message($"[EmotionalBond] damageTakenMultiplier={damageTakenMultiplier:F4}");

                float originalDamage = dinfo.Amount;

                // 원래 피해량 표시 (흰색)
                MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, $"-{originalDamage:F1}", Color.white);

                // 피해량 조정 및 최소 피해 1 보장
                float adjustedDamage = Mathf.Max(originalDamage * damageTakenMultiplier, 1f);
                dinfo.SetAmount(adjustedDamage);

                // 변경된 받는 피해량 표시 (빨간색)
                MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, $"-{adjustedDamage:F1}", Color.red);

                // 가하는 피해량 증가 적용
                Pawn instigator = dinfo.Instigator as Pawn;
                if (instigator != null && instigator.story?.traits?.HasTrait(emotionalBondTrait) == true)
                {
                    // 가하는 피해량 배율 (기분 1 -> 1.3배, 기분 0 -> 0.7배)
                    float damageDealtMultiplier = 0.7f + (mood * 0.6f);

                    float modifiedDamage = Mathf.Max(dinfo.Amount * damageDealtMultiplier, 1f);
                    dinfo.SetAmount(modifiedDamage);

                    Log.Message("[EmotionalBond] Instigator has EmotionalBond trait, damage increased.");
                    MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, $"-{modifiedDamage:F1} (가해 조정)", Color.yellow);
                }

                return true;
            }
            else
            {
                Log.Message("[EmotionalBond] Pawn does not have EmotionalBond trait, no damage adjustment.");
                return true;
            }
        }
    }
}
