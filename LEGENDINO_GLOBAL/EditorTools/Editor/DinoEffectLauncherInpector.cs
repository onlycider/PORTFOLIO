using UnityEditor;
using UnityEngine;

namespace Spine.Unity.Editor {

	using Editor = UnityEditor.Editor;
	using Event = UnityEngine.Event;

	[CustomEditor(typeof(DinoEffectLauncher)), CanEditMultipleObjects]
	public class DinoEffectLauncherInspector : Editor {
		SerializedProperty skeletonRenderer;
		DinoEffectLauncher targetEffectLauncher;
		bool needsReset;

		// #region Context Menu Item
		// [MenuItem ("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject")]
		// static void AddBoneFollowerGameObject (MenuCommand cmd) {
		// 	var skeletonRenderer = cmd.context as SkeletonRenderer;
		// 	var go = new GameObject("BoneFollower");
		// 	var t = go.transform;
		// 	t.SetParent(skeletonRenderer.transform);
		// 	t.localPosition = Vector3.zero;

		// 	var f = go.AddComponent<BoneFollower>();
		// 	f.skeletonRenderer = skeletonRenderer;

		// 	EditorGUIUtility.PingObject(t);

		// 	Undo.RegisterCreatedObjectUndo(go, "Add BoneFollower");
		// }

		// // Validate
		// [MenuItem ("CONTEXT/SkeletonRenderer/Add BoneFollower GameObject", true)]
		// static bool ValidateAddBoneFollowerGameObject (MenuCommand cmd) {
		// 	var skeletonRenderer = cmd.context as SkeletonRenderer;
		// 	return skeletonRenderer.valid;
		// }
		// #endregion

		void OnEnable () {
			skeletonRenderer = serializedObject.FindProperty("skeletonRenderer");

			targetEffectLauncher = (DinoEffectLauncher)target;
			if (targetEffectLauncher.SkeletonRenderer != null)
				targetEffectLauncher.SkeletonRenderer.Initialize(false);

			if (!targetEffectLauncher.valid || needsReset) {
				targetEffectLauncher.Initialize();
				targetEffectLauncher.LateUpdate();
				needsReset = false;
				SceneView.RepaintAll();
			}
		}

		public void OnSceneGUI () {
			var tbf = target as DinoEffectLauncher;
			var skeletonRendererComponent = tbf.skeletonRenderer;
			if (skeletonRendererComponent == null) return;

			var transform = skeletonRendererComponent.transform;
			var skeleton = skeletonRendererComponent.skeleton;

			// if (string.IsNullOrEmpty(tbf.boneName)) {
			// 	SpineHandles.DrawBones(transform, skeleton);
			// 	SpineHandles.DrawBoneNames(transform, skeleton);
			// 	Handles.Label(tbf.transform.position, "No bone selected", EditorStyles.helpBox);
			// } else {
			// 	var targetBone = tbf.bone;
			// 	if (targetBone == null) return;
			// 	SpineHandles.DrawBoneWireframe(transform, targetBone, SpineHandles.TransformContraintColor);
			// 	Handles.Label(targetBone.GetWorldPosition(transform), targetBone.Data.Name, SpineHandles.BoneNameStyle);
			// }
		}

		override public void OnInspectorGUI () {
			if (serializedObject.isEditingMultipleObjects) {
				if (needsReset) {
					needsReset = false;
					foreach (var o in targets) {
						var bf = (DinoEffectLauncher)o;
						bf.Initialize();
						bf.LateUpdate();
					}
					SceneView.RepaintAll();
				}

				EditorGUI.BeginChangeCheck();
				DrawDefaultInspector();
				needsReset |= EditorGUI.EndChangeCheck();
				return;
			}

			if (needsReset && Event.current.type == EventType.Layout) {
				targetEffectLauncher.Initialize();
				targetEffectLauncher.LateUpdate();
				needsReset = false;
				SceneView.RepaintAll();
			}
			serializedObject.Update();

			// Find Renderer
			if (skeletonRenderer.objectReferenceValue == null) {
				SkeletonRenderer parentRenderer = targetEffectLauncher.GetComponentInParent<SkeletonRenderer>();
				if (parentRenderer != null && parentRenderer.gameObject != targetEffectLauncher.gameObject) {
					skeletonRenderer.objectReferenceValue = parentRenderer;
					Debug.Log("Inspector automatically assigned BoneFollower.SkeletonRenderer");
				}
			}

			EditorGUILayout.PropertyField(skeletonRenderer);
			var skeletonRendererReference = skeletonRenderer.objectReferenceValue as SkeletonRenderer;
			if (skeletonRendererReference != null) {
				if (skeletonRendererReference.gameObject == targetEffectLauncher.gameObject) {
					skeletonRenderer.objectReferenceValue = null;
					EditorUtility.DisplayDialog("Invalid assignment.", "BoneFollower can only follow a skeleton on a separate GameObject.\n\nCreate a new GameObject for your BoneFollower, or choose a SkeletonRenderer from a different GameObject.", "Ok");
				}
			}

			if (targetEffectLauncher.valid) {
				EditorGUI.BeginChangeCheck();
				needsReset |= EditorGUI.EndChangeCheck();

			} else {
				var boneFollowerSkeletonRenderer = targetEffectLauncher.skeletonRenderer;
				if (boneFollowerSkeletonRenderer == null) {
					EditorGUILayout.HelpBox("SkeletonRenderer is unassigned. Please assign a SkeletonRenderer (SkeletonAnimation or SkeletonAnimator).", MessageType.Warning);
				} else {
					boneFollowerSkeletonRenderer.Initialize(false);

					if (boneFollowerSkeletonRenderer.skeletonDataAsset == null)
						EditorGUILayout.HelpBox("Assigned SkeletonRenderer does not have SkeletonData assigned to it.", MessageType.Warning);

					if (!boneFollowerSkeletonRenderer.valid)
						EditorGUILayout.HelpBox("Assigned SkeletonRenderer is invalid. Check target SkeletonRenderer, its SkeletonDataAsset or the console for other errors.", MessageType.Warning);
				}
			}

			var current = Event.current;
			bool wasUndo = (current.type == EventType.ValidateCommand && current.commandName == "UndoRedoPerformed");
			if (wasUndo)
				targetEffectLauncher.Initialize();

			serializedObject.ApplyModifiedProperties();
		}
	}

}
