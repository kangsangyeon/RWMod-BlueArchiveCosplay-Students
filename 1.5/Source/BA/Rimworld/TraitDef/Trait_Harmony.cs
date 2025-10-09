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

    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_BlockDamage_TakeDamage
    {
        private static int recursionDepth = 0;
        private const int MaxRecursionDepth = 10;
        private const float BlockChance = 0.9f;

        static bool Prefix(ref DamageInfo dinfo, Thing __instance)
        {
            recursionDepth++;
            if (recursionDepth > MaxRecursionDepth)
            {
                Log.Warning("Max recursion depth exceeded in TakeDamage patch. Skipping to prevent stack overflow.");
                recursionDepth--;
                return true;
            }

            try
            {
                if (!(__instance is Pawn pawn) || pawn.Dead || !pawn.Spawned || pawn.Map == null)
                    return true;

                TraitDef ignoreTrait = DefDatabase<TraitDef>.GetNamedSilentFail("Ignore");
                if (ignoreTrait == null)
                    return true;

                if (pawn.story?.traits?.HasTrait(ignoreTrait) == true)
                {
                    if (dinfo.Def == DamageDefOf.Bullet)
                    {
                        if (Rand.Value < BlockChance)
                        {
                            MoteMaker.ThrowText(pawn.Position.ToVector3Shifted(), pawn.Map, "Block!", Color.green);
                            dinfo.SetAmount(0f); // 피해 차단: Amount를 0으로 설정하여 피해 무시, 원본 메서드 실행
                            return true; // 원본 TakeDamage 진행 (0 damage)
                        }
                        else
                        {
                            dinfo.Def = DamageDefOf.Blunt;
                            return true; // 변경된 피해로 원본 TakeDamage 진행
                        }
                    }
                }
                return true;
            }
            finally
            {
                recursionDepth--;
            }
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
