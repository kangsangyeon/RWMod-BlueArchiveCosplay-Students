using RimWorld;
using Verse;

namespace BA
{
    public class CompProperties_UseEffectBAPawnGainExp : CompProperties_UseEffect
    {
        public int exp;

        public CompProperties_UseEffectBAPawnGainExp() => this.compClass = typeof(CompUseEffect_BAPawnGainExp);
    }

    public class CompUseEffect_BAPawnGainExp : CompUseEffect
    {
        public override AcceptanceReport CanBeUsedBy(Pawn p)
        {
            if (BAUtil.IsBAPawn(p, out _) == false)
                return false;
            return base.CanBeUsedBy(p);
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (BAUtil.IsBAPawn(usedBy, out var comp) == false)
                return;
            var props = this.props as CompProperties_UseEffectBAPawnGainExp;
            comp.Exp += props.exp;
        }
    }
}