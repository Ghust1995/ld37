using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using SimpleFirebaseUnity;

public class WinCondition : MonoBehaviour
{
    public GameObject[] StuffToHide;
    public GameObject[] StuffToShow;

    [SerializeField]
    private bool _gameOver = false;

    public TileMap TileMap;
    public AIController AiController;

    // Update is called once per frame
    void Update ()
	{
	    var character = FindObjectOfType<CharacterController>();
	    if (character.transform.position.y > transform.position.y && ! _gameOver)
	    {
	        _gameOver = true;
            foreach (var o in StuffToHide)
	        {
                if(o == null) continue;
                o.SetActive(false);
	        }
            foreach (var o in StuffToShow)
            {
                if (o == null) continue;
                o.SetActive(true);
            }
	        GetScoreStuff(AiController.LinesOfCode, TileMap.BlockCount);
	    }
	}

    private void GetScoreStuff(int linesOfCode, int blockCount)
    {
        var firebase = Firebase.CreateNew("ludumdare37-2c993.firebaseio.com");
        firebase.Child("Scores").Push("{ \"linesOfCode\": " + linesOfCode + ", \"blockCount\":" + blockCount + " }", true);
    }
}
