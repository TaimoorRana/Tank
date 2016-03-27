using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour {

	int player1Score;

	int player2Score;

	int roundNumber;
	Text player1Text;
	Text player2Text;
	Text roundText;

	// Use this for initialization
	void Start () {
		player1Score = 0;
		player2Score = 0;
		roundNumber = 0;
		player1Text = GameObject.Find ("Player 1").GetComponent<Text> ();
		player2Text = GameObject.Find ("Player 2").GetComponent<Text> ();
		roundText = GameObject.Find ("Round").GetComponent<Text> ();
		player1Text.text = "Player 1                   " + player1Score;
		player2Text.text = "Player 2                   " + player2Score;
		roundText.text = "Round        " + roundNumber; 
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void increaseRound(){
		roundNumber++;
		roundText.text = "Round        " + roundNumber; 
	}

	public void player1Won(){
		player1Score++;
		player1Text.text = "Player 1                   " + player1Score;
		increaseRound ();
	}

	public void player2Won(){
		player2Score++;
		player2Text.text = "Player 2                   " + player2Score;
		increaseRound ();
	}
}
