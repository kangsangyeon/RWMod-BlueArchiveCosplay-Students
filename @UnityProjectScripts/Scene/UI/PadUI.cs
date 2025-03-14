using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace BA
{
    public class PadUI : MonoBehaviour
    {
        public PadAccessor Accessor;
        private Tweener _homeButtonTweener;

        private void Start()
        {
            Accessor.MainScreen.gameObject.SetActive(true);
            Accessor.StudentListScreen.gameObject.SetActive(false);
            Accessor.ScreenOverlay_Transition.gameObject.SetActive(false);

            StartRotateHomeButton(30f);
            Accessor.HomeButton.OnPointerEnterAsObservable()
                .Subscribe(_ => StartRotateHomeButton(120f))
                .AddTo(gameObject);
            Accessor.HomeButton.OnPointerExitAsObservable()
                .Subscribe(_ => StartRotateHomeButton(30f))
                .AddTo(gameObject);
        }

        private void StartRotateHomeButton(float rotationPerSec)
        {
            _homeButtonTweener?.Kill();

            _homeButtonTweener = Accessor.HomeButton_Border.transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, 360),
                    360 / rotationPerSec)
                .SetRelative()
                .SetLoops(-1)
                .SetEase(Ease.Linear);
        }
    }
}