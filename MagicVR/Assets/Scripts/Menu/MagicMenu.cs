﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.IO;

public class MagicMenu : MonoBehaviour
{
	/// <summary>
	/// Depth of section collider
	/// </summary>
	public float Depth = .5f;
	
	/// <summary>
	/// Hand input and info
	/// </summary>
	public HandsInputScript HandsInputScript;
	
	/// <summary>
	/// Radius of section
	/// </summary>
	public float Radius = 1;
	
	/// <summary>
	/// Prefabs of the spawnable magics
	/// </summary>
	public GameObject[] MagicPrefabs;
	
	/// <summary>
	/// Pecentage of section that should be a margin
	/// </summary>
	public float Margin = .25f;
	
	/// <summary>
	/// Target of power zoom rotation
	/// </summary>
	public GameObject PowerZoomTarget;
	
	public float PreviewDistanceBehindIcon = .02f;
	
	/// <summary>
	/// Prefab of a section
	/// </summary>
	public GameObject SectionPrefab;
	
	/// <summary>
	/// Visible text output
	/// </summary>
	public GameObject TextOut;
	
	public List<string> PossibleCombinations; 
	private string IconsDirPath;

	private List<GameObject> Icons = new List<GameObject>();
	private List<string> IconFiles = new List<string>();
	public int Magics;
	private Material BackCircleMaterial;	
	public bool Closing;	
	
	private GameObject Player;
	private PowerSystemScript PowerSystem;
	
	
	private List<GameObject> Sections = new List<GameObject>();
	public List<SectionScript> SectionScripts = new List<SectionScript>();
	public List<GameObject> ShrinkingIcons = new List<GameObject>();
	private List<int> Triangles = new List<int>();
	private List<Vector2> UVs = new List<Vector2>();
	private List<Vector3> Vertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
	{
		Player = HandsInputScript.gameObject;
		PowerSystem = Player.GetComponent<PowerSystemScript>();
		TextOut = transform.Find("TextOut").gameObject;
		var bc = transform.Find("BackCircle");
		var mr = bc.GetComponent<MeshRenderer>();
		BackCircleMaterial = mr.sharedMaterial;
		BackCircleMaterial.SetFloat("Alpha", 0);
		//BuildSections();
		foreach(Transform t in transform){
			if(t.name.StartsWith("Section")){
				SectionScripts.Add(t.GetComponent<SectionScript>());
				Icons.Add(t.Find("Icon").gameObject);
			}
		}
    }

	void FixedUpdate(){
		
	}
	
    // Update is called once per frame
    void Update()
	{
		var a = BackCircleMaterial.GetFloat("Alpha");
		
		if(!Closing){
			if(a < .2f){
				// Reveal menu 
				BackCircleMaterial.SetFloat("Alpha", a+.001f);
			}
		}else{
			if(a > 0){
				// Hide menu
				BackCircleMaterial.SetFloat("Alpha", a-.005f);
				if(ShrinkingIcons.Count>0){
					if(ShrinkingIcons[0].transform.localScale.x>0){
						foreach(var si in ShrinkingIcons){
					
							si.transform.localScale -= new Vector3(.002f,.002f,.002f);
						}
					}
					
				}
				
			}else{
				HandsInputScript.MagicMenuPrelaunchStartTime = Time.time+1;
				Destroy(gameObject);
			}
		}
		
		
		
    }
    
	void OnDrawGizmos()
	{
		Gizmos.color = UnityEngine.Color.green;
		//if(transform.childCount < Magics){
		//	BuildSections();
		//}else{
		//	//foreach(var sec in Sections){
		//	//	var mf = sec.Find("Icon").GetComponent<MeshFilter>();
		//	//	Gizmos.DrawMesh(mf.mesh, transform.position);
		//	//}
		//}
		//foreach(var v in Vertices){
		//	Gizmos.DrawWireSphere(transform.position + v, .01f);
		//}
		//Debug.Log(transform.childCount);
	}
	
