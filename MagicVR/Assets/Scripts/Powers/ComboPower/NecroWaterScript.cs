using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NecroWaterScript : ShootingPowerScript
{
	public SharkScript SharkScriptPrimary;
	public SharkScript SharkScriptOff;
	// Update is called once per frame
	public override void Update()
	{
		base.Update();
		if(TriggerAmountPrimary > MinimumTrigger && Projectiles.Count < MaxProjectiles 
			&& Time.time > WaitTill && ProjectilePrimary == null){	
				
			var SharkPrimary = LoadShot();
			SharkScriptPrimary = SharkPrimary.GetComponent<SharkScript>();
			SharkScriptPrimary.Target = VisionTarget.transform;
			SharkScriptPrimary.Fired = !ChargeShots;			
			
			if(TriggerAmountOff > MinimumTrigger && Projectiles.Count < MaxProjectiles 
				&& Time.time + ProjectileInterval >= WaitTill && ProjectileOffHand == null){
		    	
				var SharkOff = LoadShotOffHand();
				SharkScriptOff = SharkOff.GetComponent<SharkScript>();
				SharkScriptOff.Target = VisionTarget.transform;
				//var offHeadRB = SharkScriptOff.Head.GetComponent<Rigidbody>();
				//offHeadRB.constraints = ChargeShots? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
				}
		    
			}else if(TriggerAmountOff > MinimumTrigger && Projectiles.Count < MaxProjectiles 
				&& Time.time > WaitTill && ProjectileOffHand == null){
				
				var SharkOff = LoadShotOffHand();
				SharkScriptOff = SharkOff.GetComponent<SharkScript>();
				SharkScriptOff.Target = VisionTarget.transform;
				//var offHeadRB = SharkScriptOff.Head.GetComponent<Rigidbody>();
				//offHeadRB.constraints = ChargeShots? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
				}
		
		
	}
}
