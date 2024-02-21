using System.Reflection;
using Verse;

namespace BlueArchiveStudents;

[StaticConstructorOnStartup]
public static class HarmonyPatches
{
    static HarmonyPatches()
    {
        new HarmonyLib.Harmony("BlueArchiveStudents").PatchAll(Assembly.GetExecutingAssembly());
    }
}

public static class BAStudents
{
    public static bool DisableIMGUI = false;
}