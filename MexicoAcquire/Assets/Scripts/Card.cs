using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card {
	int value;
	string suit;
	bool isFaceUp;
    GameObject card;


	public Card(int value, string suit, GameObject prefab){
		this.value = value;
		this.suit = suit;
        card = prefab;
	}

	public GameObject getPrefab(){
		return card;
	}

	public void switchVisibility(){
        isFaceUp = !isFaceUp;
	}

	public int getValue(){
		return value;
	}

	public string getSuit(){
		return suit;
	}

	public bool getIsFaceUp(){
		return isFaceUp;
	}

}
