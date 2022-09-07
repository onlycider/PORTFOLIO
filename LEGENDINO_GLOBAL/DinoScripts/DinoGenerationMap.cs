using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DinosaurAttribute{
	//(0 : 무속성, 1 : 불, 2 : 물, 3 : 숲, 4 : 빛, 5 : 암)
	NONE = 0,
	FIRE = 1,
	WATER = 2,
	FOREST = 3,
	LIGHT = 4,
	DARK = 5,
	GOODS_GOLD = 100,
	GOODS_GEMSTONE = 101,
	SKILL_INITIATOR = 102,
}

public class UserDinosaur {
	private int m_userDinoIndex;
	public int userDinoIndex{get{return m_userDinoIndex;}}

	private DinosaurData m_dinoData;
	public DinosaurData dinoData{get{return m_dinoData;}}

	private DinoInfo m_dinoInfo;
	public DinoInfo dinoInfo{get{return m_dinoInfo;}}

	private ItemInfo m_dnaPieceItem;
	public ItemInfo dnaPieceItem{get{return m_dnaPieceItem;}}

	private int m_dnaPieceItemIndex;
	public int dnaPieceItemIndex{get{return m_dnaPieceItemIndex;}}


	public UserDinosaur(DinoInfo info)
	{
		m_userDinoIndex = info.nUniqueIndex;
		m_dinoInfo = info;
		m_dinoData = GamingInfo.Instance.dinoSpeciesMap.dinosaurs[info.nIndex];
		m_dnaPieceItemIndex = m_dinoData.dnaPieceItemIndex;
		// m_dnaPieceItem = GamingInfo.Instance.GetItemInfo(m_dnaPieceItemIndex);

		// 필수 체크 사항
		m_dnaPieceItem = GamingInfo.Instance.GetMyItemByIdx(m_dnaPieceItemIndex);
	}

	public int GetDNAPieceCountExchangeDino()
	{
		int count = m_dinoData.exchangeDNAPieceCount;
		return count;
	}
	
}

[Serializable]
public class DinosaurData
{
	private int m_index;
	public int index{get{return m_index;}}

	private DinosaurAttribute m_attribute;
	public DinosaurAttribute attribute{get{return m_attribute;}}

	private int m_rank = 1;
	public int rank{get{return m_rank;}}

	// +0.5성(초월 등급이라고 명칭)
	private bool m_transcendence = false;
	public bool transcendence{get{return m_transcendence;}}

	private int m_generation;
	public int generation{get{return m_generation;}}

	private int m_representDinoIndex;
	public int representDinoIndex{get{return m_representDinoIndex;}}

	private int m_dnaPieceItemIndex;
	public int dnaPieceItemIndex{get{return m_dnaPieceItemIndex;}}

	private int m_exchangeDNAPieceCount;
	public int exchangeDNAPieceCount{get{return m_exchangeDNAPieceCount;}}

	private int m_pickDNAPieceCount;
	public int pickDNAPieceCount{get{return m_pickDNAPieceCount;}}

	private int m_basePower;
	public int basePower{get{return m_basePower;}}

	private bool m_isDummy = false;
	public bool isDummy{get{return m_isDummy;}}

	private int m_researchPoint = 0;
	public int researchPoint{get{return m_researchPoint;}}

	private DinoInfo m_dinoInfo;
	public DinoInfo dinoInfo
	{
		get{
			if(m_dinoInfo == null)
				m_dinoInfo = GamingInfo.Instance.GetDinoInfo(m_index);
			return m_dinoInfo;}
	}

	public DinosaurData(int _index)
	{
		m_index = _index;
	}

	public void SetAttribute(int _attribute)
	{
		m_attribute = Utils.ConvertIntToEnumData<DinosaurAttribute>(_attribute);
	}

	public void SetDinoGrade(int _rank)
	{
		if(_rank > 100)
		{
			m_rank = _rank % 100;
			m_transcendence = true;
			return;
		}

		m_rank = _rank;
		m_transcendence = false;
	}

	public void SetGeneration(int _generation)
	{
		m_generation = _generation;
	}

	public void SetRepresentDino(int _representDinoIndex)
	{
		m_representDinoIndex = _representDinoIndex;
	}

	public void SetDNAPieceItemIndex(int _dnaPieceItemIndex)
	{
		m_dnaPieceItemIndex = _dnaPieceItemIndex;
	}

	public void SetExchangeDNAPieceCount(int _exchangeDNAPieceCount)
	{
		m_exchangeDNAPieceCount = _exchangeDNAPieceCount;
	}

