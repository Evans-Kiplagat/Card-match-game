using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CardMatchGame
{
    public class UniversalGameManager : MonoBehaviour
    {
        // class instance
        public static UniversalGameManager Instance;
        public static int gameSize = 2;
 
        // Prefab for instantiating card game objects
        [SerializeField] private GameObject cardPrefab;

        // Parent game object for holding all card instances
        [SerializeField] private GameObject cardList;

        // Sprite to be used as the back of the cards
        [SerializeField] private Sprite cardBackSprite;

        // Array of sprites to be used as the front faces of the cards
        [SerializeField] private Sprite[] cardFrontSprites;

        // Array to hold references to all instantiated card game objects
        private MatchCard[] cards;

        // Game object representing the panel where the cards will be placed
        [SerializeField] private GameObject UIGamePanel;

        // other UI

        // Text UI element to display the game size
        [SerializeField] private Text sizeLabel;

        // Slider UI element to adjust the game size
        [SerializeField] private Slider sizeSlider;

        public int PlayerScore = 0;
        public int currentLevel = 2;
        public Text scoreText;
        public Text levelText;

        public Button levelButton;

        private int spriteSelected;
        private int cardSelected;
        private int cardLeft;
        private bool isGameRunning;

        void Awake()
        {
            Instance = this;
        }
        void Start()
        {
            isGameRunning = false;
            UIGamePanel.SetActive(false);
            LoadGameData();
        }

        // Start a game
        public void StartGame()
        {
            // Check if the game is already running
            //if (isGameRunning)
              //  return;

            // Set the game state to running
            isGameRunning = true;

            // Toggle the UI panel visibility
            UIGamePanel.SetActive(true);

            // Set up the game panel
            SetGamePanel();

            // Reset gameplay variables
            cardSelected = spriteSelected = -1;
            cardLeft = cards.Length;

            // Allocate sprites to cards
            SpriteCardAllocation();

            // Start the card face hiding coroutine
            StartCoroutine(HideFace());
        }

        // Initialize cards, size, and position based on size of game
        private void SetGamePanel()
        {
            // Determine if the game size is odd
            int isOdd = gameSize % 2;

            // Calculate the number of cards needed
            int cardCount = gameSize * gameSize - isOdd;
            cards = new MatchCard[cardCount];

            // Remove all existing cards from the parent
            foreach (Transform child in cardList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Get the panel size and anchor position
            RectTransform panelTransform = UIGamePanel.transform as RectTransform;
            Vector3 panelPosition = panelTransform.position;
            float row_size = panelTransform.sizeDelta.x;
            float col_size = panelTransform.sizeDelta.y;

            // Calculate the scale and position offsets
            float scale = 1.0f / gameSize;
            float xInc = row_size / gameSize;
            float yInc = col_size / gameSize;
            float curX = -xInc * (float)(gameSize / 2);
            float curY = -yInc * (float)(gameSize / 2);


            if (isOdd == 0)
            {
                curX += xInc / 2;
                curY += yInc / 2;
            }
            float initialX = curX;

            // Iterate over rows for each in y-axis
            for (int i = 0; i < gameSize; i++)
            {
                curX = initialX;
                // Iterate over rows for each in x-axis
                for (int j = 0; j < gameSize; j++)
                {
                    GameObject c;
                    // Handle the last card position if the game size is odd
                    if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                    {
                        int index = gameSize / 2 * gameSize + gameSize / 2;
                        c = cards[index].gameObject;
                    }
                    else
                    {
                        // create card prefab
                        c = Instantiate(cardPrefab);
                        // assign parent
                        c.transform.parent = cardList.transform;

                        int index = i * gameSize + j;
                        cards[index] = c.GetComponent<MatchCard>();
                        cards[index].ID = index;
                        // modify its size
                        c.transform.localScale = new Vector3(scale, scale);
                    }
                    // assign location
                    c.transform.localPosition = new Vector3(curX, curY, 0);
                    curX += xInc;

                }
                curY += yInc;
            }

        }

        // reset face-down rotation of all cards

        void ResetFace()
        {
            for (int i = 0; i < gameSize; i++)
                cards[i].ResetRotation();
        }
        // Flip all cards after a short period
        IEnumerator HideFace()
        {
            //display for a short moment before flipping
            yield return new WaitForSeconds(0.3f);
            for (int i = 0; i < cards.Length; i++)
                cards[i].Flip();
            yield return new WaitForSeconds(0.5f);
        }
        // Allocate pairs of sprite to card instances
        private void SpriteCardAllocation()
        {
            int i, j;
            int[] selectedID = new int[cards.Length / 2];
            // sprite selection
            for (i = 0; i < cards.Length / 2; i++)
            {
                // get a random sprite
                int value = Random.Range(0, cardFrontSprites.Length - 1);
                // check previous number has not been selection
                // if the number of cards is larger than number of sprites, it will reuse some sprites
                for (j = i; j > 0; j--)
                {
                    if (selectedID[j - 1] == value)
                        value = (value + 1) % cardFrontSprites.Length;
                }
                selectedID[i] = value;
            }

            // card sprite deallocation
            for (i = 0; i < cards.Length; i++)
            {
                cards[i].Active();
                cards[i].SpriteID = -1;
                cards[i].ResetRotation();
            }
            // card sprite pairing allocation
            for (i = 0; i < cards.Length / 2; i++)
                for (j = 0; j < 2; j++)
                {
                    int value = Random.Range(0, cards.Length - 1);
                    while (cards[value].SpriteID != -1)
                        value = (value + 1) % cards.Length;

                    cards[value].SpriteID = selectedID[i];
                }

        }
        // Slider update gameSize
        public void SetGameSize()
        {
            gameSize = (int)sizeSlider.value;
            sizeLabel.text = gameSize + " X " + gameSize;
        }
        // return Sprite based on its id
        public Sprite GetSprite(int spriteId)
        {
            return cardFrontSprites[spriteId];
        }
        // return card back Sprite
        public Sprite CardBack()
        {
            return cardBackSprite;
        }
        // check if clickable
        public bool canClick()
        {
            if (!isGameRunning)
                return false;
            return true;
        }
        // card onclick event
        public void cardClicked(int spriteId, int cardId)
        {
            // first card selected
            if (spriteSelected == -1)
            {
                spriteSelected = spriteId;
                cardSelected = cardId;
            }
            else
            { // second card selected
                if (spriteSelected == spriteId)
                {
                    //correctly matched
                    cards[cardSelected].Inactive();
                    cards[cardId].Inactive();
                    cardLeft -= 2;
                    CheckGameWin();

                    ScoreUpdate();
                }
                else
                {
                    // incorrectly matched
                    cards[cardSelected].Flip();
                    cards[cardId].Flip();
                }
                cardSelected = spriteSelected = -1;
            }
        }

        // check if game score 
        private void ScoreUpdate()
        {
            PlayerScore += 1;
            scoreText.text = PlayerScore.ToString();
        }
        // check if game is completed
        private void CheckGameWin()
        {
            // win game
            if (cardLeft == 0)
            {
                levelButton.gameObject.SetActive(true);
                AudioPlayer.Instance.PlayAudioClip(1);
            }
        }

    
        public void NextLevelGame()
        {
            //isGameRunning = true;
            currentLevel += 1;
            gameSize = currentLevel;
            currentLevel = gameSize;
            levelText.text = currentLevel.ToString();   
            StartGame();
        }

        public void SaveGameData()
        {
            // Save player's score
            PlayerPrefs.SetInt("PlayerScore", PlayerScore);

            // Save player's level
            PlayerPrefs.SetInt("PlayerLevel", currentLevel);

            // Optionally, you can set a flag to indicate that data has been saved
            PlayerPrefs.SetInt("DataSaved", 1);
        }

        public void LoadGameData()
        {
            // Check if data has been saved
            if (PlayerPrefs.GetInt("DataSaved", 0) == 1)
            {
                // Load player's score
                PlayerScore = PlayerPrefs.GetInt("PlayerScore", 0);

                // Load player's level
                currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
            }
            else
            {
                // No saved data, initialize with default values
                PlayerScore = 0;
                currentLevel = 1;
            }
        }

        public void ResetGameData()
        {
            PlayerPrefs.DeleteAll();
        }

        // stop game
        public void EndGame()
        {
            PlayerScore = -1;
            currentLevel = 1;
            isGameRunning = false;
            UIGamePanel.SetActive(false);
            SaveGameData();
        }
    
 
    }
}
