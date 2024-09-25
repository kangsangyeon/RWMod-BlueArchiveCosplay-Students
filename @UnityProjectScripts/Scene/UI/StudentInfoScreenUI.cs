using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StudentInfoScreenUI : MonoBehaviour
{
    public StudentInfoScreenAccessor Accessor;
    public ReactiveProperty<int> CharId = new ReactiveProperty<int>(0);

    private void Start()
    {
        // 버튼 이벤트 액션을 설정.
        Accessor.ScreenTopBar.BackButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Accessor.gameObject.SetActive(false);
                Contents.Instance.Accessor.PadCanvas.StudentListScreen.gameObject.SetActive(true);
            })
            .AddTo(gameObject);

        Accessor.ScreenTopBar.HomeButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Accessor.gameObject.SetActive(false);
                Contents.Instance.Accessor.PadCanvas.MainScreen.gameObject.SetActive(true);
            })
            .AddTo(gameObject);

        Accessor.BasicTab_WeaponButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                var _ui = Accessor.WeaponPopup.GetComponent<WeaponPopupUI>();
                var _charData = GameResource.StudentTable[CharId.Value];
                var _weaponData = GameResource.WeaponTable[_charData.WeaponId];
                _ui.UpdateUI(_weaponData, 1, 0);
                Accessor.WeaponPopup.gameObject.SetActive(true);
            })
            .AddTo(gameObject);

        // 탭 전환 버튼 이벤트 액션을 설정합니다.
        Accessor.TabButtonBox_BasicTabButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(0));
        Accessor.TabButtonBox_LevelUpTabButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(1));
        Accessor.TabButtonBox_ShinbiTabButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(2));

        Accessor.BasicTab_SkillButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(1));
        Accessor.ShinbiTab_GrowthInfo_SkillButton.OnClickAsObservable()
            .Subscribe(_ => SetVisibleTab(1));

        // 탭의 기본 활성 상태를 설정합니다.
        SetVisibleTab(0);

        CharId
            .Where(x => x > 0)
            .Subscribe(UpdateChar);

        Accessor.WeaponPopup.gameObject.SetActive(false);

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
        var _fullshotRenderAccessor = FindObjectOfType<FullshotRenderAccessor>();

        if (_data.FrontHalo)
            _fullshotRenderAccessor.FullshotHalo.sortingOrder = 10; // fullshot과 fullshot face의 order는 5~6
        else
            _fullshotRenderAccessor.FullshotHalo.sortingOrder = 4;


        // 왼쪽에 위치한 캐릭터 정보와 레벨, 경험치를 표시합니다.
        _fullshotRenderAccessor.Fullshot.sprite =
            GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_{_data.Id}");
        _fullshotRenderAccessor.FullshotHalo.sprite =
            GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_Halo_{_data.Id}");
        _fullshotRenderAccessor.FullshotBg.sprite =
            GameResource.Load<Sprite>($"Student/{_data.Id}", $"Student_Fullshot_Bg_{_data.Id}");
        _fullshotRenderAccessor.Camera.transform.localPosition = new Vector3(_data.CamPos.x, _data.CamPos.y, -10f);
        _fullshotRenderAccessor.Camera.orthographicSize = _data.CamOrthoSize;

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

        // 캐릭터와 헤일로 애니메이션을 재생합니다.
        var _fullshotRect = (RectTransform)Accessor.FullshotImage.transform;
        var _fullshotGroup = Accessor.FullshotImage.GetComponent<CanvasGroup>();
        var _fullshotTf = _fullshotRenderAccessor.Fullshot.transform;
        var _fullshotHaloTf = _fullshotRenderAccessor.FullshotHalo.transform;
        var _fullshotBgTf = _fullshotRenderAccessor.FullshotBg.transform;

        DOTween.Kill(Accessor.FullshotImage);
        _fullshotRenderAccessor.Fullshot.DOKill();
        _fullshotRenderAccessor.FullshotHalo.DOKill();

        _fullshotGroup.alpha = 0f;
        DOTween.Sequence()
            .Append(_fullshotGroup.DOFade(1f, 1f))
            .SetId(Accessor.FullshotImage);

        _fullshotTf.localPosition = Vector2.zero;
        _fullshotHaloTf.localPosition = _data.FullshotHaloPos;
        _fullshotBgTf.localPosition = _data.FullshotBgPos;

        DOTween.Sequence(_fullshotRenderAccessor.Fullshot)
            .Append(_fullshotTf.DOLocalMove(Vector2.up * 0.08f, 3f).SetEase(Ease.InOutSine))
            .Append(_fullshotTf.DOLocalMove(Vector2.zero, 3f).SetEase(Ease.InOutSine))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);

        DOTween.Sequence(_fullshotRenderAccessor.FullshotHalo)
            .Append(_fullshotHaloTf.DOLocalMove(_data.FullshotHaloPos + Vector2.up * 0.08f, 2f).SetEase(Ease.InOutSine))
            .Append(_fullshotHaloTf.DOLocalMove(_data.FullshotHaloPos, 2f).SetEase(Ease.InOutSine))
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);

        // 툴팁을 설정합니다.
        Accessor.Tooltips[0].Icon.sprite = Accessor.AttributeIcon.sprite;
        Accessor.Tooltips[0].TooltipText.text =
            GameResource.StudentAttributeTable[_data.Attribute].Description;
        Accessor.Tooltips[1].gameObject.SetActive(_data.PassiveSkillId > 0);
        if (_data.PassiveSkillId > 0)
        {
            var _passiveSkillData = GameResource.PassiveSkillTable.TryGet(_data.PassiveSkillId);
            Accessor.Tooltips[1].Icon.sprite =
                GameResource.Load<Sprite>("PassiveSkill/Icon", $"PassiveSkill_Icon_{_passiveSkillData.IconName}");
            Accessor.Tooltips[1].TooltipText.text = _passiveSkillData.Description;
        }

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
        var _skillInfoUI = Accessor.BasicTab_SkillButton_ExSkillInfo.GetComponent<ExSkillInfoUI>();
        _skillInfoUI.UpdateUI(_data, _skillData, _skillLevelData,
            _skillLevelData.Id.Level == _skillLevels.Count - 1, false);

        var _weaponData = GameResource.WeaponTable[_data.WeaponId];
        Accessor.BasicTab_WeaponButton_WeaponTypeText.text = _weaponData.Type.ToString();
        Accessor.BasicTab_WeaponButton_WeaponImage.sprite =
            GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{_weaponData.Id}");

        var _blueStarPrefab = GameResource.Load<GameObject>("Prefab/UI", "BlueStar");
        Accessor.BasicTab_WeaponButton_StarHolder.Children().Destroy();
        for (int i = 0; i < _weaponData.Star; ++i)
            Instantiate(_blueStarPrefab, Accessor.BasicTab_WeaponButton_StarHolder.transform);

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

        // 신비 탭의 내용을 표시합니다.
        Sprite _thumbnailSprite =
            GameResource.Load<Sprite>($"Skill/Icon", $"Skill_Icon_{_skillData.IconName}");

        Accessor.ShinbiTab_GrowthInfo_SkillButton_Icon.sprite = _thumbnailSprite;
        Accessor.ShinbiTab_GrowthInfo_SkillButton_HexImage.color = UIUtilProcedure.GetAttributeColor(_data.Attribute);
        Accessor.ShinbiTab_GrowthInfo_SkillButton_Name.text = _skillData.Name;

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
        Accessor.ShinbiTab.gameObject.SetActive(false);
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
            case 2:
                Accessor.ShinbiTab.gameObject.SetActive(true);
                Accessor.TabButtonBox_ShinbiTabButton.GetComponent<Image>().color = _enableButtonColor;
                Accessor.TabButtonBox_ShinbiTabButton_Text.color = _enableTextColor;
                break;
        }
    }
}