using System.Linq;
using System.Reflection;
using RimWorld;
using UniRx;
using UnityEngine;
using Verse;

namespace BA
{
    [StaticConstructorOnStartup]
    public static class BAStudents
    {
        private static readonly FieldInfo FieldInfo_PawnSkillTracker_Pawn =
            typeof(Pawn_SkillTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance);

        public static bool DisableIMGUI = false;
        public static bool DisableCamera = false;
        public static bool DisableAudio = false;

        public static CoreAccessor CoreAccessor;
        public static Contents Contents;
        public static GameObject CorePrefab;
        public static GameObject ContentsPrefab;

        private static bool _setup;

        static BAStudents()
        {
            new HarmonyLib.Harmony("BlueArchiveStudents")
                .PatchAll(Assembly.GetExecutingAssembly());

            Application.logMessageReceived += OnUnityLogMessageReceived;

            Harmony_Pawn_SpawnSetup.OnPostfix
                .Subscribe(x => OnPawnSetup(x.instance, x.map, x.respawningAfterLoad));

            Harmony_Pawn_SkillTracker_Learn.OnPostfix
                .Subscribe(x => OnPawnSkillTrackerLearn(x.instance, x.sDef, x.xp, x.direct, x.ignoreLearnRate));

            TrySetup();
        }

        private static void TrySetup()
        {
            if (_setup)
                return;
            _setup = true;

            var currentMod =
                LoadedModManager.RunningMods.FirstOrDefault(x => x.PackageId == "bluearchive.students");
            GameResource.Bundle =
                AssetBundle.LoadFromFile($"{currentMod.RootDir}/Contents/Assets/assetbundle00");
            CorePrefab =
                GameResource.Load<GameObject>("Prefab", "Core");
            ContentsPrefab =
                GameResource.Load<GameObject>("Prefab", "Contents");

            CoreAccessor = Object.Instantiate(CorePrefab).GetComponent<CoreAccessor>();
            CoreAccessor.Camera.gameObject.SetActive(false);
            Object.DontDestroyOnLoad(CoreAccessor.gameObject);
        }

        private static void OnUnityLogMessageReceived(string log, string stackTrace, LogType type)
        {
            // BAU: blue archive unity
            if (log.StartsWith("BAU"))
                Log.Message($"{type.ToString()}::{log}\n{stackTrace}");
        }

        private static void OnPawnSetup(Pawn instance, Map map, bool respawningAfterLoad)
        {
            if (instance.kindDef.race.race.Humanlike == false) // 사람이 아니면 건너뜀
                return;
            if (instance.kindDef.race.race.Animal) // 동물이면 건너뜀
                return;
            if (instance.kindDef.defName.StartsWith("BA") == false) // 우리 프로젝트에서 정의한 pawnkind 아니면 건너뜀
                return;
            instance.style.BodyTattoo = TattooDefOf.NoTattoo_Body;
            instance.style.FaceTattoo = TattooDefOf.NoTattoo_Face;
        }

        private static void OnPawnSkillTrackerLearn(Pawn_SkillTracker instance, SkillDef sDef, float xp, bool direct, bool ignoreLearnRate)
        {
            var pawn = FieldInfo_PawnSkillTracker_Pawn.GetValue(instance) as Pawn;
            if (pawn == null)
                return;
            if (pawn.kindDef.defName.StartsWith("BA") == false) // 우리 프로젝트에서 정의한 pawnkind 아니면 건너뜀
                return;
            var pawnComp = pawn.GetComp<Comp_BAPawn>();
            pawnComp.Exp += Mathf.RoundToInt(xp);
        }
    }
}