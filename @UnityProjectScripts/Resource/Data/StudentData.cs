using UnityEngine;

namespace BA
{
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
        public int PassiveSkillId;
        public int WeaponId;
        public int DefaultStar;
        public int? MaxSkillLevel;

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

        public Vector2 FullshotHaloPos;
        public Vector2 FullshotBgPos;
        public Vector2 CamPos;
        public float CamOrthoSize;
        public bool FrontHalo;
        public string OverrideBgPath;
    }
}