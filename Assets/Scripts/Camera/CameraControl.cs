using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;

public class CameraControl : NetworkBehaviour
{
    public float m_DampTime = 0.2f;                 
    public float m_ScreenEdgeBuffer = 4f;           
    public float m_MinSize = 6.5f; 
	private List<Transform> m_Targets = new List<Transform>() ; 



    private Camera m_Camera;                        
    private float m_ZoomSpeed;                      
    private Vector3 m_MoveVelocity;                 
    private Vector3 m_DesiredPosition;              


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }


    private void FixedUpdate()
    {
		addPlayers();
		if (m_Targets.Count <= 1)
			return;
        Move();
        Zoom();
    }

	private void addPlayers(){
		
		GameObject[] objects = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject o in objects) {
			if (o.GetComponent<TankMovement> () && !found(o)) {
				m_Targets.Add(o.GetComponent<Transform> ());
				Debug.Log ("tank  : " + o.name);
			}
		}
	}

	private bool found (GameObject o){
		foreach (Transform t in m_Targets) {
			if (t == null) {
				return false;
			}
			if (t.gameObject.GetInstanceID() == o.GetInstanceID()) {
				return true;
			}
		}
		return false;
	}

	public void removePlayer(GameObject player){
		if (found (player)) {
			m_Targets.Remove (player.GetComponent<Transform> ());
		}
	}



    private void Move()
    {
		
	    FindAveragePosition ();

        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
		if (m_Targets.Count == 1) {
			m_DesiredPosition = m_Targets [0].position;
			return;
		}
        Vector3 averagePos = new Vector3();
        int numTargets = 0;

		for (int i = 0; i < m_Targets.Count; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            averagePos += m_Targets[i].position;
            numTargets++;
        }

        if (numTargets > 0)
            averagePos /= numTargets;

        averagePos.y = transform.position.y;

        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        float size = 0f;

        for (int i = 0; i < m_Targets.Count; i++)
        {
            if (!m_Targets[i].gameObject.activeSelf)
                continue;

            Vector3 targetLocalPos = transform.InverseTransformPoint(m_Targets[i].position);

            Vector3 desiredPosToTarget = targetLocalPos - desiredLocalPos;

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.y));

            size = Mathf.Max (size, Mathf.Abs (desiredPosToTarget.x) / m_Camera.aspect);
        }
        
        size += m_ScreenEdgeBuffer;

        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        FindAveragePosition();

        transform.position = m_DesiredPosition;

        m_Camera.orthographicSize = FindRequiredSize();
    }
}