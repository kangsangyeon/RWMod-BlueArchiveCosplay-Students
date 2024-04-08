using System.IO;
using DG.Tweening;
using UniRx;
using Unity.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class StudentInfoScreenUI : MonoBehaviour
    {
        public StudentInfoScreenAccessor Accessor;
        public ReactiveProperty<int> CharId = new ReactiveProperty<int>(0);

        private void Start()
        {
            CharId
                .Where(x => x > 0)
                .Subscribe(UpdateChar);

            CharId.Value = 1001;
        }

        private void UpdateChar(int _id)
        {
            var _data = GameResource.StudentTable[_id];

            Accessor.FullshotImage.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_{_data.Id}");
            Accessor.FullshotHaloImage.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_Halo_{_data.Id}");

            Accessor.NameText.text = _data.Name;

            var _yellowStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "YellowStar");
            Accessor.StarHolder.Children().Destroy();
            for (int i = 0; i < 5; ++i)
                Instantiate(_yellowStarPrefab, Accessor.StarHolder.transform);

            Accessor.AttributeText.text = _data.Attribute.ToStringKr();
            // todo: attribute icon 설정
            // Accessor.AttributeIcon.sprite 

            // todo: level text 설정
            // Accessor.LevelText.text = 
            // todo: exp fill 설정
            // Accessor.ExpBar.fillAmount

            Accessor.BasicTab_StatInfo_SDFullShot.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_SD_Fullshot_{_data.Id}");

            Accessor.BasicTab_StatInfo_StatPage1Value.text =
                $"{_data.DefaultShooting}\n{_data.DefaultMelee}\n{_data.DefaultConstruction}\n{_data.DefaultMining}\n{_data.DefaultCooking}\n{_data.DefaultPlants}";
            Accessor.BasicTab_StatInfo_StatPage2Value.text =
                $"{_data.DefaultAnimals}\n{_data.DefaultCrafting}\n{_data.DefaultArtistic}\n{_data.DefaultMedical}\n{_data.DefaultSocial}\n{_data.DefaultIntellectual}";

            var _skillData = GameResource.SkillTable[_data.SkillId];
            if (string.IsNullOrEmpty(_skillData.OverrideCommonIconName))
            {
                Accessor.BasicTab_ExSkillInfo_Thumbnail.sprite =
                    GameResource.Load<Sprite>($"Skill/{_skillData.Id}", $"Skill_Icon_{_skillData.Id}");
            }
            else
            {
                Accessor.BasicTab_ExSkillInfo_Thumbnail.sprite =
                    GameResource.Load<Sprite>("Skill/Common", $"Skill_Icon_Common_{_skillData.OverrideCommonIconName}");
            }

            Accessor.BasicTab_ExSkillInfo_SkillNameText.text = _skillData.Name;
            Accessor.BasicTab_ExSkillInfo_SkillDescriptionText.text = _skillData.Description;

            var _weaponData = GameResource.WeaponTable[_data.WeaponId];
            Accessor.BasicTab_WeaponInfo_WeaponTypeText.text = _weaponData.Type.ToString();
            Accessor.BasicTab_WeaponInfo_WeaponImage.sprite =
                GameResource.Load<Sprite>($"Weapon/{_weaponData.Id}", $"Weapon_Icon_{_weaponData.Id}");

            var _blueStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "BlueStar");
            Accessor.BasicTab_WeaponInfo_StarHolder.Children().Destroy();
            for (int i = 0; i < _weaponData.Star; ++i)
                Instantiate(_blueStarPrefab, Accessor.BasicTab_WeaponInfo_StarHolder.transform);

            var _haloRect = Accessor.FullshotHaloImage.GetComponent<RectTransform>();
            _haloRect.DOKill();
            _haloRect.anchoredPosition = _data.FullshotHaloStartPos;
            _haloRect.DOAnchorPos(_data.FullshotHaloEndPos, 2f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

            var _fullshotGroup = Accessor.FullshotParent.GetComponent<CanvasGroup>();
            var _fullshotRect = Accessor.FullshotParent.GetComponent<RectTransform>();
            _fullshotRect.anchoredPosition = new Vector2(0f, -20f);
            _fullshotGroup.alpha = 0f;
            DOTween.Kill(Accessor.FullshotParent);
            DOTween.Sequence()
                .Append(_fullshotGroup.DOFade(1f, 1f))
                .Join(_fullshotRect.DOAnchorPos(Vector3.zero, 1f))
                .SetId(Accessor.FullshotParent);
        }
    }
}