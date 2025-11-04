using System.Threading.Tasks;
using Infrastructure.MvpFramework.Mono;
using BA;
using UnityEngine;

namespace BA
{
    public interface IPresenterFactory
    {
        IMonoPresenter CreatePadPresenter();

        IMonoPresenter CreateMainPagePresenter(MainPageViewState state);
    }

    public class PresenterFactory : IPresenterFactory
    {
        private readonly PadView _padView;
        private readonly MainPageView _mainPageView;

        public PresenterFactory(PadView padView, MainPageView mainPageView)
        {
            _padView = padView;
            _mainPageView = mainPageView;
        }

        public IMonoPresenter CreatePadPresenter()
        {
            var presenter = new PadPresenter(_padView);
            return presenter;
        }

        public IMonoPresenter CreateMainPagePresenter(MainPageViewState state)
        {
            var presenter = new MainPagePresenter(_mainPageView, state);
            return presenter;
        }
    }
}