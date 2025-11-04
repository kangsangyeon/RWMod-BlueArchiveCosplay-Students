using System.Threading.Tasks;
using Infrastructure.MvpFramework.Mono;
using UI.Events;
using UniRx;
using UnityEngine;

namespace BA
{
    public class PadPresenter : MonoPresenter<PadView, PadViewState>
    {
        [SerializeField] private MainPageView _mainPageView;

        private MainPagePresenter _mainPagePresenter;

        public PadPresenter(PadView view) : base(view)
        {
        }

        protected override Task OnInitialize(PadView view, PadViewState state)
        {
            // temp
            state.HomeButtonClicked.Subscribe(_ => Debug.Log("패드 홈 버튼 클릭함."))
                .AddTo(view.gameObject);

            MessageBroker.Default.Receive<ShowMainPageEvent>()
                .Subscribe(_ => SwitchPage<MainPageView>(state))
                .AddTo(view.gameObject);
            MessageBroker.Default.Receive<ShowStudentListPageEvent>()
                .Subscribe(_ => SwitchPage<ShowStudentListPageEvent>(state))
                .AddTo(view.gameObject);

            SwitchPage<MainPageView>(state);

            _mainPagePresenter = new MainPagePresenter(_mainPageView, state.MainPageViewState);

            return Task.WhenAll(
                _mainPagePresenter.InitializeAsync()
            );
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