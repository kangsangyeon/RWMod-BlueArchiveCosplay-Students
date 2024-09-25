using System.Collections;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.Video;

public class MainScreenUI : MonoBehaviour
{
    public MainScreenAccessor accessor;
    private PadAccessor padAccessor;
    private Sequence aronaPopupSequence;

    private void Start()
    {
        padAccessor = FindObjectOfType<PadAccessor>();

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
            .Subscribe(_ => StartCoroutine(ShowStudentInfoScreenCoroutine()))
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

    private IEnumerator ShowStudentInfoScreenCoroutine()
    {
        var _videoAsset = GameResource.Load<VideoClip>("Video", "Transition");
        var _rtAsset = GameResource.Load<RenderTexture>("RT", "TransitionRT");

        var _playerGo = new GameObject("VideoPlayer_Transition");
        var _player = _playerGo.AddComponent<VideoPlayer>();
        _player.clip = _videoAsset;
        _player.renderMode = VideoRenderMode.RenderTexture;
        _player.targetTexture = _rtAsset;
        _player.Play();
        _player.loopPointReached += source =>
        {
            // clear render texture
            RenderTexture.active = _rtAsset;
            GL.Clear(true, true, Color.clear);

            Destroy(_playerGo);
        };

        yield return new WaitForSeconds(1f);
        padAccessor.StudentListScreen.gameObject.SetActive(true);
        accessor.gameObject.SetActive(false);
    }
}