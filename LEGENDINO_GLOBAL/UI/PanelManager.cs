using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum PanelTag
{
    NONE,
    Adventure,
    AchievementRewardPopup,
    AccelerateIncubationPopup,
    UserNameSetPopup,
    CodeInputPopup,
    GameOptionPopup,
    LoginSelectPopup,
    AccountOverwritePopup,
    CommonMessagePopup,
    ConfirmMessagePopup,
    GachaResultPanel,
    DinoSkillSetPanel,
    EvolutionSkillSetPanel,
	ArenaMatchWaitingPopup,
	FriendsPanel,
	UserDetailInfoPopup,
	FriendlyMatchInvitePopup,
	FriendlyMatchReceivePopup,
	FriendlyMatchResultPopup,
	PlayerContentsPanel,
	ArenaRankGradeRewardPanel,
	ArenaSeasonHistoryPopup,
	ArenaSeasonRankingRewardPopup,
	ArenaNewSeasonNoticePopup,
	CameraAccessPermissionPopup,
	ArenaSeasonResultPopup,
	ArenaSeasonGradeRewardPanel,
	ReceiveItemListPanel,
	SkinReceivePanel,
	DinoDevelopListPanel,
	DinoDevelopPanel,
	DinoDevelopAbilityPanel,
	DinoDevelopLevelupPopup,
	UserDinoInventoryPanel,
	ArenaSeasonRankingRewardPanel,
	SkillLevelInitPopup,
	AbilityLevelInitPopup,
	DNAPurchasePopup,
	InventoryExpansionPopup,
	ExchangeDinoSkillInfoPopup,
	AdsErrorPopup,
	TicketInventoryPopup,
	MissionPanel,
	ArenaShopPopup,
	ArenaRankingPopup,
	ArenaGradeDungeonClearPopup,
	PVPPausePopup,
	DinoExchangeChecker,
	NicknameChangePopup,
	BoxItemSelectPanel,
	ExchangeDNAPopup,
	IncubatorsPanel,
	ConfirmExchangeDinoToDNAPopup,
	DinoEggListPopup,

	GuildJoinPanel,
	GuildCreatePopup,
	GuildLobbyPanel,
	GuildInfoPanel,
	GuildDeportPanel,
	GuildExitPanel,
	GuildPositionPanel,
	GuildSettingPanel,
	GuildEmblemPanel,
	GuildItemRequestPopup,
	GuildItemRequestConfirmPopup,
	GuildSupportItemPopup,
	GuildAttendancePopup,
	GuildTransferMasterPositionPopup,
	GuildDisbandPopup,
	GuildSupportBoxPopup,
    GuildWarInfoPanel,
	GuildPointRewardPopup,
    GuildRankingRewardPopup,
	GuildWarResultRewardPopup,
    GuildWarNoticePanel,
	DinoLevelupPopup,

	NoticePopup,
	UpdateRecommendPopup,

	ShopPanel,
	PurchaseConfirmPopup,
	GachaConfirmPopup,
	GoldPurchaseConfirmPopup,
	StaticShopPopup,
	AdSkipPopup,
	DinoGachaTicketUsePopup,
	ReviewRecommendPopup,
	EventNoticePopup,
	EventNoticeDetailPopup,
	ArenaWinPopup,
	
	
	P_RewardGetView,
	P_AdStartSetting,
	P_AdMiniMap,
	P_ItemInfo,
	P_Sell,
	P_GradeRewardsInfo,
	P_TicketChanger,
	P_QuickLvup,
	P_MyInfo,
	P_SkinView,
	P_SkinChange,
	P_MyRepreDinoSet,
	P_EvolveView,
	P_DinoInfo,
	P_DinoItemViewer,
	P_DeckStyleHint,
	P_SkillReset,
	P_Shop,
	P_EvolveBuy,
	P_ItemBuy,
	P_Atlas,
	P_Collator,
	P_Exchange,
	P_PostBox,
	P_EventShop,
	P_Inventory,
	P_Buy,
	P_DinoSelectReward,
	P_ArenaShopGradeInfo,
	P_BattleDinoInfo,
	P_DinoGachaRate,
	P_DinoSimulator,
	P_DinoFieldSkillSelecter,
	P_DinoFieldSkillViewer,
	P_DinoFieldSkillOpen,
    P_AdMission,
	P_PowerUpSuggest,
    P_Adventure_Hard,
	P_CSOption,
	P_EquipUpgrade,
	P_ColResult,

	FriendsPresentPanel,
	IncubatorExpansionPopup,
	PopupMsgPopup,
	PopupSelecterPopup,
	EventPopup,
	AlarmPopup,
	MyInfoHintPanel,
	ArenaInfoPopup,
	LanguageChangePopup,
	AdventurePausePopup,
	ForceGameEndPopup,
	MyTotalGemPopup,
	PackageBuyViewerPopup,
	CommonSpineMessagePopup,
	DinoReinforcePanel,
	ReinforceResultPanel,
	DinoEvolutionPanel,
	EvolutionMaterialDinoPopup,
	AttributeGuidePopup,
	DinoDevelopAbilityInfoPanel,
	DinoDevelopSkillInfoPanel,
	DinoDevelopAbilityLevelUpPopup,
	DinoDevelopSkillLevelUpPopup,
}


