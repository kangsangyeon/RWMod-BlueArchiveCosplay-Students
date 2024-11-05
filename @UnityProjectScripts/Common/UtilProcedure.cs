using UnityEngine;

public static class UtilProcedure
{
    public static int ChildIndexOfSelf(this Transform self)
    {
        if (self.parent == null)
            return 0;

        int childCount = self.parent.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            if (self.parent.GetChild(i) == self)
                return i;
        }

        return 0;
    }
}