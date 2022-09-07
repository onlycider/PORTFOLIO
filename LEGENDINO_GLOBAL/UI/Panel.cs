using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//매번 빼먹어서 추가.. //안붙어있는 것도 있어서 일단 주석.. => 안붙어있는것 헬퍼 필요없는경우 bAutoSet 체크를 해제하여 붙임..
[RequireComponent(typeof( AtlasSetHelper ))]
public class Panel : MonoBehaviour {
    private PanelTag m_panelTag = PanelTag.NONE;
    public PanelTag panelTag{get{return m_panelTag;} set{m_panelTag = value;}}

    private UIPanel ngui_panelComponent;
	private int m_panelDepth = 0;
	public int panelDepth{get{return m_panelDepth;}}


    Action EndAction = null;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        OnAwake();
        //SetBaseText();
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        OnStart();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        OnEnabled();
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled or inactive.
    /// </summary>
    void OnDisable()
    {
        OnDisabled();   
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    void OnDestroy()
    {
        if(EndAction != null)
        {
            Utils.InvokeAction(EndAction);
            EndAction = null;
        }
        OnDestroied();
    }

    protected virtual void OnAwake(){}
    protected virtual void OnStart(){}
    protected virtual void OnEnabled(){}
    protected virtual void OnDisabled(){}
    protected virtual void OnDestroied(){
        // Debug.Log("Panel Destroied ================================");
        UserStatusSocketManager.Instance.CheckFirstInviter();
    }

    public virtual void SetPanelInfo(object info){}

    // public virtual void SetPanelInfo<T>(T info){}

    protected virtual void SetBaseText() {}

    public void GetPanelComponent()
    {
        if(ngui_panelComponent == null)
            ngui_panelComponent = GetComponent<UIPanel>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="_depth"></param>
    public void SetPanelDepth(int _depth)
    {
        GetPanelComponent();
        if(ngui_panelComponent == null)
        {
            //Error
            return;
        }

        ngui_panelComponent.depth =  _depth;
		m_panelDepth = _depth;
        ngui_panelComponent.sortingOrder = _depth;
    }

    public void SetNPCUIPanel(int _depth)
    {
        SetPanelDepth(_depth);
        Transform[] tran =this.GetComponentsInChildren<Transform>();
        foreach(Transform t in tran)
        {
            t.gameObject.layer = LayerMask.NameToLayer("NPCUI");
        }
        transform.position = Vector3.zero;
    }

    public void SetEndAction(Action _endAction)
    {
        EndAction = _endAction;
    }

	public void EditSetPanelDepth(int _depth)
	{
		m_panelDepth = _depth;
	}

    public virtual void InitializeForTutorial(){}

    public virtual void InvokeBackKey(){}
}
