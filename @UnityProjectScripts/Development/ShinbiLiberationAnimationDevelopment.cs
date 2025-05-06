using Animation;
using UnityEngine;

namespace Development
{
    public class ShinbiLiberationAnimationDevelopment : MonoBehaviour
    {
        [SerializeField] private FullshotRenderAccessor _fullshotRenderAccessor;
        [SerializeField] private ShinbiLiberationAnimation _shinbiLiberationAnimation;
        [SerializeField] private Canvas _canvas;

        private void Start()
        {
            _shinbiLiberationAnimation.Initialize(_fullshotRenderAccessor, _canvas);
            _shinbiLiberationAnimation.Play();
        }
    }
}