using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SwordFishScript : MonoBehaviour
{
	


	
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
	public void RegisterSwordFishComponents(){
		// Get object that need components
		var spines = new List<Transform>{};
	}
}

//[CustomEditor(typeof(SwordFishScript))]
//public class SwordFishScriptEditor: Editor
//{
//	public override void OnInspectorGUI()
//	{
//		DrawDefaultInspector();

//		var sfs = (SwordFishScript)target;
//		if (GUILayout.Button("Register SwordFish Components"))
//		{
//			sfs.RegisterSwordFishComponents();
//		}
//	}
//}