﻿using System;
using BA;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class ShinbiLiberationAnimation : MonoBehaviour
    {
        private static readonly int ShaderProp_Radius = Shader.PropertyToID("_Radius");
        private static readonly int ShaderProp_Color = Shader.PropertyToID("_Color");
        private static readonly int ShaderProp_Feather = Shader.PropertyToID("_Feather");

        [SerializeField] private Image _bg1;
        [SerializeField] private Image _bg2;
        [SerializeField] private Image _bgWhite;
        [SerializeField] private Image _whiteCircleTransition;
        [SerializeField] private RawImage _fullshot;
        [SerializeField] private RectTransform _maskLine;
        [SerializeField] private ParticleSystem _starParticles;
        [SerializeField] private Button _fullscreenButton;
        private RectTransform _maskLineRectTransform;
        private Sequence _sequence;
        private bool _initialized;

        private FullshotRenderAccessor _fullshotRenderAccessor;
        private RectTransform _parentRectTransform;
        private StudentData _studentData;
        private Vector2 _fullshotCamPos = Vector2.zero;
        private float _fullshotCamOrthoSize = 5;
        private Action _onEnd;

        public void Initialize(
            FullshotRenderAccessor fullshotRenderAccessor,
            RectTransform parentRectTransform)
        {
            if (_initialized)
                return;
            _initialized = true;

            _maskLineRectTransform = (RectTransform)_maskLine.transform;
            _fullshotRenderAccessor = fullshotRenderAccessor;
            _parentRectTransform = parentRectTransform;

            _whiteCircleTransition.material = new Material(_whiteCircleTransition.material);
            _bgWhite.material = new Material(_bgWhite.material);

            _sequence = DOTween.Sequence()
                .Pause()
                .AppendCallback(OnStart)
                .Append(_bg1.DOFade(1f, 2f))
                .Join(DOTween.Sequence()
                    .AppendInterval(0.5f)
                    .AppendCallback(_starParticles.Play))
                .Append(_whiteCircleTransition.DOColor(Color.white, 2f))
                .AppendCallback(() =>
                {
                    _bgWhite.enabled = true;
                    _fullshot.enabled = true;
                    _bg2.color = Color.white;

                    // fullshot camera 설정
                    _fullshotRenderAccessor.Camera.transform.localPosition =
                        new Vector3(_fullshotCamPos.x, _fullshotCamPos.y, _fullshotRenderAccessor.Camera.transform.localPosition.z);
                    _fullshotRenderAccessor.Camera.orthographicSize = _fullshotCamOrthoSize;
                })
                .Join(_whiteCircleTransition.material.DOFloat(0.25f, ShaderProp_Radius, 2f))
                .Join(DOTween.Sequence()
                    .AppendInterval(1f)
                    .Append(_whiteCircleTransition.DOColor(new Color(1, 1, 1, 0f), 2f)))
                .Append(_maskLine.DOAnchorPosX(0f, .25f).SetEase(Ease.OutQuad))
                .SetAutoKill(false)
                .SetLink(gameObject);

            _fullscreenButton.onClick.AddListener(() =>
            {
                if (_sequence.IsComplete())
                    End();
            });

            OnStart();
        }

        public void Setup(int studentId)
        {
            _studentData = GameResource.StudentTable[studentId];
            _fullshotCamPos = _studentData.ShinbiCamPos;
            _fullshotCamOrthoSize = _studentData.ShinbiCamOrthoSize;
        }

        public void Play([CanBeNull] Action onComplete = null, [CanBeNull] Action onEnd = null)
        {
            gameObject.SetActive(true);
            _sequence.Restart();
            if (onComplete != null)
                _sequence.onComplete = onComplete.Invoke;
            _onEnd = onEnd;
        }

        public void End()
        {
            if (_sequence != null)
            {
                _sequence.Complete(true);
                _sequence.onComplete = null;
            }

            _onEnd?.Invoke();
            gameObject.SetActive(false);
        }

        private void OnStart()
        {
            _bg1.color = new Color(1, 1, 1, 0f);
            _bg2.color = new Color(1, 1, 1, 0f);
            _bgWhite.enabled = false;
            _whiteCircleTransition.material.SetFloat(ShaderProp_Radius, 0.6f);
            _whiteCircleTransition.color = new Color(1, 1, 1, 0f);
            float offsetX = _parentRectTransform.rect.width * 0.5f + _maskLineRectTransform.rect.width * 0.5f;
            _maskLine.anchoredPosition = new Vector2(-offsetX, _maskLine.anchoredPosition.y);
            _fullshot.enabled = false;
            _starParticles.Stop();
        }
    }
}