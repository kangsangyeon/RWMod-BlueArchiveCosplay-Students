using System;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

namespace BA
{
    public class ShinbiUpConfirmPopupUI : MonoBehaviour
    {
        public ShinbiUpConfirmPopupAccessor Accessor;
        private CompositeDisposable _disposables;

        private void Awake()
        {
            Accessor.QuitButton.OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(gameObject);
            Accessor.CancelButton.OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false))
                .AddTo(gameObject);
        }

        public void UpdateUI(int eligmaCost, int eligmaBudget, int silverCost, int silverBudget, [CanBeNull] Action onConfirm)
        {
            _disposables?.Dispose();
            _disposables = new CompositeDisposable();

            Accessor.Eligma_CostText.text = $"x{eligmaCost}";
            Accessor.Eligma_BudgetText.text = $"x{eligmaBudget}";
            Accessor.Silver_CostText.text = $"x{silverCost}";
            Accessor.Silver_BudgetText.text = $"x{silverBudget}";

            Accessor.ConfirmButton.OnClickAsObservable()
                .Subscribe(_ => onConfirm?.Invoke())
                .AddTo(gameObject)
                .AddTo(_disposables);
        }
    }
}