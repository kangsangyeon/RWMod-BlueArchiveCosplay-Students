using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TemplateTable;
using UnityEngine;

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
        var _studentTableJson = GameResource.Load<TextAsset>("Data", "DataTable_1000_Student").text;
        var _clubTableJson = GameResource.Load<TextAsset>("Data", "DataTable_2000_Club").text;
        var _schoolTableJson = GameResource.Load<TextAsset>("Data", "DataTable_3000_School").text;
        var _skillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_4000_Skill").text;
        var _skillLevelTableJson = GameResource.Load<TextAsset>("Data", "DataTable_5000_SkillLevel").text;
        var _weaponTableJson = GameResource.Load<TextAsset>("Data", "DataTable_6000_Weapon").text;
        var _passiveSkillTableJson = GameResource.Load<TextAsset>("Data", "DataTable_7000_PassiveSkill").text;
        var _studentAttributeTableJson = GameResource.Load<TextAsset>("Data", "DataTable_StudentAttribute").text;
        GameResource.StudentTable = LoadTemplateTable<int, StudentData>(_studentTableJson);
        GameResource.ClubTable = LoadTemplateTable<int, ClubData>(_clubTableJson);
        GameResource.SchoolTable = LoadTemplateTable<int, SchoolData>(_schoolTableJson);
        GameResource.SkillTable = LoadTemplateTable<int, SkillData>(_skillTableJson);
        GameResource.SkillLevelTable = LoadTemplateTable<(int, int), SkillLevelData>(_skillLevelTableJson);
        GameResource.WeaponTable = LoadTemplateTable<int, WeaponData>(_weaponTableJson);
        GameResource.PassiveSkillTable = LoadTemplateTable<int, PassiveSkillData>(_passiveSkillTableJson);
        GameResource.StudentAttributeTable =
            LoadTemplateTable<StudentAttribute, StudentAttributeData>(_studentAttributeTableJson);
    }

    private TemplateTable<TKey, TValue> LoadTemplateTable<TKey, TValue>(string _json)
        where TKey : IComparable
        where TValue : class, new()
    {
        var _loader = new TemplateTableJsonLoader<TKey, TValue>(
            new JsonTextReader(new StringReader(_json)),
            JsonSerializer.Create(JsonUnityHelper.DeserializerSettings), false);
        var _table = new TemplateTable<TKey, TValue>();
        _table.Load(_loader);
        return _table;
    }

    private void TryInitializeSaveData()
    {
        if (GameResource.Save == null)
        {
            var _save = new SaveData();
            _save.Pyroxenes = 0;
            _save.StudentSaveData =
                GameResource.StudentTable.Values.ToDictionary(
                    x => x.Id,
                    x => new StudentSaveData()
                    {
                        Id = x.Id,
                        Unlock = true,
                        Level = 1,
                        Exp = 0,
                    });

            GameResource.Save = _save;
            return;
        }

        foreach (var _data in GameResource.StudentTable.Values)
        {
            if (GameResource.Save.StudentSaveData.ContainsKey(_data.Id))
                continue;
            GameResource.Save.StudentSaveData.Add(_data.Id, new StudentSaveData()
            {
                Id = _data.Id,
                Unlock = true,
                Level = 1,
                Exp = 0,
            });
        }
    }
}