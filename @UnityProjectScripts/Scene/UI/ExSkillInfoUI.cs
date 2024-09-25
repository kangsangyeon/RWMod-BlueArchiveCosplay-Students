using DG.Tweening;
using Unity.Linq;
using UnityEngine;

public class ExSkillInfoUI : MonoBehaviour
{
    public ExSkillInfoAccessor Accessor;
    private bool checkScroll;
    private Sequence descriptionTextSequence;

    public void UpdateUI(StudentData _studentData, SkillData _skillData, SkillLevelData _skillLevelData,
        bool _isMaxLevel, bool _isUnlocked)
    {
        Accessor.SkillNameText.text = _skillData.Name;
        Accessor.SkillDescriptionText.text = _skillLevelData.Description;
        Accessor.Badge.gameObject.SetActive(_isMaxLevel);
        Accessor.MaxLevelImage.gameObject.SetActive(_isMaxLevel);
        Accessor.LevelText.gameObject.SetActive(!_isMaxLevel);
        Accessor.LevelText.text = $"Lv. {_skillLevelData.Id.Level}";

        Sprite _thumbnailSprite =
            GameResource.Load<Sprite>($"Skill/Icon", $"Skill_Icon_{_skillData.IconName}");
        Color _thumbnailBgColor = UIUtilProcedure.GetAttributeColor(_studentData.Attribute);

        Accessor.Thumbnail.sprite = _thumbnailSprite;
        Accessor.ThumbnailBackground.color = _thumbnailBgColor;

        // 스킬 레벨의 별만큼 별을 보여줍니다.
        // 단, 기본 정보탭의 스킬 UI에는 별을 보여주지 않습니다.
        if (Accessor.StarHolder != null)
        {
            var _yellowStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "YellowStar");
            Accessor.StarHolder.Children().Destroy();
            for (int i = 0; i < _skillLevelData.Star; ++i)
                Accessor.StarHolder.Add(_yellowStarPrefab);
        }

        if (Accessor.LockOverlay != null)
            Accessor.LockOverlay.gameObject.SetActive(_isUnlocked);
        if (Accessor.LockOverlayText != null)
            Accessor.LockOverlayText.text = $"Lv. {_skillLevelData.Id.Level}";

        checkScroll = false;
    }

    private void TryStartScroll()
    {
        if (checkScroll)
            return;
        checkScroll = true;

        Accessor.SkillDescriptionText.rectTransform.anchoredPosition = Vector2.zero;
        var _color = Accessor.SkillDescriptionText.color;
        _color.a = 1f;
        Accessor.SkillDescriptionText.color = _color;

        // Description이 Description Mask의 범위를 벗어나는 경우,
        // 세로로 텍스트를 내리는 애니메이션을 재생합니다.
        descriptionTextSequence?.Kill();
        float _heightDelta =
            Accessor.SkillDescriptionText.preferredHeight -
            Accessor.SkillDescriptionMask.rectTransform.sizeDelta.y;
        if (_heightDelta > 0)
        {
            // 대략 한 줄마다 스크롤 시간 3초 증가 (font size가 대략 한 줄 크기)
            float _scrollDuration =
                (_heightDelta / Accessor.SkillDescriptionText.fontSize) * 3f;
            descriptionTextSequence = DOTween.Sequence();
            descriptionTextSequence
                .AppendInterval(1f)
                .Append(Accessor.SkillDescriptionText.rectTransform.DOAnchorPosY(_heightDelta, _scrollDuration))
                .Append(Accessor.SkillDescriptionText.DOFade(0f, 1f))
                .AppendCallback(() => Accessor.SkillDescriptionText.rectTransform.anchoredPosition = Vector2.zero)
                .Append(Accessor.SkillDescriptionText.DOFade(1f, 1f))
                .SetLoops(-1);
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