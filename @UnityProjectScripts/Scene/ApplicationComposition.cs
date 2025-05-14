using UnityEngine;

namespace BA
{
    public class ApplicationComposition : MonoBehaviour
    {
        [SerializeField] private ContentsAccessor _contentsAccessor;

        private void Start()
        {
            _contentsAccessor.ShinbiLiberationAnimation.Initialize(
                _contentsAccessor.FullshotRender,
                _contentsAccessor.OverlayCanvas);
            _contentsAccessor.ShinbiLiberationAnimation.gameObject.SetActive(false);
        }
    }
}