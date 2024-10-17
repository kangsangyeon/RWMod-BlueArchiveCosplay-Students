using System.Collections.Generic;
using System.Reflection;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace BA
{
    public class PawnKindTemplateGenerator
    {
        public static bool currentlyGenerating = false;

        public static void Generate(PawnKindTemplateDef _templateDef, Map _map)
        {
            currentlyGenerating = true;

            if (_map != null)
            {
                Pawn _generatingPawn = GenerateWithTemplate(_templateDef);

                IntVec3 _spawnPosition = Current.CameraDriver.MapPosition;
                GenSpawn.Spawn(_generatingPawn, _spawnPosition, _map);

                var _methodInfo = typeof(DebugToolsSpawning).GetMethod("PostPawnSpawn",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                _methodInfo.Invoke(null, new object[] { _generatingPawn });

                CameraJumper.TryJump(new GlobalTargetInfo(_spawnPosition, _map));
            }

            currentlyGenerating = false;
        }

        private static void GenerateAndEquipWeapon(ThingDef _weaponDef, Pawn _pawn)
        {
            ThingWithComps _weapon = ThingMaker.MakeThing(_weaponDef) as ThingWithComps;
            _weapon.GetComp<CompBiocodable>()?.CodeFor(_pawn);
            _pawn.equipment.AddEquipment(_weapon);
        }

        private static void GenerateAndWearApparel(IEnumerable<ThingDef> _apparelDefs, Pawn _pawn)
        {
            foreach (var _apparelDef in _apparelDefs)
            {
                Apparel _newApparel = ThingMaker.MakeThing(_apparelDef) as Apparel;
                _newApparel.GetComp<CompBiocodable>()?.CodeFor(_pawn);
                _pawn.apparel.Wear(_newApparel);
            }
        }

        private static Pawn GenerateWithTemplate(PawnKindTemplateDef _templateDef)
        {
            Verse.PawnKindDef _pawnKindDef = _templateDef.pawnKindDef;

            if (_pawnKindDef == null)
            {
                // pawnKindDef 속성을 설정하지 않은 경우, 기본적으로 Colonist로 스폰합니다.
                _pawnKindDef = PawnKindDefOf.Colonist;
            }

            Pawn _pawn = PawnGenerator.GeneratePawn(_pawnKindDef, Faction.OfPlayer);
            _pawn.Name = (Name)new NameTriple(_templateDef.firstName, _templateDef.nickname, _templateDef.lastName);
            _pawn.needs.food.CurLevel = _pawn.needs.food.MaxLevel;
            _pawn.gender = _templateDef.isMale ? Gender.Male : Gender.Female;
            _pawn.story.Childhood = _templateDef.childHood ?? _pawn.story.Childhood;
            _pawn.story.Adulthood = _templateDef.adultHood ?? _pawn.story.Adulthood;
            _pawn.story.bodyType = _templateDef.bodyTypeDef ?? _pawn.story.bodyType;
            _pawn.story.headType = _templateDef.headTypeDef ?? _pawn.story.headType;
            _pawn.story.hairDef = _templateDef.hair ?? _pawn.story.hairDef;
            _pawn.style.beardDef = _templateDef.beard ?? _pawn.style.beardDef;

            // error 나이를 강제로 고치면 문제가 발생합니다.
            _pawn.ageTracker.AgeBiologicalTicks = (long)_templateDef.age * 3600000L; // 1000시간당 나이 + 1, 1시간은 3600초
            _pawn.ageTracker.AgeChronologicalTicks = (long)_templateDef.realAge * 3600000L;

            if (_templateDef.overrideSkinColor)
                _pawn.story.skinColorOverride = _templateDef.skinColor;

            if (_templateDef.overrideHairColor)
                _pawn.story.HairColor = _templateDef.hairColor;

            if (_templateDef.apparels != null)
                GenerateAndWearApparel(_templateDef.apparels, _pawn);

            if (_templateDef.weaponDef != null)
                GenerateAndEquipWeapon(_templateDef.weaponDef, _pawn);

            if (ModLister.BiotechInstalled && ModLister.HasActiveModWithName("Biotech"))
            {
                _pawn.genes = new Pawn_GeneTracker(_pawn);
                _pawn.genes.SetXenotype(XenotypeDefOf.Baseliner);
            }

            _pawn.Notify_DisabledWorkTypesChanged();

            return _pawn;
        }
    }
}