using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public Card[] hand;
    public int moneyInPool;
    public bool isTurn = false;
    public bool isFolded = false;
    public bool checking = false;
    public TexasHoldEm mainGameScript;
    public string playerType;
	public GameObject[] handObject;
	public int lastPlayerBet;

    public Player()
    {
        hand = new Card[2];
		handObject = new GameObject[2];
        moneyInPool = 100;
        playerType = "AI";
    }

	public Card[] getHand(){
		return hand;
	}

	public void resetHand(){
		hand = new Card[2];
		handObject = new GameObject[2];
	}

	public GameObject[] getObjects(){
		return handObject;
	}

    public void addToHand(Card card1, Card card2)
    {
        hand[0] = card1;
        hand[1] = card2;
    }

    public void check()
    {
        checking = true;
    }

    public void fold()
    {
        isFolded = true;
    }

    public void bet(int betAmount)
    {
		if (betAmount < mainGameScript.getLastBet ())
			return;
        moneyInPool -= betAmount;
        mainGameScript.addToPool(betAmount);
		lastPlayerBet = betAmount;
    }

	public void call()
	{
        moneyInPool -= mainGameScript.getLastBet();
        mainGameScript.addToPool(mainGameScript.getLastBet());
		lastPlayerBet = mainGameScript.getLastBet ();
    }

	public bool canCheck(){
		return lastPlayerBet >= mainGameScript.getLastBet ();
	}

	public void AIMoveFakes(){
		int nulls = 0;
		for (int i = 0; i < mainGameScript.table.Length; i++)
			if (mainGameScript.table [i] == null)
				nulls++;
		Card[] totalHand = new Card[7 - nulls];

		for (int i = 0; i < totalHand.Length; i++) {
			if (i < 2)
				totalHand [i] = hand [i];
			else
				totalHand [i] = mainGameScript.table [i - 2];
		}
		Rules r = new Rules ();
		int handValue = r.scoreHand (totalHand);
		int pot = mainGameScript.tablePool; 
		//Evaluate probability of current cards winning compared to amount in pot
		//Debug.Log(name);
		//Debug.Log (handValue);

		if (handValue >= 116)
			bet (moneyInPool);//All in for a straight flush
		else if (handValue >= 98)
			bet (moneyInPool); //All in for a four of a kind
		else if (handValue >= 84)
			bet ((int)(moneyInPool * 7 / 8)); // 7/8ths of mooney for a Full house
		else if (handValue >= 70)
			bet ((int)(moneyInPool * 3 / 4)); // 3/4ths of mooney for a flush
		else if (handValue >= 56)
			bet ((int)(moneyInPool * 1 / 2));// 1/2 of mooney for a straight
		else if (handValue >= 42)
			bet ((int)(moneyInPool * 3 / 8)); //3/8ths of mooney for a three of a kind
		else if (handValue >= 28)
			bet ((int)(moneyInPool * 1 / 8)); //1/8th of mooney for a two pair
		else if (handValue >= 14)
			bet ((int)(moneyInPool * 1 / 16)); //1/16th of mooney for a one pair
		else if (pot >= 150)
			isFolded = true;
		else
			call ();

	}

	public void AIMove(){
		Data d = mainGameScript.dataSim;
		float percent = d.getWinPercentage(d.cardToIndex(hand[0]),d.cardToIndex(hand[1]));
		int bet = (int)(moneyInPool * Mathf.Pow((percent / 100),2f));

		if (bet <= moneyInPool) {
			this.bet (bet);
		} else if (percent >= 50)
			this.bet (moneyInPool);
		else if (canCheck ())
			check ();
		else if (mainGameScript.getLastBet () < moneyInPool)
			call ();
		else
			fold ();
	}

}
