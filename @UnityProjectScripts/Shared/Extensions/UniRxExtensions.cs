using System;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Shared.Extensions
{
    public static class UniRxExtensions
    {
        /*
         * DOTween
         */

        public static IDisposable SubscribeToTween<T>(
            this UniRx.IObservable<T> source,
            Func<T, Tween> selector,
            GameObject gameObject,
            string id = "")
        {
            return source.SubscribeWithState3(selector, gameObject, id, (x, s, go, i) =>
            {
                bool iValid = !string.IsNullOrEmpty(i);
                if (iValid)
                    DOTween.Kill(go, id);
                DOTween.Complete(go);
                var tween = s(x);
                tween.SetTarget(go);
                if (iValid)
                    tween.SetId(i);
            }).AddTo(gameObject);
        }

        public static IDisposable SubscribeToTweenReusable<T>(
            this UniRx.IObservable<T> source,
            Func<Tween> selector,
            GameObject gameObject,
            string id = "")
        {
            bool iValid = !string.IsNullOrEmpty(id);
            if (iValid)
                DOTween.Kill(gameObject, id);
            var tween = selector();
            tween.SetAutoKill(false);
            tween.Pause();
            tween.SetTarget(gameObject);
            if (iValid)
                tween.SetId(id);

            gameObject.OnDestroyAsObservable().Subscribe(_ => tween.Kill());
            return source.SubscribeWithState2(tween, gameObject, (x, t, go) =>
            {
                DOTween.Complete(go, true);
                t.Restart();
            }).AddTo(gameObject);
        }
    }
}