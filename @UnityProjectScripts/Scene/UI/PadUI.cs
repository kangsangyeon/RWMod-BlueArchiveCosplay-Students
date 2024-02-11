using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace @UnityProjectScripts
{
    public class PadUI : MonoBehaviour
    {
        [SerializeField] private PadAccessor accessor;
        private Sequence aronaPopupSequence;
        private Tweener homeButtonTweener;

        private void Start()
        {
            accessor.GachaButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.MissionButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.ShopButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.StudentListButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.AronaButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.SenseiButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);
            accessor.UpdateButton.OnClickAsObservable()
                .Subscribe(_ => ShowAronaPopup())
                .AddTo(gameObject);


            StartRotateHomeButton(30f);
            accessor.HomeButton.OnPointerEnterAsObservable()
                .Subscribe(_ => StartRotateHomeButton(120f))
                .AddTo(gameObject);
            accessor.HomeButton.OnPointerExitAsObservable()
                .Subscribe(_ => StartRotateHomeButton(30f))
                .AddTo(gameObject);

            accessor.AronaPopup.GetComponent<CanvasGroup>().alpha = 0f;
        }

        private void ShowAronaPopup()
        {
            var _canvasGroup = accessor.AronaPopup.GetComponent<CanvasGroup>();
            var _rectTransform = accessor.AronaPopup.GetComponent<RectTransform>();

            _canvasGroup.alpha = 1f;
            aronaPopupSequence?.Kill();

            aronaPopupSequence = DOTween.Sequence();
            aronaPopupSequence
                .Append(
                    _rectTransform.DOShakeAnchorPos(
                        0.3f, Vector3.right * 3f, 15, 0, false, false))
                .AppendInterval(1)
                .Append(DOTween.To(() => _canvasGroup.alpha, _v => _canvasGroup.alpha = _v, 0, 0.3f));
        }

        private void StartRotateHomeButton(float _rotationPerSec)
        {
            homeButtonTweener?.Kill();

            homeButtonTweener = accessor.HomeButton_Border.transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, 360),
                    360 / _rotationPerSec)
                .SetRelative()
                .SetLoops(-1)
                .SetEase(Ease.Linear);
        }
    }
}