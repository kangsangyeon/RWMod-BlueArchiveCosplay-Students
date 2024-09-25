using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
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
        // clear render texture
        RenderTexture.active = _rt;
        GL.Clear(true, true, Color.clear);
    }

    public static VideoPlayer PlayVideoOntoRT(this VideoClip _clip, RenderTexture _rt, Action _onEnd = null)
    {
        var _playerGo = new GameObject("VideoPlayer_Transition");
        var _player = _playerGo.AddComponent<VideoPlayer>();
        _player.clip = _clip;
        _player.renderMode = VideoRenderMode.RenderTexture;
        _player.targetTexture = _rt;
        _player.Play();
        _player.loopPointReached += source =>
        {
            _onEnd?.Invoke();
            _rt.Clear();
            UnityEngine.Object.Destroy(_playerGo);
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
                    _callback.Invoke();
                    _disposable.Dispose();
                }
            });
        return _player;
    }
}