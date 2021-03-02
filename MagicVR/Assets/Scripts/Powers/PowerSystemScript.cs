using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Text;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PowerSystemScript : MonoBehaviour
{
	
	public GameObject DefaultPowerPrefab;	
	public GameObject VisionTarget;	
	
	public bool LeftHanded;
	
	public GameObject LeftHandSpawn;
	public GameObject RightHandSpawn;
	
	[SerializeField]
	private string _PowerName;
	public string PowerName {
		get { 			
			return _PowerName;
		}
		set { 
			_PowerName = value;
			SetPower(value);
		}
	}
	public PowerCombinations ComboToSet;
	
	private float PrimaryTrigger;
	private float OffTrigger;
	
	private GameObject PowerObject;
	private IComboPower ComboPowerScript;
	 
	public void Start(){

	}
	public float LeftTrigger{
		get { return LeftHanded ? PrimaryTrigger : RightTrigger;}
		set { 
			if(LeftHanded){
				
				PrimaryTrigger = value;
			}else{
				OffTrigger = value;
			}
			//Debug.Log("L");
			ComboPowerScript.UpdatePowerTrigger(value, !LeftHanded);
		}
	}
	
	public float RightTrigger{
		get { return LeftHanded ? OffTrigger : PrimaryTrigger;}
		set { 
			if(LeftHanded){
				OffTrigger = value;				
			}else{
				PrimaryTrigger = value;
			}
			//Debug.Log("R");
			ComboPowerScript.UpdatePowerTrigger(value, LeftHanded);
		}
	}
		
	//public void SetPower(PowerCombinations power){
	//	SetPower(Enum.GetName(typeof(PowerCombinations),power));
	//}
	
	private void SetPower(string power){
		//Debug.Log(power);
		if(power == string.Empty){
			if(PowerObject != null){
				ComboPowerScript.RemovePower(this);
				ComboPowerScript = null;
				Destroy(PowerObject);
			}
		}else{
			PowerCombinations combo;
			if(Enum.TryParse(power, out combo)){
				
				var prefab = Resources.Load<GameObject>("HandObjects/"+power);
				//Debug.Assert(prefab != null);
				if(prefab != null){
					PowerObject = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
					ComboPowerScript = PowerObject.GetComponent<IComboPower>();
					ComboPowerScript.VisionTarget = VisionTarget;
					ComboPowerScript.ActivatePower(this);
				}			
			}
		}
		
	}
	
	public void BuildCombinations(){
		var iconFiles = new List<string>();
		var iconsDirPath = Application.dataPath + @"\Icons\100ppi";
		iconFiles.AddRange(Directory.EnumerateFiles(iconsDirPath, "*.png"));
		
		iconFiles = iconFiles.AsQueryable().Select(x=>Path.GetFileName(x).Split('.').First()).OrderBy(x=>x).ToList();
		
		var combos = new List<string>{};
		
		for(var i = 0;i< iconFiles.Count;i++){
			for(var i2 = i+1;i2< iconFiles.Count;i2++){
				combos.Add(string.Format("{0}{1}",iconFiles[i],iconFiles[i2]));
			
			}
		}
		
		// Write enum file
		var comboText = string.Join(", ", combos);
		//Debug.Log(comboText);
		var enumText = "public enum PowerCombinations { "+comboText+" } \n";
		List<string> casesTexts = combos.AsQueryable().Select(x=>string.Format("//case PowerCombinations."+x+":\n//\t\n//\tbreak;")).ToList();
		var caseText = string.Join("\n",casesTexts);
		System.IO.File.WriteAllBytes(Application.dataPath + "/Scripts/Powers/Combinations.cs", Encoding.ASCII.GetBytes(enumText+caseText));
		AssetDatabase.Refresh();
		
		// Build prefabs
		foreach(var combo in combos){
			// Check for existing prefab
			var path = @"Assets/Resources/HandObjects/"+combo+".prefab";
			var comboPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
			// Make default if one doesn't exist 
			if(comboPrefab == null){
				comboPrefab = Instantiate(DefaultPowerPrefab, Vector3.zero, Quaternion.identity);
				PrefabUtility.CreatePrefab(path, comboPrefab);
				DestroyImmediate(comboPrefab);
			}
		}
	}
}








