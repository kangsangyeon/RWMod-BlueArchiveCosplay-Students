using RimWorld;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Verse;

namespace BA
{
    public class ContentsWindow : MainTabWindow
    {
        public override Vector2 InitialSize => new(0.0f, 0.0f);

        private bool _setup;

        public override void PreOpen()
        {
            base.PreOpen();
            TrySetup();
            BAStudents.Contents.gameObject.SetActive(true);
        }

        public override void DoWindowContents(Rect inRect)
        {
            BAStudents.Contents.Accessor.UICamera.Render();
        }

        private void TrySetup()
        {
            if (_setup)
                return;
            _setup = true;

            // tab window 속성 설정
            layer = WindowLayer.Super;
            closeOnClickedOutside = false;
            closeOnAccept = false;
            closeOnCancel = false;

            // contents 오브젝트를 생성하고 이벤트 설정
            var contents = Object.Instantiate(BAStudents.ContentsPrefab).GetComponent<Contents>();
            contents.gameObject.SetActive(false);
            BAStudents.Contents = contents;

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

            contents.ObserveEveryValueChanged(_ => BAStudents.DisableAudio)
                .Subscribe(v =>
                {
                    Current.Root.soundRoot.sourcePool.sourcePoolCamera.cameraSourcesContainer.gameObject.SetActive(!v);
                    Current.Root.soundRoot.sourcePool.sourcePoolWorld.GetSourceWorld().gameObject.SetActive(!v);
                });

            contents.gameObject.OnDestroyAsObservable()
                .Subscribe(_ => _setup = false);
        }
    }
}