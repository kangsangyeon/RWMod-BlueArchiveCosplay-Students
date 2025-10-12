namespace BA
{
    public static class BALocalizeKey
    {
        private const string StudentLastNamePrefix = "BA_Keyed_Student_LastName_";
        private const string StudentFirstNamePrefix = "BA_Keyed_Student_FirstName_";

        public static string StudentLastName(string name) => StudentLastNamePrefix + name;
        public static string StudentFirstName(string name) => StudentFirstNamePrefix + name;

        public static string DespawnMessage => "BA_DespawnMessage";
    }
}