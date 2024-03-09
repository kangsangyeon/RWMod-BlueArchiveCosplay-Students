using System.Linq;
using UnityEngine;

namespace UnityProjectScripts
{
    public class GameResourceLoader : MonoBehaviour
    {
        private void Start()
        {
            GameResource.StudentPrefab = Load<GameObject>("Prefab/UI", "Student");
            GameResource.ClubPrefab = Load<GameObject>("Prefab/UI", "Club");
            GameResource.StudentAttributeFrameSprites =
                new[] { StudentAttribute.Attack, StudentAttribute.Defense, StudentAttribute.Support }
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
    }
}