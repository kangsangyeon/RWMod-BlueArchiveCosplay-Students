using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BlueArchiveStudents
{
    public class PawnKindTemplateGenerator
    {
        public static bool currentlyGenerating = false;
        protected static Pawn generatingPawn;

        public static void Generate(PawnKindTemplateDef _pawnKindDef, Map _map)
        {
            currentlyGenerating = true;
            IntVec3 _spawnPosition;
            if (_map != null)
            {
                generatingPawn = _pawnKindDef.Spawn();

                if (_pawnKindDef.apparels != null)
                    GenerateAndWearApparel(_pawnKindDef, generatingPawn);

                Thing weapon = null;
                if (_pawnKindDef.weaponDef != null)
                    weapon = GenerateAndEquipWeapon(_pawnKindDef, generatingPawn);

                if (ModLister.BiotechInstalled)
                {
                    generatingPawn.genes = new Pawn_GeneTracker(generatingPawn);
                    generatingPawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
                }

                _spawnPosition = Current.CameraDriver.MapPosition;

                GenSpawn.Spawn(generatingPawn, _spawnPosition, _map);

                var _methodInfo = typeof(DebugToolsSpawning).GetMethod("PostPawnSpawn",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                _methodInfo.Invoke(null, new object[] { generatingPawn });

                CameraJumper.TryJump(new GlobalTargetInfo(_spawnPosition, _map));
            }

            currentlyGenerating = false;
        }

        private static ThingWithComps GenerateAndEquipWeapon(PawnKindTemplateDef _pawnKindDef, Pawn _pawn)
        {
            ThingWithComps _weapon = ThingMaker.MakeThing(_pawnKindDef.weaponDef) as ThingWithComps;
            _weapon.GetComp<CompBiocodable>().CodeFor(_pawn);
            _pawn.equipment.AddEquipment(_weapon);
            return _weapon;
        }

        private static void GenerateAndWearApparel(PawnKindTemplateDef _pawnKindDef, Pawn _pawn)
        {
            foreach (ThingDef _apparel in _pawnKindDef.apparels)
            {
                Apparel _newApparel = ThingMaker.MakeThing(_apparel) as Apparel;
                _newApparel.GetComp<CompBiocodable>()?.CodeFor(_pawn);
                _pawn.apparel.Wear(_newApparel);
            }
        }
    }
}