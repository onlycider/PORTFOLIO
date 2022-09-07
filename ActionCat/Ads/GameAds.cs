using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAds
{
    private Dictionary<int, AdsUnit> m_adsUnitInfos;
    public Dictionary<int, AdsUnit> adsUnitInfos{get{return m_adsUnitInfos;}}

    private Dictionary<int, AdsUnit> m_shopAds;

    public GameAds(Dictionary<string, object>[] datas)
    {
        m_adsUnitInfos = new Dictionary<int, AdsUnit>();
        m_shopAds = new Dictionary<int, AdsUnit>();
        foreach(Dictionary<string, object> data in datas)
        {
            AdsUnit adsUnit = new AdsUnit(data);
            m_adsUnitInfos.Add(adsUnit.index, adsUnit);

            if(adsUnit.adsType == AdsUnit.AdsType.Normal)
            {
                int rewardIndex = adsUnit.adsRewardItem.rewardIndex;
                m_shopAds.Add(rewardIndex, adsUnit);
            }
        }
    }

    public AdsUnit GetShopAds(int productId)
    {
        AdsUnit adsUnit = null;
        if(m_shopAds.ContainsKey(productId))
            adsUnit = m_shopAds[productId];

        return adsUnit;
    }
}
