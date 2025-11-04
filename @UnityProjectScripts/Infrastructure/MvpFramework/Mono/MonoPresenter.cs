using System;
using System.Threading.Tasks;

namespace Infrastructure.MvpFramework.Mono
{
    public interface IMonoPresenter
    {
        Task InitializeAsync();
    }

    public abstract class MonoPresenter<TView, TState> : IMonoPresenter
        where TView : MonoView<TState>
        where TState : MonoViewState, new()
    {
        private TView _view;
        private TState _state;
        private bool _isDisposed;
        private bool _isInitialized;

        protected MonoPresenter(TView view)
        {
            _view = view;
            _state = new TState();
        }

        protected MonoPresenter(TView view, TState state)
        {
            _view = view;
            _state = state;
        }

        public async Task InitializeAsync()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MonoPresenter<TView, TState>));
            if (_isInitialized)
                throw new InvalidOperationException($"{GetType().Name} is already initialized.");
            await _view.InitializeAsync(_state);
            await OnInitialize(_view, _state);
            _isInitialized = true;
        }

        protected abstract Task OnInitialize(TView view, TState state);
    }
}