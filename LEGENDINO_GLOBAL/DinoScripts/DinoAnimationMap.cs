using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;

public class DinoAnimationEventList
{
	private string m_animationName;
	private List<string> m_eventList;

	public string animationName{get{return m_animationName;}}
	public List<string> eventList{get{return m_eventList;}}

	public DinoAnimationEventList(Spine.Animation animation)
	{
		m_animationName = animation.Name;
		m_eventList = new List<string>(); 

		Timeline[] timelines = animation.Timelines.Items;
        for(int i = 0; i < timelines.Length; i++)
        {
            EventTimeline eventTimeline = timelines[i] as EventTimeline;

            if(eventTimeline != null)
            {
                Spine.Event[] events = eventTimeline.Events;
				SetEventList(events);
            }
        }
	}

	private void SetEventList(Spine.Event[] events)
	{
		if(events == null)
			return;

		for(int i = 0; i < events.Length; i++)
			m_eventList.Add(events[i].ToString());
	}
}

public class DinoAnimationMap {
	private Dictionary<string, DinoAnimationEventList> m_eventListDict;
	public Dictionary<string, DinoAnimationEventList> eventListDict{get{return m_eventListDict;}}

	private Dictionary<string, float> m_animationLengthDict;
	public Dictionary<string, float> animationLengthDict{get{return m_animationLengthDict;}}

	public DinoAnimationMap(Spine.Animation[] animations)
	{
		m_eventListDict = new Dictionary<string, DinoAnimationEventList>();
		m_animationLengthDict = new Dictionary<string, float>();
		for(int i = 0; i < animations.Length; i++)
		{
			m_eventListDict.Add(animations[i].Name, new DinoAnimationEventList(animations[i]));
			m_animationLengthDict.Add(animations[i].Name, animations[i].Duration);
		}
	}
}
