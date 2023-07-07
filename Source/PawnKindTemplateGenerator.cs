using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BlueArchiveStudents
{
    public class PawnKindTemplateGenerator
    {
        public static bool currentlyGenerating = false;
        protected static Pawn generatingPawn;

        public void Generate(PawnKindTemplateDef _pawnKindDef, Map _map)
        {
            currentlyGenerating = true;
            IntVec3 result;
            if (_map != null && RCellFinder.TryFindRandomPawnEntryCell(out result, _map, 0.2f))
            {
                generatingPawn = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist, Faction.OfPlayer);

                _pawnKindDef.Fill(generatingPawn);

                Thing weapon = null;
                if (_pawnKindDef.weaponDef != null)
                    weapon = GenerateAndEquipWeapon(_pawnKindDef, generatingPawn);

                if (ModLister.BiotechInstalled)
                {
                    generatingPawn.genes = new Pawn_GeneTracker(generatingPawn);
                    generatingPawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
                }

                GenSpawn.Spawn(generatingPawn, result, _map);
                CameraJumper.TryJump(new GlobalTargetInfo(result, _map));
            }

            currentlyGenerating = false;
        }

        private ThingWithComps GenerateAndEquipWeapon(PawnKindTemplateDef _pawnKindDef, Pawn _pawn)
        {
            ThingWithComps _weapon = ThingMaker.MakeThing(_pawnKindDef.weaponDef) as ThingWithComps;
            _weapon.GetComp<CompBiocodable>().CodeFor(_pawn);
            _pawn.equipment.AddEquipment(_weapon);
            return _weapon;
        }
    }
}