using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainScreenUI : MonoBehaviour
{
    public MainScreenAccessor accessor;
    private Sequence aronaPopupSequence;

    private void Start()
    {
        accessor.GachaButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);
        accessor.MissionButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);
        accessor.ShopButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);
        accessor.StudentListButton.OnClickAsObservable()
            .Subscribe(_ => ShowStudentInfoScreen())
            .AddTo(gameObject);
        accessor.AronaButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);
        accessor.SenseiButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);
        accessor.UpdateButton.OnClickAsObservable()
            .Subscribe(_ => ShowAronaPopup())
            .AddTo(gameObject);

        accessor.AronaPopup.GetComponent<CanvasGroup>().alpha = 0f;
    }

    private void ShowAronaPopup()
    {
        var _canvasGroup = accessor.AronaPopup.GetComponent<CanvasGroup>();
        var _rectTransform = accessor.AronaPopup.GetComponent<RectTransform>();

        _canvasGroup.alpha = 1f;
        aronaPopupSequence?.Kill();

        aronaPopupSequence = DOTween.Sequence();
        aronaPopupSequence
            .Append(
                _rectTransform.DOShakeAnchorPos(
                    0.3f, Vector3.right * 3f, 15, 0, false, false))
            .AppendInterval(1)
            .Append(DOTween.To(() => _canvasGroup.alpha, _v => _canvasGroup.alpha = _v, 0, 0.3f));
    }

    private void ShowStudentInfoScreen()
    {
        var _videoAsset = GameResource.Load<VideoClip>("Video", "Transition");
        var _raycaster = Contents.Instance.Accessor.PadCanvas.GetComponent<GraphicRaycaster>();
        _raycaster.enabled = false;
        _videoAsset.PlayVideoOntoRT(EVideoType.Transition, () => _raycaster.enabled = true)
            .AtTime(1f, () =>
            {
                Contents.Instance.Accessor.PadCanvas.StudentListScreen.gameObject.SetActive(true);
                accessor.gameObject.SetActive(false);
            });
    }
}