using DG.Tweening;
using UnityEngine;

public class ExSkillInfoUI : MonoBehaviour
{
    public ExSkillInfoAccessor Accessor;
    private bool _checkScroll;

    public void UpdateUI(StudentData studentData, SkillData skillData, SkillLevelData skillLevelData,
        bool isMaxLevel, bool isUnlocked)
    {
        Accessor.SkillNameText.text = skillData.Name;
        Accessor.SkillDescriptionText.text = skillLevelData.Description;
        Accessor.Badge.gameObject.SetActive(isMaxLevel);
        Accessor.MaxLevelImage.gameObject.SetActive(isMaxLevel);
        Accessor.LevelText.gameObject.SetActive(!isMaxLevel);
        Accessor.LevelText.text = $"Lv. {skillLevelData.Id.Level}";

        Sprite thumbnailSprite =
            GameResource.Load<Sprite>($"Skill/Icon", $"Skill_Icon_{skillData.IconName}");
        Color thumbnailBgColor = UIUtilProcedure.GetAttributeColor(studentData.Attribute);

        Accessor.Thumbnail.sprite = thumbnailSprite;
        Accessor.ThumbnailBackground.color = thumbnailBgColor;

        // 스킬 레벨의 별만큼 별을 보여줍니다.
        // 단, 기본 정보탭의 스킬 UI에는 별을 보여주지 않습니다.
        if (Accessor.Stars != null && Accessor.Stars.Length > 0)
        {
            for (int i = 0; i < skillLevelData.Star; ++i)
                Accessor.Stars[i].gameObject.SetActive(true);
            for (int i = skillLevelData.Star; i < 5; ++i)
                Accessor.Stars[i].gameObject.SetActive(false);
        }

        if (Accessor.LockOverlay != null)
            Accessor.LockOverlay.gameObject.SetActive(isUnlocked);
        if (Accessor.LockOverlayText != null)
            Accessor.LockOverlayText.text = $"Lv. {skillLevelData.Id.Level}";

        _checkScroll = false;
    }

    private void TryStartScroll()
    {
        if (_checkScroll)
            return;
        _checkScroll = true;

        Accessor.SkillDescriptionText.rectTransform.anchoredPosition = Vector2.zero;
        var color = Accessor.SkillDescriptionText.color;
        color.a = 1f;
        Accessor.SkillDescriptionText.color = color;

        // Description이 Description Mask의 범위를 벗어나는 경우,
        // 세로로 텍스트를 내리는 애니메이션을 재생합니다.
        DOTween.Kill(Accessor.SkillDescriptionText);
        float heightDelta =
            Accessor.SkillDescriptionText.preferredHeight -
            Accessor.SkillDescriptionMask.rectTransform.sizeDelta.y;
        if (heightDelta > 0)
        {
            // 대략 한 줄마다 스크롤 시간 3초 증가 (font size가 대략 한 줄 크기)
            float scrollDuration =
                (heightDelta / Accessor.SkillDescriptionText.fontSize) * 3f;
            DOTween.Sequence()
                .AppendInterval(1f)
                .Append(Accessor.SkillDescriptionText.rectTransform.DOAnchorPosY(heightDelta, scrollDuration))
                .Append(Accessor.SkillDescriptionText.DOFade(0f, 1f))
                .AppendCallback(() => Accessor.SkillDescriptionText.rectTransform.anchoredPosition = Vector2.zero)
                .Append(Accessor.SkillDescriptionText.DOFade(1f, 1f))
                .SetLoops(-1)
                .SetId(Accessor.SkillDescriptionText);
        }
    }

    private void OnRenderObject()
    {
        // 설명이 길다면 스크롤을 합니다.
        //   *게임 오브젝트가 생성된 프레임에는 레이아웃 계산이 이루어지지 않아
        //   height를 정확히 얻을 수 없기 때문에 한 프레임 이상 기다려야 합니다.
        //   따라서 OnRenderObject 이벤트에 이를 추가합니다.
        TryStartScroll();
    }
}