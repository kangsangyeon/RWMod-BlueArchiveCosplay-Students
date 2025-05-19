using System;

namespace Rimworld
{
    public class BridgeProcedure
    {
        public static Func<int, int, bool> CanShinbiLiberationFunc; // eligmaCost, silverCost
        public static Action<int, int, int> OnShinbiLiberation; // studentId, eligmaCost, silverCost
    }
}