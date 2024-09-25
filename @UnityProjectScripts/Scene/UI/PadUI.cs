using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class PadUI : MonoBehaviour
{
    [SerializeField] private PadAccessor accessor;
    private Tweener homeButtonTweener;

    private void Start()
    {
        accessor.MainScreen.gameObject.SetActive(true);
        accessor.StudentListScreen.gameObject.SetActive(false);
        accessor.ScreenOverlay_Transition.gameObject.SetActive(false);

        StartRotateHomeButton(30f);
        accessor.HomeButton.OnPointerEnterAsObservable()
            .Subscribe(_ => StartRotateHomeButton(120f))
            .AddTo(gameObject);
        accessor.HomeButton.OnPointerExitAsObservable()
            .Subscribe(_ => StartRotateHomeButton(30f))
            .AddTo(gameObject);
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