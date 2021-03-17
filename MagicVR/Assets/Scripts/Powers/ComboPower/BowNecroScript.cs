using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowNecroScript : ShootingPowerScript
{
	public float MinimumTrigger = .25f;
	
	public SnakeScript SnakeScriptPrimary;
	public SnakeScript SnakeScriptOff;
    // Update is called once per frame
	public override void Update()
	{
		base.Update();
		if(TriggerAmountPrimary > MinimumTrigger && Projectiles.Count < MaxProjectiles 
		&& Time.time > WaitTill && ProjectilePrimary == null){	
				
		    var snakePrimary = LoadShot();
		    SnakeScriptPrimary = snakePrimary.GetComponent<SnakeScript>();
			SnakeScriptPrimary.Target = VisionTarget.transform;
			SnakeScriptPrimary.Fired = !ChargeShots;			
			
			if(TriggerAmountOff > MinimumTrigger && Projectiles.Count < MaxProjectiles 
				&& Time.time + ProjectileInterval >= WaitTill && ProjectileOffHand == null){
		    	
				var snakeOff = LoadShotOffHand();
				SnakeScriptOff = snakeOff.GetComponent<SnakeScript>();
				SnakeScriptOff.Target = VisionTarget.transform;
				var offHeadRB = SnakeScriptOff.Head.GetComponent<Rigidbody>();
				offHeadRB.constraints = ChargeShots? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
			}
		    
		}else if(TriggerAmountOff > MinimumTrigger && Projectiles.Count < MaxProjectiles 
		&& Time.time > WaitTill && ProjectileOffHand == null){
				
		    var snakeOff = LoadShotOffHand();
		    SnakeScriptOff = snakeOff.GetComponent<SnakeScript>();
		    SnakeScriptOff.Target = VisionTarget.transform;
		    var offHeadRB = SnakeScriptOff.Head.GetComponent<Rigidbody>();
		    offHeadRB.constraints = ChargeShots? RigidbodyConstraints.FreezePosition: RigidbodyConstraints.None;
		}
		
		if(TriggerAmountPrimary <= MinimumTrigger && ProjectilePrimary != null){			
			ReleasePrimary();
		}
		if(TriggerAmountOff <= MinimumTrigger && ProjectileOffHand != null){
			ReleaseOffHand();
		}
	}
    
}
