using HarmonyLib;
using RimWorld;
using Verse;

namespace BA
{
    [HarmonyPatch(typeof(UIRoot), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUI
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }

    [HarmonyPatch(typeof(UIRoot_Entry), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUIE
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }

    [HarmonyPatch(typeof(UIRoot_Play), "UIRootOnGUI")]
    public static class PatchDisableUIRootOnGUIP
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }

    [HarmonyPatch(typeof(UIRoot), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdate
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }

    [HarmonyPatch(typeof(UIRoot_Entry), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdateE
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }

    [HarmonyPatch(typeof(UIRoot_Play), "UIRootUpdate")]
    public static class PatchDisableUIRootUpdateP
    {
        [HarmonyPrefix]
        public static bool prefix() => !BAStudents.DisableIMGUI;
    }
}