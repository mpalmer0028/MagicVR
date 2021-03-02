using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


public class WiggleScript : MonoBehaviour
{
	public bool Wiggling;
	
	public Rigidbody[] JointRigidbodys;
	public ConfigurableJoint[] ConfigurableJoints;
	
	public AnimationCurve WiggleX;
	public AnimationCurve WiggleY;
	public AnimationCurve WiggleZ;
	
	public int RepetitionsOverRigidbodys = 1;
	public float WiggleMagnitudeX = 0;
	public float WiggleMagnitudeY = 0;
	public float WiggleMagnitudeZ = 0;
	
	private float StartTime;
	
	// Time offset between Rigidbodys
	private float WiggleOffset;
	
    // Start is called before the first frame update
    void Start()
    {
	    StartTime = Time.time;
	    WiggleOffset = WiggleX.length/JointRigidbodys.Count();
	    Debug.Assert(WiggleX.length == WiggleY.length);
	    Debug.Assert(WiggleX.length == WiggleZ.length);
    }

    // Update is called once per frame
    void Update()
	{
		if(Wiggling)
	    // Wiggle
	    for(var i = 0; i < JointRigidbodys.Count();i++){
		    var t =  (Time.time-StartTime)*RepetitionsOverRigidbodys;
				
		    var torque = new Vector3(
			    (WiggleX.Evaluate(t+(WiggleOffset*i))-.5f)*WiggleMagnitudeX,
			    (WiggleY.Evaluate(t+(WiggleOffset*i))-.5f)*WiggleMagnitudeY,
			    (WiggleZ.Evaluate(t+(WiggleOffset*i))-.5f)*WiggleMagnitudeZ
		    );
		    JointRigidbodys[i].AddRelativeTorque(torque);
	    }
    }
}
