using UnityEngine;

namespace BA
{
    public class ApplicationComposition : MonoBehaviour
    {
        [SerializeField] private ContentsAccessor _contentsAccessor;

        private void Start()
        {
            _contentsAccessor.PadCanvas.ShinbiLiberationAnimation.Initialize(
                _contentsAccessor.FullshotRender,
                (RectTransform)_contentsAccessor.PadCanvas.ScreenContainer.transform);
            _contentsAccessor.PadCanvas.ShinbiLiberationAnimation.gameObject.SetActive(false);
        }
    }
}