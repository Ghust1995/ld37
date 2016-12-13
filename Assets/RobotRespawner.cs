using UnityEngine;
using System.Collections;

public class RobotRespawner : MonoBehaviour
{
    [SerializeField]
    private GameObject _robotPrefab;
    
    private GameObject _robotInstance;

    public AIController AiController;

    // Use this for initialization
    void Start ()
	{
	    Respawn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Respawn()
    {
        Destroy(_robotInstance);
        _robotInstance = (GameObject) Instantiate(_robotPrefab, transform.position, transform.rotation);
        AiController.SetCharacter(_robotInstance.GetComponent < CharacterController > ());
    }
}
