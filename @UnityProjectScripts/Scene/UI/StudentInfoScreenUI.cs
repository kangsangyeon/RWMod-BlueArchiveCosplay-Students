using System.Linq;
using DG.Tweening;
using UniRx;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityProjectScripts
{
    public class StudentInfoScreenUI : MonoBehaviour
    {
        public StudentInfoScreenAccessor Accessor;
        public ReactiveProperty<int> CharId = new ReactiveProperty<int>(0);
        private PadAccessor PadAccessor;

        private void Start()
        {
            PadAccessor = FindObjectOfType<PadAccessor>();

            Accessor.ScreenTopBar.BackButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Accessor.gameObject.SetActive(false);
                    PadAccessor.StudentListScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            Accessor.ScreenTopBar.HomeButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    Accessor.gameObject.SetActive(false);
                    PadAccessor.MainScreen.gameObject.SetActive(true);
                })
                .AddTo(gameObject);

            CharId
                .Where(x => x > 0)
                .Subscribe(UpdateChar);

            if (CharId.Value == 0)
            {
                // 개발 중, 첫 시작 화면이 학생 정보 화면일때,
                // 임시로 가장 처음에 1001번 캐릭터가 보여지도록 설정합니다.
                CharId.Value = 1001;
            }
        }

        private void UpdateChar(int _id)
        {
            var _data = GameResource.StudentTable[_id];

            // 왼쪽에 위치한 캐릭터 정보와 레벨, 경험치를 표시합니다.
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

            var _haloRect = Accessor.FullshotHaloImage.GetComponent<RectTransform>();
            var _haloGroup = Accessor.FullshotHaloImage.GetComponent<CanvasGroup>();
            var _fullshotParentGroup = Accessor.FullshotParent.GetComponent<CanvasGroup>();
            var _fullshotParentRect = Accessor.FullshotParent.GetComponent<RectTransform>();
            var _fullshotRect = Accessor.FullshotImage.GetComponent<RectTransform>();

            // fullshot의 크기를 조정합니다.
            _fullshotRect.anchorMin = Vector2.zero;
            _fullshotRect.anchorMax = Vector2.one;
            _fullshotRect.offsetMin = new Vector2(_data.FullshotPos.x, _data.FullshotPos.y);
            _fullshotRect.offsetMax = new Vector2(_data.FullshotPos.x, _data.FullshotPos.y);
            Debug.Log(_data.FullshotPos);

            // 캐릭터와 헤일로 애니메이션을 재생합니다.
            _fullshotParentRect.anchoredPosition = new Vector2(0f, -20f);
            _fullshotParentGroup.alpha = 0f;
            _haloGroup.alpha = 0f;
            _haloRect.sizeDelta = _data.FullshotHaloSize;
            DOTween.Kill(Accessor.FullshotParent);
            DOTween.Sequence()
                .Append(_fullshotParentGroup.DOFade(1f, 1f))
                .Join(_fullshotParentRect.DOAnchorPos(Vector3.zero, 1f))
                .Append(_haloGroup.DOFade(1f, 1f))
                .SetId(Accessor.FullshotParent);

            _fullshotParentRect.DOKill();
            _fullshotParentRect.anchoredPosition = Vector2.zero;
            _fullshotParentRect.DOAnchorPos(Vector2.up * 5f, 3f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            _haloRect.DOKill();
            _haloRect.anchoredPosition = _data.FullshotHaloStartPos;
            _haloRect.DOAnchorPos(_data.FullshotHaloEndPos, 2f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);


            // 기본 정보 탭의 내용을 표시합니다.
            Accessor.BasicTab_StatInfo_SDFullShot.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_SD_Fullshot_{_data.Id}");

            Accessor.BasicTab_StatInfo_StatPage1Value.text =
                $"{_data.DefaultShooting}\n{_data.DefaultMelee}\n{_data.DefaultConstruction}\n{_data.DefaultMining}\n{_data.DefaultCooking}\n{_data.DefaultPlants}";
            Accessor.BasicTab_StatInfo_StatPage2Value.text =
                $"{_data.DefaultAnimals}\n{_data.DefaultCrafting}\n{_data.DefaultArtistic}\n{_data.DefaultMedical}\n{_data.DefaultSocial}\n{_data.DefaultIntellectual}";

            var _skillData = GameResource.SkillTable[_data.SkillId];
            var _skillLevelData = GameResource.SkillLevelTable[(_data.SkillId, 1)]; // temp: 임시적으로 스킬 레벨을 1으로 간주합니다.
            var _skillLevels =
                GameResource.SkillLevelTable.Values
                    .Where(x => x.Id.SkillId == _skillData.Id).ToList();
            var _skillInfoUI = Accessor.BasicTab_ExSkillInfo.GetComponent<ExSkillInfoUI>();
            _skillInfoUI.UpdateUI(_skillData, _skillLevelData, _skillLevelData.Id.Level == _skillLevels.Count - 1,
                false);

            var _weaponData = GameResource.WeaponTable[_data.WeaponId];
            Accessor.BasicTab_WeaponInfo_WeaponTypeText.text = _weaponData.Type.ToString();
            Accessor.BasicTab_WeaponInfo_WeaponImage.sprite =
                GameResource.Load<Sprite>($"Weapon/{_weaponData.Id}", $"Weapon_Icon_{_weaponData.Id}");

            var _blueStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "BlueStar");
            Accessor.BasicTab_WeaponInfo_StarHolder.Children().Destroy();
            for (int i = 0; i < _weaponData.Star; ++i)
                Instantiate(_blueStarPrefab, Accessor.BasicTab_WeaponInfo_StarHolder.transform);

            // 레벨 업 탭의 내용을 표시합니다.
            var _exSkillInfoPrefab = GameResource.Load<GameObject>("Prefab/UI", "ExSkillInfo");
            Accessor.LevelUpTab_ExSkillInfo_ExSkillHolder.Children().Destroy();
            for (int i = 0; i < _skillLevels.Count; i++)
            {
                var _go = Instantiate(_exSkillInfoPrefab, Accessor.LevelUpTab_ExSkillInfo_ExSkillHolder.transform);
                var _accessor = _go.GetComponent<ExSkillInfoUI>();
                bool _isMaxLevel = i == _skillLevels.Count - 1;
                bool _isUnlocked = i > _skillLevelData.Id.Level - 1;
                _accessor.UpdateUI(_skillData, _skillLevels[i], _isMaxLevel, _isUnlocked);
            }

            // 현재 레벨 이후의 스킬들에 잠김 아이콘을 표시합니다.

            // 탭 전환 버튼 이벤트 액션을 설정합니다.
            Accessor.TabButtonBox_BasicTabButton.OnClickAsObservable()
                .Subscribe(_ => SetVisibleTab(0));
            Accessor.TabButtonBox_LevelUpTabButton.OnClickAsObservable()
                .Subscribe(_ => SetVisibleTab(1));

            // 탭의 기본 활성 상태를 설정합니다.
            SetVisibleTab(0);
        }

        private void SetVisibleTab(int _tabIndex)
        {
            ColorUtility.TryParseHtmlString("#D7EAF1", out var _enableButtonColor);
            ColorUtility.TryParseHtmlString("#2F363C", out var _enableTextColor);
            ColorUtility.TryParseHtmlString("#2D4A75", out var _disableButtonColor);
            ColorUtility.TryParseHtmlString("#FFFFFF", out var _disableTextColor);

            Accessor.BasicTab.gameObject.SetActive(false);
            Accessor.LevelUpTab.gameObject.SetActive(false);
            Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = _disableButtonColor;
            Accessor.TabButtonBox_BasicTabButton_Text.color = _disableTextColor;
            Accessor.TabButtonBox_LevelUpTabButton.GetComponent<Image>().color = _disableButtonColor;
            Accessor.TabButtonBox_LevelUpTabButton_Text.color = _disableTextColor;
            Accessor.TabButtonBox_ShinbiTabButton.GetComponent<Image>().color = _disableButtonColor;
            Accessor.TabButtonBox_ShinbiTabButton_Text.color = _disableTextColor;

            switch (_tabIndex)
            {
                case 0:
                    Accessor.BasicTab.gameObject.SetActive(true);
                    Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = _enableButtonColor;
                    Accessor.TabButtonBox_BasicTabButton_Text.color = _enableTextColor;
                    break;
                case 1:
                    Accessor.LevelUpTab.gameObject.SetActive(true);
                    Accessor.TabButtonBox_LevelUpTabButton.GetComponent<Image>().color = _enableButtonColor;
                    Accessor.TabButtonBox_LevelUpTabButton_Text.color = _enableTextColor;
                    break;
            }
        }
    }
}