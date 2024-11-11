using HarmonyLib;
using Verse;

namespace BA
{
    [HarmonyPatch(typeof(CameraDriver), "CameraDriverOnGUI")]
    public static class PatchDisableCameraDriverOnGUI
    {
        [HarmonyPrefix]
        public static bool Prefix() => !BAStudents.DisableCamera;
    }
}