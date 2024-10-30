using System.Linq;
using RimWorld;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Verse;
using Object = UnityEngine.Object;

namespace BA
{
    public class ContentsWindow : MainTabWindow
    {
        public override Vector2 InitialSize => new(0.0f, 0.0f);

        private CoreAccessor coreAccessor;
        private Contents contents;
        private bool isInitialized;

        public override void PreOpen()
        {
            base.PreOpen();
            TryInitialize();
            contents.gameObject.SetActive(true);
        }

        public override void DoWindowContents(Rect inRect)
        {
            contents.Accessor.UICamera.Render();
        }

        private void TryInitialize()
        {
            if (isInitialized)
                return;
            isInitialized = true;

            layer = WindowLayer.Super;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            closeOnCancel = false;

            var _currentMod =
                LoadedModManager.RunningMods.FirstOrDefault(x => x.PackageId == "bluearchive.students");
            GameResource.Bundle =
                AssetBundle.LoadFromFile($"{_currentMod.RootDir}/Contents/Assets/assetbundle00");

            var _corePrefab =
                GameResource.Load<GameObject>("Prefab", "Core");
            var _contentsPrefab =
                GameResource.Load<GameObject>("Prefab", "Contents");

            coreAccessor = Object.Instantiate(_corePrefab).GetComponent<CoreAccessor>();
            coreAccessor.Camera.gameObject.SetActive(false);
            contents = Object.Instantiate(_contentsPrefab).GetComponent<Contents>();
            contents.Accessor.gameObject.SetActive(false);

            contents.gameObject.OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    BAStudents.DisableIMGUI = true;
                    BAStudents.DisableCamera = true;
                    BAStudents.DisableAudio = true;
                    Current.Game.tickManager.CurTimeSpeed = TimeSpeed.Paused;
                });
            contents.gameObject.OnDisableAsObservable()
                .Subscribe(_ =>
                {
                    BAStudents.DisableIMGUI = false;
                    BAStudents.DisableCamera = false;
                    BAStudents.DisableAudio = false;
                    Current.Game.tickManager.CurTimeSpeed = TimeSpeed.Normal;
                });

            contents.Accessor.PadCanvas.HomeButton.OnClickAsObservable()
                .Subscribe(_ => contents.gameObject.SetActive(false));

            this.ObserveEveryValueChanged(_ => BAStudents.DisableAudio)
                .Subscribe(v =>
                {
                    Current.Root.soundRoot.sourcePool.sourcePoolCamera.cameraSourcesContainer.gameObject.SetActive(!v);
                    Current.Root.soundRoot.sourcePool.sourcePoolWorld.GetSourceWorld().gameObject.SetActive(!v);
                });
        }
    }
}