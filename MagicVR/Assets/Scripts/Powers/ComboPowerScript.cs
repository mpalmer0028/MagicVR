using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

interface IPowerBase
{
	string Name {get; set;}		
}

interface IComboPower
{
	void ActivatePower(PowerSystemScript powerSystemScript);
	void RemovePower(PowerSystemScript powerSystemScript);
	
}

public class ComboPowerScript : MonoBehaviour, IComboPower
{
    // Start is called before the first frame update
    void Start()
    {
	    transform.Find("PrimaryHand").gameObject.active = false;
	    transform.Find("OffHand").gameObject.active = false;
    }
    
	public void ActivatePower(PowerSystemScript powerSystemScript)
	{
		//Debug.Assert(powerSystemScript.LeftHandSpawn.name.StartsWith("Left"));
		
		var primaryHandSpawn = powerSystemScript.LeftHanded ? powerSystemScript.LeftHandSpawn : powerSystemScript.RightHandSpawn;
		var offHandSpawn = powerSystemScript.LeftHanded ? powerSystemScript.RightHandSpawn : powerSystemScript.LeftHandSpawn;
		
		//Debug.Assert(primaryHandSpawn.name.StartsWith("Right") && !powerSystemScript.LeftHanded);
		
		var primaryPowerTransform = transform.Find("PrimaryHand");
		var offPowerTransform = transform.Find("OffHand");
		
		//var localPosition = primaryPowerTransform.localPosition;
		//var localRotation = primaryPowerTransform.localRotation;		
		var primaryPower = Instantiate(primaryPowerTransform.gameObject,primaryHandSpawn.transform);
		var offPower = Instantiate(offPowerTransform.gameObject,offHandSpawn.transform);
		
		if(powerSystemScript.LeftHanded){
			var pt = primaryPower.transform;
			var pp = pt.localPosition;
			pt.localPosition = new Vector3(pp.x*-1,pp.y,pp.z);
			var ot = offPower.transform;
			var op = ot.localPosition;
			ot.localPosition = new Vector3(op.x*-1,op.y,op.z);
		}
		primaryPower.active = true;
		offPower.active = true;
		
	}

	public void RemovePower(PowerSystemScript powerSystemScript)
	{
		var primaryHandSpawn = powerSystemScript.LeftHanded ? powerSystemScript.LeftHandSpawn : powerSystemScript.RightHandSpawn;
		var offHandSpawn = powerSystemScript.LeftHanded ? powerSystemScript.RightHandSpawn : powerSystemScript.LeftHandSpawn;
		
		var primaryPowerTransform = primaryHandSpawn.transform.GetChild(0);
		var offPowerTransform = offHandSpawn.transform.GetChild(0);
		
		Destroy(primaryPowerTransform.gameObject);
		Destroy(offPowerTransform.gameObject);
	}
    
}
