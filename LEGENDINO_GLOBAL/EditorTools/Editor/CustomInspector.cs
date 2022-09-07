using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

// CustomEditor() 애트리뷰트를 사용해서 어떤 타입을 커스터마이즈 할 것인지를 명시해 주어야 한다.
//[CustomEditor(typeof(Dictionary<RunItemType,float>))]
public class CustomInspector : Editor
{
//	Dictionary<RunItemType,float> _DicItem_Float;
	void OnEnable()
	{
		// target은 위의 CustomEditor() 애트리뷰트에서 설정해 준 타입의 객체에 대한 레퍼런스
		// object형이므로 실제 사용할 타입으로 캐스팅 해 준다.
//		_DicItem_Float = target as Dictionary<RunItemType,float>;
	}

	public override void OnInspectorGUI()
	{
//		_DicItem_Float.Values
//		// 컨트롤들을 가로로 배치하기 위해 BeginHorizontal()/EndHorizontal() 메서드를 사용한다.
//		EditorGUILayout.BeginHorizontal();
//		// 이동 속도 레이블
//		EditorGUILayout.LabelField("Speed", null);
//		// 이동 속도는 4가지 중 하나로 선택하도록 IntPopup() 컨트롤을 사용한다.
//		string[] speedNames = new string[] { "slow", "normal", "fast", "fastest" };
//		int[] speedValues = new int[] { 1, 5, 10, 20 };
//		_movement._speed = EditorGUILayout.IntPopup(_movement._speed, speedNames, speedValues);
//		EditorGUILayout.EndHorizontal();
//		
//		// x, y, z 값을 가로로 표시하는 Vector3Field() 컨트롤을 생성
//		_movement._targetPosition = EditorGUILayout.Vector3Field("Target Position", _movement._targetPosition);
//		
		// GUI가 변경되었다면 타겟을 다시 렌더링 하도록 하기 위해 dirty 상태로 마크한다.
		if (GUI.changed)
			EditorUtility.SetDirty(target);
	}
}
