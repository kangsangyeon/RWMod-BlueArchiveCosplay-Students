﻿using System.Collections.Generic;

namespace BA
{
    public class SaveData
    {
        public int Pyroxenes;
        public int Silvers;
        public int Eligma;
        public Dictionary<int, StudentSaveData> StudentSaveData;
    }

    public class StudentSaveData
    {
        public int Id;
        public bool Unlock;
        public int Level;
        public int Exp;
        public int Shinbi;
    }
}