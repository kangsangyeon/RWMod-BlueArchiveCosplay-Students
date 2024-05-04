using UnityEngine;

namespace UnityProjectScripts
{
    public class ExSkillInfoUI : MonoBehaviour
    {
        public ExSkillInfoAccessor Accessor;

        public void UpdateUI(SkillData _skillData, SkillLevelData _skillLevelData, bool _isMaxLevel, bool _isUnlocked)
        {
            Accessor.SkillNameText.text = _skillData.Name;
            Accessor.SkillDescriptionText.text = _skillLevelData.Description;
            Accessor.Badge.gameObject.SetActive(_isMaxLevel);
            Accessor.Background.gameObject.SetActive(_isUnlocked);

            Sprite _thumbnailSprite;
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

            Accessor.Thumbnail.sprite = _thumbnailSprite;

            // Accessor.LevelImage.sprite = 

            // var _yellowStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "YellowStar");
            // Accessor.StarHolder.
        }
    }
}