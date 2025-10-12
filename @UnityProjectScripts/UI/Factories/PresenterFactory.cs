using System.Threading.Tasks;
using Infrastructure.MvpFramework.Mono;
using UI.Presenters.MainPage;
using UI.Presenters.Pad;
using UI.Views.MainPage;
using UI.Views.Pad;
using UnityEngine;

namespace UI.Factories
{
    public interface IPresenterFactory
    {
        IMonoPresenter CreatePadPresenter();

        IMonoPresenter CreateMainPagePresenter();
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

        public IMonoPresenter CreateMainPagePresenter()
        {
            var presenter = new MainPagePresenter(_mainPageView);
            return presenter;
        }
    }
}