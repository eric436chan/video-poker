using UnityEngine;
using UnityEngine.UI;

namespace VideoPoker
{
	//-//////////////////////////////////////////////////////////////////////
	///
	/// Manages UI including button events and updates to text fields
	/// 
	public class UIManager : MonoBehaviour
	{
		[SerializeField]
		private Text currentBalanceText = null;

		[SerializeField]
		private Text winningText = null;

		[SerializeField]
		private Text currentBetText = null;

		[SerializeField]
		private Button betButton = null;

		[SerializeField]
		private Button playButton = null;

		[SerializeField]
		private Button redealButton = null;




		GameManager gameManager;
		[SerializeField]
		private Sprite[] sprites;


		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Awake()
		{
			gameManager = FindObjectOfType<GameManager>();
		}

		//-//////////////////////////////////////////////////////////////////////
		/// 
		void Start()
		{
			betButton.onClick.AddListener(OnBetButtonPressed);
			playButton.onClick.AddListener(OnPlayButtonPressed);
			redealButton.onClick.AddListener(OnRedealButtonPressed);
			winningText.text = "Welcome to Video Poker!";
		}

		void Update()
		{
			betButton.interactable = !gameManager.gameStarted;
			playButton.interactable = !gameManager.gameStarted;
			redealButton.interactable = gameManager.gameStarted;

			currentBalanceText.text = string.Format("Current Balance: {0} Credits", gameManager.playerBalance);
			currentBetText.text = string.Format("Current Bet: {0} Credits", gameManager.currentBet);
		}

		//-//////////////////////////////////////////////////////////////////////
		///
		/// Event that triggers when bet button is pressed
		/// 
		private void OnBetButtonPressed()
		{
			gameManager.IncrementBet();
		}

		private void OnPlayButtonPressed()
		{
			gameManager.StartGame();
		}

		private void OnRedealButtonPressed()
		{
			gameManager.ReDeal();
		}

		public void UpdateWinningText(string text)
		{
			winningText.text = text;
		}

		public void UpdateSprite(Transform card, int suit, int value)
		{
			card.Find("CardImage").GetComponent<Image>().sprite = this.sprites[value + (suit * 13)];

		}

		public void UpdateHold(Transform card)
		{
			//does not allow you to update hold status if the game hasn't started
			if (!this.gameManager.gameStarted)
			{
				return;
			}

			bool enabled = card.GetChild(1).gameObject.activeSelf;
			card.GetChild(1).gameObject.SetActive(!enabled);

		}
	}
}