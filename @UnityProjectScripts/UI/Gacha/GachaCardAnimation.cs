using System.Collections;
using UniRx.Operators;
using UnityEngine;
using UnityEngine.UI;
public class GachaCardAnimation : MonoBehaviour
{
    public GameObject cardsResultArea;
    public GachaResultTrain grt;
    public Transform[] cards;

    public float startScale;
    public float startPosition;
    public float movingTime;
    public float waitTime;

    public Sprite[] cardSprite;

    GachaManager.GachaResultInfo[] infos;

    void Start()
    {
        StopAllCoroutines();
        for (int i = 0; i < cards.Length; ++i)
            cards[i].gameObject.SetActive(false);
    }
    bool isRencha;

    public void SetCards(params GachaManager.GachaResultInfo[] infos)
    {
        this.infos = infos;
        cardsResultArea.SetActive(false);
        for (int i = 0; i < infos.Length; ++i)
        {
            switch (infos[i].rarity)
            {
                case GachaManager.Rarity.s1:
                    cards[i].gameObject.GetComponent<Image>().sprite = cardSprite[0];
                    break;
                case GachaManager.Rarity.s2:
                    cards[i].gameObject.GetComponent<Image>().sprite = cardSprite[1];
                    break;
                default:
                    cards[i].gameObject.GetComponent<Image>().sprite = cardSprite[2];
                    break;
            }
            cards[i].gameObject.SetActive(false);
        }

        for (int i = infos.Length; i < cards.Length; ++i)
            cards[i].gameObject.SetActive(false);

        isRencha = infos.Length == 10;
    }

    public void PlayAnimation()
    {
        cardsResultArea.SetActive(true);
        if (isRencha)
            StartCoroutine(AllCardAnimationCoroutine());
        else
        {
            for (int i = 1; i < cards.Length; ++i)
                cards[i].gameObject.SetActive(false);
            StartCoroutine(cardAnimationCoroutine(0));
        }
    }

    public void SkipAnimation()
    {
        cardsResultArea.SetActive(true);
        StopAllCoroutines();
        for (int i = 0; i < cards.Length; ++i)
        {
            cards[i].gameObject.SetActive(true);
            cards[i].transform.localScale = Vector3.one;
            cards[i].transform.localPosition = Vector3.zero;
        }

        CardEnd();
    }

    IEnumerator AllCardAnimationCoroutine()
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            StartCoroutine(cardAnimationCoroutine(i));
            yield return new WaitForSecondsRealtime(waitTime);
        }
        while (!isAllCardStop())
            yield return null;
        CardEnd();
    }

    void CardEnd()
    {
        cardsResultArea.SetActive(false);
        grt.gameObject.SetActive(true);
        grt.StartResult(infos);
    }

    bool isAllCardStop()
    {
        for (int i = 0; i < cards.Length; ++i)
        {
            var t = cards[i].transform;
            var isStop = t.localPosition == Vector3.zero && t.localScale == Vector3.one;
            if (!isStop)
                return false;
        }
        return true;
    }

    IEnumerator cardAnimationCoroutine(int index)
    {
        var card = cards[index];
        var dir = new Vector3(-1, 1, 0);

        card.localScale = Vector3.one * startScale;
        card.localPosition = dir * startPosition;

        var moveSpeed = startPosition / movingTime;
        var scaleSpeed = (startScale - 1) / movingTime;

        bool complete()
        {
            return
                card.localPosition.x >= 0 &&
                card.localPosition.y <= 0 &&
                card.localScale.x <= 1 &&
                card.localScale.y <= 1;

        }

        card.gameObject.SetActive(true);
        while (!complete())
        {
            yield return new WaitForEndOfFrame();

            if (card.localPosition.x <= 0 && card.localPosition.y >= 0)
                card.localPosition += -dir * moveSpeed * Time.deltaTime;
            else
                card.localPosition = Vector3.zero;

            if (card.localScale.x >= 1 && card.localScale.y >= 1)
                card.localScale -= Vector3.one * scaleSpeed * Time.deltaTime;
            else
                card.localScale = Vector3.one;
        }
        card.localPosition = Vector3.zero;
        card.localScale = Vector3.one;
    }
}