using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SnakeScript : MonoBehaviour
{
	public Transform Target;
	public GameObject Head;
	public float CorrectionSpeed = 1000;
	public float Push = 0.5f;
	public Vector3 RotationCorrection = new Vector3(90,0,0);
	public List<Transform> NeckTransforms;
	
	private WiggleScript WS;
	private Quaternion HeadStartRotation;
    // Start is called before the first frame update
    void Start()
    {
	    WS = gameObject.GetComponent<WiggleScript>();
	    HeadStartRotation = Head.transform.localRotation;
    }

    // Update is called once per frame
	void FixedUpdate()
	{
		
	    if(Target != null){
	    	//WS.Wiggling = true;
	    	
	    	//Head.transform.LookAt(Target.transform);
	    	//Head.transform.rotation *= Quaternion.LookRotation(target.position - transform.position);
	    	var qTo = Quaternion.LookRotation(Target.position - Head.transform.position);
		    qTo = Quaternion.Slerp(transform.rotation, qTo, CorrectionSpeed * Time.deltaTime)*Quaternion.Euler(RotationCorrection);
	    	var rb = Head.GetComponent<Rigidbody>();
	    	rb.MoveRotation(qTo);
	    	//Head.transform.position += (Target.position - transform.position).normalized*.01f;
		    rb.AddForce((Target.position - Head.transform.position).normalized*Push, ForceMode.Force);
	    }else{
	    	//WS.Wiggling = false;
	    }
	    
	}
    
	public void BuildSnake()
	{
		var rig = transform.Find("SnakeRig");
		Head = rig.Find("HeadTop.000").gameObject;
		
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
		
		//WiggleScript
		WS = GetComponent<WiggleScript>();
		if(WS == null){
			WS = gameObject.AddComponent(typeof(WiggleScript)) as WiggleScript;
		}
		Transform iNeckT = rig.Find("Neck.007");
		NeckTransforms = new List<Transform>{Head.transform};
		var rbs = new List<Rigidbody>();
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
				
								
			if(iNeckT.name != "Neck.007"){
				
				
			}else{
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
		
		WS.JointRigidbodys = rbs.ToArray();
		WS.RepetitionsOverRigidbodys = 3;
		WS.WiggleMagnitudeZ = 15;
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
