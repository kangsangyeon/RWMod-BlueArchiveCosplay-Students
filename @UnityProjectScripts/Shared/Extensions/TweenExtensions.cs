using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Shared.Extensions
{
    public static class TweenExtensions
    {
        public static Tween AsReusable<T>(
            this T tween,
            GameObject gameObject,
            string id) where T : Tween
        {
            bool isIdValid = !string.IsNullOrEmpty(id);
            if (isIdValid)
                DOTween.Kill(gameObject, id);
            tween.SetAutoKill(false);
            tween.Pause();
            tween.SetTarget(gameObject);
            if (isIdValid)
                tween.SetId(id);

            gameObject.OnDestroyAsObservable().Subscribe(_ => tween.Kill());
            return tween;
        }

        public static T RewindAndPlay<T>(this T tween)
            where T : Tween
        {
            tween.Rewind();
            return tween.Play();
        }
    }
}