using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.IO;

public class MagicMenu : MonoBehaviour
{
	
	public float Depth = .5f;
	public float Margin = .25f;
	public GameObject SectionPrefab;
	public string IconsDirPath;

	private List<GameObject> Icons = new List<GameObject>();
	private List<string> IconFiles = new List<string>();
	private int Magics = 10;
	private List<GameObject> Sections = new List<GameObject>();
	private List<int> Triangles = new List<int>();
	private List<Vector2> UVs = new List<Vector2>();
	private List<Vector3> Vertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
	    //BuildSections();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		//if(transform.childCount < Magics){
		//	BuildSections();
		//}else{
		//	//foreach(var sec in Sections){
		//	//	var mf = sec.Find("Icon").GetComponent<MeshFilter>();
		//	//	Gizmos.DrawMesh(mf.mesh, transform.position);
		//	//}
		//}
		foreach(var v in Vertices){
			Gizmos.DrawWireSphere(transform.position + v, .01f);
		}
		//Debug.Log(transform.childCount);
	}
	
	/// <summary>
	/// Build hitbox, visual faces, and gizmo dots
	/// </summary>
	/// <param name="angle">Angle of the start of the section in radians</param>
	/// <param name="nextAngle">Angle of the end of the section in radians</param>
	/// <param name="index">Section index for naming</param>
	private void BuildMagicSection(float angle, float nextAngle, int index){
		// for centering the uv positions so between 0-1 
		var centerVec2 = new Vector2(.5f,.5f);
		var verts = new List<Vector3>();
		
		Vertices = new List<Vector3>();
		UVs = new List<Vector2>();

		
		
		var uvV2 = new Vector2(0,0);		
		foreach(var a in new float[]{angle, nextAngle}){
			var x = (float)Math.Cos(a);
			var y = (float)Math.Sin(a);
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
		var section = Instantiate(SectionPrefab, transform);
		section.name = string.Format("Section {0}", index);
		
		var icon = section.transform.Find("Icon").gameObject;		
		var ss = section.GetComponent<SectionScript>();		
		ss.IconFileName = Path.GetFileName(IconFiles[index]).Split('.').First();
		Icons.Add(icon);
		IQueryable<Vector3> vertsQ = verts.AsQueryable();
		var iconLocation = new Vector3(vertsQ.Average(v => v.x), vertsQ.Average(v => v.y), 0f);
		
		Texture2D tmpTexture = new Texture2D(1,1);
		byte[] tmpBytes = File.ReadAllBytes(IconFiles[index]);
		tmpTexture.LoadImage(tmpBytes);
		
		var render = icon.GetComponent<Renderer>();
		// Use temp material to prevent leak
		var tempMaterial = new Material(render.sharedMaterial);
		tempMaterial.SetTexture("ImageTexture", tmpTexture);
		render.sharedMaterial = tempMaterial;

		icon.transform.localPosition = iconLocation;
		
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
		ClearMenuChildren();
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
			BuildMagicSection(theta-marginTheta,theta - dtheta+marginTheta, i);
			theta -= dtheta;
		}

		
	}
	
	public void ClearMenuChildren(){
		while(transform.childCount != 0){
			DestroyImmediate(transform.GetChild(0).gameObject);
		}		
	}
	
	public void GetIconFiles(){
		IconFiles = new List<string>();
		IconsDirPath = Application.dataPath + @"\Icons\72ppi";
		IconFiles.AddRange(Directory.EnumerateFiles(IconsDirPath, "*.png"));
		Magics = IconFiles.Count;		
	}
}
