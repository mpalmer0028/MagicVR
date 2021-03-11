using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingPowerScript : ComboPowerScript
{
	public bool ChargeShots;
	public int MaxProjectiles = 10;
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
	
	public override void Start()
	{
		base.Start();
		WaitTill = Time.time;
	}
    
	public virtual void Update()
	{
		
		if(ProjectilePrimary != null){
			ProjectilePrimary.transform.localScale += Growth;
		}
		
		if(ProjectileOffHand != null){
			ProjectileOffHand.transform.localScale += Growth;
		}
		
	}
    
	public GameObject Shoot(){
		WaitTill = Time.time + ProjectileInterval;
		ProjectilePrimary = Instantiate(ProjectilePrefab, PrimaryPower.transform.position, PrimaryPower.transform.rotation);
		Projectiles.Add(ProjectilePrimary);
		
		var shotScript = ProjectilePrimary.GetComponent<ProjectileScript>();
		shotScript.Fired = !ChargeShots;
		return ProjectilePrimary;
	}
	
	public GameObject ShootOffHand(){
		WaitTill = Time.time + ProjectileInterval;
		ProjectileOffHand = Instantiate(ProjectilePrefab, OffPower.transform.position, PrimaryPower.transform.rotation);
		Projectiles.Add(ProjectileOffHand);
		
		var shotScript = ProjectileOffHand.GetComponent<ProjectileScript>();
		shotScript.Fired = !ChargeShots;
		return ProjectileOffHand;
	}
	
	public GameObject ReleasePrimary(){
		var shotScript = ProjectilePrimary.GetComponent<ProjectileScript>();
		var pp = ProjectilePrimary;
		
		ProjectilePrimary = null;
		shotScript.Fired = true;
		return pp;
	}
	
	
	public GameObject ReleaseOffHand(){
		var shotScript = ProjectileOffHand.GetComponent<ProjectileScript>();
		var po = ProjectileOffHand;
		
		ProjectileOffHand = null;
		shotScript.Fired = true;
		return po;
	}
	
}