	/// <summary>
	/// Build hitbox, visual faces, and gizmo dots
	/// </summary>
	/// <param name="menuPrefabContents">Prefab contents for menu</param>
	/// <param name="angle">Angle of the start of the section in radians</param>
	/// <param name="nextAngle">Angle of the end of the section in radians</param>
	/// <param name="index">Section index for naming</param>
	private void BuildMagicSection(ref GameObject menuPrefabContents, float angle, float nextAngle, int index){
		// for centering the uv positions so between 0-1 
		var centerVec2 = new Vector2(.5f,.5f);
		var verts = new List<Vector3>();
		
		Vertices = new List<Vector3>();
		UVs = new List<Vector2>();

		
		
		var uvV2 = new Vector2(0,0);		
		foreach(var a in new float[]{angle, nextAngle}){
			var x = (float)Math.Cos(a) * Radius;
			var y = (float)Math.Sin(a) * Radius;
			// look into pivoting around
			var uvRotation = Quaternion.Euler(0,0, 0);
			
			#region Front
			// Inner ring
			verts.Add((new Vector3(x,y,0)*.5f) - new Vector3(0,0,Depth));
			UVs.Add(uvRotation*((new Vector2(x,y)*.5f/2)+centerVec2));
			//UVs.Add(uvV2);
			
			// Outer ring
			verts.Add(new Vector3(x,y,0) - new Vector3(0,0,Depth));
			//UVs.Add(uvV2 + new Vector2(0,1));
			UVs.Add(uvRotation*((new Vector2(x,y)/2)+centerVec2));
			#endregion
		
			#region  Back 
			// Inner ring
			verts.Add((new Vector3(x,y,0)*.5f));
			UVs.Add(uvRotation*((new Vector2(x,y)*.5f/2)+centerVec2));
			
			// Outer ring
			verts.Add(new Vector3(x,y,0));
			UVs.Add(uvRotation*((new Vector2(x,y)/2)+centerVec2));
			#endregion	
			//uvV2 += new Vector2(1,0);
			
		}	
		
		Vertices.AddRange(verts);
			
		// Add Section
		var section = Instantiate(SectionPrefab, menuPrefabContents.transform);
		section.name = string.Format("Section {0}", index);
		
		var icon = section.transform.Find("Icon").gameObject;		
		var magicGlowParticle = icon.transform.Find("MagicGlowParticle").gameObject;
		var ss = section.GetComponent<SectionScript>();	
		ss.IconFileName = Path.GetFileName(IconFiles[index]).Split('.').First();
		
		
		
		var materialName = string.Format("Assets/Materials/Generated/SectionIcon{0}.mat",ss.IconFileName);
		var meshName = string.Format("Assets/Meshes/Generated/Section{0}ColliderMesh{1}.asset", index, ss.IconFileName);
		var textureName = string.Format("Assets/Icons/100ppi/{0}.png",ss.IconFileName);
		
		Icons.Add(icon);
		IQueryable<Vector3> vertsQ = verts.AsQueryable();
		var iconLocation = new Vector3(vertsQ.Average(v => v.x), vertsQ.Average(v => v.y), 0f);
		
		var material = (Material)AssetDatabase.LoadAssetAtPath(materialName, typeof(Material));
		Texture2D texture = (Texture2D)AssetDatabase.LoadAssetAtPath(textureName, typeof(Texture2D));
		
		var render = icon.GetComponent<Renderer>();
		if(material == null){
			// Use temp material to prevent leak
			material = new Material(render.sharedMaterial);					
			AssetDatabase.CreateAsset(material, materialName);
		}
		material.SetTexture("ImageTexture", texture);
		AssetDatabase.SaveAssets();
		render.sharedMaterial = material;

		icon.transform.localPosition = iconLocation;
		
		string previewPath = "Assets/Powers/IconPreviews/" + ss.IconFileName + ".prefab";

		GameObject previewPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(previewPath, typeof(GameObject));
		
		if(previewPrefab != null){
			GameObject preview = (GameObject)PrefabUtility.InstantiatePrefab(previewPrefab);
			preview.transform.parent = icon.transform;
			preview.transform.localPosition =  new Vector3(0,0,PreviewDistanceBehindIcon);
			
		}
		
		var mc = section.GetComponent<MeshCollider>();
		var mf = icon.GetComponent<MeshFilter>();
		var mr = icon.GetComponent<MeshRenderer>();
			
		Triangles = new List<int>();
		var colliderMesh = new Mesh();
		var iconMesh = new Mesh();
	
		var frontFace = new List<int>{1,4,0,1,5,4};
		var backFace = new List<int>{2,6,3,6,7,3};
		var outerFace = new List<int>{1,3,5,3,7,5};
		var innerFace = new List<int>{4,2,0,4,6,2};
		var leftFace = new List<int>{2,1,0,2,3,1};
		var rightFace = new List<int>{4,5,6,5,7,6};
		var iconFace = new List<int>{3,6,2,3,7,6};
				
		//foreach(var f in new List<int>[]{leftFace}){
		foreach(var f in new List<int>[]{frontFace, backFace, outerFace, innerFace, leftFace, rightFace}){
			Triangles.AddRange(f);
		}
		
		colliderMesh.SetVertices(verts);
		iconMesh.SetVertices(verts);
		//Debug.Log(String.Format("{0} {1}",verts.Count, UVs.Count));
		iconMesh.SetUVs(0,UVs);
		colliderMesh.SetTriangles(Triangles,0);
		iconMesh.SetTriangles(iconFace,0);
		colliderMesh.RecalculateNormals();
		iconMesh.RecalculateNormals();
		
		AssetDatabase.CreateAsset(colliderMesh, meshName);
		AssetDatabase.SaveAssets();
		mc.sharedMesh = colliderMesh;
		//mf.mesh = iconMesh;
		//Gizmos.DrawMesh(colliderMesh, transform.position);
		Sections.Add(section);
	}
	
