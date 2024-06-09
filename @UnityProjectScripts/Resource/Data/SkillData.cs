public class SkillLevelData
{
    public (int SkillId, int Level) Id;
    public string Description;
    public int Star;
    public SkillActionData[] ActionSequence;
}

public class SkillData
{
    public int Id;
    public string Name;
    public string OverrideCommonIconName;
}