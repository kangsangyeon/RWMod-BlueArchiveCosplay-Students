using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
            var _asset = Load<TextAsset>("Data", "DataFile");
            var _data = LitJson.JsonMapper.ToObject<DataFile>(_asset.text);
            GameResource.StudentTable =
                _data.StudentData?.ToDictionary(x => x.Id, x => x)
                ?? new Dictionary<int, StudentData>();
            GameResource.ClubTable =
                _data.ClubData?.ToDictionary(x => x.Id, x => x)
                ?? new Dictionary<int, ClubData>();
            GameResource.SchoolTable =
                _data.SchoolData?.ToDictionary(x => x.Id, x => x)
                ?? new Dictionary<int, SchoolData>();
        }
    }
}