	public void SetPickDNAPieceCount(int _pickDNAPieceCount)
	{
		m_pickDNAPieceCount = _pickDNAPieceCount;
	}

	public void SetBasePower(int _basePower)
	{
		m_basePower = _basePower;
		if(m_representDinoIndex > 0 && m_basePower <= 0)
			m_isDummy = true;
	}

	public void SetResearchPoint(int _researchPoint)
	{
		m_researchPoint = _researchPoint;
	}
}

/// <summary>
/// 공룡 전체 종에 대한 구성도
/// </summary>
public class DinoSpeciesMap {
	private Dictionary<int, DinosaurData> m_dinosaurs;
	public Dictionary<int, DinosaurData> dinosaurs{get{return m_dinosaurs;}}

	private Dictionary<int, UserDinoResearchData> m_researchDataMaps;
	public Dictionary<int, UserDinoResearchData> researchDataMaps{get{return m_researchDataMaps;}}

	private Dictionary<int, int> m_dnaDinoMaps;
	// public Dictionary<int, int> dnaDinoMaps {get{return m_dnaDinoMaps;}}

	private List<int> m_developEnableList;

	public bool existDevelopEnableDino{get{return m_developEnableList.Count > 0;}}

	public DinoSpeciesMap()
	{
		m_developEnableList = new List<int>();
		m_researchDataMaps = new Dictionary<int, UserDinoResearchData>();
		m_dnaDinoMaps = new Dictionary<int, int>();
		m_dinosaurs = DBControl.db.DBReadDinosaurData(m_researchDataMaps, m_dnaDinoMaps);
	}

	public void SetUserDinoResearchData(int representIndex, Dictionary<string, object> data)
	{
		if(m_researchDataMaps.ContainsKey(representIndex))
			m_researchDataMaps[representIndex].SetUserResearchProgressInfo(data);
		else
			Debug.LogError("data value error");
	}

	public void CheckExistDevelopEnableDino(ItemInfo itemInfo)
	{
		if(itemInfo == null)
			return;

		int uniqueItemIndex = itemInfo.nUniqueIndex;
		int itemIndex = itemInfo.nIdx;

		if(uniqueItemIndex <= 0 || itemIndex <= 0)
			return;

		ItemType type = Utils.ConvertIntToEnumData<ItemType>(itemInfo.nType);
		if(type != ItemType.DNAPiece)
			return;

		if (m_dnaDinoMaps.ContainsKey(itemIndex) == false)
			Debug.LogError("Not = " + itemIndex);
//			return;
		int representDinoIndex = m_dnaDinoMaps[itemIndex];

		UserDinoResearchData researchData = m_researchDataMaps[representDinoIndex];

		// if(researchData.developStep == researchData.maxDevelopStep)
		// {
		// 	if(m_developEnableList.Contains(itemIndex))
		// 		m_developEnableList.Remove(itemIndex);
		//
		// 	return;
		// }
		//
		// int dnaNeedCount = m_researchDataMaps[representDinoIndex].stepUpNeedPieceCount;
		// int itemCount = itemInfo.nHaveCnt;
		//
		// if(itemCount > dnaNeedCount)
		// {
		// 	if(m_developEnableList.Contains(itemIndex) == false)
		// 		m_developEnableList.Add(itemIndex);
		// }
		// else
		// {
		// 	if(m_developEnableList.Contains(itemIndex))
		// 		m_developEnableList.Remove(itemIndex);
		// }
	}


}

/// <summary>
/// 하나의 공룡 종에 관한 세대 구성도
/// </summary>
[Serializable]
public class DinoGenerationMap {
	// public int representDinoIndex{get{return m_representDinoIndex;}}

	private DinosaurData m_representDino;
	public DinosaurData representDino{get{return m_representDino;}}

	private Dictionary<int, DinosaurData> m_dinos;
	public Dictionary<int, DinosaurData> dinos{get{return m_dinos;}}

	// private int m_currentDevelopStep = 1;
	// public int currentDevelopStep{get{return m_currentDevelopStep;}}

	public void AddGenerationDino(DinosaurData dinosaurData)
	{
		if(m_dinos == null)
			m_dinos = new Dictionary<int, DinosaurData>();
		
		int generation = dinosaurData.generation;
		if(m_dinos.ContainsKey(generation))
		{
			Debug.LogError("한세대에 두개의 개체가 들어갈 수 없습니다.");
			Debug.LogError(dinosaurData.representDinoIndex);
			Debug.LogError(generation);
			return;
		}

		m_dinos.Add(generation, dinosaurData);

		if(dinosaurData.index == dinosaurData.representDinoIndex)
			m_representDino = dinosaurData;
	}
}
