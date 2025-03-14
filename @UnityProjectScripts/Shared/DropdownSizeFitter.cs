using UnityEngine;

public class DropdownSizeFitter : MonoBehaviour
{
    public RectTransform source;
    public RectTransform target;
    public float maxHeight;

    void Update()
    {
        float height = source.sizeDelta.y;

        var tf = source.parent;
        while (tf != null && tf != target.transform)
        {
            var rect = tf.GetComponent<RectTransform>();
            height += rect.offsetMax.y * -1;
            height += rect.offsetMin.y;

            tf = tf.parent;
        }

        var sizeDelta = target.sizeDelta;
        sizeDelta.y = Mathf.Min(height, maxHeight);
        target.sizeDelta = sizeDelta;
    }
}