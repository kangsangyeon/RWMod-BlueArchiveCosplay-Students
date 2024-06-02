using UnityEngine;

public enum StudentAttribute
{
    Attack,
    Defense,
    Support,
    Heal
}

public class StudentData
{
    public int Id;
    public string Name;
    public int Age;
    public StudentAttribute Attribute;
    public int SkillId;
    public int WeaponId;

    public int DefaultShooting;
    public int DefaultMelee;
    public int DefaultConstruction;
    public int DefaultMining;
    public int DefaultCooking;
    public int DefaultPlants;
    public int DefaultAnimals;
    public int DefaultCrafting;
    public int DefaultArtistic;
    public int DefaultMedical;
    public int DefaultSocial;
    public int DefaultIntellectual;

    public Vector2 FullshotHaloSize;
    public Vector3 FullshotHaloStartPos;
    public Vector3 FullshotHaloEndPos;
    public Vector4 FullshotRectOffset;
}