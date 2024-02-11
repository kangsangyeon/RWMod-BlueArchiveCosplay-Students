using UnityEngine;
using UnityEngine.Video;

public class PlayVideo : MonoBehaviour
{
    public static System.Action<PlayVideo> onPlayStart;
    public static System.Action<PlayVideo> onPlayEnd;

    private GameObject videoUiPanel;
    private VideoPlayer videoPlayer;

    private void Start()
    {
        videoUiPanel = GameObject.Find("VideoPanel");
        videoPlayer = GetComponentInChildren<VideoPlayer>();

        videoUiPanel.gameObject.SetActive(true);
        videoPlayer.Play();
        onPlayStart?.Invoke(this);

        videoPlayer.loopPointReached += source =>
        {
            videoUiPanel.gameObject.SetActive(false);
            videoPlayer.targetTexture.Release();

            onPlayEnd?.Invoke(this);
            Destroy(gameObject);
        };
    }
}