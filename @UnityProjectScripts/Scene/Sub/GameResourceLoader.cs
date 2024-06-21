using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TemplateTable;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityProjectScripts
{
    public class GameResourceLoader : MonoBehaviour
    {
        private void Awake()
        {
            LoadDataTable();
            TryInitializeSaveData();

            GameResource.StudentPrefab = Load<GameObject>("Prefab/UI", "Student");
            GameResource.ClubPrefab = Load<GameObject>("Prefab/UI", "Club");
            GameResource.SynergyActivatedSprite = Load<Sprite>("Sprite/UI/Access/Synergy", "Synergy_Icon_True");
            GameResource.SynergyDeactivatedSprite = Load<Sprite>("Sprite/UI/Access/Synergy", "Synergy_Icon_False");
            GameResource.SchoolLogoSprites =
                GameResource.SchoolTable.ToDictionary(
                    x => x.Key,
                    x => Load<Sprite>($"School/Icon", $"School_Icon_{x.Key}"));
            GameResource.StudentPortraitSprites =
                GameResource.StudentTable.ToDictionary(
                    x => x.Key,
                    x => Load<Sprite>($"Student/{x.Key}", $"Student_Portrait_{x.Key}"));
            GameResource.StudentAttributeFrameSprites = new[]
                {
                    StudentAttribute.Attack,
                    StudentAttribute.Defense,
                    StudentAttribute.Support,
                    StudentAttribute.Heal
                }
                .Select(x =>
                    Load<Sprite>(
                        "Sprite/UI/Access/Student/Attribute",
                        $"Student_Attribute_Frame_{x.ToString()}"))
                .ToList();
        }

        private T Load<T>(string _resourcesDirectoryAddress, string _bundleAssetName) where T : Object
        {
            if (GameResource.Bundle != null)
                return GameResource.Bundle.LoadAsset<T>(_bundleAssetName);
            else
                return Resources.Load<T>(_resourcesDirectoryAddress + "/" + _bundleAssetName);
        }

        private void LoadDataTable()
        {
            var _studentTableJson = Load<TextAsset>("Data", "DataTable_1000_Student").text;
            var _clubTableJson = Load<TextAsset>("Data", "DataTable_2000_Club").text;
            var _schoolTableJson = Load<TextAsset>("Data", "DataTable_3000_School").text;
            var _skillTableJson = Load<TextAsset>("Data", "DataTable_4000_Skill").text;
            var _skillLevelTableJson = Load<TextAsset>("Data", "DataTable_5000_SkillLevel").text;
            var _weaponTableJson = Load<TextAsset>("Data", "DataTable_6000_Weapon").text;
            var _passiveSkillTableJson = Load<TextAsset>("Data", "DataTable_7000_PassiveSkill").text;
            var _studentAttributeTableJson = Load<TextAsset>("Data", "DataTable_StudentAttribute").text;
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
}