using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class SwordWaterScript : ComboPowerScript
{
	public AnimationCurve Growth;
	public float GrowthMagnitude = 1;
	
	public AnimationCurve Wiggle;
	public float WiggleMagnitude = 1;
	
	public float PointForce = 10;
	
	private bool BodyLocked = false;
	private ConfigurableJoint[] ConfigurableJoints;
	private Vector2[] StartRigidness;
	private Vector2[] StartDamping;
	private float StartTime;
	private float WiggleOffset;
	private ParticleSystem WaterPS;
	private AudioSource WaterAudioSource;
	private Rigidbody[] FreezableRigidbodys;
	private Rigidbody TipRigidbody;
	
	public override void Start(){
		base.Start();
		StartTime = Time.time;
		var jc = PrimaryPower.GetComponent<JointCollection>();
		ConfigurableJoints = jc.ConfigurableJoints;
				
		var sfs = PrimaryPower.GetComponent<SwordFishScript>();
		WaterPS = sfs.WaterPS;
		WaterAudioSource = sfs.WaterAudioSource;
		FreezableRigidbodys = sfs.JointRigidbodys;
		TipRigidbody = sfs.TipRigidbody;
				
		StartRigidness = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.spring,x.angularYZLimitSpring.spring)).ToArray();
		StartDamping = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.damper,x.angularYZLimitSpring.damper)).ToArray();
		
		WiggleOffset = Wiggle.length/FreezableRigidbodys.Count();
	}
	
	void Update(){
		if(!WaterAudioSource.isPlaying){
			BodyLocked = false;
			WaterPS.enableEmission = false;
		}
		if(BodyLocked){
			// Grow/shrink
			var growth = (Growth.Evaluate(Time.time-StartTime)*.1f*GrowthMagnitude)+1;
			var scale =  new Vector3(growth,growth,growth);			
			for(var i = 0; i < FreezableRigidbodys.Count();i++){
				FreezableRigidbodys[i].transform.localScale = scale;

			}
			// Point
			TipRigidbody.AddForce((VisionTarget.transform.position-PrimaryPower.transform.position)*PointForce);
		}else{
			// Wiggle
			for(var i = 0; i < FreezableRigidbodys.Count();i++){
				var t =  (Time.time-StartTime)*2;
				
				var wiggle = (Wiggle.Evaluate(t+(WiggleOffset*i))-.5f)*WiggleMagnitude;
				var torque = new Vector3(0,0,wiggle);
				FreezableRigidbodys[i].AddRelativeTorque(torque);
			}
			
			// Correct scale
			var amountOff = FreezableRigidbodys[0].transform.localScale.x-1;
			var step = .001f;
			if(!(amountOff<step && amountOff>-step)){
				if(amountOff>0){
					step *= -1;
				}
				for(var i = 0; i < FreezableRigidbodys.Count();i++){
					FreezableRigidbodys[i].transform.localScale += new Vector3(step, step, step);
				}
			}
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
				StartTime = Time.time;
				
				// Start spray
				WaterPS.enableEmission = true;
				WaterAudioSource.Play();
				
				for(var i = 0; i < FreezableRigidbodys.Count();i++){
					//FreezableRigidbodys[i].constraints = RigidbodyConstraints.FreezeAll;
				}
			}
			if(triggerAmount <= .25f && BodyLocked){
				BodyLocked = false;
				
				// Start spray
				WaterPS.enableEmission = false;
				WaterAudioSource.Stop();
				
				for(var i = 0; i < FreezableRigidbodys.Count();i++){
					//FreezableRigidbodys[i].constraints = RigidbodyConstraints.None;
				}
			}
		}
	}
}
