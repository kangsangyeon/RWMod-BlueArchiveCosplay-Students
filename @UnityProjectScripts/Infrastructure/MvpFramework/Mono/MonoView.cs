using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Infrastructure.MvpFramework.Mono
{
    public abstract class MonoView<TState> : MonoBehaviour, IDisposable
        where TState : MonoViewState
    {
        private bool _isInitialized;
        private bool _isDisposed;

        public CompositeDisposable Disposables { get; } = new CompositeDisposable();

        public async Task InitializeAsync(TState state)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            await OnInitialize(state);
        }

        public void Dispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MonoView<TState>));
            _isDisposed = true;
            Disposables.Dispose();
            OnDispose();
        }

        protected abstract Task OnInitialize(TState state);

        protected virtual void OnDispose()
        {
        }
    }
}