/// <summary>
/// 추후 패널 동적생성 관리 체계 들어갈 예정
/// </summary>
public class PanelManager : MonoBehaviour {
	public static PanelManager Instance = null;

	private const string DIR_PANEL = "Panels/{0}";
	private const string TUTORIALS_DIR = "Tutorials/{0}";
	private List<Panel> panelList;

	private const int baseDepth = 90;
	private const int gap = 10;

	private Scene m_currentScene;
	
	/// <summary>
	/// Awake is called when the script instance is being loaded.
	/// </summary>
	void Awake()
	{
        //Debug.Log("####### PanelManager Awake ######## ");
        if (Instance != null)
        {
            if (this != Instance)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            panelList = new List<Panel>();
			m_currentScene = SceneManager.GetActiveScene();
        }
	}

    private void OnDestroy()
    {
        Instance = null;

		if(panelList != null) panelList.Clear();
        panelList = null;

        //Debug.Log("####### PanelManager OnDestroy ######## ");
    }

	public void PreloadPanelResource(PanelTag panelTag)
	{
		string path = string.Format(DIR_PANEL, panelTag.ToString());
		Resources.Load(path);
	}



    /// <summary>
    /// Adds the panel.
    /// </summary>
    /// <returns>The panel.</returns>
    /// <param name="panelTag">Panel.</param>
    /// <param name="info">Info.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
	public T AddPanel<T>(PanelTag panelTag, object info = null) where T : Panel
	{
		string path = string.Format(DIR_PANEL, panelTag.ToString());

		GameObject prefab = Resources.Load(path) as GameObject;
		GameObject o = Instantiate(prefab) as GameObject;

		o.transform.SetParent(transform);
		o.transform.localScale = Vector3.one;
		o.transform.localPosition = Vector3.zero;

		//
		int depth = baseDepth + (panelList.Count * gap);

		//불러오는 패널에는 무조건 Panel Monobehavior가 추가 되어있어야함
		Panel p = o.GetComponent<Panel>();
		p.panelTag = panelTag;
		p.SetPanelDepth(depth);
        
		panelList.Add(p);
        if (info != null)
            p.SetPanelInfo(info);

		o.SetActive(true);

		return p as T;
	}

	public TutorialChapter AddTutorialChapter(TutoChapterNum tutoChapterNum)
	{
		string path = string.Format(TUTORIALS_DIR, tutoChapterNum.ToString());
		GameObject prefab = Resources.Load(path) as GameObject;
		GameObject o = Instantiate(prefab) as GameObject;

		o.transform.SetParent(transform);
		o.transform.localScale = Vector3.one;
		o.transform.localPosition = Vector3.zero;


		int depth = 8000;
		TutorialChapter chapter = o.GetComponent<TutorialChapter>();
		chapter.SetPanelDepth(depth);
		o.SetActive(true);

		return chapter;
	}

	// public T AddPanel<T, K>(PanelTag panelTag, K info) where T : Panel
	// {
	// 	string path = string.Format(DIR_PANEL, panelTag.ToString());

	// 	GameObject prefab = Resources.Load(path) as GameObject;
	// 	GameObject o = Instantiate(prefab) as GameObject;

	// 	o.transform.SetParent(transform);
	// 	o.transform.localScale = Vector3.one;
	// 	o.transform.localPosition = Vector3.zero;

	// 	//
	// 	int depth = baseDepth + (panelList.Count * gap);

	// 	//불러오는 패널에는 무조건 Panel Monobehavior가 추가 되어있어야함
	// 	Panel p = o.GetComponent<Panel>();
	// 	p.panelTag = panelTag;
	// 	p.SetPanelDepth(depth);
        
