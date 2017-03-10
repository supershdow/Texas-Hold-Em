using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data : MonoBehaviour{
	
	
	// Update is called once per frame
	void Update () {
		
	}
	public int[,,] data;
	public int numGames = 10000;

	void Start(){
		data = new int[52, 52, 2];
		gatherData (numGames);
	}

	public void gatherData(int games){
		int[] scores = new int[4];
		Rules r = new Rules ();
		Card[,] Hands = new Card[4, 2];
		Card[] table = new Card[5];
		Deck deck = new Deck ();
		for (int i = 0; i < numGames; i++) {
			for (int j = 0; j < 4; j++)
				for (int k = 0; k < 2; k++)
					Hands [j, k] = deck.deal ();
			for (int j = 0; j < 5; j++)
				table [j] = deck.deal ();
			Card[] handScore = new Card[7];
			for (int j = 0; j < 4; j++) {
				for (int k = 0; k < 7; k++) {
					if (k < 2)
						handScore [k] = Hands [j, k];
					else
						handScore [k] = table [k - 2];
				}
				scores [j] = r.scoreHand (handScore);
			}
			int highestScore = -1;
			int highestScoreIndex = -1;
			for (int j = 0; j < scores.Length; j++) {
				if (highestScore < scores [j]) {
					highestScore = scores [j];
					highestScoreIndex = j;
				}
			}
			for (int j = 0; j < 4; j++) {
				Card[] currentHand = new Card[2];
				for (int k = 0; k < 2; k++)
					currentHand [k] = Hands [j, k];
				/*Debug.Log (currentHand [0].getSuit () + " | " + currentHand [1].getSuit ());
				Debug.Log (suitValue1 + " | " + suitValue2);*/
				if (j == highestScoreIndex)
					data [cardToIndex(currentHand[0]), cardToIndex(currentHand[1]), 0]++;
				else
					data [cardToIndex(currentHand[0]), cardToIndex(currentHand[1]), 1]++;
				//Debug.Log (currentHand [0].getValue () + " | " + currentHand [0].getSuit ());
				//Debug.Log ((currentHand [0].getValue () - 2) * 4 + suitValue1);

			}
		}
	}

	public int getTotalWins(int card1, int card2){
		return data [card1, card2, 0] + data [card2, card1, 0];
	}

	public int getTotalLosses(int card1, int card2){
		return data [card1, card2, 1] + data [card2, card1, 1];
	}

	public float getWinPercentage(int card1, int card2){
		return 100f*((float)getTotalWins (card1, card2) / (getTotalWins (card1, card2) + getTotalLosses (card1, card2)));
	}

	public int cardToIndex(Card c){
		/*Calculate each cards place in the array
					Each card takes four spaces for each suit value
					-2 since our values are [2,14]
					++ to increment Wins value*/
		int suitValue = 0;
		string[] suits = { "Diamond", "Club", "Heart", "Spades" };
		for (int k = 0; k < 4; k++)
			if (suits [k].Equals (c.getSuit ())) {
				suitValue = k;
			}
		return (c.getValue () - 2) * 4 + suitValue;
	}
}
