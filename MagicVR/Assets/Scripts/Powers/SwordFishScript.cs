using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFishScript : MonoBehaviour
{
	public bool BodyLocked {
		get{
			return this.GetComponentsInChildren<ConfigurableJoint>()[0].angularXMotion == ConfigurableJointMotion.Locked;
		}
		set{
			var motion = value ? ConfigurableJointMotion.Locked : ConfigurableJointMotion.Limited;
			foreach(var j in this.GetComponentsInChildren<ConfigurableJoint>()){
				j.angularXMotion = motion;
				j.angularYMotion = motion;
				j.angularZMotion = motion;
			}
		}
	}
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
	
}
