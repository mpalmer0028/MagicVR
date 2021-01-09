
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CircleBackgroundGenerator))]
public class CircleBackgroundGeneratorEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var circleBackgroundGeneratorEditor = (CircleBackgroundGenerator)target;
		if (GUILayout.Button("Rebuild Background"))
		{
			circleBackgroundGeneratorEditor.MakeBackCircleImage();
		}
	}
}
