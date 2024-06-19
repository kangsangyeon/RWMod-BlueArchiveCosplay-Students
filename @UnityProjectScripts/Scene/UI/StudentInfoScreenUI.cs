using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
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

            Image _haloImage;
            if (_data.FrontHalo)
            {
                _haloImage = Accessor.FullshotFrontHaloImage;
                Accessor.FullshotFrontHaloImage.gameObject.SetActive(true);
                Accessor.FullshotBackHaloImage.gameObject.SetActive(false);
            }
            else
            {
                _haloImage = Accessor.FullshotBackHaloImage;
                Accessor.FullshotFrontHaloImage.gameObject.SetActive(false);
                Accessor.FullshotBackHaloImage.gameObject.SetActive(true);
            }

            // 왼쪽에 위치한 캐릭터 정보와 레벨, 경험치를 표시합니다.
            Accessor.FullshotImage.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_{_data.Id}");
            _haloImage.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_Halo_{_data.Id}");

            Accessor.NameText.text = _data.Name;

            var _yellowStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "YellowStar");
            Accessor.StarHolder.Children().Destroy();
            for (int i = 0; i < 5; ++i)
                Instantiate(_yellowStarPrefab, Accessor.StarHolder.transform);

            var _clubData =
                GameResource.ClubTable.Values
                    .First(x => x.StudentList.Contains(_id));
            var _schoolData =
                GameResource.SchoolTable.Values
                    .First(x => x.ClubList.Contains(_clubData.Id));
            string _bgPath = string.IsNullOrEmpty(_data.OverrideBgPath)
                ? _schoolData.BgPath
                : _data.OverrideBgPath;
            var _directory = Path.GetDirectoryName(_bgPath);
            var _fileName = Path.GetFileName(_bgPath);
            Accessor.Background.sprite = GameResource.Load<Sprite>(_directory, _fileName);

            Accessor.AttributeText.text = _data.Attribute.ToStringKr();
            Accessor.AttributeIcon.sprite = GameResource.Load<Sprite>(
                "Sprite/UI/Access/Student/Attribute",
                $"Student_Attribute_Icon_{_data.Attribute.ToString()}");

            // AttributeText의 길이가 달라지면 레이아웃이 이를 감지하지 못해 텍스트와 아이콘과 겹치는 문제가 발생합니다.
            // 따라서 레이아웃을 강제로 새로고침합니다.
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Accessor.AttributeBox.transform);

            // todo: level text 설정
            // Accessor.LevelText.text = 
            // todo: exp fill 설정
            // Accessor.ExpBar.fillAmount

            var _haloRect = _haloImage.GetComponent<RectTransform>();
            var _haloGroup = _haloImage.GetComponent<CanvasGroup>();
            var _fullshotParentGroup = Accessor.FullshotParent.GetComponent<CanvasGroup>();
            var _fullshotParentRect = Accessor.FullshotParent.GetComponent<RectTransform>();
            var _fullshotRect = Accessor.FullshotImage.GetComponent<RectTransform>();

            // fullshot의 크기를 조정합니다.
            _fullshotRect.anchorMin = Vector2.zero;
            _fullshotRect.anchorMax = Vector2.one;
            _fullshotRect.offsetMin = new Vector2(_data.FullshotRectOffset.x, _data.FullshotRectOffset.w);
            _fullshotRect.offsetMax = new Vector2(-_data.FullshotRectOffset.z, -_data.FullshotRectOffset.y);

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

            // 툴팁을 설정합니다.
            Accessor.Tooltips[0].Icon.sprite = Accessor.AttributeIcon.sprite;
            Accessor.Tooltips[0].TooltipText.text =
                GameResource.StudentAttributeTable[_data.Attribute].Description;
            Accessor.Tooltips[1].gameObject.SetActive(false); // todo: passive를 가지고 있는 경우, 인덱스1 툴팁의 속성을 설정해야 함
            for (int i = 0; i < 2; ++i)
            {
                var _canvasGroup = Accessor.Tooltips[i].TooltipBox.GetComponent<CanvasGroup>();
                _canvasGroup.alpha = 0f;
                Accessor.Tooltips[i].HexImage.OnPointerEnterAsObservable()
                    .Subscribe(_ =>
                    {
                        _canvasGroup.DOKill();
                        _canvasGroup.DOFade(1f, .2f);
                    })
                    .AddTo(gameObject);
                Accessor.Tooltips[i].HexImage.OnPointerExitAsObservable()
                    .Subscribe(_ =>
                    {
                        _canvasGroup.DOKill();
                        _canvasGroup.DOFade(0f, .2f);
                    })
                    .AddTo(gameObject);
            }


            // 기본 정보 탭의 내용을 표시합니다.
            Accessor.BasicTab_StatInfo_SDFullShot.sprite =
                GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_SD_Fullshot_{_data.Id}");

            Accessor.BasicTab_StatInfo_StatPage1Value.text =
                $"{_data.DefaultShooting}\n{_data.DefaultMelee}\n{_data.DefaultConstruction}\n{_data.DefaultMining}\n{_data.DefaultCooking}\n{_data.DefaultPlants}";
            Accessor.BasicTab_StatInfo_StatPage2Value.text =
                $"{_data.DefaultAnimals}\n{_data.DefaultCrafting}\n{_data.DefaultArtistic}\n{_data.DefaultMedical}\n{_data.DefaultSocial}\n{_data.DefaultIntellectual}";

            var _skillData = GameResource.SkillTable[_data.SkillId];
            var _skillLevelData = GameResource.SkillLevelTable[(_data.SkillId, 1)]; // temp: 임시적으로 스킬 레벨을 1으로 간주합니다.
            var _skillLevels = new List<SkillLevelData>();
            int _maxSkillLevel = _data.MaxSkillLevel ?? 5;
            for (int i = 0; i < _maxSkillLevel; ++i)
                _skillLevels.Add(GameResource.SkillLevelTable[(_data.SkillId, i + 1)]);
            var _skillInfoUI = Accessor.BasicTab_ExSkillInfo.GetComponent<ExSkillInfoUI>();
            _skillInfoUI.UpdateUI(_data, _skillData, _skillLevelData,
                _skillLevelData.Id.Level == _skillLevels.Count - 1, false);

            var _weaponData = GameResource.WeaponTable[_data.WeaponId];
            Accessor.BasicTab_WeaponInfo_WeaponTypeText.text = _weaponData.Type.ToString();
            Accessor.BasicTab_WeaponInfo_WeaponImage.sprite =
                GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{_weaponData.Id}");

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
                _accessor.UpdateUI(_data, _skillData, _skillLevels[i], _isMaxLevel, _isUnlocked);
            }

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