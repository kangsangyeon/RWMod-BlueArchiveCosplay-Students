using Infrastructure.MvpFramework.Mono;
using UI.Events;
using UI.Presenters.MainPage;
using UI.Views.MainPage;
using UI.Views.Pad;
using UniRx;
using UnityEngine;

namespace UI.Presenters.Pad
{
    public class PadPresenter : MonoPresenter<PadView, PadViewState>
    {
        private MainPagePresenter _mainPagePresenter;

        public PadPresenter(PadView view) : base(view)
        {
        }

        protected override void OnInitialize(PadView view, PadViewState state)
        {
            state.HomeButtonClicked.Subscribe(_ => Debug.Log("패드 홈 버튼 클릭함."))
                .AddTo(view.gameObject);

            MessageBroker.Default.Receive<ShowMainPageEvent>()
                .Subscribe(_ => SwitchPage<MainPageView>(state))
                .AddTo(view.gameObject);

            SwitchPage<MainPageView>(state);
        }

        private void SwitchPage<T>(PadViewState state)
        {
            DisableAllPages(state);

            var type = typeof(T);
            if (type == typeof(MainPageView))
                state.MainPageViewState.Active.Value = true;
            // todo: 다른 page 활성화
        }

        private void DisableAllPages(PadViewState state)
        {
            state.MainPageViewState.Active.Value = false;
            // todo: 다른 page들 비활성화
        }
    }
}