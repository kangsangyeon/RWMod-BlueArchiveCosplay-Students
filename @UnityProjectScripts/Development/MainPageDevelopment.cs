using BA;
using UniRx;
using UnityEngine;

namespace Development
{
    public class MainPageDevelopment : MonoBehaviour
    {
        [SerializeField] private MainPageView _view;
        private MainPageViewState _viewState = new();

        private void Start()
        {
            _viewState.Active.Value = true;
            _viewState.GachaButtonClicked.Subscribe(_ => Debug.Log("GachaButtonClicked"))
                .AddTo(gameObject);
            _viewState.MissionButtonClicked.Subscribe(_ => Debug.Log("MissionButtonClicked"))
                .AddTo(gameObject);
            _viewState.ShopButtonClicked.Subscribe(_ => Debug.Log("ShopButtonClicked"))
                .AddTo(gameObject);
            _viewState.StudentListButtonClicked.Subscribe(_ => Debug.Log("StudentListButtonClicked"))
                .AddTo(gameObject);
            _viewState.AronaButtonClicked.Subscribe(_ => Debug.Log("AronaButtonClicked"))
                .AddTo(gameObject);
            _viewState.SenseiButtonClicked.Subscribe(_ => Debug.Log("SenseiButtonClicked"))
                .AddTo(gameObject);
            _viewState.UpdateButtonClicked.Subscribe(_ => Debug.Log("UpdateButtonClicked"))
                .AddTo(gameObject);
            _viewState.BlueStoneCount.Value = "1234567";
            _view.InitializeAsync(_viewState);
        }
    }
}