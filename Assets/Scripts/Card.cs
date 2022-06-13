using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{

    int suit;
    int value;

    public Card(int suit, int value)
    {
        this.suit = suit;
        this.value = value;
    }

    public int GetSuit()
    {
        return suit;
    }

    public int GetValue()
    {
        return value;
    }

    public void DebugCard()
    {
        Debug.Log(string.Format("SUIT: {0} VALUE: {1}", this.suit, this.value));
    }

}
