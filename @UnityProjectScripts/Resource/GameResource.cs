using System.Collections.Generic;
using UnityEngine;
using TemplateTable;

namespace BA
{
    public static class GameResource
    {
        public static SaveData Save;

        public static Const Const;
        public static TemplateTable<int, SchoolData> SchoolTable;
        public static TemplateTable<int, ClubData> ClubTable;
        public static TemplateTable<int, StudentData> StudentTable;
        public static TemplateTable<int, SkillData> SkillTable;
        public static TemplateTable<(int, int), SkillLevelData> SkillLevelTable;
        public static TemplateTable<int, WeaponData> WeaponTable;
        public static TemplateTable<int, PassiveSkillData> PassiveSkillTable;
        public static TemplateTable<StudentAttribute, StudentAttributeData> StudentAttributeTable;
        public static TemplateTable<(StudentAttribute, int), StudentAttributeLevelData> StudentAttributeLevelTable;
        public static TemplateTable<int, StudentLevelRequiredExpData> StudentLevelRequiredExpTable;
        public static TemplateTable<int, StudentLevelLimitData> StudentLevelLimitTable;
        public static TemplateTable<string, object> ConstValueTable;

        public static AssetBundle Bundle;
        public static GameObject ClubPrefab;
        public static GameObject StudentPrefab;
        public static GameObject ExSkillInfoPrefab;
        public static Sprite SynergyActivatedSprite;
        public static Sprite SynergyDeactivatedSprite;
        public static Dictionary<int, Sprite> SchoolLogoSprites;
        public static Dictionary<int, Sprite> StudentPortraitSprites;
        public static List<Sprite> StudentAttributeIconSprites;

        public static ParticleSystem TouchFxPrefab;
        public static RenderTexture FullshotRT;
        public static RenderTexture TransitionRT;

        public static T Load<T>(string resourcesDirectoryAddress, string bundleAssetName) where T : Object
        {
            string fullAddress = string.Empty;
            T resource = null;

            if (Bundle != null)
            {
                fullAddress = bundleAssetName;
                if (typeof(Component).IsAssignableFrom(typeof(T)))
                {
                    var prefab = Bundle.LoadAsset<GameObject>(bundleAssetName);
                    resource = prefab.GetComponent<T>();
                }
                else
                {
                    resource = Bundle.LoadAsset<T>(bundleAssetName);
                }
            }
            else
            {
                fullAddress = string.IsNullOrEmpty(resourcesDirectoryAddress)
                    ? bundleAssetName
                    : $"{resourcesDirectoryAddress}/{bundleAssetName}";
                resource = Resources.Load<T>(fullAddress);
            }

            if (resource != null)
                Debug.Log($"Success to Load Resource `{fullAddress}`.");
            else
                Debug.LogError($"Failed to Load Resource `{fullAddress}`.");
            return resource;
        }
    }
}