using DG.Tweening.Plugins;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

public class GachaManager : MonoBehaviour
{
    public enum Rarity
    {
        s1,
        s2,
        s3,
        fes,
        pickup,
    }
    public GachaSelector gs;

    public bool danchaBtn;
    public bool renchaBtn;
    public bool toManyDanchaBtn;
    public bool toManyRenchaBtn;
    public int howMany = 100000;
    void Update()
    {
        if (danchaBtn)
        {
            danchaBtn = false;
            Debug.Log(Dancha());
        }

        if (renchaBtn)
        {
            renchaBtn = false;
            Debug.Log(string.Join(" ", Rencha()));
        }

        if (toManyDanchaBtn)
        {
            toManyDanchaBtn = false;
            ToManyDancha();
        }
        if (toManyRenchaBtn)
        {
            toManyRenchaBtn = false;
            ToManyRencha();
        }
    }

    public int Dancha(bool isLast = false)
    {
        var now = gs.GetNowInfo;
        var r = GetGachaedRarity(now.per, isLast);
        var table = now.getTable(r);

        return table[Random.Range(0, table.Count)];
    }

    public int[] Rencha()
    {
        int[] r = new int[10];
        for (int i = 0; i < 9; ++i)
            r[i] = Dancha(false);
        r[9] = Dancha(true);

        return r;
    }

    void ToManyDancha()
    {
        Dictionary<int, int> dic = new Dictionary<int, int>();
        for (int i = 0; i < howMany; ++i)
        {
            int g = Dancha();
            if (dic.ContainsKey(g))
                ++dic[g];
            else
                dic.Add(g, 1);
        }
        dic = dic.OrderBy(v => v.Key).ToDictionary(x => x.Key, x => x.Value);

        StringBuilder result = new StringBuilder();
        foreach (var i in dic)
        {
            result.AppendLine($"{i.Key}\t{i.Value}");
        }

        Debug.Log(result.ToString());
    }

    void ToManyRencha()
    {
        Dictionary<int, int> dic = new Dictionary<int, int>();
        for (int i = 0; i < howMany; ++i)
        {
            var rg = Rencha();
            for(int j = 0; j < rg.Length; ++j)
            {
                int g = rg[j];
                if (dic.ContainsKey(g))
                    ++dic[g];
                else
                    dic.Add(g, 1);
            }
        }
        dic = dic.OrderBy(v => v.Key).ToDictionary(x => x.Key, x => x.Value);

        StringBuilder result = new StringBuilder();
        foreach (var i in dic)
        {
            result.AppendLine($"{i.Key}\t{i.Value}");
        }

        Debug.Log(result.ToString());
    }

    void GachaTest()
    {
        int[][] results = new int[4][];
        for (int i = 0; i < 4; ++i)
            results[i] = new int[6];
        int num = 100000;

        GachaSelector.PerInfo testNor = new GachaSelector.PerInfo();
        GachaSelector.PerInfo testFes = new GachaSelector.PerInfo();

        string result = "-\t1\t2\t3\tfes\tpickup\n";

        testNor = new GachaSelector.PerInfo(0.7f, 0, 3, 18.5f, 78.5f);
        testFes = new GachaSelector.PerInfo(0.7f, 0.3f, 6, 18.5f, 78.5f);
        for (int i = 0; i < num; ++i)
        {
            ++results[0][(int)GetGachaedRarity(testNor)];
            ++results[1][(int)GetGachaedRarity(testNor, true)];
            ++results[2][(int)GetGachaedRarity(testFes)];
            ++results[3][(int)GetGachaedRarity(testFes, true)];
        }
        result += ($"일반 모집:\t{string.Join("\t", results[0])}") + "\n";
        result += ($"일반 막타:\t{string.Join("\t", results[1])}") + "\n";
        result += ($"페스 모집:\t{string.Join("\t", results[2])}") + "\n";
        result += ($"페스 막타:\t{string.Join("\t", results[3])}") + "\n";

        Debug.Log(result);
    }

    bool canIGacha()
    {
        return true;
    }

    /// <summary>
    /// </summary>
    /// <param name="per"></param>
    /// <param name="isLast"></param>
    /// <returns></returns>
    Rarity GetGachaedRarity(GachaSelector.PerInfo per, bool isLast = false)
    {
        float total = per.per_s1 + per.per_s2 + per.per_s3;
        float roll = Random.Range(0, total);
        int result;

        // 1~3
        if (roll < per.per_s1)
            result = 0;
        else if (roll < per.per_s1 + per.per_s2)
            result = 1;
        else
            result = 2;

        if (isLast && result == 0)
            result = 1;

        if (result == 2)
        {
            float highRoll = Random.Range(0, per.per_s3);
            if (highRoll < per.fes)
                result = 3; // fes
            else if (highRoll < per.fes + per.pickup)
                result = 4; // pickup
        }

        return (Rarity)result;
    }
}
