using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TankHealth : NetworkBehaviour
{
    public float m_StartingHealth = 200f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
	private Vector3 initialPosition;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;  
	[SyncVar]
    private float m_CurrentHealth;  
    private bool m_Dead;
	private ScoreManager scoreManager;
	private static int idCounter = 0;
	private int id = 0;

	public void Start(){
		initialPosition = transform.position;
		scoreManager = GameObject.FindObjectOfType<ScoreManager> ();
		idCounter++;
		id = idCounter;
	}

    private void Awake()
    {
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();

        m_ExplosionParticles.gameObject.SetActive(false);
    }


    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;
        SetHealthUI();
    }
    

    public void TakeDamage(float amount)
    { 
		m_CurrentHealth -= amount ;
		SetHealthUI ();

		if (m_CurrentHealth <= 0f && !m_Dead) {
			CmdOnDeath ();
		}
    }
		
	private void OnReset(){
		transform.position = initialPosition;
		OnEnable ();
	}

    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
		m_Slider.value = m_CurrentHealth;
		m_FillImage.color = Color.Lerp (m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

	[Command]
	private void CmdOnDeath(){
		RpcOnDeath ();
	}

	[ClientRpc]
    private void RpcOnDeath()
    {
        // Play the effects for the death of the tank and deactivate it.
		m_Dead = true;
		Debug.Log ("OnDeath");
		//GameObject.FindObjectOfType<CameraControl> ().removePlayer (gameObject);
		m_ExplosionParticles.transform.position = transform.position;
		m_ExplosionPrefab.SetActive (true);
		m_ExplosionAudio.Play ();
		m_ExplosionParticles.Play ();
		OnReset();
		if (id == 2) {
			scoreManager.player1Won ();
		} else {
			scoreManager.player2Won ();
		}
			
    }
}