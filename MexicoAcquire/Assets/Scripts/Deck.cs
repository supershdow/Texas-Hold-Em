using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    private Card[] cards;
    private GameObject[] cardPrefabs;
	private string[] suits = { "Heart", "Diamond", "Spades", "Club" };
	public int numDecks = 1;

	// Use this for initialization
	void Start () {
		if (cards != null)
			return;
		cardPrefabs = Resources.LoadAll<GameObject> ("Free_Playing_Cards");
        resetTable();
	}

    public void resetTable()
    {
        createDeck();
        shuffle();
    }

    public void createDeck()
    {
        cards = new Card[52 * numDecks];
        int k = 0;
        for (int u = 0; u < numDecks; u++)
            for (int i = 0; i < 13; i++)
                for (int j = 0; j < 4; j++)
                {
                    if (i + 2 <= 10)            
                        cards[k++] = new Card(i + 2, suits[j], search("PlayingCards_" + (i + 2) + suits[j]));                    
                    else if (i + 2 == 11)
                        cards[k++] = new Card(i + 2, suits[j], search("PlayingCards_J" + suits[j]));
                    else if (i + 2 == 12)                    
                        cards[k++] = new Card(i + 2, suits[j], search("PlayingCards_Q" + suits[j]));                   
                    else if (i + 2 == 13)
                        cards[k++] = new Card(i + 2, suits[j], search("PlayingCards_K" + suits[j]));
                    else if (i + 2 == 14)
                        cards[k++] = new Card(i + 2, suits[j], search("PlayingCards_A" + suits[j]));
                }
    }

	public GameObject search(string cardName){
		if (cardPrefabs == null)
			cardPrefabs = Resources.LoadAll<GameObject> ("Free_Playing_Cards");
		for (int i = 0; i < cardPrefabs.Length; i++)
			if (cardName.Equals (cardPrefabs [i].name))
				return cardPrefabs [i];
		return null;
	}

	public void shuffle(){
		LinkedList<Card> temp = new LinkedList<Card>();
		LinkedList<int> availableIdices = new LinkedList<int> ();
		for (int i = 0; i < 52; i++)
			availableIdices.AddLast (i);
		for (int i = 0; i < cards.Length; i ++) {
			temp.AddLast (cards[i]);
			cards [i] = null;
		}
		int rand = Random.Range(0, temp.Count - 1);
		foreach (Card c in temp) {
			if (!availableIdices.Contains (rand)) {
				rand = Random.Range (0, temp.Count - 1);
			}
			availableIdices.Remove (rand);
			cards [rand] = c;
		}
			
	}
	public Card deal(){
		if (cards == null) {
			resetTable();
		}
		for (int i = cards.Length - 1; i >= 0; i--) {
			if (cards [i] != null) {
				Card card = cards [i];
				cards [i] = null;
				return card;
			}
		}
		resetTable();
		return deal();
	}
		
}
