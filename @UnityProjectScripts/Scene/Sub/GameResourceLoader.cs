using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Resource.Data;
using TemplateTable;
using UnityEngine;
using Object = System.Object;

public class GameResourceLoader : MonoBehaviour
{
    private void Awake()
    {
        LoadDataTable();
        TryInitializeSaveData();

        GameResource.StudentPrefab = GameResource.Load<GameObject>("UI/Prefab", "Student");
        GameResource.ClubPrefab = GameResource.Load<GameObject>("UI/Prefab", "Club");
        GameResource.SynergyActivatedSprite = GameResource.Load<Sprite>("UI/Texture/Access/Synergy", "Synergy_Icon_True");
        GameResource.SynergyDeactivatedSprite = GameResource.Load<Sprite>("UI/Texture/Access/Synergy", "Synergy_Icon_False");
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
    }

    private void LoadDataTable()
    {
        var studentTableJson = GameResource.Load<TextAsset>("Data", "DataTable_1000_Student").text;
        var clubTableJson = GameResource.Load<TextAsset>("Data", "DataTable_2000_Club").text;
        var schoolTableJson = GameResource.Load<TextAsset>("Data", "DataTable_3000_School").text;
        var skillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_4000_Skill").text;
        var skillLevelTableJson = GameResource.Load<TextAsset>("Data", "DataTable_5000_SkillLevel").text;
        var weaponTableJson = GameResource.Load<TextAsset>("Data", "DataTable_6000_Weapon").text;
        var passiveSkillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_7000_PassiveSkill").text;
        var studentAttributeTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentAttribute").text;
        var studentAttributeLevelTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentAttributeLevel").text;
        var studentLevelRequiredExpTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentLevelRequiredExp").text;
        var studentLevelLimitTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentLevelLimit").text;
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
        GameResource.StudentAttributeLevelTable =
            LoadTemplateTable<(StudentAttribute, int), StudentAttributeLevelData>(studentAttributeLevelTableJson);
        GameResource.StudentLevelRequiredExpTable =
            LoadTemplateTable<int, StudentLevelRequiredExpData>(studentLevelRequiredExpTableJson);
        GameResource.StudentLevelLimitTable =
            LoadTemplateTable<int, StudentLevelLimitData>(studentLevelLimitTableJson);

        var constValueTable = LoadTemplateTable<string, ConstValue>(constValueTableJson);
        GameResource.Const = new Const()
        {
            PawnCompSettings = constValueTable.TryGet(nameof(PawnCompSettings)).Value as PawnCompSettings,
        };
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
                });

        GameResource.Save = save;
    }
}