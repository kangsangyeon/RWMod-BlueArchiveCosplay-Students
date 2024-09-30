using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPopupUI : MonoBehaviour
{
    public WeaponPopupAccessor Accessor;
    public bool ViewMaxLevelInfo;
    private bool checkScroll;

    private void Start()
    {
        Accessor.QuitButton.OnClickAsObservable()
            .Subscribe(_ => Accessor.gameObject.SetActive(false));
        Accessor.ConfirmButton.OnClickAsObservable()
            .Subscribe(_ => Accessor.gameObject.SetActive(false));
        Accessor.TabContents_BasicTab_ViewMaxLevelInfoCheckboxButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ViewMaxLevelInfo = !ViewMaxLevelInfo;
                Accessor.TabContents_BasicTab_ViewMaxLevelInfoCheckboxButton_Check.gameObject.SetActive(ViewMaxLevelInfo);
            });

        Accessor.TabContents_BasicTab_ViewMaxLevelInfoCheckboxButton_Check.gameObject.SetActive(ViewMaxLevelInfo);

        // 탭 전환 버튼 이벤트 액션을 설정합니다.
        Accessor.TabButtonBox_BasicTabButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(0));
        Accessor.TabButtonBox_DetailTabButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(1));

        // 탭의 기본 활성 상태를 설정합니다.
        SetVisibleTab(0);
    }

    public void UpdateUI(WeaponData _data, int _level, int _exp)
    {
        Accessor.WeaponNameMask_Text.text = _data.Name;
        Accessor.TabContents_DetailTab_DescriptionMask_Text.text =
            UIUtilProcedure.FormatWeaponPopupDescription(_data.Description);
        Accessor.Type.text = _data.Type.ToString();
        Accessor.Level.text = $"<i>Lv.{_level}</i>";
        Accessor.WeaponIcon.sprite =
            GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{_data.Id}");
        for (int i = 0; i < _data.Star; ++i)
            Accessor.Stars[i].gameObject.SetActive(true);
        for (int i = _data.Star; i < 5; ++i)
            Accessor.Stars[i].gameObject.SetActive(false);
    }

    private void SetVisibleTab(int _tabIdx)
    {
        ColorUtility.TryParseHtmlString("#F1FBFD", out var _enableButtonColor);
        ColorUtility.TryParseHtmlString("#636293", out var _enableTextColor);
        ColorUtility.TryParseHtmlString("#A9E0F4", out var _disableButtonColor);
        ColorUtility.TryParseHtmlString("#636293", out var _disableTextColor);

        Accessor.TabContents_BasicTab.gameObject.SetActive(false);
        Accessor.TabContents_DetailTab.gameObject.SetActive(false);
        Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = _disableButtonColor;
        Accessor.TabButtonBox_BasicTabButton_Text.color = _disableTextColor;
        Accessor.TabButtonBox_DetailTabButton.GetComponent<Image>().color = _disableButtonColor;
        Accessor.TabButtonBox_DetailTabButton_Text.color = _disableTextColor;

        switch (_tabIdx)
        {
            case 0:
                Accessor.TabContents_BasicTab.gameObject.SetActive(true);
                Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = _enableButtonColor;
                Accessor.TabButtonBox_BasicTabButton_Text.color = _enableTextColor;
                break;
            case 1:
                Accessor.TabContents_DetailTab.gameObject.SetActive(true);
                Accessor.TabButtonBox_DetailTabButton.GetComponent<Image>().color = _enableButtonColor;
                Accessor.TabButtonBox_DetailTabButton_Text.color = _enableTextColor;
                break;
        }
    }

    private void TryStartScroll()
    {
        if (checkScroll)
            return;
        checkScroll = true;

        Accessor.WeaponNameMask_Text.rectTransform.anchoredPosition = Vector2.zero;
        var _color = Accessor.WeaponNameMask_Text.color;
        _color.a = 1f;
        Accessor.WeaponNameMask_Text.color = _color;

        // Name이 Name Mask의 범위를 벗어나는 경우,
        // 세로로 텍스트를 내리는 애니메이션을 재생합니다.
        DOTween.Kill(Accessor.WeaponNameMask_Text);
        float _widthDelta =
            Accessor.WeaponNameMask_Text.preferredWidth -
            Accessor.WeaponNameMask.rectTransform.sizeDelta.x;
        if (_widthDelta > 0)
        {
            // 대략 한 글자마다 스크롤 시간 1초 증가 (font size가 대략 한 글자 크기)
            float _scrollDuration =
                (_widthDelta / Accessor.WeaponNameMask_Text.fontSize) * 1;
            DOTween.Sequence()
                .AppendInterval(1f)
                .Append(Accessor.WeaponNameMask_Text.rectTransform.DOAnchorPosX(_widthDelta * -1, _scrollDuration))
                .Append(Accessor.WeaponNameMask_Text.DOFade(0f, 1f))
                .AppendCallback(() => Accessor.WeaponNameMask_Text.rectTransform.anchoredPosition = Vector2.zero)
                .Append(Accessor.WeaponNameMask_Text.DOFade(1f, 1f))
                .SetLoops(-1);
        }
    }

    private void OnRenderObject()
    {
        TryStartScroll();
    }
}