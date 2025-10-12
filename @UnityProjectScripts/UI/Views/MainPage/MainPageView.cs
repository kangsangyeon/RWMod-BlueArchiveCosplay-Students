using System;
using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.MvpFramework.Mono;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views.MainPage
{
    public class MainPageView : MonoView<MainPageViewState>
    {
        [SerializeField] private Button _gachaButton;
        [SerializeField] private Button _missionButton;
        [SerializeField] private Button _shopButton;
        [SerializeField] private Button _studentListButton;
        [SerializeField] private Button _aronaButton;
        [SerializeField] private Button _senseiButton;
        [SerializeField] private Button _updateButton;
        [SerializeField] private Text _blueStoneText;
        [SerializeField] private GameObject _aronaPopup;
        [SerializeField] private CanvasGroup _aronaPopupCanvasGroup;
        [SerializeField] private RectTransform _aronaPopupRectTransform;
        private Tween _reusableAronaPopupTween;

        protected override Task OnInitialize(MainPageViewState state)
        {
            _gachaButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeGachaButtonClicked())
                .AddTo(gameObject);

            _missionButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeMissionButtonClicked())
                .AddTo(gameObject);

            _shopButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeShopButtonClicked())
                .AddTo(gameObject);

            _studentListButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeStudentListButtonClicked())
                .AddTo(gameObject);

            _aronaButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeAronaButtonClicked())
                .AddTo(gameObject);

            _senseiButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeSenseiButtonClicked())
                .AddTo(gameObject);

            _updateButton.OnPointerClickAsObservable()
                .Subscribe(_ => state.InvokeUpdateButtonClicked())
                .AddTo(gameObject);

            state.BlueStoneCount
                .Subscribe(count => _blueStoneText.text = count)
                .AddTo(gameObject);

            _reusableAronaPopupTween = CreateShowAronaPopupTween();
            state.SetPlayShowAronaPopupTween(() => _reusableAronaPopupTween);

            return Task.CompletedTask;
        }

        private Tween CreateShowAronaPopupTween()
        {
            return DOTween.Sequence()
                .AppendCallback(() => { _aronaPopupCanvasGroup.alpha = 1f; })
                .Append(_aronaPopupRectTransform.DOShakeAnchorPos(0.3f, Vector3.right * 3f, 15, 0, false, false))
                .AppendInterval(1)
                .Append(_aronaPopupCanvasGroup.DOFade(0f, 0.3f));
        }
    }

    public class MainPageViewState : MonoViewState
    {
        private readonly Subject<Unit> _gachaButtonClicked = new();
        private readonly Subject<Unit> _missionButtonClicked = new();
        private readonly Subject<Unit> _shopButtonClicked = new();
        private readonly Subject<Unit> _studentListButtonClicked = new();
        private readonly Subject<Unit> _aronaButtonClicked = new();
        private readonly Subject<Unit> _senseiButtonClicked = new();
        private readonly Subject<Unit> _updateButtonClicked = new();
        private Func<Tween> _playShowAronaPopupTween;

        public UniRx.IObservable<Unit> GachaButtonClicked => _gachaButtonClicked;
        public UniRx.IObservable<Unit> MissionButtonClicked => _missionButtonClicked;
        public UniRx.IObservable<Unit> ShopButtonClicked => _shopButtonClicked;
        public UniRx.IObservable<Unit> StudentListButtonClicked => _studentListButtonClicked;
        public UniRx.IObservable<Unit> AronaButtonClicked => _aronaButtonClicked;
        public UniRx.IObservable<Unit> SenseiButtonClicked => _senseiButtonClicked;
        public UniRx.IObservable<Unit> UpdateButtonClicked => _updateButtonClicked;
        public ReactiveProperty<bool> Active { get; } = new();
        public ReactiveProperty<string> BlueStoneCount { get; } = new();

        internal void SetPlayShowAronaPopupTween(Func<Tween> func) => _playShowAronaPopupTween = func;

        public void InvokeGachaButtonClicked() => _gachaButtonClicked.OnNext(Unit.Default);
        public void InvokeMissionButtonClicked() => _missionButtonClicked.OnNext(Unit.Default);
        public void InvokeShopButtonClicked() => _shopButtonClicked.OnNext(Unit.Default);
        public void InvokeStudentListButtonClicked() => _studentListButtonClicked.OnNext(Unit.Default);
        public void InvokeAronaButtonClicked() => _aronaButtonClicked.OnNext(Unit.Default);
        public void InvokeSenseiButtonClicked() => _senseiButtonClicked.OnNext(Unit.Default);
        public void InvokeUpdateButtonClicked() => _updateButtonClicked.OnNext(Unit.Default);

        public Tween GetShowAronaPopupTween()
        {
            return _playShowAronaPopupTween.Invoke();
        }

        protected override void OnDispose()
        {
            _gachaButtonClicked.Dispose();
            _missionButtonClicked.Dispose();
            _shopButtonClicked.Dispose();
            _studentListButtonClicked.Dispose();
            _aronaButtonClicked.Dispose();
            _senseiButtonClicked.Dispose();
            _updateButtonClicked.Dispose();
            Active.Dispose();
            BlueStoneCount.Dispose();
        }
    }
}