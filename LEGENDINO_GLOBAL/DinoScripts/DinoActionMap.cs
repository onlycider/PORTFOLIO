using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공룡 액션 종류
/// COUNTER 는 사실 거의 필요가 없음 일단 남겨둠
/// </summary>
public enum DinoActionType
{
	TEST,
	IDLE,
	SOUL,
	ATTACK,
	DEFENCE,
	ATTACK_STRONG,
	DODGE,
	COUNTER,
	DAMAGE,
	DIE,
	ACTIVESKILL_FIRST_NOMINEE_1, //첫번째 액티브 스킬 후보
	ACTIVESKILL_FIRST_NOMINEE_2,
	ACTIVESKILL_FIRST_NOMINEE_3,
	ACTIVESKILL_SECOND_NOMINEE_1,
	ACTIVESKILL_SECOND_NOMINEE_2,
	ACTIVESKILL_SECOND_NOMINEE_3,
}


/// <summary>
/// 공룡애니메이션 루프 세팅값 저장
/// </summary>
[Serializable]
public class DinoActionAnimationLoopSetting{
	[SerializeField]private string m_action;
	[SerializeField]private string m_animationName;
	[SerializeField]private float m_playTime;

	public DinoActionType action{get{return Utils.ConvertStringToEnumData<DinoActionType>(m_action);}}
	public string animationName{get{return m_animationName;}}
	public float playtime{get{return m_playTime;} set{m_playTime = value;}} 

	public DinoActionAnimationLoopSetting(DinoActionType _action, string _animationName, float _playtime)
	{
		m_action = _action.ToString();
		m_animationName = _animationName;
		m_playTime = _playtime;
	}	
}

/// <summary>
/// 공룡의 액션에는 애니메이션을 설정할수 있음
/// </summary>
[Serializable]
public class DinoActionMap {

	[SerializeField]private string m_action;
	[SerializeField]private string m_animationName;
	[SerializeField]private string m_eventName;

	[SerializeField]private Transform m_bone;

	//발동 이펙트
	[SerializeField]private string m_activeEffIndex;
	//상대방이 받는 이펙트 추가
	[SerializeField]private string m_targetTakeEffIndex;

	[SerializeField]private GameObject m_activeEffectObject;
	[SerializeField]private GameObject m_targetTakeEffectObject;

	public DinoActionType action{get{return Utils.ConvertStringToEnumData<DinoActionType>(m_action);}}
	public string animationName{get {return m_animationName;}}
	public string eventName{get{return m_eventName;} set{m_eventName = value;}}
	public string activeEffIndex{get{return m_activeEffIndex;} set{m_activeEffIndex = value;}}
	public string targetTakeEffIndex{get{return m_targetTakeEffIndex;} set{m_targetTakeEffIndex = value;}}

	public Transform bone{get{return m_bone;} set{m_bone = value;}}

	public GameObject activeEffectObject{get{return m_activeEffectObject;} set{m_activeEffectObject = value;}}
	public GameObject targetTakeEffectObject{get{return m_targetTakeEffectObject;} set{m_targetTakeEffectObject = value;}}

	private const string noneValue = "None";
	private const string animationStartEvent = "start";


	/// <summary>
	/// 
	/// </summary>
	/// <param name="_action"></param>
	/// <param name="_animationName"></param>
	public DinoActionMap(DinoActionType _action, string _animationName)
	{
		m_action = _action.ToString();
		m_animationName = _animationName;

		//아래부터 기본 세팅

		if(m_animationName.Equals(noneValue.ToLower()))
			m_eventName = noneValue.ToLower();
		else
			m_eventName = animationStartEvent;


		
		m_activeEffIndex = noneValue;
		m_targetTakeEffIndex = noneValue;

		m_activeEffectObject = null;
		m_targetTakeEffectObject = null;
	}


}
