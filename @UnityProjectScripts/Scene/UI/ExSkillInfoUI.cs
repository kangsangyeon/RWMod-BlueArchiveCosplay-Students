using Unity.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class ExSkillInfoUI : MonoBehaviour
    {
        public ExSkillInfoAccessor Accessor;

        public void UpdateUI(StudentData _studentData, SkillData _skillData, SkillLevelData _skillLevelData,
            bool _isMaxLevel, bool _isUnlocked)
        {
            Accessor.SkillNameText.text = _skillData.Name;
            Accessor.SkillDescriptionText.text = _skillLevelData.Description;
            Accessor.Badge.gameObject.SetActive(_isMaxLevel);
            Accessor.LevelImage.gameObject.SetActive(_isMaxLevel);
            Accessor.Background.gameObject.SetActive(_isUnlocked);

            Sprite _thumbnailSprite;
            Color _thumbnailBgColor = Color.white;

            if (string.IsNullOrEmpty(_skillData.OverrideCommonIconName))
            {
                _thumbnailSprite =
                    GameResource.Load<Sprite>($"Skill/{_skillData.Id}", $"Skill_Icon_{_skillData.Id}");
            }
            else
            {
                _thumbnailSprite =
                    GameResource.Load<Sprite>("Skill/Common", $"Skill_Icon_Common_{_skillData.OverrideCommonIconName}");
            }

            if (_studentData.Attribute == StudentAttribute.Attack)
                ColorUtility.TryParseHtmlString("#CC1A25", out _thumbnailBgColor);
            else if (_studentData.Attribute == StudentAttribute.Defense)
                ColorUtility.TryParseHtmlString("#065BAB", out _thumbnailBgColor);
            else if (_studentData.Attribute == StudentAttribute.Support)
                ColorUtility.TryParseHtmlString("#BD8801", out _thumbnailBgColor);
            else if (_studentData.Attribute == StudentAttribute.Heal)
                ColorUtility.TryParseHtmlString("#88F284", out _thumbnailBgColor);

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
        }
    }
}