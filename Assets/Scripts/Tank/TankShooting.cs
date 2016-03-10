using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TankShooting : NetworkBehaviour
{
    public int m_PlayerNumber = 1;       
	public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 1000f; 
    public float m_MaxLaunchForce = 1000f; 
    public float m_MaxChargeTime = 0.75f;

    
    private string m_FireButton;         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;                


    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_FireButton = "Fire" + m_PlayerNumber;

        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }
    

    private void Update()
    {
		if (!isLocalPlayer)
			return;
        // Track the current state of the fire button and make decisions based on the current launch force.
		m_AimSlider.value = m_MinLaunchForce;

		if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
			m_CurrentLaunchForce = m_MaxLaunchForce;
			Fire ();

 		} else if (Input.GetButtonDown (m_FireButton) && !m_Fired) {
			m_Fired = false;
			m_CurrentLaunchForce = m_MinLaunchForce;
			m_ShootingAudio.clip = m_ChargingClip;
			m_ShootingAudio.Play();
		} else if (Input.GetButton (m_FireButton)) {
			m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
			m_AimSlider.value = m_CurrentLaunchForce;
		} else if (Input.GetButtonUp(m_FireButton)) {
			Fire ();

		}
    }

	private void Fire(){
		// Set the fired flag so only Fire is only called once.
		m_Fired = true;
		CmdCreateBullets ();
		// Change the clip to the firing clip and play it.
		m_ShootingAudio.clip = m_FireClip;
		m_ShootingAudio.Play ();
		// Reset the launch force.  This is a precaution in case of missing button events.
		m_CurrentLaunchForce = m_MinLaunchForce;
	}

	[Command]
	private void CmdCreateBullets()
    {
		RpclocalBullet ();



    }

	[ClientRpc]
	private void RpclocalBullet(){
		GameObject shellInstance = (GameObject)
			Instantiate (m_Shell, m_FireTransform.position, m_FireTransform.rotation) ;
		// Set the shell's velocity to the launch force in the fire position's forward direction.
		shellInstance.GetComponent<Rigidbody>().velocity = 25f * m_FireTransform.forward; 

		//NetworkServer.Spawn (shellInstance);
	}

}