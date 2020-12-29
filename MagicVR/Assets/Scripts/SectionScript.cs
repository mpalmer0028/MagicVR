using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionScript : MonoBehaviour
{
	public string IconFileName;
	public float FocusSlide = -.2f;
	public float FocusTime = 1f;
	
	public bool InTheZone = false;
	private float AnimationStartTime;
	public GameObject Icon;

	void OnCollisionEnter(Collision collision)
	{
		InTheZone = true;
		AnimationStartTime = Time.time;
		//Debug.Log("in");
	}

	void OnCollisionExit(Collision collisionInfo)
	{
		InTheZone = false;
		AnimationStartTime = Time.time;
	}
	
    // Start is called before the first frame update
    void Start()
	{
		Icon = transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
	{
		var t = Icon.transform;
		var scaleAmount = .05f;
		if(InTheZone){
			//Icon.transform.localPosition = new Vector3(0,0,-.5f);
	    	if(Icon.transform.localPosition.z > FocusSlide){
	    		
	    		t.localPosition -= new Vector3(0,0,.01f);
	    	}
			if(Icon.transform.localScale.x < 2){
	    		
				t.localScale += new Vector3(scaleAmount,scaleAmount,scaleAmount);
	    	}
		}else{
			//Icon.transform.localPosition = new Vector3(0,0,0);
			if(Icon.transform.localPosition.z < 0){
				t.localPosition += new Vector3(0,0,.01f);
			}
			if(Icon.transform.localScale.x > 1){
	    		
				t.localScale -= new Vector3(scaleAmount,scaleAmount,scaleAmount);
			}
	    }
    }
}
