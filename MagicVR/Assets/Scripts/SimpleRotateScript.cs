using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotateScript : MonoBehaviour
{

	public float X;
	public float Y;
	public float Z;

    // Update is called once per frame
	void FixedUpdate()
    {
	    transform.rotation *=  Quaternion.Euler(X,Y,Z);
    }
}
