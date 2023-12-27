using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using RimWorld;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
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

                // Unity Project Assembly를 불러옵니다.
                var _assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(x => x.FullName.Contains("BA"))
                    .ToList();
                foreach (var x in _assemblies)
                    Log.Message(x.FullName);

                Asset = AssetBundle.LoadFromFile($"{_currentMod.RootDir}/Contents/Assets/assetbundle00");

                if (Asset == null)
                    Log.Error("AssetBundleLoader.Asset is null");

                var _canvasPrefab = Asset.LoadAsset<GameObject>("UICanvas_StudentGacha");
                if (_canvasPrefab == null)
                    Log.Error("UICanvas_StudentGacha is null");

                var _uiCameraPrefab = Asset.LoadAsset<GameObject>("UICamera");
                if (_uiCameraPrefab == null)
                    Log.Error("UICamera is null");

                var _eventSystemPrefab = Asset.LoadAsset<GameObject>("EventSystem");
                if (_eventSystemPrefab == null)
                    Log.Error("EventSystem is null");

                var _videoPlayerGachaPrefab = Asset.LoadAsset<GameObject>("VideoPlayer_Gacha");
                if (_videoPlayerGachaPrefab == null)
                    Log.Error("VideoPlayer_Gacha Prefab is null");
                else
                    Log.Message("VideoPlayer_Gacha Prefab is not null");

                var _canvas = Object.Instantiate(_canvasPrefab).GetComponent<Canvas>();
                var _uiCamera = Object.Instantiate(_uiCameraPrefab).GetComponent<Camera>();
                var _eventSystem = Object.Instantiate(_eventSystemPrefab).GetComponent<EventSystem>();
                // var _videoPlayerGacha = Object.Instantiate(_videoPlayerGachaPrefab);
                // var _playVideo = _videoPlayerGacha.AddComponent<PlayVideo>();
                // if (_playVideo != null)
                // {
                //     Log.Message($"PlayVideo component added, (t:{_playVideo.GetType().Name})");
                // }
                //
                // var _components = _videoPlayerGacha.GetComponents<Behaviour>();
                // Debug.Log($"components count: {_components.Length}");
                // foreach (var _component in _components)
                // {
                //     if (_component != null)
                //         Debug.Log(_component.GetType().Name);
                //     else
                //         Debug.LogError("component is null!");
                // }

                _canvas.worldCamera = _uiCamera;

                canvasCam = _uiCamera;
                canvasCam.clearFlags = CameraClearFlags.Depth;
            }

            if (Time.time - lastClickedTime > 0.1f)
            {
                // 한 프레임에 이 콜백이 여러 번 호출되는 문제와
                // 너무 빠른 시간동안 클릭을 두 번 이상 하는 의도치 않은 입력을 무시합니다.
                ToggleShowWindow();

                lastClickedTime = Time.time;
            }

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