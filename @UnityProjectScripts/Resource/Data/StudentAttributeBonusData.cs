namespace BA
{
    public enum BonusTarget
    {
        None,
        DamageAddition,
        WeaponRangeAddition,
        StunResistanceAddition,
        TargetAttentionRateAddition,
        CooldownReductionAddition,
        MoveSpeedAddition,
        SelfHealingAddition,
        MaxCount
    }

    public enum BonusOperation
    {
        Add,
        Mul,
        Set,
    }

    public class StudentAttributeBonusData
    {
        public string Id;
        public StudentAttribute Attribute;
        public BonusTarget Bonus;
        public BonusOperation Operation;
        public int Value;
    }
}