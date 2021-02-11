using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSpawnerScript : MonoBehaviour
{
	public GameObject LeftHand;
	public GameObject RightHand;
	public GameObject Head;


    // Update is called once per frame
    void Update()
	{
		var lowest = Mathf.Min(LeftHand.transform.position.y,RightHand.transform.position.y);
		
		var headPos = Head.transform.position;
		var betweenHands = Vector3.Lerp(LeftHand.transform.position,RightHand.transform.position, .5f);
		var depthFactor = Mathf.Max(2-Vector3.Distance(betweenHands,Head.transform.position),1);
		var infrontHands = ((betweenHands-Head.transform.position)*depthFactor*1.25f)+Head.transform.position;
		var pos = Vector3.Lerp(infrontHands,Head.transform.position, .25f);
		
		//pos.y = pos.y < lowest ? lowest : pos.y;
		//pos.y = Head.transform.position.y;
		
		transform.position = pos;
		transform.LookAt(new Vector3(headPos.x,betweenHands.y,headPos.z));
    }
    
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		var size = .1f;
		Gizmos.DrawWireCube(transform.position, new Vector3(size,size,size));
	}
}
