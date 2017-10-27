//Script written by Inan Evin
//Support: giovanni.cartella@hotmail.com || www.giovannicartella.weebly.com
//You are allowed to use this only if you have "Horror Development Kit" license, so only if you bought it officially

using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(HeadBobController))]
public class HeadBobControllerEditor : Editor {

	public override void OnInspectorGUI()
	{
		HeadBobController _Target = (HeadBobController)target;
		serializedObject.Update();

		SerializedProperty SelectSettings = serializedObject.FindProperty("_SelectSettings");
		SerializedProperty WalkBobStyle = serializedObject.FindProperty("_WalkBobStyle");
		SerializedProperty RunBobStyle = serializedObject.FindProperty("_RunBobStyle");

		EditorGUILayout.LabelField("General Settings");

		_Target.t_Target = EditorGUILayout.ObjectField("Target Transform", _Target.t_Target, typeof(Transform), true) as Transform;
		_Target.f_ResetSpeed = EditorGUILayout.FloatField("Reset Speed", _Target.f_ResetSpeed);

		EditorGUILayout.LabelField("Bob Settings");


		EditorGUILayout.PropertyField(SelectSettings, new GUIContent("Select To Edit"), true);

		if(_Target._SelectSettings == HeadBobController.SelectSettings.WalkSettings)
		{
			EditorGUILayout.LabelField("WalkSettings");
			EditorGUILayout.PropertyField(WalkBobStyle, new GUIContent("Walk Bob Style"), true);

			if(_Target._WalkBobStyle == HeadBobController.WalkBobStyle.OnlyPosition)
			{
				EditorGUILayout.LabelField("Position");
				EditorGUI.indentLevel ++;
				_Target.v3_WALK_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_WALK_POS_Amounts);
				_Target.v3_WALK_POS_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_WALK_POS_Speeds);
				_Target.f_WALK_POS_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_WALK_POS_Smooth);
				EditorGUI.indentLevel --;
			}
			else if(_Target._WalkBobStyle == HeadBobController.WalkBobStyle.OnlyRotation)
			{
				EditorGUILayout.LabelField("Rotation");
				EditorGUI.indentLevel ++;
				_Target.v3_WALK_EUL_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_WALK_EUL_Amounts);
				_Target.v3_WALK_EUL_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_WALK_EUL_Speeds);
				_Target.f_WALK_EUL_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_WALK_EUL_Smooth);
				EditorGUI.indentLevel --;
			}
			else if(_Target._WalkBobStyle == HeadBobController.WalkBobStyle.Both)
			{
				EditorGUILayout.LabelField("Position");
				EditorGUI.indentLevel ++;
				_Target.v3_WALK_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_WALK_POS_Amounts);
				_Target.v3_WALK_POS_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_WALK_POS_Speeds);
				_Target.f_WALK_POS_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_WALK_POS_Smooth);
				EditorGUI.indentLevel --;
				EditorGUILayout.LabelField("Rotation");
				EditorGUI.indentLevel ++;
				_Target.v3_WALK_EUL_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_WALK_EUL_Amounts);
				_Target.v3_WALK_EUL_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_WALK_EUL_Speeds);
				_Target.f_WALK_EUL_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_WALK_EUL_Smooth);
				EditorGUI.indentLevel --;
			}
		}
		else if(_Target._SelectSettings == HeadBobController.SelectSettings.RunSettings)
		{
			EditorGUILayout.LabelField("Run Settings");
			EditorGUILayout.PropertyField(RunBobStyle, new GUIContent("Run Bob Style"), true);
			
			if(_Target._RunBobStyle == HeadBobController.RunBobStyle.OnlyPosition)
			{
				EditorGUILayout.LabelField("Position");
				EditorGUI.indentLevel ++;
				_Target.v3_RUN_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_RUN_POS_Amounts);
				_Target.v3_RUN_POS_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_RUN_POS_Speeds);
				_Target.f_RUN_POS_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_RUN_POS_Smooth);
				EditorGUI.indentLevel --;
			}
			else if(_Target._RunBobStyle == HeadBobController.RunBobStyle.OnlyRotation)
			{
				EditorGUILayout.LabelField("Rotation");
				EditorGUI.indentLevel ++;
				_Target.v3_RUN_EUL_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_RUN_EUL_Amounts);
				_Target.v3_RUN_EUL_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_RUN_EUL_Speeds);
				_Target.f_RUN_EUL_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_RUN_EUL_Smooth);
				EditorGUI.indentLevel --;
			}
			else if(_Target._RunBobStyle == HeadBobController.RunBobStyle.Both)
			{
				EditorGUILayout.LabelField("Position");
				EditorGUI.indentLevel ++;
				_Target.v3_RUN_POS_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_RUN_POS_Amounts);
				_Target.v3_RUN_POS_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_RUN_POS_Speeds);
				_Target.f_RUN_POS_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_RUN_POS_Smooth);
				EditorGUI.indentLevel --;
				EditorGUILayout.LabelField("Rotation");
				EditorGUI.indentLevel ++;
				_Target.v3_RUN_EUL_Amounts = EditorGUILayout.Vector3Field("Amounts", _Target.v3_RUN_EUL_Amounts);
				_Target.v3_RUN_EUL_Speeds = EditorGUILayout.Vector3Field("Speeds", _Target.v3_RUN_EUL_Speeds);
				_Target.f_RUN_EUL_Smooth = EditorGUILayout.FloatField("Smooth", _Target.f_RUN_EUL_Smooth);
				EditorGUI.indentLevel --;
			}
		}
		else if(_Target._SelectSettings == HeadBobController.SelectSettings.JumpSettings)
		{
			EditorGUILayout.LabelField("Jump Settings");

			_Target.v3_JUMPSTART_MaximumIncrease = EditorGUILayout.Vector3Field("Maximum Increase", _Target.v3_JUMPSTART_MaximumIncrease);
			EditorGUI.indentLevel ++;
			_Target.f_JUMPSTART_InterpolationSpeed = EditorGUILayout.FloatField("Speed", _Target.f_JUMPSTART_InterpolationSpeed);
			EditorGUI.indentLevel --;
			_Target.v3_JUMPEND_MaximumDecrease = EditorGUILayout.Vector3Field("Maximum Decrease", _Target.v3_JUMPEND_MaximumDecrease);
			EditorGUI.indentLevel ++;
			_Target.f_JUMPEND_InterpolationSpeed = EditorGUILayout.FloatField("In Speed", _Target.f_JUMPEND_InterpolationSpeed);
			_Target.f_JUMPEND_ResetSpeed = EditorGUILayout.FloatField("Out Speed", _Target.f_JUMPEND_ResetSpeed);
			EditorGUI.indentLevel --;
		}

		serializedObject.ApplyModifiedProperties();
	}
}
