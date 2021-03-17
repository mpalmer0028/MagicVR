using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
	public float Life = 10;
	public Transform Target;
	public ShootingPowerScript ShootingPowerScript;
	
	[SerializeField]
	protected bool _Fired;
	public virtual bool Fired{
		get{
			return _Fired;
		}
		set{
			_Fired = value;
			Destroy(gameObject, Life);
		}
	}
	public bool Homing;
    
	public virtual void OnDestroy(){
		ShootingPowerScript.Projectiles.Remove(gameObject);
	}
}
