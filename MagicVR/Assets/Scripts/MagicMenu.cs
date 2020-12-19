using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MagicMenu : MonoBehaviour
{
	public int Magics = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
	void OnDrawGizmos()
	{
		float dtheta = (float)(2 * Math.PI / Magics);
		float theta = 0;
		Gizmos.color = Color.green;
		for(var i = 0; i < Magics; i++){
			var x = (float)Math.Cos(theta);
			var y = (float)Math.Sin(theta);
			Gizmos.DrawWireSphere(transform.position + (new Vector3(x,y,0)/5), .01f);
			theta += dtheta;
		}
		
	}
}
