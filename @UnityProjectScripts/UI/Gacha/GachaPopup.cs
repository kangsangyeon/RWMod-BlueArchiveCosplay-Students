using System.Collections;
using System.Linq;
using UnityEngine;
public class GachaPopup : MonoBehaviour
{
    public ConfirmPopup popup;
    public GachaManager gacha;
    public GachaVideoManager videoManager;
    public GachaCardAnimation cardAnimation;

    GameObject _top;

    GameObject top
    {
        get
        {
            if (_top == null)
                _top = FindObjectOfType<ScreenTopBarAccessor>().gameObject;
            return _top;
        }
    }

    public GameObject gachaResult;

    public void Dancha()
    {
        popup.Popup("알림", "단차 ㄱ?", () => DanchaBtn());
    }

    public void Rencha()
    {
        popup.Popup("알림", "10연차 ㄱ?", () => RenchaBtn());
    }

    void DanchaBtn()
    {
        var info = gacha.Dancha();
        bool isRare = info.rarity != GachaManager.Rarity.s1 && info.rarity != GachaManager.Rarity.s2;
        cardAnimation.SetCards(info);

        gachaResult.SetActive(true);
        videoManager.VideoStart(isRare, false);
        CloseTop();
    }

    void RenchaBtn()
    {
        var infos = gacha.Rencha();
        bool hasRare = (from i in infos
                       where i.rarity != GachaManager.Rarity.s1
                       && i.rarity != GachaManager.Rarity.s2
                       select i)
                       .Count() > 0;
        cardAnimation.SetCards(infos);

        gachaResult.SetActive(true);
        videoManager.VideoStart(hasRare, true);
        CloseTop();
    }
    void CloseTop()
    {
        top.SetActive(false);
    }
}