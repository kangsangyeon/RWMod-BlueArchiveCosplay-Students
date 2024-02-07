using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public static System.Action<PlayVideo> onPlayStart;
    public static System.Action<PlayVideo> onPlayEnd;

    private CanvasGroup videoUiPanel;
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoUiPanel = GameObject.Find("VideoPanel").GetComponent<CanvasGroup>();
        videoPlayer = GetComponentInChildren<VideoPlayer>();

        videoUiPanel.alpha = 1f;
        videoPlayer.Play();
        onPlayStart?.Invoke(this);

        videoPlayer.loopPointReached += source =>
        {
            videoUiPanel.alpha = 0f;
            videoPlayer.targetTexture.Release();

            onPlayEnd?.Invoke(this);
            Destroy(gameObject);
        };
    }
}