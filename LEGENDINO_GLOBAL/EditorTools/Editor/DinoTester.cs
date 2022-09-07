using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DinoTester : EditorWindow
{
    private int m_skillIndex;
    private int m_attribute;

    public DinoObject source;
    [MenuItem("Dev/Dinosaur tester")]
	public static void UseFavoriteSceneTool()
	{
		EditorWindow.GetWindow(typeof(DinoTester));
	}

    void OnGUI()
	{
        source = EditorGUILayout.ObjectField(source, typeof(DinoObject), true) as DinoObject;
        if (GUILayout.Button("Set Player Dino"))
        {
            GameObject playerParent = GameObject.Find("PlayerObject");
            source = playerParent.GetComponentInChildren<DinoObject>();
        }

        if(source == null && Application.isPlaying)
        {
            GameObject playerParent = GameObject.Find("PlayerObject");
            source = playerParent.GetComponentInChildren<DinoObject>();
        }
        
        m_skillIndex = EditorGUILayout.IntField("input skill index : ", m_skillIndex);
        if(source != null)
            source.SetSelectedSkillIndex(m_skillIndex);

        if(GUILayout.Button("idle"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
        }
        
        if(GUILayout.Button("attack_1"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "attackready_1", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "attack_1", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("attack_2"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "attackready_2", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "attack_2", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("attack_3"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "attackready_3", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "attack_3", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("attack_4"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "attackready_4", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "attack_4", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("special_1"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "specialready_1", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "special_1", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("special_2"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "specialready_2", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "special_2", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("special_3"))
        {
            if(source == null)
                return;
            source.skeletonAnimation.AnimationState.SetAnimation(0, "specialready_3", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "special_3", false, 0f);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }

        if(GUILayout.Button("soul"))
        {
            if(source == null)
                return;

            source.SetSelectedSkillIndex(1);
            source.skeletonAnimation.AnimationState.SetAnimation(0, "soul", true);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0.7f);
        }

        if(GUILayout.Button("defense"))
        {
            if(source == null)
                return;


            source.SetSelectedSkillIndex(4);
            source.skeletonAnimation.AnimationState.SetAnimation(0, "defense", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 1f);
        }

        if(GUILayout.Button("dodge"))
        {
            if(source == null)
                return;

            source.SetSelectedSkillIndex(5);
            source.skeletonAnimation.AnimationState.SetAnimation(0, "dodge", true);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0.7f);
        }

        if(GUILayout.Button("die"))
        {
            if(source == null)
                return;

            source.skeletonAnimation.AnimationState.SetAnimation(0, "die", false);
            source.skeletonAnimation.AnimationState.AddAnimation(0, "idle", true, 0f);
        }



        m_attribute = EditorGUILayout.IntField("skill attribute : ", m_attribute);
        if(GUILayout.Button("Floor"))
        {
            if(source == null)
                return;
                
            source.PlaySpecialSkillStartEffect(m_attribute);
        }
    }
}
