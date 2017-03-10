using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexasHoldEm : MonoBehaviour {

    // Bet, Raise, Check, Fold
    public Button[] actionButtons;
    public Player[] allPlayers;

    public HumanPlayer human;

    private Deck deck;
    private Rules referee;
    public Card[] table;

    public int tablePool;

    private bool gameInProgress;

    private int currentPlayer = 0;

    private int turn = 0;

    private int lastBet;

    private bool playerTurnFinished = false;

    private int smallBlind = 25;
    private int bigBlind = 50;
	// Use this for initialization
	void Start () {
		gameObject.AddComponent<Deck> ();
		deck = GetComponent<Deck> ();
        referee = new Rules();
        table = new Card[5];

        for (int i = 0; i < actionButtons.Length; i++)
        {
            actionButtons[i].onClick.AddListener(delegate { playerInput(actionButtons[i].name); });
        }

        StartCoroutine(startGame());

		/*allPlayers = new Player[4];
		for (int i = 0; i < allPlayers.Length; i++)
			if (i == 0)
				allPlayers [i] = new HumanPlayer ();
			else
				allPlayers [i] = new Player (); */
		//dealPlayers ();
	}
	
	// Update is called once per frame
	void Update () {

	}

    public IEnumerator startGame()
    {
        // While gameInProgress
        // Deal all players
        // Start with human player
        // Wait for Check, Bet or Fold
        // If any of the buttons are pressed and processed
        // AI Checks, Bets, Folds or Raises until back to Player
        // Deal first 3 cards to table
        // Repeat
        // Deal 1 card to table
        // Repeat
        // Deal 1 card to table
        // Process final hands of all players
        // Give winning player all the money in the pool
        // Deal all players
        // Switch starting player to person on the left
        // Move blinds?
        // Repeat above


        while (allPlayers.Length != 1)
        {
            dealPlayers();
            for (int i = 0; i < allPlayers.Length; i++)
            {
                currentPlayer = i;
                lastBet = bigBlind;
                allPlayers[(i + 1) % allPlayers.Length].bet(smallBlind);
                allPlayers[(i + 2) % allPlayers.Length].bet(bigBlind);


                if (allPlayers[currentPlayer].playerType == "Human")
                {
                    playerTurnFinished = false;
                    yield return StartCoroutine(waitForPlayerInput());
                }


                if (i == allPlayers.Length - 1)
                    i = 0;

            }
        }

        //displayCards ();
        /*if (allPlayers[currentPlayer].isTurn && !allPlayers[currentPlayer].folded)
            referee.giveTurnTo(allPlayers[currentPlayer]);
        currentPlayer++;*/
    }

    void displayCards(){
		for (int i = 0; i < allPlayers.Length; i++) {
			for (int j = 0; j < allPlayers [i].getHand().Length; j++) {
				Instantiate (allPlayers [i].getHand() [j].getPrefab(), new Vector3 (Mathf.Cos(180*i/Mathf.PI), Mathf.Sin(180*i/Mathf.PI)), Quaternion.identity);
			}

		}
			

	}

    public void dealPlayers()
    {
        foreach (Player p in allPlayers)
            p.addToHand(deck.deal(), deck.deal());
    }

    public void dealTable()
    {
        for (int i = 0; i < table.Length; i++) {
            if (table[i] == null) {
                table[i] = deck.deal();
                return;
            }
        }
        
    }

    public void addToPool(int moneyAmount)
    {
        tablePool += moneyAmount;
        lastBet = moneyAmount;
    }

    public int getLastBet()
    {
        return lastBet;
    }

    public void playerInput(string buttonName)
    {
        if (currentPlayer == 0)
            if (!playerTurnFinished) {
                switch (buttonName) {
                    case "Bet":
                        human.parseBetInput();
                        playerTurnFinished = true;
                        break;
                    case "Raise":
                        human.raise();
                        playerTurnFinished = true;
                        break;
                    case "Check":
                        human.check();
                        playerTurnFinished = true;
                        break;
                    case "Fold":
                        human.fold();
                        playerTurnFinished = true;
                        break;
                    default:
                        break;
                }
            }        
    }

    public IEnumerator waitForPlayerInput()
    {
        while (!playerTurnFinished)
        { 
            yield return 0;
        }
    }

}
