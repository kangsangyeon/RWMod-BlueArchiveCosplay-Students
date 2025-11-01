using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GachaSelector : MonoBehaviour
{
    public class GachaInfo
    {
        public string schedule;
        public string infoText;
        public VideoClip backVideoClip;
        public Sprite btnImage;

        public PerInfo per;

        List<int>[] _table = new List<int>[5];

        public List<int> getTable(GachaManager.Rarity rarity)
        {
            if (_table[(int)rarity] == null)
                _table[(int)rarity] = new List<int>();
            return _table[(int)rarity];
        }
    }

    [System.Serializable]
    struct GachaInfoJsonWrapper
    {
        public PerInfo per;
        public string schedule;
        public string infoText;
        public int[] pickup;
        public int[] fes;
        public int[] s3;
        public int[] s2;
        public int[] s1;
    }

    [System.Serializable]
    public struct PerInfo
    {
        public float pickup, fes, per_s3, per_s2, per_s1;
        public override string ToString()
        {
            return $"pickup: {pickup}\nfes: {fes}\ns3: {per_s3}\ts2: {per_s2}\ts1: {per_s1}";
        }

        public PerInfo(float pickup, float fes, float per_s3, float per_s2, float per_s1)
        {
            this.pickup = pickup;
            this.fes = fes;
            this.per_s3 = per_s3;
            this.per_s2 = per_s2;
            this.per_s1 = per_s1;
        }
    }

    [Header("buttons")]
    public GameObject btnPrefab;
    public VideoPlayer backVideoPlayer;
    public Transform bannerBtnsParent;
    [Space]
    [Header("info")]
    public GameObject gachaSchedulPrefab;
    public GameObject infoTextPrefab;
    public GameObject hrPrefab;
    public Transform gachaInfoTextParent;

    List<GachaInfo> gachaInfos = new List<GachaInfo>();
    int nowSelect = 0;

    public GachaInfo GetNowInfo
    {
        get { return gachaInfos[nowSelect]; }
    }

    readonly string address = "UI/GachaList/";

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Gacha List Load Start");
        ListReset();
        CreateBannerBtn();
    }

    void ListReset()
    {
        gachaInfos.Clear();

        GachaInfoJsonWrapper wrapper = new GachaInfoJsonWrapper();
        string json, add;
        int i = 0;


        while (true)
        {
            ++i;

            GachaInfo temp = new GachaInfo();
            add = Path.Combine(address, i.ToString());
            var asset = Resources.Load<TextAsset>(add);
            if (asset != null)
            {
                json = asset.text;
                wrapper = JsonConvert.DeserializeObject<GachaInfoJsonWrapper[]>(json)[0];
            }
            else
            {
                Debug.Log(add + "is not found");
                break;
            }

            temp.schedule = wrapper.schedule;
            temp.infoText = wrapper.infoText;
            temp.backVideoClip = Resources.Load<VideoClip>(add);
            temp.btnImage = img();

            temp.per = wrapper.per;

            temp.getTable(GachaManager.Rarity.s1).AddRange(wrapper.s1);
            temp.getTable(GachaManager.Rarity.s2).AddRange(wrapper.s2);
            temp.getTable(GachaManager.Rarity.s3).AddRange(wrapper.s3);
            temp.getTable(GachaManager.Rarity.fes).AddRange(wrapper.fes);
            temp.getTable(GachaManager.Rarity.pickup).AddRange(wrapper.pickup);


            gachaInfos.Add(temp);
        }

        Sprite img()
        {
            Texture2D t = Resources.Load<Texture2D>(add);
            return Sprite.Create(t, new Rect(0, 0, t.width, t.height), Vector2.one / 2f);
        }
    }

    void CreateBannerBtn()
    {
        List<GameObject> btns = new List<GameObject>();
        var group = bannerBtnsParent.GetComponent<ToggleGroup>();

        for (int i = 0; i < gachaInfos.Count; ++i)
        {
            int index = i;
            GameObject btn = Instantiate(btnPrefab, bannerBtnsParent);
            var gi = gachaInfos[i];

            btn.GetComponent<Image>().sprite = gi.btnImage;
            var tgl = btn.GetComponent<Toggle>();
            tgl.group = group;
            tgl.onValueChanged.AddListener((value) => PressBannerBtn(value, index));

            btns.Add(btn);
        }

        btns[0].GetComponent<Toggle>().isOn = true;
    }

    public void PressBannerBtn(bool selected, int index)
    {
        if (!selected)
            return;

        var gi = gachaInfos[index];

        for (int i = 0; i < gachaInfoTextParent.childCount; ++i)
            Destroy(gachaInfoTextParent.GetChild(i).gameObject);

        if (!(string.IsNullOrEmpty(gi.schedule) && string.IsNullOrWhiteSpace(gi.schedule)))
        {
            var gs = Instantiate(gachaSchedulPrefab, gachaInfoTextParent);
            var gst = gs.GetComponentInChildren<Text>();
            gst.transform.parent.gameObject.SetActive(true);
            gst.text = gi.schedule;
        }

        string[] texts = gi.infoText.Split(new string[] { "<hr>" }, System.StringSplitOptions.None);
        for (int i = 0; i < texts.Length; ++i)
        {
            var g = Instantiate(infoTextPrefab, gachaInfoTextParent);
            g.GetComponent<Text>().text = texts[i];
            if (i != texts.Length - 1)
                Instantiate(hrPrefab, gachaInfoTextParent);
        }

        Debug.Log(gi.per.ToString());

        backVideoPlayer.clip = gi.backVideoClip;
        backVideoPlayer.Play();
    }
}
