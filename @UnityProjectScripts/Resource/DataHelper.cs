using System;

public static class DataHelper
{
    public static string ToStringKr(this StudentAttribute _value)
    {
        switch (_value)
        {
            case StudentAttribute.Attack:
                return "딜러";
            case StudentAttribute.Defense:
                return "탱커";
            case StudentAttribute.Support:
                return "서포터";
            case StudentAttribute.Heal:
                return "힐러";
            default:
                throw new ArgumentOutOfRangeException(nameof(_value), _value, null);
        }
    }
}