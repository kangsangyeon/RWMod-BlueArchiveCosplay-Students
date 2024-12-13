using System.Collections.Generic;
using Resource.Data;
using UnityEngine;
using TemplateTable;

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
        if (Bundle != null)
        {
            if (typeof(Component).IsAssignableFrom(typeof(T)))
            {
                var prefab = Bundle.LoadAsset<GameObject>(bundleAssetName);
                return prefab.GetComponent<T>();
            }

            return Bundle.LoadAsset<T>(bundleAssetName);
        }

        return Resources.Load<T>(resourcesDirectoryAddress + "/" + bundleAssetName);
    }
}