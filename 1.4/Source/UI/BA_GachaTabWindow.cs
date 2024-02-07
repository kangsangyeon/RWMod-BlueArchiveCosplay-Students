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
        private bool isOpen = false;
        private Coroutine coroutine;
        private AssetBundle Asset;
        private float lastClickedTime;

        public override void DoWindowContents(Rect inRect)
        {
            Log.Message("BA_GachaTabWindow::DoWindowContents()");

            if (isInitialized == false)
            {
                isInitialized = true;

                var _currentMod =
                    LoadedModManager.RunningMods.FirstOrDefault(x => x.PackageId == "bluearchive.students");
                Asset = AssetBundle.LoadFromFile($"{_currentMod.RootDir}/Contents/Assets/assetbundle00");

                var _eventSystemPrefab = Asset.LoadAsset<GameObject>("EventSystem");
                var _uiCameraPrefab = Asset.LoadAsset<GameObject>("UICamera");
                var _canvasPrefab = Asset.LoadAsset<GameObject>("UICanvas_StudentGacha");
                var _videoPlayerGachaPrefab = Asset.LoadAsset<GameObject>("VideoPlayer_Gacha");

                var _eventSystem = Object.Instantiate(_eventSystemPrefab).GetComponent<EventSystem>();
                var _uiCamera = Object.Instantiate(_uiCameraPrefab).GetComponent<Camera>();
                var _canvas = Object.Instantiate(_canvasPrefab).GetComponent<Canvas>();
                var _videoPlayerGacha = Object.Instantiate(_videoPlayerGachaPrefab);

                canvasCam = _uiCamera;
                canvasCam.clearFlags = CameraClearFlags.Depth;
                _canvas.worldCamera = _uiCamera;
            }

            Log.Message($"Rect size: {inRect.size}, center: {inRect.center}");
            this.Close();
        }

        private void ToggleShowWindow()
        {
            Log.Message("BA_GachaTabWindow::ToggleShowWindow()");

            isOpen = !isOpen;
        }

        private IEnumerator WindowCoroutine()
        {
            while (isOpen)
            {
                if (canvasCam != null)
                    canvasCam.Render();

                yield return null;
            }
        }
    }
}