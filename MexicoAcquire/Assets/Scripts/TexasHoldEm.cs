using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexasHoldEm : MonoBehaviour
{

    // Bet, Raise, Check, Fold
    public Button[] actionButtons;
    public InputField input;
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

    private int startingPlayer = 0;
    private bool threeFolded = false;

    private LinkedList<GameObject> allCards;
    private List<int> notFolded = new List<int>();

	public Data dataSim;

    private bool turnOver = false;
	private string winner = "";


    // Use this for initialization
    void Start()
    {
		gameObject.AddComponent<Data> ();
		dataSim = gameObject.GetComponent<Data> ();
        gameObject.AddComponent<Deck>();
        deck = GetComponent<Deck>();
        referee = new Rules();
        table = new Card[5];

        allCards = new LinkedList<GameObject>();

        StartCoroutine(startGame());
    }

    public IEnumerator startGame()
    {

        // If there are 2 or more players left playing
        while (allPlayers.Length != 1)
        {

            dealPlayers();
            displayCards();
            lastBet = bigBlind;

            // Until all 5 cards are dealt to the table,
            for (int turn = 0; turn < 3; turn++)
            {

                // If three have not folded.
                if (!threeFolded)
                {
                    // Loop through all the players and have them have their turns
                    for (int i = 0; i < allPlayers.Length; i++)
                    {

                        currentPlayer = (i + startingPlayer) % allPlayers.Length;

                        if (turn == 0 && i == 0)
                        {
                            allPlayers[(startingPlayer + 1) % allPlayers.Length].bet(smallBlind);
                            allPlayers[(startingPlayer + 2) % allPlayers.Length].bet(bigBlind);

                            //Debug.Log((i + 1) % allPlayers.Length);
                            //Debug.Log((i + 2) % allPlayers.Length);
                        }



                        if (allPlayers[currentPlayer].playerType.Equals("Human") && allPlayers[currentPlayer].isFolded == false)
                        {
                            playerTurnFinished = false;
                            human.checking = false;
                            yield return StartCoroutine(waitForPlayerInput());
                            //Debug.Log("Player Turn Over");
                        }
                        else if (allPlayers[currentPlayer].playerType.Equals("AI") && allPlayers[currentPlayer].isFolded == false)
                        {
                            //Debug.Log("AI Turn");
                            allPlayers[currentPlayer].AIMove();
                        }

                        bool duplicatePlayer = false;
                        foreach (int x in notFolded)
                            if (x == currentPlayer)
                                duplicatePlayer = true;

                        if (!duplicatePlayer)
                            if(allPlayers[currentPlayer].isFolded)
                                notFolded.Add(currentPlayer);

                        if (notFolded.Count == allPlayers.Length - 1)
                            threeFolded = true;

                    }

                    //Add cards to the table
                    if (turn == 0)
                        for (int cardNumber = 0; cardNumber < 3; cardNumber++)
                            dealTable();
                    else
                        dealTable();
					//Debug.Log ("END TURN");
					lastBet = 0;


                }
            }

            //Show cards and determine winner
            int[] scores = new int[4];
            for (int i = 1; i < allPlayers.Length; i++)
                if (!allPlayers[i].isFolded)
                {
                    Card[] Hand = new Card[7];
                    for (int a = 0; a < 2; a++)
                        Hand[a] = allPlayers[i].getHand()[a];
                    for (int b = 0; b < 5; b++)
                        Hand[b + 2] = table[b];
                    scores[i] = referee.scoreHand(Hand);
                    for (int j = 0; j < allPlayers[i].getHand().Length; j++)
                        allPlayers[i].handObject[j].transform.Rotate(new Vector3(0, 180, 0));

                }

            //DEBUG
           /* for (int i = 0; i < scores.Length; i++)
                Debug.Log("Index: " + i + " Score: " + scores[i]); 
           */


            int highestScore = -1;
            int highestScoreIndex = -1;
            for (int i = 0; i < scores.Length; i++)
            {
                if (highestScore < scores[i])
                {
                    highestScore = scores[i];
                    highestScoreIndex = i;
                }
            }

            allPlayers[highestScoreIndex].moneyInPool = allPlayers[highestScoreIndex].moneyInPool + tablePool;
			winner = allPlayers [highestScoreIndex].name;
            //Debug.Log("Winner: " + allPlayers[highestScoreIndex].name);

            yield return new WaitForSeconds(5);

            //Cleanup between rounds
            resetTable();
            startingPlayer++;

            if(startingPlayer == 4)
            {
                startingPlayer = 0;
            }


			dataSim.gatherData (1000);
        }


        //displayCards ();
        /*if (allPlayers[currentPlayer].isTurn && !allPlayers[currentPlayer].isFolded)
            referee.giveTurnTo(allPlayers[currentPlayer]);
        currentPlayer++;*/
    }

    void displayCards()
    {
        Quaternion rot;
        for (int i = 0; i < allPlayers.Length; i++)
        {
            for (int j = 0; j < allPlayers[i].getHand().Length; j++)
            {
                rot = Quaternion.identity;
                float xshift = 0f;
                float yshift = 0f;
                if (allPlayers[i].playerType.Equals("Human"))
                    rot = Quaternion.Euler(0, 180, 0);
                if (i == 1 || i == 3)
                {
                    //Ensures cards appear in the way they should
                    yshift = .4f * (j - .5f);
                    xshift = 0f;
                    rot = Quaternion.Euler(0, 0, -90 * (i - 2));
                }
                else
                {
                    yshift = 0f;
                    xshift = .4f * (j - .5f);
                }

                GameObject temp = Instantiate(allPlayers[i].getHand()[j].getPrefab(), new Vector3(.5f * -Mathf.Cos(Mathf.PI * (i - 1) / 2) + xshift, 0.5f * Mathf.Sin(Mathf.PI * (i - 1) / 2) + 1 + yshift, (float)-9), rot);
                temp.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                temp.transform.localScale = temp.transform.localScale * 3;

                if (allPlayers[i].getObjects().Length == 0)
                    allPlayers[i].handObject = new GameObject[2];
                allPlayers[i].getObjects()[j] = temp;
                allCards.AddLast(temp);
            }

        }


    }

    public void resetTable()
    {
        foreach (GameObject go in allCards)
            //go.SetActive (false);
            Destroy(go);
        table = new Card[5];
        allCards.Clear();

        foreach (Player p in allPlayers)
        {
            p.isFolded = false;
            p.hand = new Card[2];
        }

        tablePool = 0;
        threeFolded = false;
        notFolded.Clear();

    }


    public void dealPlayers()
    {
        foreach (Player p in allPlayers)
            p.addToHand(deck.deal(), deck.deal());
    }

    public void dealTable()
    {
        for (int i = 0; i < table.Length; i++)
        {
            if (table[i] == null)
            {
                table[i] = deck.deal();
                GameObject temp = Instantiate(table[i].getPrefab(), new Vector3(.2f * i - .3f, 1f, (float)-9), Quaternion.Euler(0, 180, 0));
                temp.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                temp.transform.localScale = temp.transform.localScale * 2.5f;
                allCards.AddLast(temp);
                return;
            }
        }

    }

    public void addToPool(int moneyAmount)
    {
        tablePool += moneyAmount;
        lastBet = moneyAmount;
		//Debug.Log (lastBet);
    }

    public int getLastBet()
    {
        return lastBet;
    }
    public void setLastBet(int lb)
    {
        lastBet = lb;
    }

    public void playerInput(string buttonName)
    {
        if (currentPlayer == 0)
            if (!playerTurnFinished)
            {
                switch (buttonName)
                {
                    case "Bet":
                        //Debug.Log("Bet Pressed");
                        bool betParsed = human.parseBetInput();
                        if (betParsed)
                            playerTurnFinished = true;
                        break;
                    case "Call":
                        //Debug.Log("Raise Pressed");
				human.call();
                        playerTurnFinished = true;
                        break;
			case "Check":
                        //Debug.Log("Check Pressed");
				if (human.canCheck ()) {
					human.check ();
					playerTurnFinished = true;
				}
                        break;
                    case "Fold":
                        //Debug.Log("Fold Pressed");
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
            //Debug.Log("Waiting");
            yield return 0;
        }
        StopCoroutine(waitForPlayerInput());
    }


    public void OnGUI()
    {
        float xshift = 80f;
        float yscale = 30f;
        actionButtons[0].transform.position = new Vector3(xshift, Screen.height - 20);
        input.transform.position = new Vector3(xshift, Screen.height - 20 - yscale);
        actionButtons[1].transform.position = new Vector3(xshift, Screen.height - 20 - yscale * 2);
        actionButtons[2].transform.position = new Vector3(xshift, Screen.height - 20 - yscale * 3);
        actionButtons[3].transform.position = new Vector3(xshift, Screen.height - 20 - yscale * 4);
        //Debug.Log (Screen.height * Camera.main.ScreenToWorldPoint(new Vector3 (0, Screen.height)));
        GUI.Label(new Rect(10, 200, 400, 20), "Pool: " + tablePool);
		GUI.Label (new Rect (10, 220, 400, 20), "Last Bet: " + getLastBet ());
        //GUI.Label(new Rect(10, 220, 400, 20), "Current Player: " + allPlayers[currentPlayer].name);
        GUI.Label(new Rect(10, 240, 400, 200), "Big Blind: " + bigBlind);
        GUI.Label(new Rect(10, 260, 400, 200), "Small Blind: " + smallBlind);
        //GUI.Label(new Rect(10, 280, 400, 200), "Player Turn Finished: " + playerTurnFinished);
        //GUI.Label(new Rect(10, 300, 400, 200), "Player Checked: " + human.checking);
        GUI.Label(new Rect(10, 320, 400, 200), "Player Pool: " + human.moneyInPool);
        GUI.Label(new Rect(10, 340, 400, 200), "Player Folded: " + human.isFolded);
        GUI.Label(new Rect(10, 360, 400, 200), "Peal Pool: " + allPlayers[1].moneyInPool);
        GUI.Label(new Rect(10, 380, 400, 200), "Peal Folded: " + allPlayers[1].isFolded);

        GUI.Label(new Rect(10, 400, 400, 200), "Mack Pool: " + allPlayers[2].moneyInPool);
        GUI.Label(new Rect(10, 420, 400, 200), "Mack Folded: " + allPlayers[2].isFolded);

        GUI.Label(new Rect(10, 440, 400, 200), "Zim Pool: " + allPlayers[3].moneyInPool);
        GUI.Label(new Rect(10, 460, 400, 200), "Zim Folded: " + allPlayers[3].isFolded);

		GUI.Label (new Rect (10, 480, 400, 200), "Who won: " + winner);


    }

}
