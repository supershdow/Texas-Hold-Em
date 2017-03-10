using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HumanPlayer : Player {

    public InputField betInput;

    public HumanPlayer() : base()
    {
        playerType = "Human";
    }

    public bool parseBetInput()
    {
        //Debug.Log(betInput.text);
        string betString = betInput.text;
        int betAmount;

        if (int.TryParse(betString, out betAmount))
        {
            Debug.Log("Good Input");
            moneyInPool -= betAmount;
            betInput.text = "";
            bet(betAmount);
            return true;
        }
        else
        {
            Debug.Log("Bad Input");
            betInput.text = "";
        }
        return false;
    }
}
