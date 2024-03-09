using System.Collections.Generic;
using UnityEngine;

public static class GameResource
{
    public static Dictionary<string, SchoolData> SchoolTable;
    public static Dictionary<string, ClubData> ClubTable;
    public static Dictionary<string, StudentData> StudentTable;

    public static AssetBundle Bundle;
    public static GameObject ClubPrefab;
    public static GameObject StudentPrefab;
    public static Dictionary<string, Sprite> StudentThumbnailSprites;
    public static List<Sprite> StudentAttributeFrameSprites;
}