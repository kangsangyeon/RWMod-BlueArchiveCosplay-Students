using System;
using System.Threading.Tasks;

namespace Infrastructure.MvpFramework.Mono
{
    public abstract class DocumentPresenter<TView, TState>
        where TView : MonoView<TState>
        where TState : MonoViewState, new()
    {
        private TView _view;
        private TState _state = new TState();
        private bool _isDisposed;
        private bool _isInitialized;

        protected DocumentPresenter(TView view)
        {
            _view = view;
        }

        public async Task InitializeAsync()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(DocumentPresenter<TView, TState>));
            if (_isInitialized)
                throw new InvalidOperationException($"{GetType().Name} is already initialized.");
            OnInitialize(_view, _state);
            await _view.InitializeAsync(_state);
            _isInitialized = true;
        }

        protected abstract void OnInitialize(TView view, TState state);
    }
}