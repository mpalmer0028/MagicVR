using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MagicMenu))]
public class MagicMenuEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var magicMenuScript = (MagicMenu)target;
		if(GUILayout.Button("Rebuild Icons"))
		{
			//Debug.Log(magicMenuScript.gameObject.name);
			magicMenuScript.BuildSections();
		}
	}
}
