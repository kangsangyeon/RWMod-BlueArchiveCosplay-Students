using Verse;

namespace BA
{
    public class Gene_Immutable : Gene
    {
        public override void PostAdd()
        {
            base.PostAdd();
            ApplyMetabolismLock();
            pawn.health.capacities.Notify_CapacityLevelsDirty();
        }

        public override void PostRemove()
        {
            base.PostRemove();
            pawn.health.capacities.Notify_CapacityLevelsDirty();
        }

        private void ApplyMetabolismLock()
        {
            if (pawn?.genes == null) return;

            foreach (Gene gene in pawn.genes.GenesListForReading)
            {
                if (gene != this && gene.def != null && gene.def.biostatMet != 0)
                {
                    gene.def.biostatMet = 0;
                }
            }
        }
    }
}