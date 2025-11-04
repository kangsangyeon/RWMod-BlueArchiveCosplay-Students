using UnityEngine;

namespace BA
{
    public class ApplicationComposition : MonoBehaviour
    {
        [SerializeField] private ContentsAccessor _contentsAccessor;
        [SerializeField] private PadView _padView;

        private void Start()
        {
            var padPresenter = new PadPresenter(_padView);

            _contentsAccessor.PadCanvas.ShinbiLiberationAnimation.Initialize(
                _contentsAccessor.FullshotRender,
                (RectTransform)_contentsAccessor.PadCanvas.ScreenContainer.transform);

            // temp: 임시적으로 이곳에서 초기 page 활성화를 제어함.
            _contentsAccessor.PadCanvas.MainScreen.gameObject.SetActive(true);
            _contentsAccessor.PadCanvas.StudentListScreen.gameObject.SetActive(false);
            _contentsAccessor.PadCanvas.StudentInfoScreen.gameObject.SetActive(false);
            _contentsAccessor.PadCanvas.ShinbiLiberationAnimation.gameObject.SetActive(false);

            _ = padPresenter.InitializeAsync();
        }
    }
}