﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandsInputScript : MonoBehaviour
{
	/// <summary>
	/// Collider object for hand
	/// </summary>
	public GameObject LeftHand;
	
	public GameObject MagicMenuPrefab;
	
	/// <summary>
	/// Collider object for hand
	/// </summary>
	public GameObject RightHand;
	
	public GameObject SpawnPoint;
	
	// Inputs
	public SteamVR_Action_Single TriggerInput;
	public SteamVR_Action_Boolean DismissInput;
	
	public float MagicMenuWarmupTime;
	
	public GameObject MagicMenu;
	
	public string PowerNameL;
	public string PowerNameR;
	
	public float MagicMenuPrelaunchStartTime = 1f;
	private float WaitTill;
    // Start is called before the first frame update
    void Start()
    {
	    WaitTill = Time.time;
    }

	void FixedUpdate()
	{
		var lTrigger = TriggerInput.GetAxis(SteamVR_Input_Sources.LeftHand);
		var rTrigger = TriggerInput.GetAxis(SteamVR_Input_Sources.RightHand);
		var lDismiss = DismissInput.GetState(SteamVR_Input_Sources.LeftHand);
		var rDismiss = DismissInput.GetState(SteamVR_Input_Sources.RightHand);
		
		if(lDismiss || rDismiss){
			// Cancel
			CloseMagicMenu();
		}
		
		if(WaitTill < Time.time){
			if(MagicMenu == null){
				if(lTrigger > 0 || rTrigger > 0){
					if(MagicMenuPrelaunchStartTime == 0){
						MagicMenuPrelaunchStartTime = Time.time;
					}else if(Time.time-MagicMenuPrelaunchStartTime > MagicMenuWarmupTime){
						LaunchMagicMenu();
					}
				
				}else{
					MagicMenuPrelaunchStartTime = 0;				
				}
			}else{
				if(lTrigger > 0 || rTrigger > 0){
				
					if(MagicMenuPrelaunchStartTime == 0){
						MagicMenuPrelaunchStartTime = Time.time;
					}else if(Time.time-MagicMenuPrelaunchStartTime > MagicMenuWarmupTime){
						// Select power
						MagicMenu.GetComponent<MagicMenu>().SelectPowers();
					}
				}else{
					MagicMenuPrelaunchStartTime = 0;				
				}
				
				
			}
		}
		
	}
    
	void LaunchMagicMenu(){
		WaitTill = Time.time + 1f;
		MagicMenuPrelaunchStartTime = 0;
		MagicMenu = Instantiate(MagicMenuPrefab, SpawnPoint.transform.position, SpawnPoint.transform.rotation*Quaternion.Euler(0,180,0), transform);
		var mms = MagicMenu.GetComponent<MagicMenu>();
		mms.HandsInputScript = this;
	}
	
	void CloseMagicMenu(){
		WaitTill = Time.time + .5f;
		MagicMenuPrelaunchStartTime = 0;
		if(MagicMenu != null){
			MagicMenu.GetComponent<MagicMenu>().Dismiss();
		}
	}
}
