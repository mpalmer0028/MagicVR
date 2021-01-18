using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionScript : MonoBehaviour
{
	public string IconFileName;
	public float FocusSlide = -.1f;
	public float FocusTime = 1f;
	
	public bool InTheZoneL = false;
	public bool InTheZoneR = false;
	
	public GameObject Icon;
	public GameObject PowerZoomTarget;
	public GameObject MagicGlowParticle;
	public ParticleSystem PS;
	public ParticleSystem.EmissionModule Emission;
	
	private float AnimationStartTime;
	private Quaternion StartRotation;
	private Quaternion EndRotation;
	private Vector3 StartPosition;
	private Vector3 EndPosition;
	private MagicMenu MagicMenu;
	private float SlideI;

	void OnCollisionEnter(Collision collision)
	{
		if(MagicMenu != null){
			if(collision.collider.name == MagicMenu.HandsInputScript.LeftHand.name){
				MagicMenu.HandsInputScript.PowerNameL = IconFileName;
				InTheZoneL = true;
			}else if(collision.collider.name == MagicMenu.HandsInputScript.RightHand.name){
				MagicMenu.HandsInputScript.PowerNameR = IconFileName;
				InTheZoneR = true;
			}
		}
		AnimationStartTime = Time.time;
		//Debug.Log("in");
	}

	void OnCollisionExit(Collision collisionInfo)
	{
		if(MagicMenu.HandsInputScript != null){
			if(collisionInfo.collider.name == MagicMenu.HandsInputScript.LeftHand.name){
				MagicMenu.HandsInputScript.PowerNameL = string.Empty;
				InTheZoneL = false;
			}else if(collisionInfo.collider.name == MagicMenu.HandsInputScript.RightHand.name){
				MagicMenu.HandsInputScript.PowerNameR = string.Empty;
				InTheZoneR = false;
			}
		}
		
		AnimationStartTime = Time.time;
	}
	
    // Start is called before the first frame update
    void Start()
	{
		PowerZoomTarget = transform.parent.Find("PowerZoomTarget").gameObject;
		
		SlideI = 0f;
		Icon = transform.Find("Icon").gameObject;
		
		MagicGlowParticle = Icon.transform.Find("MagicGlowParticle").gameObject;
		PS = MagicGlowParticle.GetComponent<ParticleSystem>();
		Emission = PS.emission;
		
		StartRotation = Icon.transform.localRotation;		
		EndRotation = Quaternion.FromToRotation(Vector3.forward, Icon.transform.localPosition-PowerZoomTarget.transform.localPosition);
		
		StartPosition = Icon.transform.localPosition;
		EndPosition = StartPosition + new Vector3(0,0,-.25f);
		
		MagicMenu = transform.parent.GetComponent<MagicMenu>();
    }

    // Update is called once per frame
    void Update()
	{
		var t = Icon.transform;
		var scaleAmount = .05f;
		if(InTheZoneL || InTheZoneR){
			if(Icon.transform.localScale.x < 2){
				t.localScale += new Vector3(scaleAmount,scaleAmount,scaleAmount);
			}

			if(SlideI<1f){
				SlideI+=.03f;
			}
		}else{
			if(Icon.transform.localScale.x > 1){
	    		
				t.localScale -= new Vector3(scaleAmount,scaleAmount,scaleAmount);
			}
			if(SlideI>0){
				SlideI-=.03f;
			}
		}
		t.localRotation = Quaternion.Lerp(StartRotation, EndRotation, SlideI);
		t.localPosition = Vector3.Lerp(StartPosition, EndPosition, SlideI);
		Emission.rate = SlideI*50;
	}
    
    
	void OnDrawGizmos()
	{
		Gizmos.color = UnityEngine.Color.green;
		if(transform.parent != null){
			PowerZoomTarget = transform.parent.Find("PowerZoomTarget").gameObject;
			Icon = transform.GetChild(0).gameObject;
			// From icon to point where menu focuses
			Gizmos.DrawRay(PowerZoomTarget.transform.position,transform.position);
			// Focus point
			Gizmos.color = new UnityEngine.Color(1,0,1);
			Gizmos.DrawWireSphere(PowerZoomTarget.transform.position, .05f);
		}

	}
    
    
    
    
}
