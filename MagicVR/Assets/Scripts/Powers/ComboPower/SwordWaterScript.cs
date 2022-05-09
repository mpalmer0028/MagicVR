using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Valve.VR;
using Valve.VR.InteractionSystem;
using UnityEditor;

public class SwordWaterScript : ComboPowerScript
{
    private const double MaximumForce = 3.402823e+38;

    // How much the fish can grow while shooting
    public AnimationCurve Growth;
	public float GrowthMagnitude = 1;
	
	// Amount of wiggling
	public AnimationCurve Wiggle;
	public float WiggleMagnitude = 1;
	
	// How strong the tip is pulled toward where the player is looking
	public float PointForce = 10;

	// SwordFishScript props
	// Water particles
	public ParticleSystem WaterPS;
	public AudioSource WaterAudioSource;

	//public Rigidbody[] JointRigidbodys;
	public Rigidbody TipRigidbody;

	private bool BodyLocked = false;
	private ConfigurableJoint[] ConfigurableJoints;
	private Vector2[] StartRigidness;
	private Vector2[] StartDamping;
	private float StartTime;
	private float WiggleOffset;
    protected ParticleSystem.EmissionModule Emission;
    private Rigidbody[] FreezableRigidbodys;
	
	public override void Start(){
		base.Start();
		//StartTime = Time.time;
		//var rig = transform.Find("PrimaryHand").Find("SwordfishSimple8");
		//ConfigurableJoints = new ConfigurableJoint[] { 
		//	rig.Find("Spine.000").GetComponent<ConfigurableJoint>(), 
		//	rig.Find("Spine.001").Find("Spine.002").GetComponent<ConfigurableJoint>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").GetComponent<ConfigurableJoint>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004").GetComponent<ConfigurableJoint>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").GetComponent<ConfigurableJoint>(), 
		//	 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").GetComponent<ConfigurableJoint>(), 
		//	 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").GetComponent<ConfigurableJoint>(), 
		//};
		//FreezableRigidbodys = new Rigidbody[] { 
		//	rig.Find("Spine.000").GetComponent<Rigidbody>(), 
		//	rig.Find("Spine.001").Find("Spine.002").GetComponent<Rigidbody>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").GetComponent<Rigidbody>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004").GetComponent<Rigidbody>(), 
		//	rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").GetComponent<Rigidbody>(), 
		//	 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").GetComponent<Rigidbody>(), 
		//	 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").GetComponent<Rigidbody>(), 
		//};

		
		//Debug.Log(rig
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007")
		//	.Find("SpitSpawn")
		//	);
		//WaterAudioSource = rig
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn").GetComponent<AudioSource>();
		//WaterPS = rig
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn")
		//	.Find("WaterParticle").Find("Gush").GetComponent<ParticleSystem>();
		//StartRigidness = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.spring,x.angularYZLimitSpring.spring)).ToArray();
		//StartDamping = ConfigurableJoints.AsQueryable().Select(x=> new Vector2(x.angularXLimitSpring.damper,x.angularYZLimitSpring.damper)).ToArray();
		
		//WiggleOffset = Wiggle.length/FreezableRigidbodys.Count();

		
		
	}
	
	void Update(){
		Emission = WaterPS.emission;
		if (!WaterAudioSource.isPlaying){
			BodyLocked = false;
			//Emission.enabled = false;
			//WaterPS.enableEmission = false;
		}
		if(BodyLocked){
			// Grow/shrink
			var growth = (Growth.Evaluate(Time.time-StartTime)*.1f*GrowthMagnitude)+1;
			var scale =  new Vector3(growth,growth,growth);			
			for(var i = 0; i < FreezableRigidbodys.Count();i++){
				FreezableRigidbodys[i].transform.localScale = scale;

			}
			// Point
			TipRigidbody.AddForce((VisionTarget.transform.position-PrimaryPower.transform.position)*PointForce);
		}else{
			// Wiggle
			for(var i = 0; i < FreezableRigidbodys.Count();i++){
				var t =  (Time.time-StartTime)*2;
				
				var wiggle = (Wiggle.Evaluate(t+(WiggleOffset*i))-.5f)*WiggleMagnitude;
				var torque = new Vector3(0,0,wiggle);
				FreezableRigidbodys[i].AddRelativeTorque(torque);
			}
			
			// Correct scale
			var amountOff = FreezableRigidbodys[0].transform.localScale.x-1;
			var step = .001f;
			if(!(amountOff<step && amountOff>-step)){
				if(amountOff>0){
					step *= -1;
				}
				for(var i = 0; i < FreezableRigidbodys.Count();i++){
					FreezableRigidbodys[i].transform.localScale += new Vector3(step, step, step);
				}
			}
		}
	}
	
