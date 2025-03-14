using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace BA
{
    public class MainScreenUI : MonoBehaviour
    {
        public MainScreenAccessor Accessor;
        private Sequence _aronaPopupSequence;

        private void Start()
        {
            Accessor.GachaButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            Accessor.MissionButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            Accessor.ShopButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            Accessor.StudentListButton.OnClickAsObservable()
                .Subscribe(_ => ShowStudentInfoScreen())
                .AddTo(gameObject);
            Accessor.AronaButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            Accessor.SenseiButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            Accessor.UpdateButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);

            Accessor.AronaPopup.GetComponent<CanvasGroup>().alpha = 0f;
        }

        private void ShowAronaPopup()
        {
            var canvasGroup = Accessor.AronaPopup.GetComponent<CanvasGroup>();
            var rectTransform = Accessor.AronaPopup.GetComponent<RectTransform>();

            canvasGroup.alpha = 1f;
            _aronaPopupSequence?.Kill();

            _aronaPopupSequence = DOTween.Sequence();
            _aronaPopupSequence
                .Append(
                    rectTransform.DOShakeAnchorPos(
                        0.3f, Vector3.right * 3f, 15, 0, false, false))
                .AppendInterval(1)
                .Append(DOTween.To(() => canvasGroup.alpha, v => canvasGroup.alpha = v, 0, 0.3f));
        }

        private void ShowStudentInfoScreen()
        {
            var videoAsset = GameResource.Load<VideoClip>("UI/Video", "Transition");
            var raycaster = Contents.Instance.Accessor.PadCanvas.GetComponent<GraphicRaycaster>();
            raycaster.enabled = false;
            videoAsset.PlayVideoOntoRT(EVideoType.Transition, () => raycaster.enabled = true)
                .AtTime(1f, () =>
                {
                    Contents.Instance.Accessor.PadCanvas.StudentListScreen.gameObject.SetActive(true);
                    Accessor.gameObject.SetActive(false);
                });
        }
    }
}