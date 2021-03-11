using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
	public Transform Target;
	
	[SerializeField]
	protected bool _Fired;
	public virtual bool Fired{
		get{
			return _Fired;
		}
		set{
			_Fired = value;
		}
	}
	public bool Homing;
    
}
