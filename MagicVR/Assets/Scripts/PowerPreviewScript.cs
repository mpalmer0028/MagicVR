using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPreviewScript : MonoBehaviour
{
	/// <summary>
	/// How strong should a preview's effects be. Bettween 0-1
	/// </summary>
	public float Factor {get
		{
			return this._factor;
		}

		set
		{  
			this._factor = value; 
			UpdateFactor();
		}}
		
	private float _factor;
	
	// Fire
	private ParticleSystem FireRedsPS;
	private float FireRedsEmission;
	private ParticleSystem FirePS;
	private float FireEmission;
	
	// Eletric
	private ParticleSystem ElectricCoolPS;
	private float ElectricCoolEmission;
	private ParticleSystem ElectricWarmPS;
	private float ElectricWarmEmission;
	
	private ParticleSystem.Burst ElectricWarmBurst0;
	private float ElectricWarmBurst0Probability;
	private ParticleSystem.Burst ElectricWarmBurst1;
	private float ElectricWarmBurst1Probability;
	
	private ParticleSystem.Burst ElectricCoolBurst0;
	private float ElectricCoolBurst0Probability;
	private ParticleSystem.Burst ElectricCoolBurst1;
	private float ElectricCoolBurst1Probability;
	
	// Ice
	private ParticleSystem FlakesPS;
	private float FlakesEmission;
	private ParticleSystem DotsPS;
	private float DotsEmission;
	
	// Water
	private ParticleSystem GushPS;
	private float GushEmission;
	private ParticleSystem CorePS;
	private float CoreEmission;
	private ParticleSystem TrailsPS;
	private float TrailsEmission;
	
	// Sword
	private ParticleSystem KnifePS;
	private float KnifeEmission;
	private ParticleSystem SwordsPS;
	private float SwordsEmission;
	
	// Hammer
	private ParticleSystem HammersPS;
	private float HammersEmission;
	private ParticleSystem FistPS;
	private float FistEmission;
	private ParticleSystem ClubsPS;
	private float ClubsEmission;
	
	//// 
	//private ParticleSystem PS;
	//private float Emission;

	
    // Start is called before the first frame update
    void Start()
	{
		// Power specific - Get full on info
		switch(gameObject.name){			
			case "Fire":
				FireRedsPS = transform.Find("Reds").GetComponent<ParticleSystem>();
				FireRedsEmission = FireRedsPS.emissionRate;
				FirePS = transform.Find("Fire").GetComponent<ParticleSystem>();
				FireEmission = FirePS.emissionRate;
				break;
			case "Electric":
				ElectricCoolPS = transform.Find("ElectricCool").GetComponent<ParticleSystem>();
				ElectricCoolEmission = ElectricCoolPS.emissionRate;
				ElectricWarmPS = transform.Find("ElectricWarm").GetComponent<ParticleSystem>();
				ElectricWarmEmission = ElectricWarmPS.emissionRate;
				
				ElectricWarmBurst0 = ElectricWarmPS.emission.GetBurst(0);
				ElectricWarmBurst1 = ElectricWarmPS.emission.GetBurst(1);
				ElectricWarmBurst0Probability = ElectricWarmBurst0.probability;
				ElectricWarmBurst1Probability = ElectricWarmBurst1.probability;
				
				ElectricCoolBurst0 = ElectricCoolPS.emission.GetBurst(0);
				ElectricCoolBurst1 = ElectricCoolPS.emission.GetBurst(1);
				ElectricCoolBurst0Probability = ElectricCoolBurst0.probability;
				ElectricCoolBurst1Probability = ElectricCoolBurst1.probability;
				break;
			case "Ice":
				FlakesPS = transform.Find("Flakes").GetComponent<ParticleSystem>();
				FlakesEmission = FlakesPS.emissionRate;
				DotsPS = transform.Find("Dots").GetComponent<ParticleSystem>();
				DotsEmission = DotsPS.emissionRate;
				break;
			case "Water":
				GushPS = transform.Find("Gush").GetComponent<ParticleSystem>();
				GushEmission = GushPS.emissionRate;
				CorePS = transform.Find("Core").GetComponent<ParticleSystem>();
				CoreEmission = CorePS.emissionRate;
				TrailsPS = transform.Find("Trails").GetComponent<ParticleSystem>();
				TrailsEmission = TrailsPS.emissionRate;
				break;
			case "Sword":
				SwordsPS = transform.Find("Swords").GetComponent<ParticleSystem>();
				SwordsEmission = SwordsPS.emissionRate;
				KnifePS = transform.Find("Knife").GetComponent<ParticleSystem>();
				KnifeEmission = KnifePS.emissionRate;
				break;
			case "Hammer":
				HammersPS = transform.Find("Hammers").GetComponent<ParticleSystem>();
				HammersEmission = HammersPS.emissionRate;
				FistPS = transform.Find("Fist").GetComponent<ParticleSystem>();
				FistEmission = FistPS.emissionRate;
				ClubsPS = transform.Find("Clubs").GetComponent<ParticleSystem>();
				ClubsEmission = ClubsPS.emissionRate;
				break;
			//case "":
			//	PS = transform.Find("").GetComponent<ParticleSystem>();
			//	Emission = PS.emissionRate;
			//	break;
		}
		
	    Factor = 0;
	    
	    
    }

	private void UpdateFactor(){
		// Power specific
		switch(gameObject.name){
			case "Fire":
				FirePS.emissionRate = Mathf.Lerp(0, FireEmission, Factor);
				FireRedsPS.emissionRate = Mathf.Lerp(0, FireRedsEmission, Factor);
				break;
		    
			case "Electric":
				ElectricCoolPS.emissionRate = Mathf.Lerp(0, ElectricCoolEmission, Factor);
				ElectricWarmPS.emissionRate = Mathf.Lerp(0, ElectricWarmEmission, Factor);
				
				ElectricWarmBurst0.probability = Mathf.Lerp(0, ElectricWarmBurst0Probability, Factor);
				ElectricWarmBurst1.probability = Mathf.Lerp(0, ElectricWarmBurst1Probability, Factor);
				ElectricCoolBurst0.probability = Mathf.Lerp(0, ElectricCoolBurst0Probability, Factor);
				ElectricCoolBurst1.probability = Mathf.Lerp(0, ElectricCoolBurst1Probability, Factor);
				
				ElectricWarmPS.emission.SetBursts(new ParticleSystem.Burst[]{
					ElectricWarmBurst0,
					ElectricWarmBurst1
				});
				ElectricCoolPS.emission.SetBursts(new ParticleSystem.Burst[]{
					ElectricCoolBurst0,
					ElectricCoolBurst1
				});
				break;
				
			case "Ice":
				FlakesPS.emissionRate = Mathf.Lerp(0, FlakesEmission, Factor);
				DotsPS.emissionRate = Mathf.Lerp(0, DotsEmission, Factor);
				break;				
				
			case "Water":
				GushPS.emissionRate = Mathf.Lerp(0, GushEmission, Factor);
				CorePS.emissionRate = Mathf.Lerp(0, CoreEmission, Factor);
				TrailsPS.emissionRate = Mathf.Lerp(0, TrailsEmission, Factor);
				break;
				
			case "Sword":
				SwordsPS.emissionRate = Mathf.Lerp(0, SwordsEmission, Factor);
				KnifePS.emissionRate = Mathf.Lerp(0, KnifeEmission, Factor);
				break;
				
			case "Hammer":
				HammersPS.emissionRate = Mathf.Lerp(0, HammersEmission, Factor);
				FistPS.emissionRate = Mathf.Lerp(0, FistEmission, Factor);
				ClubsPS.emissionRate = Mathf.Lerp(0, ClubsEmission, Factor);
				break;
				
			//case "":
			//	PS.emissionRate = Mathf.Lerp(0, Emission, Factor);
			//	break;
				
		}
	}
}
