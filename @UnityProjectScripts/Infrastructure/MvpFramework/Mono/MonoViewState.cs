using System;

namespace Infrastructure.MvpFramework.Mono
{
    public abstract class MonoViewState : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MonoViewState));
            OnDispose();
            _isDisposed = true;
        }

        protected abstract void OnDispose();
    }
}