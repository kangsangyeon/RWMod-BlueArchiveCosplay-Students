using UnityEngine;

public class DropdownSizeFitter : MonoBehaviour
{
    public RectTransform source;
    public RectTransform target;
    public float maxHeight;

    void Update()
    {
        float _height = source.sizeDelta.y;

        var _transform = source.parent;
        while (_transform != null && _transform != target.transform)
        {
            var _rect = _transform.GetComponent<RectTransform>();
            _height += _rect.offsetMax.y * -1;
            _height += _rect.offsetMin.y;

            _transform = _transform.parent;
        }

        var _sizeDelta = target.sizeDelta;
        _sizeDelta.y = Mathf.Min(_height, maxHeight);
        target.sizeDelta = _sizeDelta;
    }
}