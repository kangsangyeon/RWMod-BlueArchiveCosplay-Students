using System.Collections;
using System.Linq;
using RimWorld;
using UnityEngine;
using UnityEngine.EventSystems;
using Verse;
using Object = UnityEngine.Object;

namespace BlueArchiveStudents.UI
{
    public class BA_GachaTabWindow : MainTabWindow
    {
        public override Vector2 InitialSize => new Vector2(0.0f, 0.0f);

        private bool isInitialized = false;
        private Camera canvasCam;
        private Coroutine coroutine;
        private AssetBundle Asset;
        private float lastClickedTime;

        public override void PreOpen()
        {
            base.PreOpen();

            if (isInitialized == false)
            {
                isInitialized = true;

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
                var _padCanvas = Object.Instantiate(_padCanvasPrefab).GetComponent<Canvas>();

                canvasCam = _uiCamera;
                canvasCam.clearFlags = CameraClearFlags.Depth;
                // _canvas.worldCamera = _uiCamera;
                // _padCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                _padCanvas.worldCamera = _uiCamera;
            }
        }

        public override void DoWindowContents(Rect inRect)
        {
        }

        private IEnumerator WindowCoroutine()
        {
            while (IsOpen)
            {
                if (canvasCam != null)
                    canvasCam.Render();

                yield return null;
            }
        }
    }
}