using Verse;

namespace BA;

public class BAUtil
{
    public static bool IsBAPawn(Pawn pawn, out Comp_BAPawn comp)
    {
        comp = null;
        if (pawn == null)
            return false;
        if (pawn.kindDef.race.race.Humanlike == false) // 사람이 아니면 건너뜀
            return false;
        if (pawn.kindDef.race.race.Animal) // 동물이면 건너뜀
            return false;
        if (pawn.kindDef.defName.StartsWith("BA") == false) // 우리 프로젝트에서 정의한 pawnkind 아니면 건너뜀
            return false;
        comp = pawn.GetComp<Comp_BAPawn>();
        return true;
    }
}