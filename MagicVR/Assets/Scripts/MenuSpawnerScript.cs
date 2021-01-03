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
		var pos = ((betweenHands-headPos)*1.3f)+headPos;
		
		pos.y = pos.y < lowest ? lowest : pos.y;
		
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
