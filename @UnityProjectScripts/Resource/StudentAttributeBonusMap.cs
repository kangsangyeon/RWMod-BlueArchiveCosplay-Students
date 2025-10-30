namespace BA
{
    public class StudentAttributeBonusMap
    {
        public StudentAttribute Id;
        public int[] Values = new int[(int)BonusTarget.MaxCount];

        public int GetValue(BonusTarget target)
        {
            return Values[(int)target];
        }
    }
}