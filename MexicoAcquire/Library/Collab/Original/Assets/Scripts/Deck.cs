using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck {
	LinkedList<Card> dieck;

	public Deck(){
		string[] suits = { "Hearts", "Diamonds", "Spades", "Clubs" };
		dieck = new LinkedList<Card> ();
		for (int i = 0; i < 13; i++)
			for (int j = 0; j < suits.Length; j++)
				dieck.AddLast(new Card (i, suits [j]));
		shuffle ();
	}

	public void shuffle(){
		LinkedList<Card> temp = new LinkedList<Card> ();
		for (int i = 0; i < 52; i++)
			temp.AddLast (dieck.Remove (dieck.getIndex(int)Random.Range(0, dieck.Count)));
	}



}
