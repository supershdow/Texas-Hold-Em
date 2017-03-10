using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules {

    Player playerHolder;

    public void giveTurnTo(Player p)
    {
        playerHolder = p;
    }

    public int scoreHand(Card[] cardsToScore)
    {

		int value = 0;
		cardsToScore = sortHand (cardsToScore);


		Card pair = null, three = null, straight = null, flush = null;

		//Check for pair/pairs
		for (int i = cardsToScore.Length-1; i >=0; i--)
			for (int j = i-1; j >= 0; j--) {
				if (i == j)
					continue;
				else if (cardsToScore [i].getValue () == cardsToScore [j].getValue ()) {
					if (value != 0) {
						if (value > cardsToScore [i].getValue ()+14)
							value += 14;
						else
							value = cardsToScore [i].getValue () + 28;
						break;
					}


					}
				if (cardsToScore [i].getValue () == cardsToScore [j].getValue ()) {
					pair = cardsToScore [i];
					value = 14 + cardsToScore [i].getValue ();
					i--;
					j = i - 1;
				}
			}

		//Check for three of a kind
		for (int i = 0; i <= cardsToScore.Length - 3; i++)
			if (cardsToScore [i].getValue () == cardsToScore [i + 1].getValue () && cardsToScore [i + 1].getValue () == cardsToScore [i + 2].getValue ()) {
				value = 42 + cardsToScore [i].getValue ();
				three = cardsToScore [i];
			}

		//Check for Straight
		for (int i = 0; i <= cardsToScore.Length - 5; i++)
			for (int j = i; j < i + 4; j++) {
				if (cardsToScore [j].getValue () != cardsToScore [j + 1].getValue () - 1)
					break;
				else if (j == i + 3) {
					value = cardsToScore [j].getValue () + 56;
					straight = cardsToScore [i];
				}
			}

		//Check for Flush
		for (int i = 0; i <= cardsToScore.Length - 5; i++)
			for (int j = i; j < i + 4; j++) {
				if (!cardsToScore [j].getSuit ().Equals (cardsToScore [j + 1].getSuit ()))
					break;
				else if (j == i + 3) {
					value = cardsToScore [j].getValue () + 70;
					flush = cardsToScore [i];
				}
			}
			
		//Check for Full-House
		if (pair != null && three != null)
			value = three.getValue () + 84;

		//Check for Four of a Kind
		for (int i = 0; i <= cardsToScore.Length - 4; i++)
			for (int j = i + 1; j < i + 4; j++) {
				if (cardsToScore [j].getValue () != cardsToScore [i].getValue())
					break;
				else if (j == i + 3)
					value = cardsToScore [j].getValue () + 98;
			}
		
		//Check for Straight Flush
		if (straight != null && flush != null)
			value = (straight.getValue () + 4) + 112;


        return value;
    }

	public Card[] sortHand(Card[] cardsToSort){
		if (cardsToSort.Length == 0)
			return cardsToSort;
		Card[] ret = new Card[cardsToSort.Length];
		Card min = cardsToSort[0];
		for (int i = 0; i < cardsToSort.Length; i++) {
			int mindex = 0;
			for (int j = 0; j < cardsToSort.Length; j++) {
				if (min == null) {
					min = cardsToSort [j];
					mindex = j;
				} else if (cardsToSort [j] != null && cardsToSort [j].getValue () < min.getValue ()) {
					min = cardsToSort [j];
					mindex = j;
				}
			}
			ret [i] = min;
			cardsToSort [mindex] = null;
			min = null;
		}

		return ret;
			
	}

}
