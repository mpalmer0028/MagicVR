using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PowerSystemScript))]
public class PowerSystemScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var powerSystemScript = (PowerSystemScript)target;
		if(GUILayout.Button("Rebuild Combinations"))
		{
			powerSystemScript.BuildCombinations();
		}
	}
}
