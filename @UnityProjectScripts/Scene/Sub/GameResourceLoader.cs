using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coffee.UIExtensions;
using Newtonsoft.Json;
using TemplateTable;
using UnityEngine;

namespace BA
{
    public class GameResourceLoader : MonoBehaviour
    {
        private void Awake()
        {
            LoadDataTable();
            TryInitializeSaveData();

            GameResource.StudentPrefab = GameResource.Load<GameObject>("Prefab/UI", "Student");
            GameResource.ClubPrefab = GameResource.Load<GameObject>("Prefab/UI", "Club");
            GameResource.ExSkillInfoPrefab = GameResource.Load<GameObject>("Prefab/UI", "ExSkillInfo");
            GameResource.SynergyActivatedSprite =
                GameResource.Load<Sprite>("UI/Texture/Access/Synergy", "Synergy_Icon_True");
            GameResource.SynergyDeactivatedSprite =
                GameResource.Load<Sprite>("UI/Texture/Access/Synergy", "Synergy_Icon_False");
            GameResource.SchoolLogoSprites =
                GameResource.SchoolTable.ToDictionary(
                    x => x.Key,
                    x => GameResource.Load<Sprite>($"School/Icon", $"School_Icon_{x.Key}"));
            GameResource.StudentPortraitSprites =
                GameResource.StudentTable.ToDictionary(
                    x => x.Key,
                    x => GameResource.Load<Sprite>($"Student/{x.Key}", $"Student_Portrait_{x.Key}"));
            GameResource.StudentAttributeIconSprites = new[]
                {
                    StudentAttribute.Attack,
                    StudentAttribute.Defense,
                    StudentAttribute.Support,
                    StudentAttribute.Heal
                }
                .Select(x =>
                    GameResource.Load<Sprite>(
                        "UI/Texture/Access/Student/Attribute",
                        $"Student_Attribute_Icon_{x.ToString()}"))
                .ToList();

            GameResource.TouchFxPrefab =
                GameResource.Load<ParticleSystem>("Vfx/Prefab", "PS_TouchFx");
            GameResource.FullshotRT =
                GameResource.Load<RenderTexture>("UI/RT", "RT_Fullshot");
            GameResource.TransitionRT =
                GameResource.Load<RenderTexture>("UI/RT", "RT_Transition");

            ShinyEffectForUGUI.RuntimeInitializeOnLoad();
        }

        private void LoadDataTable()
        {
            var studentTableJson = GameResource.Load<TextAsset>("Data", "DataTable_Student").text;
            var clubTableJson = GameResource.Load<TextAsset>("Data", "DataTable_Club").text;
            var schoolTableJson = GameResource.Load<TextAsset>("Data", "DataTable_School").text;
            var skillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_Skill").text;
            var skillLevelTableJson = GameResource.Load<TextAsset>("Data", "DataTable_SkillLevel").text;
            var weaponTableJson = GameResource.Load<TextAsset>("Data", "DataTable_Weapon").text;
            var passiveSkillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_PassiveSkill").text;
            var studentAttributeTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentAttribute").text;
            var studentAttributeBonusTableJson =
                GameResource.Load<TextAsset>("Data", "DataTable_StudentAttributeBonus").text;
            var studentAttributeLevelTableJson =
                GameResource.Load<TextAsset>("Data", "DataTable_StudentAttributeLevel").text;
            var studentLevelRequiredExpTableJson =
                GameResource.Load<TextAsset>("Data", "DataTable_StudentLevelRequiredExp").text;
            var shinbiTableJson = GameResource.Load<TextAsset>("Data", "DataTable_Shinbi").text;
            var constValueTableJson = GameResource.Load<TextAsset>("Data", "DataTable_ConstValue").text;
            GameResource.StudentTable = LoadTemplateTable<int, StudentData>(studentTableJson);
            GameResource.ClubTable = LoadTemplateTable<int, ClubData>(clubTableJson);
            GameResource.SchoolTable = LoadTemplateTable<int, SchoolData>(schoolTableJson);
            GameResource.SkillTable = LoadTemplateTable<int, SkillData>(skillTableJson);
            GameResource.SkillLevelTable = LoadTemplateTable<(int, int), SkillLevelData>(skillLevelTableJson);
            GameResource.WeaponTable = LoadTemplateTable<int, WeaponData>(weaponTableJson);
            GameResource.PassiveSkillTable = LoadTemplateTable<int, PassiveSkillData>(passiveSkillTableJson);
            GameResource.StudentAttributeTable =
                LoadTemplateTable<StudentAttribute, StudentAttributeData>(studentAttributeTableJson);
            GameResource.StudentAttributeBonusTable =
                LoadTemplateTable<string, StudentAttributeBonusData>(studentAttributeBonusTableJson);
            GameResource.StudentAttributeLevelTable =
                LoadTemplateTable<(StudentAttribute, int), StudentAttributeLevelData>(studentAttributeLevelTableJson);
            GameResource.StudentLevelRequiredExpTable =
                LoadTemplateTable<int, StudentLevelRequiredExpData>(studentLevelRequiredExpTableJson);
            GameResource.ShinbiTable =
                LoadTemplateTable<int, ShinbiData>(shinbiTableJson);

            var constValueTable = LoadTemplateTable<string, ConstValue>(constValueTableJson);
            GameResource.Const = new Const()
            {
                PawnCompSettings = constValueTable.TryGet(nameof(PawnCompSettings)).Value as PawnCompSettings,
            };

            var studentAttributeBonusMaps = new Dictionary<StudentAttribute, StudentAttributeBonusMap>();
            foreach (var pair in GameResource.StudentAttributeBonusTable)
            {
                if (!studentAttributeBonusMaps.ContainsKey(pair.Value.Attribute))
                    studentAttributeBonusMaps[pair.Value.Attribute] = new StudentAttributeBonusMap();
                ref int target = ref studentAttributeBonusMaps[pair.Value.Attribute].Values[(int)pair.Value.Bonus];
                switch (pair.Value.Operation)
                {
                    case BonusOperation.Add:
                        target += pair.Value.Value;
                        break;
                    case BonusOperation.Set:
                        target = pair.Value.Value;
                        break;
                }
            }

            GameResource.StudentAttributeBonusMaps = studentAttributeBonusMaps;
        }

        private TemplateTable<TKey, TValue> LoadTemplateTable<TKey, TValue>(string json)
            where TKey : IComparable
            where TValue : class, new()
        {
            var loader = new TemplateTableJsonLoader<TKey, TValue>(
                new JsonTextReader(new StringReader(json)),
                JsonSerializer.Create(JsonUnityHelper.DeserializerSettings), false);
            var table = new TemplateTable<TKey, TValue>();
            table.Load(loader);
            return table;
        }

        private void TryInitializeSaveData()
        {
            if (GameResource.Save != null)
                return;
            var save = new SaveData();
            save.Pyroxenes = 0;
            save.StudentSaveData =
                GameResource.StudentTable.Values.ToDictionary(
                    x => x.Id,
                    x => new StudentSaveData()
                    {
                        Id = x.Id,
                        Unlock = true,
                        Level = 1,
                        Exp = 0,
                        Shinbi = GameResource.StudentTable[x.Id].DefaultStar,
                    });

            GameResource.Save = save;
        }
    }
}