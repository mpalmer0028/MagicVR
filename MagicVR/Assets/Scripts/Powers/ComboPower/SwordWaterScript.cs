using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class SwordWaterScript : ComboPowerScript
{
	
	
	private bool BodyLocked = false;
	private ConfigurableJoint[] ConfigurableJoints;
	private Vector2[] StartRigidness;
	private Vector2[] StartDamping;
	private ParticleSystem WaterPS;
	private AudioSource WaterAudioSource;
	
	public override void Start(){
		base.Start();
		var jc = PrimaryPower.GetComponent<JointCollection>();
		ConfigurableJoints = jc.ConfigurableJoints;
				
		var sfs = PrimaryPower.GetComponent<SwordFishScript>();
		WaterPS = sfs.WaterPS;
		WaterAudioSource = sfs.WaterAudioSource;
				
		StartRigidness = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.spring,x.angularYZLimitSpring.spring)).ToArray();
		StartDamping = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.damper,x.angularYZLimitSpring.damper)).ToArray();
	}
	
	void Update(){
		if(!WaterAudioSource.isPlaying){
			WaterPS.enableEmission = false;
		}
	}
	
	public override void UpdatePowerTrigger(float triggerAmount, bool offHand){
		base.UpdatePowerTrigger(triggerAmount, offHand);
		//Debug.Log(offHand);
		if(!offHand){
			//Debug.Log(triggerAmount);
			//Debug.Log(BodyLocked);
			if(triggerAmount > .25f && !BodyLocked){
				BodyLocked = true;
				// Start spray
				WaterPS.enableEmission = true;
				WaterAudioSource.Play();
				
				//for(var i = 0; i < ConfigurableJoints.Count();i++){
				//	ConfigurableJoints[i].angularXLimitSpring = new SoftJointLimitSpring{spring = 10000, damper = 600};
				//	ConfigurableJoints[i].angularYZLimitSpring = new SoftJointLimitSpring{spring = 10000, damper = 600};
				//}
			}
			if(triggerAmount <= .25f && BodyLocked){
				BodyLocked = false;
				// Start spray
				WaterPS.enableEmission = false;
				WaterAudioSource.Stop();
				//for(var i = 0; i < ConfigurableJoints.Count();i++){
				//	ConfigurableJoints[i].angularXLimitSpring = new SoftJointLimitSpring{spring = StartRigidness[i].x, damper = StartDamping[i].x};
				//	ConfigurableJoints[i].angularYZLimitSpring = new SoftJointLimitSpring{spring = StartRigidness[i].y, damper = StartDamping[i].y};
				//}
			}
		}
	}
}
