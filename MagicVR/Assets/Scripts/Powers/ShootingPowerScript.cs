using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPowerScript : ComboPowerScript
{
	public bool ChargeShots;
	public int MaxProjectiles = 10;
	public float MinimumTrigger = .25f;
	public float ProjectileInterval = 1f;
	
	public GameObject ProjectilePrefab;
	public GameObject ProjectilePrimary;
	public GameObject ProjectileOffHand;

	
	public List<GameObject> Projectiles;
	
	/// <summary>
	///  Scale growth per frame
	/// </summary>
	public Vector3 Growth;
	
	public float WaitTill;
	
	private Transform SpawnPrimary;
	private Transform SpawnOffHand;
	
	public override void Start()
	{
		base.Start();
		Projectiles = new List<GameObject>();
		WaitTill = Time.time;
		SpawnPrimary = transform.Find("PrimaryHand").GetChild(0);
		SpawnOffHand = transform.Find("OffHand").GetChild(0);
		if(SpawnPrimary != null){
			var pp =  SpawnPrimary.localPosition;
			var pr =  SpawnPrimary.localRotation;
			SpawnPrimary.parent = PrimaryPower.transform;
			SpawnPrimary.localPosition = pp;
			SpawnPrimary.localRotation = pr;
		}
		if(SpawnOffHand != null){
			var op =  SpawnOffHand.localPosition;
			var or =  SpawnOffHand.localRotation;
			SpawnOffHand.parent = OffPower.transform;
			SpawnOffHand.localPosition = op;
			SpawnOffHand.localRotation = or;
			
		}
	}
    
	public virtual void Update()
	{
		
		if(ProjectilePrimary != null){
			ProjectilePrimary.transform.localScale += Growth;
		}
		
		if(ProjectileOffHand != null){
			ProjectileOffHand.transform.localScale += Growth;
		}
		
		if(TriggerAmountPrimary <= MinimumTrigger && ProjectilePrimary != null){			
			ReleasePrimary();
		}
		if(TriggerAmountOff <= MinimumTrigger && ProjectileOffHand != null){
			ReleaseOffHand();
		}
	}
    
	public GameObject LoadShot(){
		WaitTill = Time.time + ProjectileInterval;
		var spawnT = SpawnPrimary == null ? PrimaryPower.transform : SpawnPrimary;
		
		ProjectilePrimary = Instantiate(ProjectilePrefab, spawnT.position, spawnT.rotation);
		Projectiles.Add(ProjectilePrimary);
		
		var shotScript = ProjectilePrimary.GetComponent<ProjectileScript>();
		shotScript.Fired = !ChargeShots;
		shotScript.ShootingPowerScript = this;
		return ProjectilePrimary;
	}
	
	public GameObject LoadShotOffHand(){
		WaitTill = Time.time + ProjectileInterval;
		var spawnT = SpawnOffHand == null ? OffPower.transform : SpawnOffHand;
		
		ProjectileOffHand = Instantiate(ProjectilePrefab, spawnT.position, spawnT.rotation);
		Projectiles.Add(ProjectileOffHand);
		
		var shotScript = ProjectileOffHand.GetComponent<ProjectileScript>();
		shotScript.Fired = !ChargeShots;
		shotScript.ShootingPowerScript = this;
		return ProjectileOffHand;
	}
	
	public virtual GameObject ReleasePrimary(){
		var shotScript = ProjectilePrimary.GetComponent<ProjectileScript>();
		var pp = ProjectilePrimary;
		
		ProjectilePrimary = null;
		shotScript.Fired = true;
		return pp;
	}
	
	
	public virtual GameObject ReleaseOffHand(){
		var shotScript = ProjectileOffHand.GetComponent<ProjectileScript>();
		var po = ProjectileOffHand;
		
		ProjectileOffHand = null;
		shotScript.Fired = true;
		return po;
	}
	
}
