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

            GameResource.StudentPrefab = Load<GameObject>("Prefab/UI", "Student");
            GameResource.ClubPrefab = Load<GameObject>("Prefab/UI", "Club");
            GameResource.SynergeActivatedSprite = Load<Sprite>("Sprite/UI/Various/Synergy", "Synergy_Icon_True");
            GameResource.SynergeDeactivatedSprite = Load<Sprite>("Sprite/UI/Various/Synergy", "Synergy_Icon_False");
            GameResource.SchoolLogoSprites =
                GameResource.SchoolTable.ToDictionary(
                    x => x.Key,
                    x => Load<Sprite>("Sprite/UI/School/Icon", $"School_Icon_{x.Key}"));
            GameResource.StudentPortraitSprites =
                GameResource.StudentTable.ToDictionary(
                    x => x.Key,
                    x => Load<Sprite>("Sprite/UI/Student/Portrait", $"Student_Portrait_{x.Key}"));
            GameResource.StudentAttributeFrameSprites = new[]
                {
                    StudentAttribute.Attack,
                    StudentAttribute.Defense,
                    StudentAttribute.Support,
                    StudentAttribute.Heal
                }
                .Select(x => Load<Sprite>("Sprite/UI/Student/Attribute", $"Student_Attribute_Frame_{x.ToString()}"))
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
            var _weaponTableJson = Load<TextAsset>("Data", "DataTable_5000_Weapon").text;
            GameResource.StudentTable = LoadTemplateTable<int, StudentData>(_studentTableJson);
            GameResource.ClubTable = LoadTemplateTable<int, ClubData>(_clubTableJson);
            GameResource.SchoolTable = LoadTemplateTable<int, SchoolData>(_schoolTableJson);
            GameResource.SkillTable = LoadTemplateTable<int, SkillData>(_skillTableJson);
            GameResource.WeaponTable = LoadTemplateTable<int, WeaponData>(_weaponTableJson);
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
    }
}