	public override void UpdatePowerTrigger(float triggerAmount, bool offHand){
		base.UpdatePowerTrigger(triggerAmount, offHand);
		
		//Debug.Log(offHand);
		if (!offHand){
			//Debug.Log(triggerAmount);
			//Debug.Log(BodyLocked);
			if(triggerAmount > .25f && !BodyLocked){
				BodyLocked = true;
				StartTime = Time.time;

				// Start spray
				Emission.enabled = false;
				//WaterPS.enableEmission = true;
				WaterAudioSource.Play();
				
				//for(var i = 0; i < FreezableRigidbodys.Count();i++){
				//	//FreezableRigidbodys[i].constraints = RigidbodyConstraints.FreezeAll;
				//}
			}
			if(triggerAmount <= .25f && BodyLocked){
				BodyLocked = false;

				// Start spray
				Emission.enabled = false;
				//WaterPS.enableEmission = false;
				WaterAudioSource.Stop();
				
				//for(var i = 0; i < FreezableRigidbodys.Count();i++){
				//	//FreezableRigidbodys[i].constraints = RigidbodyConstraints.None;
				//}
			}
		}
	}
	public override void GetPropertiesFromInstantiateHandObjects(GameObject primaryPower, GameObject offPower)
	{
		base.GetPropertiesFromInstantiateHandObjects(primaryPower, offPower);
		StartTime = Time.time;
		var rig = primaryPower.transform.Find("SwordfishSimple8");
		ConfigurableJoints = new ConfigurableJoint[] {
			rig.Find("Spine.000").GetComponent<ConfigurableJoint>(),
			rig.Find("Spine.001").Find("Spine.002").GetComponent<ConfigurableJoint>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").GetComponent<ConfigurableJoint>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004").GetComponent<ConfigurableJoint>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").GetComponent<ConfigurableJoint>(),
			 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").GetComponent<ConfigurableJoint>(),
			 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").Find("Spine.007").GetComponent<ConfigurableJoint>(),
		};
		FreezableRigidbodys = new Rigidbody[] {
			rig.Find("Spine.000").GetComponent<Rigidbody>(),
			rig.Find("Spine.001").Find("Spine.002").GetComponent<Rigidbody>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").GetComponent<Rigidbody>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004").GetComponent<Rigidbody>(),
			rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").GetComponent<Rigidbody>(),
			 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").GetComponent<Rigidbody>(),
			 rig.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").Find("Spine.007").GetComponent<Rigidbody>(),
		};


		Debug.Log(rig
			.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").Find("Spine.007")
			.Find("SpitSpawn")
			);
		WaterAudioSource = rig
			.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn").GetComponent<AudioSource>();
		WaterPS = rig
			.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
			.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn")
			.Find("WaterParticle").Find("Gush").GetComponent<ParticleSystem>();
		StartRigidness = ConfigurableJoints.AsQueryable().Select(x => new Vector2(x.angularXLimitSpring.spring, x.angularYZLimitSpring.spring)).ToArray();
		StartDamping = ConfigurableJoints.AsQueryable().Select(x => new Vector2(x.angularXLimitSpring.damper, x.angularYZLimitSpring.damper)).ToArray();

		WiggleOffset = Wiggle.length / FreezableRigidbodys.Count();


		TipRigidbody = FreezableRigidbodys[6];

	}
	public void RegisterSwordFishComponents()
	{
		var swordWaterPrefab = (GameObject)AssetDatabase.LoadMainAssetAtPath("Assets/Resources/HandObjects/SwordWater.prefab");  // Loads the imported prefab asset into memory (not the source, even though you specified the path to the source asset)

		//{ "Spine.000",
		//  "Spine.001",
		//  "Spine.002",
		//  "Spine.003",
		//  "Spine.004",
		//  "Spine.006",
		//  "Spine.005",
		//  "Spine.007",
		//}
		// Get object that need components
		var spines = new List<Transform> { swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8").Find("Spine.000") };
		spines.Add(swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8").Find("Spine.001"));
		spines.Add(spines[1].Find("Spine.002"));
		spines.Add(spines[2].Find("Spine.003"));
		spines.Add(spines[3].Find("Spine.004"));
		spines.Add(spines[4].Find("Spine.006"));
		spines.Add(spines[5].Find("Spine.005"));
		spines.Add(spines[6].Find("Spine.007"));

		// Configurable joint data
		var connectedBodiesIndexes = new List<int?>() { 1, null, 1, 2, 3, 4, 5, 6 };
		var noConfigurableJointIndexes = new List<int> { 1 };

		// float?[] = {limit, bounciness, contact distance}
		var lowAngularXLimitValues = new List<float[]> { new float[] {-17.35707f, 0, 0},
			null,
			new float[] {2.964449f, 0, 0},
			new float[] {2.964449f, 0, 0},
			new float[] {2.964449f, 0, 0},
			new float[] {2.964449f, 0, 0},
			new float[] {2.964449f, 0, 0},
			new float[] {2.964449f, 0, 0}
		};

		var highAngularXLimitValues = new List<float[]> { new float[] { 21.05471f, 0, 0},
			null,
			new float[] { 7.917106f, 0, 0},
			new float[] { 7.917106f, 0, 0},
			new float[] { 7.917106f, 0, 0},
			new float[] { 7.917106f, 0, 0},
			new float[] { 7.917106f, 0, 0},
			new float[] { 7.917106f, 0, 0}
		};

		// float?[] = {spring, damper}
		var angularYZLimitSpringValues = new List<float[]> { new float[] {0, 0},
			null,
			new float[] { 60f, 0},
			new float[] { 60f, 0},
			new float[] { 60f, 0},
			new float[] { 60f, 0},
			new float[] { 60f, 0},
			new float[] { 60f, 0}

		};

		// float?[] = {limit, bounciness, contact distance}
		var angularYLimitValues = new List<float[]> { new float[] { 36.12329f, 0, 0},
			null,
			new float[] {9.984356f, 0, 0},
			new float[] {9.984356f, 0, 0},
			new float[] {9.984356f, 0, 0},
			new float[] {9.984356f, 0, 0},
			new float[] {9.984356f, 0, 0},
			new float[] {9.984356f, 0, 0}

		};

		var angularZLimitValues = new List<float[]> { new float[] { 69.41476f, 0, 0},
			null,
			new float[] { 27.9753f, 0, 0},
			new float[] { 27.9753f, 0, 0},
			new float[] { 27.9753f, 0, 0},
			new float[] { 27.9753f, 0, 0},
			new float[] { 27.9753f, 0, 0},
			new float[] { 27.9753f, 0, 0}
		};

		// vector3?
		var targetPosValues = new List<Vector3?> { new Vector3(0,0,0),
			null,
			new Vector3(0,0,1),
			new Vector3(0,0,1),
			new Vector3(0,0,1),
			new Vector3(0,0,1),
			new Vector3(0,0,1),
			new Vector3(0,0,1)
		};
		
		var targetVelocityValues = new List<Vector3?> { new Vector3(0,0,0),
			null,
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0)
		};

		// X Y Z drives
		var positionSpring = 10d; 
		var XYZDriveValues = new double[,,]
		{
            { 
				{ 0,0, MaximumForce },
				{ 0,0,0},
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce }
			},
			{
				{ 0,0, MaximumForce },
				{ 0,0,0},
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce }
			},
			{
				{ 0,0, MaximumForce },
				{ 0,0,0},
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce },
				{ positionSpring,0, MaximumForce }
			}
		};

		// Box collider data for each spine
		var colliderCenters = new List<Vector3> { new Vector3(0, .3f, 0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,0,0),
			new Vector3(0,.5f,0)
		};
		var colliderSizes = new List<Vector3> { new Vector3(.1f, .1f, .71f),
			new Vector3(.2f,.2f,.2f),
			new Vector3(.2f,.2f,.25f),
			new Vector3(.2f,.2f,.3f),
			new Vector3(.2f,.2f,.35f),
			new Vector3(.2f,.2f,.4f),
			new Vector3(.2f,.2f,.35f),
			new Vector3(.1f,1.25f,.1f)
		};

		// Rigidbody data for each spine
		var rigidbodyConstraints = new List<RigidbodyConstraints> { RigidbodyConstraints.None,
			RigidbodyConstraints.FreezeAll,
			RigidbodyConstraints.None,
			RigidbodyConstraints.None,
			RigidbodyConstraints.None,
			RigidbodyConstraints.None,
			RigidbodyConstraints.None,
			RigidbodyConstraints.None
		};
		var rbs = new Rigidbody[rigidbodyConstraints.Count];

		
		
		// Add components
		for (var i = 0; i < spines.Count; i++)
		{
			// Add box colliders
			var bc = spines[i].gameObject.GetComponent<BoxCollider>();
			//Debug.Log(spines[i].gameObject.name);
			//Debug.Log(bc);
			if (bc == null)
			{
				bc = spines[i].gameObject.AddComponent<BoxCollider>();
			}
			//Debug.Log(bc);
			bc.center = colliderCenters[i];
			bc.size = colliderSizes[i];
			//Debug.Log(bc);

			// Add Rigidbody
			rbs[i] = spines[i].gameObject.GetComponent<Rigidbody>();
			if (rbs[i] == null)
			{
				rbs[i] = spines[i].gameObject.AddComponent<Rigidbody>();
			}

			rbs[i].constraints = rigidbodyConstraints[i];

			// Add Configurable joints
			if (!noConfigurableJointIndexes.Contains(i))
            {
				var cj = spines[i].gameObject.GetComponent<ConfigurableJoint>();
				if (cj == null)
				{
					cj = spines[i].gameObject.AddComponent<ConfigurableJoint>();
				}
				cj.connectedBody = rbs[connectedBodiesIndexes[i].Value];
				cj.autoConfigureConnectedAnchor = true;
				cj.xMotion = ConfigurableJointMotion.Locked;
				cj.yMotion = ConfigurableJointMotion.Locked;
				cj.zMotion = ConfigurableJointMotion.Locked;
				cj.angularXMotion = ConfigurableJointMotion.Limited;
				cj.angularYMotion = ConfigurableJointMotion.Limited;
				cj.angularZMotion = ConfigurableJointMotion.Limited;

				var lowAngularXLimit = cj.lowAngularXLimit;
				lowAngularXLimit.limit = lowAngularXLimitValues[i][0];
				lowAngularXLimit.bounciness = lowAngularXLimitValues[i][1];
				lowAngularXLimit.contactDistance = lowAngularXLimitValues[i][2];

				var highAngularXLimit = cj.highAngularXLimit;
				highAngularXLimit.limit = highAngularXLimitValues[i][0];
				highAngularXLimit.bounciness = highAngularXLimitValues[i][1];
				highAngularXLimit.contactDistance = highAngularXLimitValues[i][2];

				var angularYZLimitSpring = cj.angularYZLimitSpring;
				angularYZLimitSpring.spring = angularYZLimitSpringValues[i][0];
				angularYZLimitSpring.damper = angularYZLimitSpringValues[i][1];

				var angularYLimit = cj.angularYLimit;
				angularYLimit.limit = angularYLimitValues[i][0];
				angularYLimit.bounciness = angularYLimitValues[i][1];
				angularYLimit.contactDistance = angularYLimitValues[i][2];

				var angularZLimit = cj.angularZLimit;
				angularZLimit.limit = angularZLimitValues[i][0];
				angularZLimit.bounciness = angularZLimitValues[i][1];
				angularZLimit.contactDistance = angularZLimitValues[i][2];

				cj.targetPosition = targetPosValues[i].Value;
				cj.targetVelocity = targetVelocityValues[i].Value;

				var xDrive = cj.xDrive;
				var yDrive = cj.yDrive;
				var zDrive = cj.zDrive;

				xDrive.positionSpring = (float)XYZDriveValues[0, i, 0];
				xDrive.positionDamper = (float)XYZDriveValues[0, i, 1];
				xDrive.maximumForce = (float)XYZDriveValues[0, i, 2];

				yDrive.positionSpring = (float)XYZDriveValues[1, i, 0];
				yDrive.positionDamper = (float)XYZDriveValues[1, i, 1];
				yDrive.maximumForce = (float)XYZDriveValues[1, i, 2];

				zDrive.positionSpring = (float)XYZDriveValues[2, i, 0];
				zDrive.positionDamper = (float)XYZDriveValues[2, i, 1];
				zDrive.maximumForce = (float)XYZDriveValues[2, i, 2];

			}
		}

		var cjSpine000 = spines[0].gameObject.GetComponent<ConfigurableJoint>();
		if (cjSpine000 == null)
		{
			cjSpine000 = spines[0].gameObject.AddComponent<ConfigurableJoint>();
		}
		cjSpine000.connectedBody = rbs[1];


		// Add SpitSpawn
		//var spitSpawn = Instantiate(
		//	swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8").gameObject,
		//	Vector3.zero, Quaternion.identity, spines[7]
		//	);
		//spitSpawn.transform.parent = spines[7];
		var sws = swordWaterPrefab.GetComponent<SwordWaterScript>();

		//Debug.Log(swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8")
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007")
		//	.Find("SpitSpawn")
		//	);
		//sws.WaterAudioSource = swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8")
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn").GetComponent<AudioSource>();
		//sws.WaterPS = swordWaterPrefab.transform.Find("PrimaryHand").Find("SwordfishSimple8")
		//	.Find("Spine.001").Find("Spine.002").Find("Spine.003").Find("Spine.004")
		//	.Find("Spine.006").Find("Spine.005").Find("Spine.007").Find("SpitSpawn")
		//	.Find("WaterParticle").Find("Gush").GetComponent<ParticleSystem>();

		PrefabUtility.SavePrefabAsset(swordWaterPrefab);
	}
}

[CustomEditor(typeof(SwordWaterScript))]
public class SwordWaterScriptEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		var sfs = (SwordWaterScript)target;
        if (GUILayout.Button("Register SwordFish Components"))
        {
            sfs.RegisterSwordFishComponents();
            //Debug.Log("start");
        }
    }
}