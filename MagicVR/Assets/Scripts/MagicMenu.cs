﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MagicMenu : MonoBehaviour
{
	public int Magics = 10;
	public float Depth = .5f;
	public GameObject SectionPrefab;
	private List<GameObject> Sections = new List<GameObject>();
	private List<GameObject> Icons = new List<GameObject>();
	private List<int> Triangles = new List<int>();
	private List<Vector2> UVs = new List<Vector2>();
	private List<Vector3> Vertices = new List<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
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

		foreach(var a in new float[]{angle, nextAngle}){
			var x = (float)Math.Cos(a);
			var y = (float)Math.Sin(a);
			
			#region Front
			// Inner ring
			verts.Add((new Vector3(x,y,0)*.2f) - new Vector3(0,0,Depth));
			UVs.Add((new Vector2(x,y)*.2f/2)+centerVec2);
			
			// Outer ring
			verts.Add(new Vector3(x,y,0) - new Vector3(0,0,Depth));
			UVs.Add((new Vector2(x,y)/2)+centerVec2);
			#endregion
		
			#region  Back 
			// Inner ring
			verts.Add((new Vector3(x,y,0)*.2f));
			UVs.Add((new Vector2(x,y)*.2f/2)+centerVec2);
			
			// Outer ring
			verts.Add(new Vector3(x,y,0));
			UVs.Add((new Vector2(x,y)/2)+centerVec2);
			#endregion	
		}	
		
		Vertices.AddRange(verts);
			
		// Add Section
		var section = Instantiate(SectionPrefab, transform);
		section.name = string.Format("Section {0}", index);
		
		var icon = section.transform.Find("Icon").gameObject;
		Icons.Add(icon);
		
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
		colliderMesh.SetTriangles(Triangles,0);
		iconMesh.SetTriangles(iconFace,0);
		colliderMesh.RecalculateNormals();
		iconMesh.RecalculateNormals();
		
		mc.sharedMesh = colliderMesh;
		mf.mesh = iconMesh;
		//Gizmos.DrawMesh(colliderMesh, transform.position);
		Sections.Add(section);
	}
	
	/// <summary>
	/// calculate vertices needed for hit boxs
	/// </summary>
	public void BuildSections(){
		ClearMenuChildren();
		Vertices = new List<Vector3>();
		Sections = new List<GameObject>();
		Icons = new List<GameObject>();
		// amount of rotation  needed per magic section
		float dtheta = (float)(2 * Math.PI / Magics);
		
		// offset to make first spawning section the spawn at the top of the circle
		float offsetToStartAtTop = (float)(2 * Math.PI / 4);
		// set starting point
		float theta = offsetToStartAtTop;
		
		for(var i = 0; i < Magics; i++){
			BuildMagicSection(theta,theta - dtheta, i);
			theta -= dtheta;
		}
		
		
	}
	
	public void ClearMenuChildren(){
		while(transform.childCount != 0){
			DestroyImmediate(transform.GetChild(0).gameObject);
		}		
	}
}
