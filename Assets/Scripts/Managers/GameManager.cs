using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace VideoPoker
{
	//-//////////////////////////////////////////////////////////////////////
	/// 
	/// The main game manager
	/// 
	public class GameManager : MonoBehaviour
	{

		UIManager uiManager;

		public int playerBalance;
		public int currentBet;
		public bool gameStarted;

		List<Card> deck = new List<Card>();
		List<Card> playerHand = new List<Card>();

		//value as index, count as value stored
		[SerializeField]
		private int[] cardCount = new int[13];

		public GameObject cardPanel;

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Awake()
		{
			gameStarted = false;
			playerBalance = 500;
			currentBet = 0;
			uiManager = FindObjectOfType<UIManager>();
		}

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{

			GenerateDeck();
			ShuffleDeck();

			//show pre game visual (better looking than just aces)
			for (int child = 0; child < this.cardPanel.transform.childCount; child++)
			{
				Card card = deck[0];
				Transform cardPrefab = this.cardPanel.transform.GetChild(child);
				uiManager.UpdateSprite(cardPrefab, card.GetSuit(), card.GetValue());

				//add event lister to the card prefab
				cardPrefab.GetComponent<Button>().onClick.AddListener(() => uiManager.UpdateHold(cardPrefab));


				playerHand.Add(card);
				deck.Remove(card);

			}
		}

		//-//////////////////////////////////////////////////////////////////////
		/// 

		public void IncrementBet()
		{
			this.currentBet += 5;
			this.playerBalance -= 5;
		}

		void GenerateDeck()
		{
			//handles suits
			for (int i = 0; i < 4; i++)
			{
				//handles values
				for (int j = 0; j < 13; j++)
				{

					Card card = new Card(i, j);
					this.deck.Add(card);
				}
			}
		}

		void ShuffleDeck()
		{
			System.Random rand = new System.Random();

			int initialPlace = deck.Count;

			while (initialPlace > 0)
			{
				int newPlace = rand.Next(initialPlace);
				//swap
				initialPlace--;
				Card temp = deck[newPlace];
				deck[newPlace] = deck[initialPlace];
				deck[initialPlace] = temp;

			}

		}

		public void StartGame()
		{

			this.deck.AddRange(this.playerHand);
			this.playerHand.Clear();

			gameStarted = true;

			//handles player pressing play without betting, uses standard bet amount
			if (currentBet == 0)
			{
				currentBet = 5;
			}
			ShuffleDeck();
			for (int child = 0; child < this.cardPanel.transform.childCount; child++)
			{
				Card card = deck[0];
				Transform cardPrefab = this.cardPanel.transform.GetChild(child);
				uiManager.UpdateSprite(cardPrefab, card.GetSuit(), card.GetValue());



				playerHand.Add(card);
				deck.Remove(card);

			}
		}

		public void ReDeal()
		{

			//resets counts array
			for (int i = 0; i < cardCount.Length; i++)
			{
				cardCount[i] = 0;
			}


			for (int child = 0; child < this.cardPanel.transform.childCount; child++)
			{
				//updates cards that are not held
				Transform cardPrefab = this.cardPanel.transform.GetChild(child);
				if (!cardPrefab.GetChild(1).gameObject.activeSelf)
				{
					Card card = this.deck[0];
					Card temp = this.playerHand[child];
					this.playerHand[child] = card;
					this.deck.RemoveAt(0);
					this.deck.Add(temp);

					uiManager.UpdateSprite(cardPrefab, card.GetSuit(), card.GetValue());

				}
				else
				{
					cardPrefab.GetChild(1).gameObject.SetActive(false);

				}
			}

			//update card count array for score calculation
			foreach (Card c in this.playerHand)
			{
				cardCount[c.GetValue()]++;
			}

			CalculateScore();


		}

		void CalculateScore()
		{
			int multiplier = 0;

			if (RoyalFlush())
			{
				multiplier = 800;
				this.uiManager.UpdateWinningText(string.Format("Royal Flush! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (StraightFlush())
			{
				multiplier = 50;
				this.uiManager.UpdateWinningText(string.Format("Straight Flush! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (FourOfKind())
			{
				multiplier = 25;
				this.uiManager.UpdateWinningText(string.Format("Four of a Kind! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (FullHouse())
			{
				multiplier = 9;
				this.uiManager.UpdateWinningText(string.Format("Full House! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (Flush())
			{
				multiplier = 6;
				this.uiManager.UpdateWinningText(string.Format("Flush! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (Straight() || StraightAces())
			{
				multiplier = 4;
				this.uiManager.UpdateWinningText(string.Format("Straight! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (ThreeOfKind())
			{
				multiplier = 3;
				this.uiManager.UpdateWinningText(string.Format("Three of a Kind! You won {0} credits.", (currentBet * multiplier).ToString()));

			}
			else if (TwoPair())
			{
				multiplier = 2;
				this.uiManager.UpdateWinningText(string.Format("Two Pair! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else if (JackOrBetter())
			{
				multiplier = 1;
				this.uiManager.UpdateWinningText(string.Format("Jacks or Better! You won {0} credits.", (currentBet * multiplier).ToString()));
			}
			else
			{
				this.uiManager.UpdateWinningText("No Winning Hand!");
			}
			//updates player balance based on the current bet and the multiplier based on the hand
			playerBalance += currentBet * multiplier;
			currentBet = 0;
			gameStarted = false;
		}

		bool JackOrBetter()
		{

			//checks the aces counts
			if (cardCount[0] == 2)
			{
				return true;
			}

			//jacks to kings
			for (int i = 10; i < cardCount.Length; i++)
			{
				if (cardCount[i] == 2)
				{
					return true;
				}
			}

			return false;
		}

		bool TwoPair()
		{
			int pairs = 0;
			foreach (int i in cardCount)
			{
				if (i == 2)
				{
					pairs++;
				}
			}

			return pairs >= 2;
		}

		bool ThreeOfKind()
		{
			foreach (int i in cardCount)
			{
				if (i == 3)
				{
					return true;
				}
			}

			return false;
		}

		bool Flush()
		{
			for (int i = 1; i < this.playerHand.Count; i++)
			{
				Card card = this.playerHand[i];
				if (card.GetSuit() != (this.playerHand[i - 1].GetSuit()))
				{
					return false;
				}

			}
			return true;
		}

		bool Straight()
		{

			int firstIndex = 0;

			for (int index = 0; index < cardCount.Length; index++)
			{
				//return false because if count is 2, no way for a straight to exist
				if (cardCount[index] > 1)
				{
					return false;
				}

				//get index of first occurrence of a count of 1
				if (cardCount[index] == 1)
				{
					firstIndex = index;
					break;
				}

			}

			//checks the next four card values
			for (int index = firstIndex + 1; index < firstIndex + 5; index++)
			{
				if (cardCount[index] != 1)
				{
					return false;
				}
			}
			return true;
		}


		//10 to aces straight
		bool StraightAces()
		{


			//checks to see if aces exist or if more than one ace exists
			if (cardCount[0] != 1)
			{
				return false;
			}

			for (int index = 9; index < cardCount.Length; index++)
			{
				if (cardCount[index] != 1)
				{
					return false;
				}
			}
			return true;
		}

		bool FourOfKind()
		{

			foreach (int i in cardCount)
			{
				if (i == 4)
				{
					return true;
				}
			}

			return false;
		}

		bool FullHouse()
		{

			return ThreeOfKind() && OnePair();
		}

		bool StraightFlush()
		{

			return Straight() && Flush();
		}

		bool RoyalFlush()
		{
			return StraightAces() && Flush();
		}

		//helper function for full house
		bool OnePair()
		{
			foreach (int i in cardCount)
			{
				if (i == 2)
				{
					return true;
				}
			}

			return false;
		}

	}
}