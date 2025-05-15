using LudeonTK;
using Verse;

namespace BA
{
    public static class BAUtilDevelopment
    {
        [DebugAction(Const.DebugActionCategory, nameof(BAUtilDevelopment) + "::" + nameof(LogTotalBudget),
            false, false, false, false,
            0, false)]
        public static void LogTotalBudget()
        {
            var silver = BAUtil.GetTotalSilver();
            var eligma = BAUtil.GetTotalEligma();
            Log.Message($"total silver: {silver}, total eligma: {eligma}");
        }
    }
}