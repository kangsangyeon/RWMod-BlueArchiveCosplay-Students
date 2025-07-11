namespace BA
{
    public interface ISkillLevelLocalizationData
    {
        public (int SkillId, int Level) Id { get; }
        public string Description { get; }
    }

    public interface ISkillLocalizationData
    {
        public int Id { get; }
        public string Name { get; }
    }
}