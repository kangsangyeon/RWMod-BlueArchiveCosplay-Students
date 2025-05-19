using System;
using System.Linq;
using System.Reflection;
using Rimworld;
using RimWorld;
using UniRx;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

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

            BridgeProcedure.CanShinbiLiberationFunc = CanShinbiLiberation;
            BridgeProcedure.OnShinbiLiberation = OnShinbiLiberation;

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
            // BA 네임스페이스에 포함된 로그 코드이거나, "BA."으로 시작하는 로그 메세지를 출력함.
            bool isBALog = stackTrace
                .Split('\n')
                .Any(x => x.StartsWith("BA."));
            if (!isBALog)
                isBALog = log.StartsWith("BA.");
            if (isBALog == false)
                return;
            var message = $"{type}::{log}\n{stackTrace}";
            switch (type)
            {
                case LogType.Error:
                case LogType.Assert:
                case LogType.Exception:
                    Log.Error(message);
                    break;
                case LogType.Warning:
                    Log.Warning(message);
                    break;
                case LogType.Log:
                    Log.Message(message);
                    break;
            }
        }

        private static void OnPawnSetup(Pawn instance, Map map, bool respawningAfterLoad)
        {
            if (BAUtil.IsBAPawn(instance, out _) == false)
                return;
            instance.style.BodyTattoo = TattooDefOf.NoTattoo_Body;
            instance.style.FaceTattoo = TattooDefOf.NoTattoo_Face;
        }

        private static void OnPawnSkillTrackerLearn(Pawn_SkillTracker instance, SkillDef sDef, float xp, bool direct, bool ignoreLearnRate)
        {
            var pawn = FieldInfo_PawnSkillTracker_Pawn.GetValue(instance) as Pawn;
            if (BAUtil.IsBAPawn(pawn, out var comp) == false)
                return;
            comp.OnSkillTrackerLearn(sDef, xp);
        }

        private static bool CanShinbiLiberation(int eligmaCost, int silverCost)
        {
            return BAUtil.GetTotalEligma() >= eligmaCost
                   && BAUtil.GetTotalSilver() >= silverCost;
        }

        private static void OnShinbiLiberation(int studentId, int eligmaCost, int silverCost)
        {
            var result = BAUtil.TryConsumeEligma(eligmaCost);
            result &= BAUtil.TryConsumeSilver(silverCost);
            if (!result)
            {
                Log.Error("뭔가 잘못됐다... 신비 해방 비용 없어서 실패해야 함.");
                return;
            }

            var pawn = BAUtil.GetFirstPawnOf(studentId);
            var comp = pawn.GetComp<BA.Comp_BAPawn>();
            ++comp.Shinbi;
            Log.Message($"신비 해방 성공. shinbi: {comp.Shinbi}");
        }
    }
}