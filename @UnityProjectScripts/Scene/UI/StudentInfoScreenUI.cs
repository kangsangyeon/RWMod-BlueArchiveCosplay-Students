using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DG.Tweening;
using Rimworld;
using UniRx;
using UniRx.Triggers;
using Unity.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BA
{
    public class StudentInfoScreenUI : MonoBehaviour
    {
        public StudentInfoScreenAccessor Accessor;
        public ReactiveProperty<int> CharId = new ReactiveProperty<int>(0);
        private bool _checkScroll;
        private CompositeDisposable _charDisposables;

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
                    var ui = Accessor.WeaponPopup.GetComponent<WeaponPopupUI>();
                    var charData = GameResource.StudentTable[CharId.Value];
                    var weaponData = GameResource.WeaponTable[charData.WeaponId];
                    ui.UpdateUI(weaponData, 1, 0);
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

        private void OnDestroy()
        {
            _charDisposables?.Dispose();
        }

        private void UpdateChar(int id)
        {
            var data = GameResource.StudentTable[id];
            var fullshotRenderAccessor = FindObjectOfType<FullshotRenderAccessor>();
            var saveData = GameResource.Save.StudentSaveData[CharId.Value];

            // 이전 내용을 정리합니다.
            _charDisposables?.Dispose();
            _charDisposables = new CompositeDisposable();

            // 왼쪽에 위치한 캐릭터 정보와 레벨, 경험치를 표시합니다.
            fullshotRenderAccessor.Fullshot.sprite =
                GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_{data.Id}");
            fullshotRenderAccessor.FullshotHalo.sprite =
                GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_Halo_{data.Id}");
            fullshotRenderAccessor.FullshotBg.sprite =
                GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_Bg_{data.Id}");
            fullshotRenderAccessor.Camera.transform.localPosition = new Vector3(data.CamPos.x, data.CamPos.y, -10f);
            fullshotRenderAccessor.Camera.orthographicSize = data.CamOrthoSize;

            Accessor.NameText.text = data.Name;

            var starCount = data.DefaultStar;
            for (int i = 0; i < starCount; ++i)
                Accessor.Stars[i].gameObject.SetActive(true);
            for (int i = starCount; i < 5; ++i)
                Accessor.Stars[i].gameObject.SetActive(false);

            var clubData =
                GameResource.ClubTable.Values
                    .First(x => x.StudentList.Contains(id));
            var schoolData =
                GameResource.SchoolTable.Values
                    .First(x => x.ClubList.Contains(clubData.Id));
            string bgPath = string.IsNullOrEmpty(data.OverrideBgPath)
                ? schoolData.BgPath
                : data.OverrideBgPath;
            var directory = Path.GetDirectoryName(bgPath);
            var fileName = Path.GetFileName(bgPath);
            Accessor.Background.sprite = GameResource.Load<Sprite>(directory, fileName);

            Accessor.AttributeText.text = data.Attribute.ToStringKr();
            Accessor.AttributeIcon.sprite = GameResource.Load<Sprite>(
                "UI/Texture/Access/Student/Attribute",
                $"Student_Attribute_Icon_{data.Attribute.ToString()}");

            // AttributeText의 길이가 달라지면 레이아웃이 이를 감지하지 못해 텍스트와 아이콘과 겹치는 문제가 발생합니다.
            // 따라서 레이아웃을 강제로 새로고침합니다.
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)Accessor.AttributeBox.transform);


            // 캐릭터와 헤일로 애니메이션을 재생합니다.
            var fullshotRect = (RectTransform)Accessor.FullshotImage.transform;
            var fullshotGroup = Accessor.FullshotImage.GetComponent<CanvasGroup>();
            var fullshotTf = fullshotRenderAccessor.Fullshot.transform;
            var fullshotHaloTf = fullshotRenderAccessor.FullshotHalo.transform;
            var fullshotBgTf = fullshotRenderAccessor.FullshotBg.transform;

            if (data.FrontHalo)
                fullshotRenderAccessor.FullshotHalo.sortingOrder = 10; // fullshot과 fullshot face의 order는 5~6
            else
                fullshotRenderAccessor.FullshotHalo.sortingOrder = 4;

            DOTween.Kill(Accessor.FullshotImage);
            fullshotRenderAccessor.Fullshot.DOKill();
            fullshotRenderAccessor.FullshotHalo.DOKill();

            fullshotGroup.alpha = 0f;
            DOTween.Sequence()
                .Append(fullshotGroup.DOFade(1f, 1f))
                .SetId(Accessor.FullshotImage);

            fullshotTf.localPosition = Vector2.zero;
            fullshotHaloTf.localPosition = data.FullshotHaloPos;
            fullshotBgTf.localPosition = data.FullshotBgPos;

            DOTween.Sequence(fullshotRenderAccessor.Fullshot)
                .Append(fullshotTf.DOLocalMove(Vector2.up * 0.08f, 3f).SetEase(Ease.InOutSine))
                .Append(fullshotTf.DOLocalMove(Vector2.zero, 3f).SetEase(Ease.InOutSine))
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);

            DOTween.Sequence(fullshotRenderAccessor.FullshotHalo)
                .Append(fullshotHaloTf.DOLocalMove(data.FullshotHaloPos + Vector2.up * 0.08f, 2f).SetEase(Ease.InOutSine))
                .Append(fullshotHaloTf.DOLocalMove(data.FullshotHaloPos, 2f).SetEase(Ease.InOutSine))
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);

            // 툴팁을 설정합니다.
            Accessor.Tooltips[0].Icon.sprite = Accessor.AttributeIcon.sprite;
            Accessor.Tooltips[0].TextMask_Text.text =
                GameResource.StudentAttributeTable[data.Attribute].Description;
            Accessor.Tooltips[1].gameObject.SetActive(data.PassiveSkillId > 0);
            if (data.PassiveSkillId > 0)
            {
                var passiveSkillData = GameResource.PassiveSkillTable.TryGet(data.PassiveSkillId);
                Accessor.Tooltips[1].Icon.sprite =
                    GameResource.Load<Sprite>("PassiveSkill/Icon", $"PassiveSkill_Icon_{passiveSkillData.IconName}");
                Accessor.Tooltips[1].TextMask_Text.text = passiveSkillData.Description;
                TryStartScroll();
            }

            for (int i = 0; i < 2; ++i)
            {
                var canvasGroup = Accessor.Tooltips[i].TooltipBox.GetComponent<CanvasGroup>();
                canvasGroup.alpha = 0f;
                Accessor.Tooltips[i].HexImage.OnPointerEnterAsObservable()
                    .Subscribe(_ =>
                    {
                        canvasGroup.DOKill();
                        canvasGroup.DOFade(1f, .2f);
                    })
                    .AddTo(gameObject);
                Accessor.Tooltips[i].HexImage.OnPointerExitAsObservable()
                    .Subscribe(_ =>
                    {
                        canvasGroup.DOKill();
                        canvasGroup.DOFade(0f, .2f);
                    })
                    .AddTo(gameObject);
            }


            // 기본 정보 탭의 내용을 표시합니다.
            Accessor.BasicTab_StatInfo_SDFullShot.sprite =
                GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_SD_Fullshot_{data.Id}");

            Accessor.BasicTab_StatInfo_StatPage1Value.text =
                $"{data.DefaultShooting}\n{data.DefaultMelee}\n{data.DefaultConstruction}\n{data.DefaultMining}\n{data.DefaultCooking}\n{data.DefaultPlants}";
            Accessor.BasicTab_StatInfo_StatPage2Value.text =
                $"{data.DefaultAnimals}\n{data.DefaultCrafting}\n{data.DefaultArtistic}\n{data.DefaultMedical}\n{data.DefaultSocial}\n{data.DefaultIntellectual}";

            var skillData = GameResource.SkillTable[data.SkillId];
            var skillLevelData = GameResource.SkillLevelTable[(data.SkillId, 1)]; // temp: 임시적으로 스킬 레벨을 1으로 간주합니다.
            var skillLevels = new List<SkillLevelData>();
            int maxSkillLevel = data.MaxSkillLevel ?? 5;
            for (int i = 0; i < maxSkillLevel; ++i)
                skillLevels.Add(GameResource.SkillLevelTable[(data.SkillId, i + 1)]);
            var skillInfoUI = Accessor.BasicTab_SkillButton_ExSkillInfo.GetComponent<ExSkillInfoUI>();
            skillInfoUI.UpdateUI(data, skillData, skillLevelData,
                skillLevelData.Id.Level == skillLevels.Count - 1, false);

            var weaponData = GameResource.WeaponTable[data.WeaponId];
            Accessor.BasicTab_WeaponButton_WeaponTypeText.text = weaponData.Type.ToString();
            Accessor.BasicTab_WeaponButton_WeaponImage.sprite =
                GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{weaponData.Id}");

            for (int i = 0; i < weaponData.Star; ++i)
                Accessor.BasicTab_WeaponButton_Stars[i].gameObject.SetActive(true);
            for (int i = weaponData.Star; i < 5; ++i)
                Accessor.BasicTab_WeaponButton_Stars[i].gameObject.SetActive(false);

            // 레벨 업 탭의 내용을 표시합니다.
            Accessor.LevelUpTab_ExSkillInfo_ExSkillHolder.Children().Destroy();
            for (int i = 0; i < skillLevels.Count; i++)
            {
                var go = Instantiate(GameResource.ExSkillInfoPrefab, Accessor.LevelUpTab_ExSkillInfo_ExSkillHolder.transform);
                var accessor = go.GetComponent<ExSkillInfoUI>();
                bool isMaxLevel = i == skillLevels.Count - 1;
                bool isUnlocked = i > skillLevelData.Id.Level - 1;
                accessor.UpdateUI(data, skillData, skillLevels[i], isMaxLevel, isUnlocked);
            }

            // 신비 탭의 내용을 표시합니다.
            Sprite thumbnailSprite =
                GameResource.Load<Sprite>($"Skill/Icon", $"Skill_Icon_{skillData.IconName}");

            Accessor.ShinbiTab_GrowthInfo_SkillButton_Icon.sprite = thumbnailSprite;
            Accessor.ShinbiTab_GrowthInfo_SkillButton_HexImage.color = UIUtilProcedure.GetAttributeColor(data.Attribute);
            Accessor.ShinbiTab_GrowthInfo_SkillButton_Name.text = skillData.Name;
            Accessor.ShinbiTab_ShinbiUpBox_Button.onClick.AddListener(() => OnShinbiLevelUpButtonClicked(saveData.Shinbi));

            SetVisibleTab(0);

            // 텍스트 스크롤 여부를 다시 확인하고, 그 크기에 맞게 다시 스크롤해야 함.
            _checkScroll = false;

            // save data 변경에 구독
            // (level text, exp fill 설정)
            UpdateStudentSave(saveData);
            GameResource.Save.StudentSaveData[CharId.Value]
                .ObserveEveryValueChanged(x => x.Level)
                .Subscribe(_ => UpdateStudentSave(saveData))
                .AddTo(_charDisposables);
            GameResource.Save.StudentSaveData[CharId.Value]
                .ObserveEveryValueChanged(x => x.Exp)
                .Subscribe(_ => UpdateStudentSave(saveData))
                .AddTo(_charDisposables);
        }

        private void SetVisibleTab(int tabIndex)
        {
            ColorUtility.TryParseHtmlString("#D7EAF1", out var enableButtonColor);
            ColorUtility.TryParseHtmlString("#2F363C", out var enableTextColor);
            ColorUtility.TryParseHtmlString("#2D4A75", out var disableButtonColor);
            ColorUtility.TryParseHtmlString("#FFFFFF", out var disableTextColor);

            Accessor.BasicTab.gameObject.SetActive(false);
            Accessor.LevelUpTab.gameObject.SetActive(false);
            Accessor.ShinbiTab.gameObject.SetActive(false);
            Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = disableButtonColor;
            Accessor.TabButtonBox_BasicTabButton_Text.color = disableTextColor;
            Accessor.TabButtonBox_LevelUpTabButton.GetComponent<Image>().color = disableButtonColor;
            Accessor.TabButtonBox_LevelUpTabButton_Text.color = disableTextColor;
            Accessor.TabButtonBox_ShinbiTabButton.GetComponent<Image>().color = disableButtonColor;
            Accessor.TabButtonBox_ShinbiTabButton_Text.color = disableTextColor;

            switch (tabIndex)
            {
                case 0:
                    Accessor.BasicTab.gameObject.SetActive(true);
                    Accessor.TabButtonBox_BasicTabButton.GetComponent<Image>().color = enableButtonColor;
                    Accessor.TabButtonBox_BasicTabButton_Text.color = enableTextColor;
                    break;
                case 1:
                    Accessor.LevelUpTab.gameObject.SetActive(true);
                    Accessor.TabButtonBox_LevelUpTabButton.GetComponent<Image>().color = enableButtonColor;
                    Accessor.TabButtonBox_LevelUpTabButton_Text.color = enableTextColor;
                    break;
                case 2:
                    Accessor.ShinbiTab.gameObject.SetActive(true);
                    Accessor.TabButtonBox_ShinbiTabButton.GetComponent<Image>().color = enableButtonColor;
                    Accessor.TabButtonBox_ShinbiTabButton_Text.color = enableTextColor;
                    break;
            }
        }

        private void UpdateStudentSave(StudentSaveData data)
        {
            var requiredExp =
                GameResource.StudentLevelRequiredExpTable[data.Level].Value;
            var star =
                GameResource.StudentTable[data.Id].DefaultStar + data.Shinbi;
            var levelLimit =
                GameResource.StudentLevelLimitTable[star];

            Accessor.LevelText.text = $"Lv.{data.Level}";
            Accessor.ExpBar.FillAmount = (float)data.Exp / requiredExp;

            if (data.Level >= levelLimit.Value)
            {
                requiredExp = 0;
                Accessor.ExpText.gameObject.SetActive(false);
                Accessor.ExpMaxText.gameObject.SetActive(true);
                Accessor.ExpText.text = $"{data.Exp} / {requiredExp}"; // 레벨 상한에 도달했으면 0/0으로 표기. 사실 "Max" 텍스트에 가려질 것임.
            }
            else
            {
                Accessor.ExpText.gameObject.SetActive(true);
                Accessor.ExpMaxText.gameObject.SetActive(false);
                Accessor.ExpText.text = $"{data.Exp} / {requiredExp}";
            }
        }

        private void TryStartScroll()
        {
            if (_checkScroll)
                return;
            _checkScroll = true;

            Accessor.Tooltips[1].TextMask_Text.rectTransform.anchoredPosition = Vector2.zero;
            var color = Accessor.Tooltips[1].TextMask_Text.color;
            color.a = 1f;
            Accessor.Tooltips[1].TextMask_Text.color = color;

            // Description이 Description Mask의 범위를 벗어나는 경우,
            // 세로로 텍스트를 내리는 애니메이션을 재생합니다.
            DOTween.Kill(Accessor.Tooltips[1].TextMask_Text);
            float heightDelta =
                Accessor.Tooltips[1].TextMask_Text.preferredHeight -
                Accessor.Tooltips[1].TextMask.rectTransform.rect.height;
            if (heightDelta > 0)
            {
                // 대략 한 줄마다 스크롤 시간 3초 증가 (font size가 대략 한 줄 크기)
                float scrollDuration =
                    (heightDelta / Accessor.Tooltips[1].TextMask_Text.fontSize) * 3f;
                DOTween.Sequence()
                    .AppendInterval(1f)
                    .Append(Accessor.Tooltips[1].TextMask_Text.rectTransform.DOAnchorPosY(heightDelta, scrollDuration))
                    .Append(Accessor.Tooltips[1].TextMask_Text.DOFade(0f, 1f))
                    .AppendCallback(() => Accessor.Tooltips[1].TextMask_Text.rectTransform.anchoredPosition = Vector2.zero)
                    .Append(Accessor.Tooltips[1].TextMask_Text.DOFade(1f, 1f))
                    .SetLoops(-1)
                    .SetId(Accessor.Tooltips[1].TextMask_Text);
            }
        }

        private void OnShinbiLevelUpButtonClicked(int currentShinbi)
        {
            if (BridgeProcedure.CanShinbiLiberationFunc == null)
            {
                Debug.LogWarning("CanShinbiLiberationFunc is null.");
                return;
            }

            var data = GameResource.ShinbiLiberationCostTable.TryGet(currentShinbi);
            var result = BridgeProcedure.CanShinbiLiberationFunc.Invoke(data.Value);
            if (result)
            {
                BridgeProcedure.OnShinbiLiberation.Invoke(data.Value);
                Contents.Instance.Accessor.ShinbiLiberationAnimation.Play(
                    onComplete: () => UpdateChar(CharId.Value));
            }
        }
    }
}