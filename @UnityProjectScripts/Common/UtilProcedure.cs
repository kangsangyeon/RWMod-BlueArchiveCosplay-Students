using UnityEngine;

public static class UtilProcedure
{
    public static int ChildIndexOfSelf(this Transform _self)
    {
        if (_self.parent == null)
            return 0;

        int _childCount = _self.parent.childCount;
        for (int i = 0; i < _childCount; ++i)
        {
            if (_self.parent.GetChild(i) == _self)
                return i;
        }

        return 0;
    }
}