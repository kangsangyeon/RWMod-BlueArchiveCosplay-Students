using System;
using System.Text;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public static class UIUtilProcedure
{
    public static Color GetAttributeColor(StudentAttribute attribute)
    {
        Color color = Color.white;

        if (attribute == StudentAttribute.Attack)
            ColorUtility.TryParseHtmlString("#B02730", out color);
        else if (attribute == StudentAttribute.Defense)
            ColorUtility.TryParseHtmlString("#065BAB", out color);
        else if (attribute == StudentAttribute.Support)
            ColorUtility.TryParseHtmlString("#BD8801", out color);
        else if (attribute == StudentAttribute.Heal)
            ColorUtility.TryParseHtmlString("#65C16F", out color);

        return color;
    }

    public static void Clear(this RenderTexture rt)
    {
        var rtOrigin = RenderTexture.active;
        RenderTexture.active = rt;
        GL.Clear(true, true, Color.clear); // clear render texture
        RenderTexture.active = rtOrigin;
    }

    public static VideoPlayer PlayVideoOntoRT(this VideoClip clip, EVideoType type, Action onEnd = null)
    {
        RenderTexture rt = null;
        RawImage rawImage = null;
        switch (type)
        {
            case EVideoType.Transition:
                rt = GameResource.TransitionRT;
                rawImage = Contents.Instance.Accessor.PadCanvas.ScreenOverlay_Transition;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        var playerGo = new GameObject("VideoPlayer_Transition");
        var player = playerGo.AddComponent<VideoPlayer>();
        player.clip = clip;
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = rt;
        player.Play();
        rawImage.gameObject.SetActive(true);
        player.loopPointReached += source =>
        {
            onEnd?.Invoke();
            rt.Clear();
            UnityEngine.Object.Destroy(playerGo);
            rawImage.gameObject.SetActive(false);
        };
        return player;
    }

    public static VideoPlayer AtTime(this VideoPlayer player, float time, Action callback)
    {
        IDisposable disposable = null;
        disposable = player.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (player.time >= time)
                {
                    callback?.Invoke();
                    disposable.Dispose();
                }
            });
        return player;
    }

    public static Texture2D ToTexture2D(this RenderTexture rt)
    {
        Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        var oldRt = RenderTexture.active;
        RenderTexture.active = rt;

        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = oldRt;
        return tex;
    }

    public static string FormatWeaponPopupDescription(string str)
    {
        var sb = new StringBuilder(str)
            .Replace("<h>", "<size=28>")
            .Replace("</h>", "</size>");
        return sb.ToString();
    }
}