	// 	panelList.Add(p);
    //     // if (info != null)
	// 	p.SetPanelInfo<K>(info);

	// 	o.SetActive(true);

	// 	return p as T;
	// }




	public T GetPanel<T>(PanelTag panelTag) where T : Panel
	{
		Panel panel = null;

		if (panelList.Count == 0)
			return null;

		for(int i = 0; i < panelList.Count; i++)
		{
			if (panelList[i].panelTag.Equals(panelTag))
				panel = panelList[i];
		}

		if (panel == null)
			return null;
		
		return panel as T;
	}

    public Panel GetTopPanel()
    {
        Panel panel = null;

        if(panelList == null || panelList.Count == 0)
            return panel;
        
        int lastIndex = panelList.Count - 1;

        panel = panelList[lastIndex]; 
        return panel;
    }

	//TopPanelDepth 가져오기..
	public int GetTopPanelDepth()
	{
		if(panelList == null || panelList.Count == 0)
			return 0;
		
		PanelTag _Tag =PanelTag.NONE;
		int lastIndex = panelList.Count -1;
		int resultDepth = 0;
		do
		{
			if( lastIndex < 0) return 0;

			_Tag = panelList[lastIndex].panelTag;
			resultDepth =  panelList[lastIndex].panelDepth;
			--lastIndex;
		}while( IsSkipDepth(_Tag) );



		return resultDepth+IsAddDepth(_Tag); 
	}
	bool IsSkipDepth(PanelTag _tag)
	{
		if (_tag == PanelTag.P_DinoInfo) return true;

		return false;
	}
	int IsAddDepth(PanelTag _tag)
	{
		if (_tag == PanelTag.P_MyInfo) return 8;

		return 0;
	}

	/// <summary>
	/// 최상위 패널 삭제
	/// </summary>
	public void RemoveTopPanel()
	{
		if(panelList.Count < 1)
		{
			//ERROR
			return;
		}
		int topIndex = panelList.Count - 1;
		Panel p = panelList[topIndex];

		Destroy(p.gameObject);
		panelList.Remove(p);
	}

	/// <summary>
	/// 팝업 지정 삭제
	/// </summary>
	/// <param name="panelTag"></param>
	public void RemovePanel(PanelTag panelTag)
	{
		if(panelList.Count == 0)
		{
			//ERROR
			return;
		}
		Panel p = null;
		for(int i = 0; i < panelList.Count; i++){
			if(panelList[i].panelTag.Equals(panelTag))
				p = panelList[i];
		}

		if(p != null)
		{
			Destroy(p.gameObject);
			panelList.Remove(p);
		}
		// if(panelObjects.ContainsKey(panel))
		// {
		// 	Destroy(panelObjects[panel]);
		// 	panelObjects.Remove(panel);
		// }
	}

    /// <summary>
    /// Removes the panel.
    /// </summary>
    /// <param name="panelObj">Panel object.</param>
	public void RemovePanel(Panel panelObj)
	{
		if (panelList.Count == 0)
        {
            //ERROR
            return;
        }

		//닫을때 동작중인 코루틴 스탑..
//		panelObj.StopAllCoroutines ();

		Destroy(panelObj.gameObject);
		panelList.Remove(panelObj);
	}


    //사용 금지
    public void SetActiveAllPanel(bool active)
    {
        if (panelList == null || panelList.Count < 1)
            return;

        for (int i = 0; i < panelList.Count; i++)
        {
            panelList[i].gameObject.SetActive(active);
        }
    }

	// 팝업이 뜬게 있는지 없는지 확인한다.
	public bool DisplayedPanel()
	{
		bool displayed = false;
		// 동적 생성 팝업이 있는지 체크
		if(panelList.Count > 0)
		{
			displayed = true;
			return displayed;
		}

		return displayed;
	}

	public bool GetCurrentActiveSceneIsMainLobby()
	{
		bool isMainLobby = false;
		if (m_currentScene.name == "WorldMap_2")
			isMainLobby = true;
		
		return isMainLobby;
	}

	public int GetPanelListCnt()
	{
		return panelList.Count;
	}

	public void RemoveAllPanel()
	{
		if (panelList.Count == 0)
        {
            //ERROR
            return;
        }

		foreach(Panel panel in panelList)
		{
			Destroy(panel.gameObject);
		}

		panelList.Clear();
	}


	

}


