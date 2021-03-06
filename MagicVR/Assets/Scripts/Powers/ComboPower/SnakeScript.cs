﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnakeScript : ProjectileScript
{
	//public Animator SnakeAnimator;
	public GameObject Head;
	
	public float CorrectionSpeed = 1000;
	public float Push = 0.5f;
	public float FadeAmount = 0.05f;
	public Vector3 RotationCorrection = new Vector3(90,0,0);
	public List<Transform> NeckTransforms;
	
	private AudioSource AudioSource;
	private Animator Animator;
	
	private Transform FadeInSnake;
	private Transform MeshSnake;
	private Transform MeshRig;
	private Transform AnimationRig;
	private Transform NeckStart;
	private Transform[] AnimationOnlyBones;
	private Transform[] AnimationOnlyReferenceBones;
	
	private WiggleScript WS;
	private Quaternion HeadStartRotation;
	private SkinnedMeshRenderer SnakeSkinnedMeshRenderer;
	private SkinnedMeshRenderer FadeInSnakeSkinnedMeshRenderer;
	
	public override bool Fired{
		get{
			return _Fired;
		}
		set{
			_Fired = value;
			
			var headRB = Head.GetComponent<Rigidbody>();
			if(value && AnimationRig != null){
				Destroy(gameObject, 10);
				Destroy(AnimationRig.gameObject, 10);
			}
		
			
			headRB.constraints = !Fired ? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
		}
	}
	
    // Start is called before the first frame update
    void Start()
	{
		AudioSource = GetComponent<AudioSource>();
		SnakeSkinnedMeshRenderer = transform.Find("SnakeProjectile").Find("Snake").GetComponent<SkinnedMeshRenderer>();
		
		WS = GetComponent<WiggleScript>();
		
	    HeadStartRotation = Head.transform.localRotation;
	    var headRB = Head.GetComponent<Rigidbody>();
		headRB.constraints = !Fired ? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
		
		AnimationRig = transform.Find("SnakeAnimator");
		Animator = AnimationRig.GetComponent<Animator>();
		
		AnimationRig.parent = null;
		AnimationRig.position = Vector3.zero;
		
		MeshRig = transform.Find("SnakeProjectile").Find("SnakeRig");
		AnimationOnlyBones = new Transform[]{
			MeshRig.Find("HeadTop.000").Find("HeadTop.001"),
			MeshRig.Find("HeadTop.000").Find("HeadTop.001").Find("Fangs"),
			MeshRig.Find("HeadTop.000").Find("HeadTop.001").Find("Jaw.000"),
			MeshRig.Find("HeadTop.000").Find("HeadTop.001").Find("Jaw.000").Find("Jaw.001"),
			MeshRig.Find("HeadTop.000").Find("Tongue.000"),
			MeshRig.Find("HeadTop.000").Find("Tongue.000").Find("Tongue.001")
		};
		
		AnimationOnlyReferenceBones = new Transform[]{
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("HeadTop.001"),
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("HeadTop.001").Find("Fangs"),
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("HeadTop.001").Find("Jaw.000"),
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("HeadTop.001").Find("Jaw.000").Find("Jaw.001"),
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("Tongue.000"),
			AnimationRig.Find("SnakeRig").Find("HeadTop.000").Find("Tongue.000").Find("Tongue.001")
		};
		NeckStart = AnimationRig.Find("SnakeRig").Find("HeadTop.000");
		//foreach(var z in AnimationOnlyBones){
		//	Debug.Log(z);
		//}
		
		// Disable visible snake
		WS.enabled = false;
		MeshSnake = transform.Find("SnakeProjectile");
		FadeInSnake = transform.Find("FadeInSnake");
		
		FadeInSnake.gameObject.active = true;
		MeshSnake.gameObject.active = false;
		
		FadeInSnakeSkinnedMeshRenderer = FadeInSnake.Find("Snake").GetComponent<SkinnedMeshRenderer>();
		foreach(var m in FadeInSnakeSkinnedMeshRenderer.materials){
			if(m != null){
				var c = m.color;
				c.a = 0;
				m.color = c;
			}
			
		}
    }

    // Update is called once per frame
	void FixedUpdate()
	{
		
		if(Target != null && MeshSnake.gameObject.active){
	    	//WS.Wiggling = true;
	    	
	    	//Head.transform.LookAt(Target.transform);
	    	//Head.transform.rotation *= Quaternion.LookRotation(target.position - transform.position);
	    	var qTo = Quaternion.LookRotation(Target.position - Head.transform.position)*NeckStart.localRotation;
		    qTo = Quaternion.Slerp(transform.rotation, qTo, CorrectionSpeed * Time.deltaTime)*Quaternion.Euler(RotationCorrection)*AnimationRig.Find("SnakeRig").Find("HeadTop.000").localRotation;
	    	var rb = Head.GetComponent<Rigidbody>();
	    	rb.MoveRotation(qTo);
		    rb.AddForce((Target.position - Head.transform.position).normalized*Push, ForceMode.Force);
		    //SnakeAnimator.SetFloat("", Vector3.Distance(Target.position, Head.transform.position));
		    var distance = Vector3.Distance(Head.transform.position, Target.position);
		    Animator.SetFloat("Distance", distance);
		    //Debug.Log(distance);
		    if(distance < 5 && !AudioSource.isPlaying){
		    	AudioSource.Play();
		    }
		
		}else if(Target != null && !MeshSnake.gameObject.active){
			if(FadeInSnakeSkinnedMeshRenderer.sharedMaterials[0].color.a < 1){
				var sm = FadeInSnakeSkinnedMeshRenderer.sharedMaterials;
				sm[0].color += new Color(0,0,0,FadeAmount);
				sm[3].color += new Color(0,0,0,FadeAmount);
				FadeInSnakeSkinnedMeshRenderer.sharedMaterials = sm;
				var qTo = Quaternion.LookRotation(Target.position - FadeInSnake.position);
				qTo = Quaternion.Slerp(transform.rotation, qTo, CorrectionSpeed * Time.deltaTime);
				
				FadeInSnake.rotation = qTo;
			}else{
				MeshSnake.gameObject.active = true;
				FadeInSnake.gameObject.active = false;
			}
	    	//WS.Wiggling = false;
	    }
	    
		//if(SnakeSkinnedMeshRenderer.materials[0].color.a <1){
		//	Debug.Log(SnakeSkinnedMeshRenderer.materials[0].color);
		//	foreach(var m in SnakeSkinnedMeshRenderer.materials){
		//		var c = m.color;
		//		c.a+= .01f;
		//		m.color = c;				
		//		//m.color.a+= new Color(m.color.r, m.color.b, m.color.g, SnakeSkinnedMeshRenderer.materials[0].color.a + .01f);
		//	}
		//}else if(SnakeSkinnedMeshRenderer.materials[0].GetFloat("_Surface") == 1){
		//	foreach(var m in SnakeSkinnedMeshRenderer.materials){
				
		//		m.SetFloat("_Surface",0);				
		//		//m.color.a+= new Color(m.color.r, m.color.b, m.color.g, SnakeSkinnedMeshRenderer.materials[0].color.a + .01f);
		//	}
		//}
		
		
		for(var i = 0; i<AnimationOnlyReferenceBones.Length; i++){
			AnimationOnlyBones[i].localRotation = AnimationOnlyReferenceBones[i].localRotation;
		}
		// Fangs
		AnimationOnlyBones[1].localScale = AnimationOnlyReferenceBones[1].localScale;
		AnimationOnlyBones[4].localScale = AnimationOnlyReferenceBones[4].localScale;
		AnimationOnlyBones[5].localScale = AnimationOnlyReferenceBones[5].localScale;
	}
	
	public void BuildSnake()
	{
		
		MeshSnake = transform.Find("SnakeProjectile");
		MeshRig = MeshSnake.Find("SnakeRig");
		Head = MeshRig.Find("HeadTop.000").gameObject;
		
		// WiggleScript
		WS = GetComponent<WiggleScript>();
		if(WS == null){
			WS = gameObject.AddComponent(typeof(WiggleScript)) as WiggleScript;
		}
		
		// Audio
		AudioSource = GetComponent<AudioSource>();
		if(AudioSource == null){
			AudioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		}
		AudioSource.clip = (AudioClip)AssetDatabase.LoadAssetAtPath("Assets/Audio/hiss.001.mp3", typeof(AudioClip));
		
		// Build animation rig
		AnimationRig = transform.Find("SnakeAnimator");
		if(AnimationRig == null){
			AnimationRig = Instantiate(MeshSnake, transform);
			AnimationRig.name = "SnakeAnimator";
			
		}
		transform.Find("SnakeAnimator").Find("Snake").gameObject.active = false;
		Animator = AnimationRig.GetComponent<Animator>();
		if(Animator == null){
			Animator = AnimationRig.gameObject.AddComponent(typeof(Animator)) as Animator;
		}
		Animator.runtimeAnimatorController = (RuntimeAnimatorController)AssetDatabase.LoadAssetAtPath("Assets/Animation/SnakeAnimator.controller", typeof(RuntimeAnimatorController));
		
		// Build fade in snake
		FadeInSnake = transform.Find("FadeInSnake");
		if(FadeInSnake == null){
			FadeInSnake = Instantiate(MeshSnake, transform);
			FadeInSnake.name = "FadeInSnake";
		}
		// Set FadeInSnake materials 		
		FadeInSnakeSkinnedMeshRenderer = FadeInSnake.Find("Snake").GetComponent<SkinnedMeshRenderer>();
		var fadeMats = FadeInSnakeSkinnedMeshRenderer.sharedMaterials;
		fadeMats[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/SnakeSkinFade.mat", typeof(Material));
		fadeMats[1] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/Clear.mat", typeof(Material));
		fadeMats[2] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/Clear.mat", typeof(Material));
		fadeMats[3] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/SnakeEyeFade.mat", typeof(Material));
		fadeMats[4] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/Clear.mat", typeof(Material));
		FadeInSnakeSkinnedMeshRenderer.sharedMaterials = fadeMats;

		// Box Collider
		BoxCollider bcHead = Head.gameObject.GetComponent<BoxCollider>();
		if(bcHead == null){
			bcHead = Head.gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
		}
				
		bcHead.center = new Vector3(0,0.00075f,0);
		bcHead.size = new Vector3(0.0008f,0.0015f,0.0005f);
		//bc.material = PhysicMaterial
			
		// Rigidbody 
		var rbHead = Head.GetComponent<Rigidbody>();
		if(rbHead == null){
			rbHead = Head.AddComponent(typeof(Rigidbody)) as Rigidbody;
		}
		rbHead.mass = 0.1f;
		rbHead.useGravity = false;
		
		
		// Z - sideways motion
		var curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(5, 1));
		curve.preWrapMode = WrapMode.PingPong;
		curve.postWrapMode = WrapMode.PingPong;		
		WS.WiggleZ = curve;
		
		// other motion
		curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(5, 0));
		curve.preWrapMode = WrapMode.PingPong;
		curve.postWrapMode = WrapMode.PingPong;
		
		WS.WiggleX = curve;
		WS.WiggleY = curve;
		
		// Joints for neck
		NeckTransforms = new List<Transform>{Head.transform};
		var rbs = new List<Rigidbody>();
		Transform iNeckT = MeshRig.Find("Neck.007");
		while(((Transform)iNeckT).name != "Neck.000_end")
		{
			
			NeckTransforms.Add(iNeckT);
			
			// Box Collider
			BoxCollider bc = ((Transform)iNeckT).gameObject.GetComponent<BoxCollider>();
			if(bc == null){
				bc = ((Transform)iNeckT).gameObject.AddComponent(typeof(BoxCollider)) as BoxCollider;
			}
				
			bc.center = new Vector3(0,0.00105f,0);
			bc.size = new Vector3(0.0005f,0.0015f,0.0005f);
			//bc.material = PhysicMaterial
			
			// Rigidbody 
			var rb = ((Transform)iNeckT).gameObject.GetComponent<Rigidbody>();
			if(rb == null){
				rb = ((Transform)iNeckT).gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
			}
				
			rb.mass = .01f;
			rb.angularDrag = .02f;
			rb.useGravity = false;
			
			rbs.Add(rb);
			
			// Config joint 
			var cj = ((Transform)iNeckT).gameObject.GetComponent<ConfigurableJoint>();
			if(cj == null){
				cj = ((Transform)iNeckT).gameObject.AddComponent(typeof(ConfigurableJoint)) as ConfigurableJoint;
			}
			cj.connectedBody = NeckTransforms[NeckTransforms.Count-2].GetComponent<Rigidbody>();
				
								
			if(iNeckT.name == "Neck.007"){
				cj.connectedBody = Head.GetComponent<Rigidbody>();
				
			}

			
			iNeckT = iNeckT.GetChild(0);
			cj.xMotion = ConfigurableJointMotion.Locked;
			cj.yMotion = ConfigurableJointMotion.Locked;
			cj.zMotion = ConfigurableJointMotion.Locked;
			cj.angularXMotion = ConfigurableJointMotion.Limited;
			cj.angularYMotion = ConfigurableJointMotion.Limited;
			cj.angularZMotion = ConfigurableJointMotion.Limited;
				
			cj.lowAngularXLimit = new SoftJointLimit{limit = -45f};
			cj.highAngularXLimit = new SoftJointLimit{limit = 45f};
			cj.angularYLimit = new SoftJointLimit{limit = 10f};
			cj.angularZLimit = new SoftJointLimit{limit = 45f};
			
		}
		
		
		// wiggle settings
		WS.JointRigidbodys = rbs.ToArray();
		WS.RepetitionsOverRigidbodys = 3;
		WS.WiggleMagnitudeZ = 15;
		
		// Set main materials 		
		SnakeSkinnedMeshRenderer = MeshSnake.Find("Snake").GetComponent<SkinnedMeshRenderer>();
		//Debug.Assert(SnakeSkinnedMeshRenderer != null);
		var mats = SnakeSkinnedMeshRenderer.sharedMaterials;
		mats[0] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/SnakeSkin.mat", typeof(Material));
		mats[1] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/SnakeMouth.mat", typeof(Material));
		mats[2] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/Tongue.mat", typeof(Material));
		mats[3] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/SnakeEye.mat", typeof(Material));
		mats[4] = (Material)AssetDatabase.LoadAssetAtPath("Assets/Materials/Powers/Snake/Teeth.mat", typeof(Material));
		SnakeSkinnedMeshRenderer.sharedMaterials = mats;
		//for(var i = 0; i<SnakeSkinnedMeshRenderer.sharedMaterials[0].shader.GetPropertyCount();i++){
		//	Debug.Log(SnakeSkinnedMeshRenderer.sharedMaterials[0].shader.GetPropertyName(i)+" "+ SnakeSkinnedMeshRenderer.sharedMaterials[0].shader.GetPropertyType(i).ToString());
		//	//Debug.Log();
		//}
		
	}
	
	
}



[CustomEditor(typeof(SnakeScript))]
public class SnakeScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var snakeScript = (SnakeScript)target;
		if(GUILayout.Button("Rebuild Snek"))
		{
			//Debug.Log(magicMenuScript.gameObject.name);
			snakeScript.BuildSnake();
		}
	}
}
