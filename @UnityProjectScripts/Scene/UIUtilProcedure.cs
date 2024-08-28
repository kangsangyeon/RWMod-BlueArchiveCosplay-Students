using UnityEngine;

public static class UIUtilProcedure
{
    public static Color GetAttributeColor(StudentAttribute _attribute)
    {
        Color _color = Color.white;

        if (_attribute == StudentAttribute.Attack)
            ColorUtility.TryParseHtmlString("#CC1A25", out _color);
        else if (_attribute == StudentAttribute.Defense)
            ColorUtility.TryParseHtmlString("#065BAB", out _color);
        else if (_attribute == StudentAttribute.Support)
            ColorUtility.TryParseHtmlString("#BD8801", out _color);
        else if (_attribute == StudentAttribute.Heal)
            ColorUtility.TryParseHtmlString("#88F284", out _color);

        return _color;
    }
}