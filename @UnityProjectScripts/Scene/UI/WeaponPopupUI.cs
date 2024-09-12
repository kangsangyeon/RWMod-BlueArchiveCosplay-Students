using System;
using UniRx;
using UnityEngine;

public class WeaponPopupUI : MonoBehaviour
{
    public WeaponPopupAccessor Accessor;
    public bool ViewMaxLevelInfo;

    private void Start()
    {
        Accessor.QuitButton.OnClickAsObservable()
            .Subscribe(_ => Accessor.gameObject.SetActive(false));
        Accessor.ConfirmButton.OnClickAsObservable()
            .Subscribe(_ => Accessor.gameObject.SetActive(false));
        Accessor.TabContents_ViewMaxLevelInfoCheckboxButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                ViewMaxLevelInfo = !ViewMaxLevelInfo;
                Accessor.TabContents_ViewMaxLevelInfoCheckboxButton_Check.gameObject.SetActive(ViewMaxLevelInfo);
            });

        Accessor.TabContents_ViewMaxLevelInfoCheckboxButton_Check.gameObject.SetActive(ViewMaxLevelInfo);
    }

    public void UpdateUI(WeaponData _data, int _level, int _exp)
    {
        Accessor.WeaponName.text = _data.Name;
        Accessor.Type.text = _data.Type.ToString();
        Accessor.Level.text = $"<i>Lv.{_level}</i>";
        Accessor.WeaponIcon.sprite =
            GameResource.Load<Sprite>($"Weapon", $"Weapon_Icon_{_data.Id}");
        for (int i = 0; i < _data.Star; ++i)
            Accessor.Stars[i].gameObject.SetActive(true);
        for (int i = _data.Star; i < 5; ++i)
            Accessor.Stars[i].gameObject.SetActive(false);
    }
}