using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public RectTransform videoUiPanel;
    public VideoPlayer videoPlayer;
    public event System.Action onPlayStart;
    public event System.Action onPlayEnd;

    private void Start()
    {
        if (videoUiPanel == null)
            videoUiPanel = GameObject.Find("VideoPanel").GetComponent<RectTransform>();

        videoUiPanel.gameObject.SetActive(true);
        videoPlayer.Play();
        onPlayStart?.Invoke();

        videoPlayer.loopPointReached += source =>
        {
            onPlayEnd?.Invoke();
            Debug.Log("video end");
            videoUiPanel.gameObject.SetActive(false);
            videoPlayer.targetTexture.Release();
            Destroy(gameObject);
        };
    }
}