using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameDatabase
{
    public ShopProduct shopProducts 
    {
        get
        {
            if(m_shopProducts == null)
                m_shopProducts = new ShopProduct(
                    LoadJsonData("ShopProduct"),
                    LoadJsonData("PackageInfo"),
                    LoadJsonData("PackageContents"));
            
            return m_shopProducts;
        }
    }
    private ShopProduct m_shopProducts;

    public WorldRewardInfo worldRewardInfos
    {
        get
        {
            if (m_worldRewardInfos == null)
                m_worldRewardInfos = new WorldRewardInfo(LoadJsonData("WorldReward"));
        
            return m_worldRewardInfos;
        }
    }
    private WorldRewardInfo m_worldRewardInfos;

    public PassiveCardUpPriceInfo passiveCardUpPriceInfos
    {
        get
        {
            if (m_passiveCardUpPriceInfos == null)
                m_passiveCardUpPriceInfos = new PassiveCardUpPriceInfo(LoadJsonData("PassiveCardUpPrice"));

            return m_passiveCardUpPriceInfos;
        }
    }
    private PassiveCardUpPriceInfo m_passiveCardUpPriceInfos;

    public HeroStat heroStats
    {
        get
        {
            if(m_heroStats == null)
                m_heroStats = new HeroStat(LoadJsonData("HeroStat"));
            
            return m_heroStats;
        }
    }
    
    private HeroStat m_heroStats;


    public HeroStarData heroStarDatas
    {
        get
        {
            if(m_heroStarDatas == null)
                m_heroStarDatas = new HeroStarData(LoadJsonData("HeroStar"));
            
            return m_heroStarDatas;
        }
    }
    
    private HeroStarData m_heroStarDatas;

    public Dictionary<int, int> heroUpItems
    {
        get
        {
            if(m_HeroUpItems == null)
            {
                m_HeroUpItems = new Dictionary<int, int>();
                Dictionary<string, object>[] datas = LoadJsonData("HeroUpItem");
                foreach(Dictionary<string, object> data in datas)
                {
                     m_HeroUpItems.Add( (int)data["hero_id"] , (int)data["item_Id"]);
                }
                
            }
            return m_HeroUpItems;
        }
    }
    
    private Dictionary<int, int> m_HeroUpItems;

    public HeroUseable heroUseable
    {
        get
        {
            if(m_heroUseable == null)
                m_heroUseable = new HeroUseable(LoadJsonData("HeroUseable"));
            
            return m_heroUseable;
        }
    }
    
    private HeroUseable m_heroUseable;

    public HeroUpPrice heroUpPrices
    {
        get
        {
            if(m_HeroUpPrices == null)
                m_HeroUpPrices = new HeroUpPrice(LoadJsonData("HeroLevelUpPrice"));
            
            return m_HeroUpPrices;
        }
    }

    private HeroUpPrice m_HeroUpPrices;

    public Items items
    {
        get
        {
            if(m_items == null)
                m_items = new Items(LoadJsonData("Item"));

            return m_items;
        }
    }

    private Items m_items;

    public UserLevelExp userLvExps
    {
        get
        {
            if(m_UserLvExps == null)
                m_UserLvExps = new UserLevelExp( LoadJsonData("UserLvExp") );
                
            return m_UserLvExps;
        }
    }
    private UserLevelExp m_UserLvExps;

    public EquipStats equipStats
    {
        get
        {
            if(m_equipStats == null)
                m_equipStats = new EquipStats(LoadJsonData("EquipStat"));

            return m_equipStats;
        }
    }

    private EquipStats m_equipStats;

    public PassiveCards passiveCards
    {
        get
        {
            if(m_passiveCards == null)
                m_passiveCards = new PassiveCards(LoadJsonData("PassiveCard"));

            return m_passiveCards;
        }
    }

    private PassiveCards m_passiveCards;


     public SkillStats skillstats
    {
        get
        {
            if(m_skillstats == null)
                m_skillstats = new SkillStats(LoadJsonData("SkillStat"));

            return m_skillstats;
        }
    }

    private SkillStats m_skillstats;



    public SpecailSkills specailSkills
    {
        get
        {
            if(m_specailSkills == null)
                m_specailSkills = new SpecailSkills(LoadJsonData("SpecailSkillStat"));

            return m_specailSkills;
        }
    }

    private SpecailSkills m_specailSkills;

    public MonsterStats monsterStats
    {
        get
        {
            if(m_monsterStats == null)
                m_monsterStats = new MonsterStats(LoadJsonData("MonsterStat"));

            return m_monsterStats;
        }
    }

    private MonsterStats m_monsterStats;

    public EquipReinForcePrices equipReinForcePrice
    {
        get
        {
            if(m_equipReinForcePrice == null)
                m_equipReinForcePrice = new EquipReinForcePrices(LoadJsonData("EquipReinForcePrice"));

            return m_equipReinForcePrice;
        }
    }

    private EquipReinForcePrices m_equipReinForcePrice;


    public EquipReinForceRates EquipReinForceRate
    {
        get
        {
            if(m_equipReinForceRate == null)
                m_equipReinForceRate = new EquipReinForceRates(LoadJsonData("EquipReinForceRate"));

            return m_equipReinForceRate;
        }
    }

    private EquipReinForceRates m_equipReinForceRate;

    



    public UnLockCardSlotInfo unLockCardSlotInfo
    {
        get
        {
            if (m_unLockCardSlotInfo == null)
                m_unLockCardSlotInfo = new UnLockCardSlotInfo(LoadJsonData("UnLockCardSlot"));

            return m_unLockCardSlotInfo;
        }
    }
    private UnLockCardSlotInfo m_unLockCardSlotInfo;

    public StageRewards stageRewards
    {
        get
        {
            if (m_stageReward == null)
                m_stageReward = new StageRewards(LoadJsonData("StageReward"));

            return m_stageReward;
        }
    }
    private StageRewards m_stageReward;

    public InAppList inAppList
    {
        get
        {
            if(m_inappList == null)
                m_inappList = new InAppList(LoadJsonData("InAppList"));

            return m_inappList;
        }
    }
    private InAppList m_inappList;

    public InventoryExpansionList inventoryExpansionInfo
    {
        get
        {
            if (m_inventoryExpansionInfo == null)
                m_inventoryExpansionInfo = new InventoryExpansionList(LoadJsonData("InventoryExpansion"));

            return m_inventoryExpansionInfo;
        }
    }
    private InventoryExpansionList m_inventoryExpansionInfo;

    public NativeShopProducts nativeShopProducts
    {
        get
        {
            if (m_nativeShopProducts == null)
                m_nativeShopProducts = new NativeShopProducts(LoadJsonData("NativeShopList"));

            return m_nativeShopProducts;
        }
    }
    private NativeShopProducts m_nativeShopProducts;

    public GameAds adsRewards
    {
        get
        {
            if(m_adRewards == null)
                m_adRewards = new GameAds(LoadJsonData("AdsReward"));

            return m_adRewards;
        }
    }
    
    private GameAds m_adRewards;

    public StageGroupData StageGroupDatas
    {
        get
        {
            if (m_StageGroupData == null)
                m_StageGroupData = new StageGroupData(LoadJsonData("StageGroupData"));

            return m_StageGroupData;
        }
    }
    private StageGroupData m_StageGroupData;
    


#region <Server Only..>
    public RankRewards m_RankSeasonRewards;
    public RankSeason m_RankSeasons;
    public  BattlePassSeason m_battlePassSeason;
    public BattlePassRewards m_battlePassRewards;
#endregion
}
