using BA;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Animation
{
    public class ShinbiLiberationAnimation : MonoBehaviour
    {
        private static readonly int ShaderProp_Radius = Shader.PropertyToID("_Radius");
        private static readonly int ShaderProp_Color = Shader.PropertyToID("_Color");
        private static readonly int ShaderProp_Feather = Shader.PropertyToID("_Feather");

        [SerializeField] private FullshotRenderAccessor _fullshotRenderAccessor;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Image _bg1;
        [SerializeField] private Image _bg2;
        [SerializeField] private Image _bgWhite;
        [SerializeField] private Image _whiteCircleTransition;
        [SerializeField] private RawImage _fullshot;
        [SerializeField] private RectTransform _maskLine;
        [SerializeField] private ParticleSystem _starParticles;
        private RectTransform _canvasRect;

        private void Start()
        {
            _whiteCircleTransition.material = new Material(_whiteCircleTransition.material);
            _bgWhite.material = new Material(_bgWhite.material);
            _canvasRect = (RectTransform)_canvas.transform;
            Play(1001); // test
        }

        public void Play(int studentId)
        {
            var studentData = GameResource.StudentTable[studentId];

            _fullshotRenderAccessor.Camera.transform.localPosition =
                new Vector3(studentData.ShinbiCamPos.x, studentData.ShinbiCamPos.y, _fullshotRenderAccessor.Camera.transform.localPosition.z);
            // _fullshotRenderAccessor.Camera.orthographicSize = studentData.ShinbiCamOrthoSize;
            _fullshotRenderAccessor.Camera.orthographicSize = 5; // test

            DOTween.Kill(gameObject);
            DOTween.Sequence()
                .SetId(gameObject)
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
                })
                .Join(_whiteCircleTransition.material.DOFloat(0.25f, ShaderProp_Radius, 2f))
                .Join(DOTween.Sequence()
                    .AppendInterval(1f)
                    .Append(_whiteCircleTransition.DOColor(new Color(1, 1, 1, 0f), 2f)))
                .Append(_maskLine.DOAnchorPosX(0f, .25f).SetEase(Ease.OutQuad))
                .Play();
        }

        private void OnStart()
        {
            _bg1.color = new Color(1, 1, 1, 0f);
            _bg2.color = new Color(1, 1, 1, 0f);
            _bgWhite.enabled = false;
            _whiteCircleTransition.material.SetFloat(ShaderProp_Radius, 0.6f);
            _whiteCircleTransition.color = new Color(1, 1, 1, 0f);
            _maskLine.anchoredPosition = new Vector2(-_canvasRect.sizeDelta.x, _maskLine.anchoredPosition.y);
            _fullshot.enabled = false;
            _starParticles.Stop();
        }
    }
}