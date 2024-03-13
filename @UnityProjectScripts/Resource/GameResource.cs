using System.Collections.Generic;
using UnityEngine;

public static class GameResource
{
    public static Dictionary<int, SchoolData> SchoolTable;
    public static Dictionary<int, ClubData> ClubTable;
    public static Dictionary<int, StudentData> StudentTable;

    public static AssetBundle Bundle;
    public static GameObject ClubPrefab;
    public static GameObject StudentPrefab;
    public static Sprite SynergeActivatedSprite;
    public static Sprite SynergeDeactivatedSprite;
    public static Dictionary<int, Sprite> SchoolLogoSprites;
    public static Dictionary<int, Sprite> StudentPortraitSprites;
    public static List<Sprite> StudentAttributeFrameSprites;
}