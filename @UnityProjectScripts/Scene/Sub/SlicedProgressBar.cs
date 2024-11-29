using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(RectTransform))]
[ExecuteAlways]
public class SlicedProgressBar : MonoBehaviour
{
    [Range(0f, 1f)] public float FillAmount = 0f;

    private RectTransform _parentRect;
    private RectTransform _rect;
    private Image _image;
    private float _minWidth;
    private float _cacheFillAmount;
    private Sprite _cacheSprite;
    private bool _needsUpdateImage;
    private bool _needsUpdateFill;
    private bool _needsInit;
    private bool _needsInitOnUpdate;
    private DrivenRectTransformTracker _tracker;

    private void OnEnable()
    {
        CacheComponents();
        InitComponentsIfNeeds();
        TryMarkUpdateImage();
        TryMarkUpdateFill();
    }

    private void OnDisable()
    {
        _tracker.Clear();
        _rect.hideFlags &= ~HideFlags.NotEditable;
        LayoutRebuilder.MarkLayoutForRebuild(_rect);
    }

    private void OnValidate()
    {
        CacheComponents();
        InitComponentsIfNeeds();
        TryMarkUpdateImage();
        TryMarkUpdateFill();
    }

    private void Update()
    {
        InitComponentsOnUpdateIfNeeds();

        TryMarkUpdateImage();
        TryMarkUpdateFill();
        UpdateImageIfNeeds();
        UpdateFillIfNeeds();
    }

    private void CacheComponents()
    {
        if (_parentRect == null)
            _parentRect = (RectTransform)transform.parent;
        if (_rect == null)
            _rect = (RectTransform)transform;
        if (_image == null)
            _image = GetComponent<Image>();
    }

    private void InitComponentsIfNeeds()
    {
        if (_needsInit)
            return;
        _needsInit = true;

        _rect.hideFlags |= HideFlags.NotEditable;

        // _tracker.Clear();
        // _tracker.Add(this, _rect,
        //     DrivenTransformProperties.Anchors | DrivenTransformProperties.AnchoredPosition |
        //     DrivenTransformProperties.SizeDelta | DrivenTransformProperties.Pivot |
        //     DrivenTransformProperties.Rotation | DrivenTransformProperties.Scale);
    }

    private void InitComponentsOnUpdateIfNeeds()
    {
        if (_needsInitOnUpdate)
            return;
        _needsInitOnUpdate = true;
        _needsUpdateFill = _needsUpdateImage = true;

        _rect.anchorMin = Vector2.zero;
        _rect.anchorMax = Vector2.one;
        _rect.pivot = Vector2.one * .5f;
        _rect.offsetMin = Vector2.zero;
        _rect.offsetMax = Vector2.zero;
    }

    public void TryMarkUpdateFill()
    {
        if (_cacheFillAmount != FillAmount)
        {
            _needsUpdateFill = true;
        }
    }

    public void TryMarkUpdateImage()
    {
        if (_cacheSprite != _image.sprite)
        {
            _needsUpdateImage = true;
        }
    }

    public void UpdateFillIfNeeds()
    {
        if (!_needsUpdateFill)
            return;
        _needsUpdateFill = false;
        UpdateFill();
    }

    public void UpdateImageIfNeeds()
    {
        if (!_needsUpdateImage)
            return;
        _needsUpdateImage = false;
        UpdateImage();
    }

    public void UpdateFill()
    {
        _cacheFillAmount = FillAmount;

        float parentWidth = _parentRect.rect.width;
        float width = (parentWidth - _minWidth) * FillAmount + _minWidth;
        _rect.offsetMax = new Vector2(width - parentWidth, _rect.offsetMax.y);
    }

    public void UpdateImage()
    {
        _cacheSprite = _image.sprite;

        _minWidth = _image.sprite != null
            ? _image.sprite.border.x + _image.sprite.border.z
            : 0;
    }
}