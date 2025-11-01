using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GachaVideoManager : MonoBehaviour
{
    [Header("normal")]
    public VideoClip aronaNormalClip;
    public VideoClip waitNormalClip;
    public VideoClip endNormalClip;

    [Header("rare")]
    public VideoClip[] aronaRareClip;
    public VideoClip[] waitRareClip;
    public VideoClip[] endRareClip;

    [Space]
    public VideoPlayer player;
    public GachaCardAnimation gca;
    public Button skipBtn;

    private Queue<VideoClip> clipQueue = new();
    private bool currentRenchaFlag;

    public enum VideoState { Start, Wait, End, Result }
    public VideoState currentState;

    public GameObject resultBackgroundPrefab;
    GameObject resultBackground;
    public Sprite resultBackgroundNormal;
    public Sprite resultBackgroundRare;

    bool includeRare;

    public void VideoStart(bool includeRare, bool isRencha)
    {
        clipQueue.Clear();
        currentRenchaFlag = isRencha;
        if (resultBackground == null)
            resultBackground = Instantiate(resultBackgroundPrefab);
        resultBackground.SetActive(false);

        this.includeRare = includeRare;

        if (includeRare)
        {
            int i = Random.Range(0, aronaRareClip.Length);
            clipQueue.Enqueue(aronaRareClip[i]);
            clipQueue.Enqueue(waitRareClip[i]);
            clipQueue.Enqueue(endRareClip[i]);
        }
        else
        {
            clipQueue.Enqueue(aronaNormalClip);
            clipQueue.Enqueue(waitNormalClip);
            clipQueue.Enqueue(endNormalClip);
        }

        ToStart();
    }

    #region Start
    private void ToStart()
    {
        currentState = VideoState.Start;

        player.loopPointReached -= OnStartClipEnd;
        player.loopPointReached -= OnWaitClipEnd;
        player.loopPointReached -= OnEndClipEnd;

        if (clipQueue.Count > 0)
            player.clip = clipQueue.Dequeue();
        player.time = 0;
        player.isLooping = false;
        player.Play();

        skipBtn.onClick.RemoveAllListeners();
        skipBtn.onClick.AddListener(SkipBtn);

        player.loopPointReached += OnStartClipEnd;
    }

    private void OnStartClipEnd(VideoPlayer vp)
    {
        player.loopPointReached -= OnStartClipEnd;
        ToWaitClip();
    }
    #endregion

    #region Wait
    private void ToWaitClip(bool skip = false)
    {
        currentState = VideoState.Wait;

        player.loopPointReached -= OnWaitClipEnd;
        player.loopPointReached += OnWaitClipEnd;

        if (clipQueue.Count > 0)
            if (!skip)
                player.clip = clipQueue.Dequeue();
            else
                player.clip = clipQueue.Peek();

        player.time = 0;
        player.isLooping = true;
        player.Play();
    }

    private void OnWaitClipEnd(VideoPlayer vp)
    {
        // wait는 루프이므로 특별히 처리 안함
    }
    #endregion

    #region End
    private void ToEndClip()
    {
        currentState = VideoState.End;

        player.loopPointReached -= OnEndClipEnd;

        if (clipQueue.Count > 0)
            player.clip = clipQueue.Dequeue();

        player.isLooping = false;
        player.time = 0;
        player.Play();

        skipBtn.onClick.RemoveAllListeners();
        player.loopPointReached += OnEndClipEnd;
    }

    private void OnEndClipEnd(VideoPlayer vp)
    {
        ToResult();
        player.loopPointReached -= OnEndClipEnd;
    }
    #endregion

    #region Result
    private void ToResult()
    {
        currentState = VideoState.Result;

        if (clipQueue.Count > 0)
        {
            player.clip = clipQueue.Dequeue();

            player.isLooping = true;
            player.time = 0;
            player.Play();
        }
        else
        {
            player.clip = null;
            resultBackground.SetActive(true);
            resultBackground.GetComponentInChildren<SpriteRenderer>().sprite =
                includeRare ? resultBackgroundRare : resultBackgroundNormal;
        }

        skipBtn.onClick.AddListener(SkipBtn);
        gca.PlayAnimation();
    }
    #endregion

    #region Skip
    public void SkipBtn()
    {
        player.loopPointReached -= OnStartClipEnd;
        player.loopPointReached -= OnWaitClipEnd;
        player.loopPointReached -= OnEndClipEnd;
        switch (currentState)
        {
            case VideoState.Start:
                ToWaitClip();
                break;

            case VideoState.Wait:
                ToEndClip();
                break;

            case VideoState.End:
                ToResult();
                break;

            case VideoState.Result:
                gca.SkipAnimation();
                break;
        }
    }
    #endregion
}