	/// <summary>
	/// calculate vertices needed for hit boxs
	/// </summary>
	public void BuildSections(){
		//Debug.Log(Application.dataPath);
		// Get the Prefab Asset root GameObject and its asset path.
		var prefab = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
		var prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(prefab);
		//Debug.Log(prefabPath);

		// Load the contents of the Prefab Asset.
		GameObject menuPrefabContents = PrefabUtility.LoadPrefabContents(prefabPath);
		ClearMenuChildren(ref menuPrefabContents);
		GetIconFiles();
		
		Vertices = new List<Vector3>();
		UVs = new List<Vector2>();
		Sections = new List<GameObject>();
		Icons = new List<GameObject>();
		
		// amount of rotation  needed per magic section
		float dtheta = (float)(2 * Math.PI / Magics);
		var marginTheta = Margin * dtheta;
		
		// offset to make first spawning section the spawn at the top of the circle
		float offsetToStartAtTop = (float)(2 * Math.PI / 4);
		// set starting point
		float theta = offsetToStartAtTop;
		
		
		
		for(var i = 0; i < Magics; i++){
			BuildMagicSection(ref menuPrefabContents, theta-marginTheta,theta - dtheta+marginTheta, i);
			theta -= dtheta;
		}

		PossibleCombinations = GetPossibleCombinations(ref menuPrefabContents);
		menuPrefabContents.GetComponent<MagicMenu>().PossibleCombinations = PossibleCombinations;
		//Debug.Log(String.Join(", ", x));

		PrefabUtility.SaveAsPrefabAsset(menuPrefabContents, prefabPath);
		PrefabUtility.UnloadPrefabContents(menuPrefabContents);
	}
	
	/// <summary>
	/// Remove old sections from menu
	/// </summary>
	/// <param name="menuPrefabContents">Prefab contents for menu</param>
	public void ClearMenuChildren(ref GameObject menuPrefabContents){
		var notSectionObjectsCount = 0;
		foreach(Transform o in menuPrefabContents.transform){
			if(!o.name.StartsWith("Section")){
				notSectionObjectsCount++;
			}
		}
		var i = 0;
		// Account for non section objects && catch incase something goes wrong
		while(menuPrefabContents.transform.childCount != notSectionObjectsCount && i < menuPrefabContents.transform.childCount+1){
			var s = menuPrefabContents.transform.GetChild(i).gameObject;
			if(s.name.StartsWith("Section")){
				DestroyImmediate(s);
			}else{
				i++;
			}
			
		}		
		//Debug.Log("Exited");
	}
	
	/// <summary>
	/// Get file paths for the magic icons
	/// </summary>
	public void GetIconFiles(){
		IconFiles = new List<string>();
		IconsDirPath = Application.dataPath + @"\Icons\100ppi";
		IconFiles.AddRange(Directory.EnumerateFiles(IconsDirPath, "*.png"));
		Magics = IconFiles.Count;		
	}
	
	public List<string> GetPossibleCombinations(ref GameObject menuPrefabContents){
		var sectionNames = new List<string>();
		var combos = new List<string>();
		foreach(Transform o in menuPrefabContents.transform){
			if(o.name.StartsWith("Section")){
				var ss = o.gameObject.GetComponent<SectionScript>();
				sectionNames.Add(ss.IconFileName);
				//Debug.Log(ss.IconFileName);
			}
		}
		var format = "{0} {1}";
		for(var i = 0; i < sectionNames.Count; i++){
			combos.Add(string.Format(format, sectionNames[i],sectionNames[i]));
			for(int i2 = i+1; i2 < sectionNames.Count; i2++){
				combos.Add(string.Format(format, sectionNames[i],sectionNames[i2]));
				//Debug.Log(string.Format("{0} {1}", sectionNames[i],sectionNames[i2]));
			}
			
		}
		
		return combos;
	}
	
	//public void SelectPowers(){
		
	//}
	
	
	public void Dismiss(){
		Closing = true;
		ShrinkingIcons = Icons;
	}
	
}





