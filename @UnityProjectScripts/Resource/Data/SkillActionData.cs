public class SkillActionData
{
    public float Time;
}

public class CircleRangeAttack : SkillActionData
{
    public float Radius;
    public int AttackPercentage;
    public string Sfx;
    public string Vfx;
}

public class LineRangeAttack : SkillActionData
{
    public float Width;
    public float Length;
    public int AttackPercentage;
    public string Sfx;
    public string Vfx;
}