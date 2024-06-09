using System.Collections.Generic;

public class SaveData
{
    public int Pyroxenes;
    public Dictionary<int, StudentSaveData> StudentSaveData;
}

public class StudentSaveData
{
    public int Id;
    public bool Unlock;
    public int Level;
    public int Exp;
}