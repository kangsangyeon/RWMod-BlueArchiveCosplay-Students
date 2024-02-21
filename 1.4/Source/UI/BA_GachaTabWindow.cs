using System.Collections;
using System.Linq;
using RimWorld;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using Verse;
using Object = UnityEngine.Object;

namespace BlueArchiveStudents.UI
{
    public class BA_GachaTabWindow : MainTabWindow
    {
        public override Vector2 InitialSize => new(0.0f, 0.0f);

        private AssetBundle Asset;
        private bool isInitialized = false;
        private bool bIsOpen = false;
        private Camera canvasCam;
        private Canvas padCanvas;
        private PadAccessor padAccessor;

        public override void PreOpen()
        {
            base.PreOpen();

            TryInitialize();

            bIsOpen = true;
            padCanvas.gameObject.SetActive(true);
        }

        public override void DoWindowContents(Rect inRect)
        {
            if (canvasCam != null)
                canvasCam.Render();
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
            Asset = AssetBundle.LoadFromFile($"{_currentMod.RootDir}/Contents/Assets/assetbundle00");

            var _eventSystemPrefab = Asset.LoadAsset<GameObject>("EventSystem");
            var _uiCameraPrefab = Asset.LoadAsset<GameObject>("UICamera");
            var _canvasPrefab = Asset.LoadAsset<GameObject>("UICanvas_Gacha");
            var _videoPlayerGachaPrefab = Asset.LoadAsset<GameObject>("VideoPlayer_Gacha");
            var _padCanvasPrefab = Asset.LoadAsset<GameObject>("UICanvas_Pad");

            var _eventSystem = Object.Instantiate(_eventSystemPrefab).GetComponent<EventSystem>();
            var _uiCamera = Object.Instantiate(_uiCameraPrefab).GetComponent<Camera>();
            // var _canvas = Object.Instantiate(_canvasPrefab).GetComponent<Canvas>();
            // var _videoPlayerGacha = Object.Instantiate(_videoPlayerGachaPrefab);
            padCanvas = Object.Instantiate(_padCanvasPrefab).GetComponent<Canvas>();
            padAccessor = padCanvas.GetComponentInChildren<PadAccessor>();
            padCanvas.gameObject.SetActive(false);

            canvasCam = _uiCamera;
            canvasCam.clearFlags = CameraClearFlags.Depth;
            // _canvas.worldCamera = _uiCamera;
            // _padCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            padCanvas.worldCamera = _uiCamera;

            padCanvas.gameObject.OnEnableAsObservable()
                .Subscribe(_ =>
                {
                    BAStudents.DisableIMGUI = true;
                    Current.Game.tickManager.CurTimeSpeed = TimeSpeed.Paused;
                });
            padCanvas.gameObject.OnDisableAsObservable()
                .Subscribe(_ =>
                {
                    BAStudents.DisableIMGUI = false;
                    Current.Game.tickManager.CurTimeSpeed = TimeSpeed.Normal;
                });

            padAccessor.HomeButton.OnClickAsObservable()
                .Subscribe(_ => padCanvas.gameObject.SetActive(false));
        }

        private IEnumerator BAWindowCoroutine()
        {
            while (bIsOpen)
            {
                yield return null;
            }
        }
    }
}