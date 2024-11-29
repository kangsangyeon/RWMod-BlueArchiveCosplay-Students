public class ConstValue
{
    public string Id;
    public object Value;
}

public class Const
{
    public PawnCompSettings PawnCompSettings;
}

public class PawnCompSettings
{
    public int MaxLevel;
    public int Tick;
    public int GainExpPerTick;
    public float GainExpBonusThreshold;
    public float GainExpBonusMultiplier;
    public int GainExpDayLimitCount;
    public float GainExpDayLimitMultiplier;
}