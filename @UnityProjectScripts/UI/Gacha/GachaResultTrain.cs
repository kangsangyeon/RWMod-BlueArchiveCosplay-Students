using BA;
using DG.Tweening;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultTrain : MonoBehaviour
{
    enum Status
    {
        rare, logo, charactor
    }
    Status nowStatus;

    GachaManager.GachaResultInfo[] infos;

    public Image backgroundImage;
    public Image CardImage;
    public Image logoImage;
    public RectTransform fullShotImage;
    public UnityEngine.Animation aniPlayer;
    public GameObject[] resultArea;

    public Image infoLogo;
    public Text infoSchool, infoClub, infoRole, infoGun, infoBirth;

    public Transform nameStar;
    public Text nameText;

    public Button skipButton;

    public AnimationClip r1, r2, r3;
    public Sprite sr1, sr2, sr3;
    Color blue = new Color(0.65f, 1f, 1f, 1f);
    Color yello = new Color(1f, 0.89f, 0.63f, 1f);
    Color pink = new Color(0.94f, 0.8f, 0.98f, 1f);

    Sprite logoSprite;

    int index;

    public bool btn;
    public GachaManager.Rarity rarity;
    public int id;

    void Update()
    {
        if (btn)
        {
            GachaManager.GachaResultInfo info = new();
            info.rarity = rarity;
            info.id = id;
            CharDataSet(info);
            btn = false;
        }

        if (aniPlayer[aniPlayer.clip.name].normalizedTime >= 1)
            aniSkip();
    }
    public void StartResult(GachaManager.GachaResultInfo[] infos)
    {
        string s = "";
        foreach(var v in infos)
            s += $"{v.id}: {v.rarity}\n";
        Debug.Log(s);
        ResultsSet(infos);
        Next();
    }

    void ResultsSet(GachaManager.GachaResultInfo[] infos)
    {
        this.infos = infos;
        index = 0;
    }

    void Next()
    {
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(aniSkip);
        if (index < infos.Length)
        {
            CharDataSet(infos[index]);
            aniPlayer[aniPlayer.clip.name].time = 0;
            Debug.Log(aniPlayer.clip.name);
            aniPlayer.Play();
            ++index;
        }
        else
            AllSkip();
    }

    public void aniSkip()
    {
        skipButton.onClick.RemoveAllListeners();
        skipButton.onClick.AddListener(Next);

        aniPlayer[aniPlayer.clip.name].normalizedTime = 1;
    }

    public void AllSkip()
    {
        for (int i = 0; i < resultArea.Length; ++i)
            resultArea[i].SetActive(false);
    }

    void CharDataSet(GachaManager.GachaResultInfo info)
    {
        Debug.Log($"{info.id}: {info.rarity}");
        var id = info.id;
        var data = GameResource.StudentTable[id];
        var fra = FindObjectOfType<FullshotRenderAccessor>();
        fra.Fullshot.sprite =
            GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_{data.Id}");
        fra.FullshotHalo.sprite =
            GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_Halo_{data.Id}");
        fra.FullshotBg.sprite =
            GameResource.Load<Sprite>($"Student/{data.Id}", $"Student_Fullshot_Bg_{data.Id}");
        Vector3 v = data.CamPos; v.z = -10f;
        fra.Camera.transform.localPosition = v;
        fra.Camera.orthographicSize = data.CamOrthoSize;

        var clubData = GameResource.ClubTable.Values
        .First(x => x.StudentList.Contains(id));
        var schoolData = GameResource.SchoolTable.Values
        .First(x => x.ClubList.Contains(clubData.Id));
        string bgPath = string.IsNullOrEmpty(data.OverrideBgPath) ?
            schoolData.BgPath : data.OverrideBgPath;

        switch (info.rarity)
        {
            case GachaManager.Rarity.s1:
                aniPlayer.clip = r1;
                CardImage.sprite = sr1;
                backgroundImage.sprite = null;
                backgroundImage.color = blue;
                break;
            case GachaManager.Rarity.s2:
                aniPlayer.clip = r2;
                CardImage.sprite = sr2;
                backgroundImage.sprite = null;
                backgroundImage.color = yello;
                break;
            default:
                aniPlayer.clip = r3;
                CardImage.sprite = sr3;
                backgroundImage.color = Color.white;
                var directory = Path.GetDirectoryName(bgPath);
                var fileName = Path.GetFileName(bgPath);
                backgroundImage.sprite = GameResource.Load<Sprite>(directory, fileName);
                break;
        }

        var fullshotRect = (RectTransform)fullShotImage.transform;
        var fullshotTf = fra.Fullshot.transform;
        var fullshotHaloTf = fra.FullshotHalo.transform;
        var fullshotBgTf = fra.FullshotBg.transform;


        if (data.FrontHalo)
            fra.FullshotHalo.sortingOrder = 10; // fullshot과 fullshot face의 order는 5~6
        else
            fra.FullshotHalo.sortingOrder = 4;

        var schoolTable = GameResource.SchoolTable;

        logoSprite = GameResource.SchoolLogoSprites[schoolData.Id];
        logoImage.sprite = logoSprite;

        nameText.text = data.Name;
        int rare = Mathf.Min((int)rarity, 2) + 1;
        for(int i = 0; i < nameStar.childCount; ++i)
        {
            var s = nameStar.GetChild(i);
            if (i < rare)
                s.gameObject.SetActive(true);
            else
                s.gameObject.SetActive(false);
        }

        var gunData = GameResource.WeaponTable[data.WeaponId];

        infoLogo.sprite = logoSprite;
        //infoSchool, infoClub, infoRole, infoGun, infoBirth
        infoSchool.text = schoolData.Name;
        infoClub.text = clubData.Name;
        infoRole.text = data.Attribute.ToStringKr();
        infoGun.text = gunData.Type.ToString();

        string[] t = new string[] { "도망쳐...", "살려줘...", "살..."};
        try
        {
            infoBirth.text = "<color=#ff0000>" + t[Random.Range(0, t.Length + 1)] + "</color>";
        }
        catch(System.Exception e)
        {
            infoBirth.text = "<color=#ff0000>" + e + "</color>";
        }
    }
}