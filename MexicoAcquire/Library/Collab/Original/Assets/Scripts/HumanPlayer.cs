using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : Player {

	public Text betInput;

    public HumanPlayer() : base()
    {
        playerType = "Human";
    }

    public void parseBetInput()
    {
        string betString = betInput.GetComponent<Text>().text;
        int betAmount;

        if (int.TryParse(betString, out betAmount))
        {
            moneyInPool -= betAmount;
            betInput.GetComponent<Text>().text = "";
            bet(betAmount);
        }
        else
        {
            betInput.GetComponent<Text>().text = "";
        }
    }
}
