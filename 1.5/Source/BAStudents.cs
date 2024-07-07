using System.Reflection;
using Verse;

namespace BlueArchiveStudents
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatcher
    {
        static HarmonyPatcher()
        {
            new HarmonyLib.Harmony("BlueArchiveStudents")
                .PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    public static class BAStudents
    {
        public static bool DisableIMGUI = false;
    }
}