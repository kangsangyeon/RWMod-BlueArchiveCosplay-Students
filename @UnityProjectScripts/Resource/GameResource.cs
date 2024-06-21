using System.Collections.Generic;
using UnityEngine;
using TemplateTable;

public static class GameResource
{
    public static SaveData Save;

    public static TemplateTable<int, SchoolData> SchoolTable;
    public static TemplateTable<int, ClubData> ClubTable;
    public static TemplateTable<int, StudentData> StudentTable;
    public static TemplateTable<int, SkillData> SkillTable;
    public static TemplateTable<(int, int), SkillLevelData> SkillLevelTable;
    public static TemplateTable<int, WeaponData> WeaponTable;
    public static TemplateTable<int, PassiveSkillData> PassiveSkillTable;
    public static TemplateTable<StudentAttribute, StudentAttributeData> StudentAttributeTable;

    public static AssetBundle Bundle;
    public static GameObject ClubPrefab;
    public static GameObject StudentPrefab;
    public static Sprite SynergyActivatedSprite;
    public static Sprite SynergyDeactivatedSprite;
    public static Dictionary<int, Sprite> SchoolLogoSprites;
    public static Dictionary<int, Sprite> StudentPortraitSprites;
    public static List<Sprite> StudentAttributeFrameSprites;

    public static T Load<T>(string _resourcesDirectoryAddress, string _bundleAssetName) where T : Object
    {
        if (Bundle != null)
            return Bundle.LoadAsset<T>(_bundleAssetName);
        else
            return Resources.Load<T>(_resourcesDirectoryAddress + "/" + _bundleAssetName);
    }
}