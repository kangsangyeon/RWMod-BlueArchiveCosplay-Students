using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPopupUI : MonoBehaviour
{
    public WeaponPopupAccessor Accessor;
    public bool ViewMaxLevelInfo;
    private bool _checkScroll;

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

    public void UpdateUI(WeaponData data, int level, int exp)
    {
        Accessor.WeaponNameMask_Text.text = data.Name;
        Accessor.TabContents_DetailTab_DescriptionMask_Text.text =
            UIUtilProcedure.FormatWeaponPopupDescription(data.Description);
        Accessor.Type.text = data.Type.ToString();
        Accessor.Level.text = $"<i>Lv.{level}</i>";
        Accessor.WeaponIcon.sprite =
            GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{data.Id}");
        for (int i = 0; i < data.Star; ++i)
            Accessor.Stars[i].gameObject.SetActive(true);
        for (int i = data.Star; i < 5; ++i)
            Accessor.Stars[i].gameObject.SetActive(false);

        // 텍스트 스크롤 여부를 다시 확인하고, 그 크기에 맞게 다시 스크롤해야 함.
        _checkScroll = false;
    }

    private void SetVisibleTab(int tabIdx)
    {
        ColorUtility.TryParseHtmlString("#F1FBFD", out var enableButtonColor);
        ColorUtility.TryParseHtmlString("#636293", out var enableTextColor);
        ColorUtility.TryParseHtmlString("#A9E0F4", out var disableButtonColor);
        ColorUtility.TryParseHtmlString("#636293", out var disableTextColor);

        Accessor.TabContents_BasicTab.gameObject.SetActive(false);
        Accessor.TabContents_DetailTab.gameObject.SetActive(false);
        Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = disableButtonColor;
        Accessor.TabButtonBox_BasicTabButton_Text.color = disableTextColor;
        Accessor.TabButtonBox_DetailTabButton.GetComponent<Image>().color = disableButtonColor;
        Accessor.TabButtonBox_DetailTabButton_Text.color = disableTextColor;

        switch (tabIdx)
        {
            case 0:
                Accessor.TabContents_BasicTab.gameObject.SetActive(true);
                Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = enableButtonColor;
                Accessor.TabButtonBox_BasicTabButton_Text.color = enableTextColor;
                break;
            case 1:
                Accessor.TabContents_DetailTab.gameObject.SetActive(true);
                Accessor.TabButtonBox_DetailTabButton.GetComponent<Image>().color = enableButtonColor;
                Accessor.TabButtonBox_DetailTabButton_Text.color = enableTextColor;
                break;
        }
    }

    private void TryStartScroll()
    {
        if (_checkScroll)
            return;
        _checkScroll = true;

        Accessor.WeaponNameMask_Text.rectTransform.anchoredPosition = Vector2.zero;
        var color = Accessor.WeaponNameMask_Text.color;
        color.a = 1f;
        Accessor.WeaponNameMask_Text.color = color;

        // Name이 Name Mask의 범위를 벗어나는 경우,
        // 세로로 텍스트를 내리는 애니메이션을 재생합니다.
        DOTween.Kill(Accessor.WeaponNameMask_Text);
        float widthDelta =
            Accessor.WeaponNameMask_Text.preferredWidth -
            Accessor.WeaponNameMask.rectTransform.sizeDelta.x;
        if (widthDelta > 0)
        {
            // 대략 한 글자마다 스크롤 시간 1초 증가 (font size가 대략 한 글자 크기)
            float scrollDuration =
                (widthDelta / Accessor.WeaponNameMask_Text.fontSize) * 1;
            DOTween.Sequence()
                .AppendInterval(1f)
                .Append(Accessor.WeaponNameMask_Text.rectTransform.DOAnchorPosX(widthDelta * -1, scrollDuration))
                .Append(Accessor.WeaponNameMask_Text.DOFade(0f, 1f))
                .AppendCallback(() => Accessor.WeaponNameMask_Text.rectTransform.anchoredPosition = Vector2.zero)
                .Append(Accessor.WeaponNameMask_Text.DOFade(1f, 1f))
                .SetLoops(-1)
                .SetId(Accessor.WeaponNameMask_Text);
        }
    }

    private void OnRenderObject()
    {
        TryStartScroll();
    }
}