using System.Threading.Tasks;
using Infrastructure.MvpFramework.Mono;
using UI.Views.MainPage;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.Pad
{
    public class PadView : MonoView<PadViewState>
    {
        [SerializeField] private Button _homeButton;
        [SerializeField] private Image _homeButton_Border;
        [SerializeField] private Canvas _root;
        [SerializeField] private RawImage _screenOverlay_Transition;
        [SerializeField] private MainPageView _mainPageView;

        protected override Task OnInitialize(PadViewState state)
        {
            _homeButton.OnClickAsObservable()
                .Subscribe(_ => state.InvokeHomeButtonClicked())
                .AddTo(gameObject);

            return Task.WhenAll(
                _mainPageView.InitializeAsync(state.MainPageViewState)
            );
        }
    }

    public class PadViewState : MonoViewState
    {
        private readonly Subject<Unit> _homeButtonClicked = new();

        public IObservable<Unit> HomeButtonClicked => _homeButtonClicked;
        public MainPageViewState MainPageViewState { get; } = new();

        public void InvokeHomeButtonClicked() => _homeButtonClicked.OnNext(Unit.Default);

        protected override void OnDispose()
        {
            _homeButtonClicked.Dispose();
            MainPageViewState.Dispose();
        }
    }
}