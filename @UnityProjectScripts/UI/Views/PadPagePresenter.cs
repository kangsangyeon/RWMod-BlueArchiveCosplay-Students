using Infrastructure.MvpFramework.Mono;
using UniRx;
using UniRx.Triggers;

namespace UI.Views
{
    public abstract class PadPagePresenter<TView, TState> : MonoPresenter<TView, TState>
        where TView : MonoView<TState>
        where TState : MonoViewState, new()
    {
        private const int DisposableCapacity = 20;
        private CompositeDisposable _disposables = new(DisposableCapacity);

        public PadPagePresenter(TView view) : base(view)
        {
        }

        protected override void OnInitialize(TView view, TState state)
        {
            view.gameObject.OnEnableAsObservable()
                .Subscribe(_ => OnEnable(_disposables));
            view.gameObject.OnDisableAsObservable()
                .Subscribe(_ => OnDisable());
        }

        protected virtual void OnEnable(CompositeDisposable disposables)
        {
        }

        protected virtual void OnDisable()
        {
            _disposables.Dispose();
            _disposables = new(DisposableCapacity);
        }
    }
}