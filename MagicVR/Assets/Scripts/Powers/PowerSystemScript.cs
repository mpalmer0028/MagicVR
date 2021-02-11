using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;
using System.Text;

public class PowerSystemScript : MonoBehaviour
{
	public GameObject DefaultPowerPrefab;	
	
	public bool LeftHanded;
	
	public GameObject LeftHandSpawn;
	public GameObject RightHandSpawn;
	
	public string PowerName {
		get { return _PowerName;}
		set { 
			_PowerName = value;
			SetPower(value);
		}
	}
	
	private string _PowerName = string.Empty;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	public void SetPower(string power){
		PowerCombinations combo;
		if(Enum.TryParse(power, out combo)){
			Debug.Log(power);
			var prefab = Resources.Load<GameObject>("HandObjects/"+power);
			Debug.Assert(prefab != null);
			if(prefab != null){
				var powerObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
				var comboPowerScript = powerObj.GetComponent<IComboPower>();
				comboPowerScript.ActivatePower(this);
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








