public enum WeaponType
{
    AR,
    SR,
    SMG,
    HG,
    GL,
    MG,
    MountMG,
    MT,
    RF,
    RG,
    RL,
    SG,
    DualSG,
    DualSMG
}

public class WeaponData
{
    public int Id;
    public string Name;
    public int Star;
    public WeaponType Type;
    public string Description;
}