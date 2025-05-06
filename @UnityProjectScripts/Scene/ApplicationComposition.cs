using UnityEngine;

namespace BA
{
    public class ApplicationComposition : MonoBehaviour
    {
        private ContentsAccessor _contentsAccessor;
        private Canvas _canvas;

        private void Start()
        {
            _contentsAccessor.ShinbiLiberationAnimation.Initialize(
                _contentsAccessor.FullshotRender,
                _canvas);
        }
    }
}