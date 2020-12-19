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
		Gizmos.color = Color.green;
		
		// amount of rotation  needed per magic section
		float dtheta = (float)(2 * Math.PI / Magics);
		// offset to make first spawning section the spawn at the top of the circle
		float offsetToStartAtTop = (float)(2 * Math.PI / 4);
		// set starting point
		float theta = offsetToStartAtTop;
		
		for(var i = 0; i < Magics; i++){
			var x = (float)Math.Cos(theta);
			var y = (float)Math.Sin(theta);
			// Inner ring
			Gizmos.DrawWireSphere(transform.position + (new Vector3(x,y,0)*.2f), .01f);
			// Outer ring
			Gizmos.DrawWireSphere(transform.position + (new Vector3(x,y,0)*.5f), .01f);
			theta += dtheta;
		}
		
	}
}
