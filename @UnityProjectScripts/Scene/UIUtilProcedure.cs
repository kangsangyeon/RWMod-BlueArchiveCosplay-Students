using System;
using System.Text;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public static class UIUtilProcedure
{
    public static Color GetAttributeColor(StudentAttribute _attribute)
    {
        Color _color = Color.white;

        if (_attribute == StudentAttribute.Attack)
            ColorUtility.TryParseHtmlString("#CC1A25", out _color);
        else if (_attribute == StudentAttribute.Defense)
            ColorUtility.TryParseHtmlString("#065BAB", out _color);
        else if (_attribute == StudentAttribute.Support)
            ColorUtility.TryParseHtmlString("#BD8801", out _color);
        else if (_attribute == StudentAttribute.Heal)
            ColorUtility.TryParseHtmlString("#88F284", out _color);

        return _color;
    }

    public static void Clear(this RenderTexture _rt)
    {
        var _rtOrigin = RenderTexture.active;
        RenderTexture.active = _rt;
        GL.Clear(true, true, Color.clear); // clear render texture
        RenderTexture.active = _rtOrigin;
    }

    public static VideoPlayer PlayVideoOntoRT(this VideoClip _clip, EVideoType _type, Action _onEnd = null)
    {
        RenderTexture _rt = null;
        RawImage _rawImage = null;
        switch (_type)
        {
            case EVideoType.Transition:
                _rt = GameResource.TransitionRT;
                _rawImage = Contents.Instance.Accessor.PadCanvas.ScreenOverlay_Transition;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(_type), _type, null);
        }

        var _playerGo = new GameObject("VideoPlayer_Transition");
        var _player = _playerGo.AddComponent<VideoPlayer>();
        _player.clip = _clip;
        _player.renderMode = VideoRenderMode.RenderTexture;
        _player.targetTexture = _rt;
        _player.Play();
        _rawImage.gameObject.SetActive(true);
        _player.loopPointReached += source =>
        {
            _onEnd?.Invoke();
            _rt.Clear();
            UnityEngine.Object.Destroy(_playerGo);
            _rawImage.gameObject.SetActive(false);
        };
        return _player;
    }

    public static VideoPlayer AtTime(this VideoPlayer _player, float _time, Action _callback)
    {
        IDisposable _disposable = null;
        _disposable = _player.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (_player.time >= _time)
                {
                    _callback?.Invoke();
                    _disposable.Dispose();
                }
            });
        return _player;
    }

    public static Texture2D ToTexture2D(this RenderTexture _rt)
    {
        Texture2D tex = new Texture2D(_rt.width, _rt.height, TextureFormat.RGB24, false);
        var _oldRt = RenderTexture.active;
        RenderTexture.active = _rt;

        tex.ReadPixels(new Rect(0, 0, _rt.width, _rt.height), 0, 0);
        tex.Apply();

        RenderTexture.active = _oldRt;
        return tex;
    }

    public static string FormatWeaponPopupDescription(string _str)
    {
        var _sb = new StringBuilder(_str)
            .Replace("<h>", "<size=28>")
            .Replace("</h>", "</size>");
        return _sb.ToString();
    }
}