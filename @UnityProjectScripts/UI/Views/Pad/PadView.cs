using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.MvpFramework.Mono;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace BA
{
    public class PadView : MonoView<PadViewState>
    {
        private const string RotateTweenId = "Rotate";

        [SerializeField] private Button _homeButton;
        [SerializeField] private Image _homeButton_Border;
        [SerializeField] private RawImage _screenOverlay_Transition;
        [SerializeField] private MainPageView _mainPageView;

        protected override Task OnInitialize(PadViewState state)
        {
            _homeButton.OnClickAsObservable()
                .Subscribe(_ => state.InvokeHomeButtonClicked())
                .AddTo(gameObject);

            state.HomeButtonBorderRotationPerSec
                .Subscribe(x => PlayRotateHomeButtonTween(x))
                .AddTo(gameObject);

            return Task.WhenAll(
                _mainPageView.InitializeAsync(state.MainPageViewState)
            );
        }

        private Tween PlayRotateHomeButtonTween(float rotationPerSec)
        {
            DOTween.Kill(gameObject, RotateTweenId);
            return CreateRotateHomeButtonTween(rotationPerSec)
                .SetTarget(gameObject)
                .SetId(RotateTweenId);
        }

        private Tween CreateRotateHomeButtonTween(float rotationPerSec)
        {
            return _homeButton_Border.transform
                .DOLocalRotate(
                    new Vector3(0f, 0f, 360),
                    360 / rotationPerSec)
                .SetRelative()
                .SetLoops(-1)
                .SetEase(Ease.Linear);
        }
    }

    public class PadViewState : MonoViewState
    {
        private readonly Subject<Unit> _homeButtonClicked = new();

        public ReactiveProperty<float> HomeButtonBorderRotationPerSec { get; } = new();
        public MainPageViewState MainPageViewState { get; } = new();
        public IObservable<Unit> HomeButtonClicked => _homeButtonClicked;

        public void InvokeHomeButtonClicked() => _homeButtonClicked.OnNext(Unit.Default);

        protected override void OnDispose()
        {
            HomeButtonBorderRotationPerSec.Dispose();
            MainPageViewState.Dispose();
            _homeButtonClicked.Dispose();
        }
    }
}