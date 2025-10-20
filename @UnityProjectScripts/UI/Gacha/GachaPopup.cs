using System.Collections;
using UnityEngine;
public class GachaPopup : MonoBehaviour
{
    public ConfirmPopup popup;
    public GachaManager gacha;

    public void Dancha()
    {
        popup.Popup("알림", "단차 ㄱ?", () => Debug.Log(gacha.Dancha()));
    }

    public void Rencha()
    {
        popup.Popup("알림", "10연차 ㄱ?", () => DebugRencha());
    }
    void DebugRencha()
    {
        Debug.Log(string.Join(" ", gacha.Rencha()));
    }
}