using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.MvpFramework.Mono;
using UI.Events;
using BA;
using UniRx;

namespace BA
{
    public class MainPagePresenter : MonoPresenter<MainPageView, MainPageViewState>
    {
        public MainPagePresenter(MainPageView view, MainPageViewState state) : base(view, state)
        {
        }

        protected override Task OnInitialize(MainPageView view, MainPageViewState state)
        {
            state.StudentListButtonClicked.Subscribe(_ => StudentListButtonClicked())
                .AddTo(view.gameObject);

            // 임시적으로 학생 목록 버튼 제외한 버튼들을 누르면 안내 팝업을 띄움.
            state.GachaButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);
            state.MissionButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);
            state.ShopButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);
            state.AronaButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);
            state.SenseiButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);
            state.UpdateButtonClicked.Subscribe(_ => PlayAronaPopupTween(state))
                .AddTo(view.gameObject);

            return Task.CompletedTask;
        }

        private void StudentListButtonClicked()
        {
            // temp: 페이지 이동 구현 필요
            MessageBroker.Default.Publish(new ShowStudentListPageEvent());
        }

        private void PlayAronaPopupTween(MainPageViewState state)
        {
            state.GetShowAronaPopupTween().Play();
        }
    }
}