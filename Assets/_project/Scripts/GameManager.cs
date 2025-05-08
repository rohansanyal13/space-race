using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Platformer
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;

        public int coinsCounter = 0;
        public int fuelCounter = 0;

        public static int player1DeathCounter = 0;
        public static int player2DeathCounter = 0;

        public GameObject player1GameObject;
        public GameObject player2GameObject;

        private PlayerController player1;
        private PlayerController player2;

        public GameObject player1DeathPrefab;
        public GameObject player2DeathPrefab;

        public Text coinText;
        public Text fuelText;
        public Text player1DeathText;
        public Text player2DeathText;

        private int checkpointCoins = 0;
        private int checkpointFuel = 0;
        private bool checkpointReached = false;

        private Vector3 checkpointPlayer1Pos;
        private Vector3 checkpointPlayer2Pos;

        private HashSet<string> collectedCoins = new HashSet<string>();
        private HashSet<string> collectedFuel = new HashSet<string>();

        // Pause Menu Variables
        [Header("Pause Menu")]
        public GameObject pauseMenuPanel;
        public KeyCode pauseKey = KeyCode.Escape;
        private bool isPaused = false;

        // Blur effect variables
        [Header("Blur Effect")]
        public Material blurMaterial;
        public float blurSize = 2.0f;
        private Image blurPanel;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;

                if (SceneManager.GetActiveScene().buildIndex == 0)
                {
                    PlayerPrefs.DeleteAll();
                    PlayerPrefs.Save();
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            FindAllUI();
            FindPlayers();
            UpdateAllUI();

            // Initialize pause menu
            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);

                // Set up blur effect if not already set up
                SetupBlurEffect();
            }
        }

        void Update()
        {
            UpdateAllUI();

            // Check for pause input
            if (Input.GetKeyDown(pauseKey))
            {
                TogglePause();
            }

            if (player1 != null && player1.deathState)
            {
                HandlePlayerDeath(player1GameObject, player1DeathPrefab, 1);
                player1.deathState = false;
            }

            if (player2 != null && player2.deathState)
            {
                HandlePlayerDeath(player2GameObject, player2DeathPrefab, 2);
                player2.deathState = false;
            }
        }

        private void FindAllUI()
        {
            coinText = GameObject.Find("CoinText")?.GetComponent<Text>();
            fuelText = GameObject.Find("FuelText")?.GetComponent<Text>();
            player1DeathText = GameObject.Find("Player1DeathText")?.GetComponent<Text>();
            player2DeathText = GameObject.Find("Player2DeathText")?.GetComponent<Text>();

            // Find pause menu if it exists in the scene
            if (pauseMenuPanel == null)
                pauseMenuPanel = GameObject.Find("PauseMenuPanel")?.gameObject;
        }

        private void SetupBlurEffect()
        {
            if (pauseMenuPanel != null)
            {
                // Get or add a background image component for the blur
                blurPanel = pauseMenuPanel.GetComponent<Image>();
                if (blurPanel == null)
                    blurPanel = pauseMenuPanel.AddComponent<Image>();

                // If we have a blur material, apply it to the panel
                if (blurMaterial != null)
                {
                    // Create a new instance of the material to avoid modifying the original
                    Material instanceMaterial = new Material(blurMaterial);
                    blurPanel.material = instanceMaterial;

                    // Set the blur size parameter if the shader supports it
                    if (instanceMaterial.HasProperty("_BlurSize"))
                        instanceMaterial.SetFloat("_BlurSize", blurSize);
                }
                else
                {
                    // If no blur material is assigned, use a semi-transparent background
                    blurPanel.color = new Color(0, 0, 0, 0.5f);
                }
            }
        }

        private void FindPlayers()
        {
            player1GameObject = GameObject.FindWithTag("Player1");
            player2GameObject = GameObject.FindWithTag("Player2");

            if (player1GameObject != null)
                player1 = player1GameObject.GetComponent<PlayerController>();
            if (player2GameObject != null)
                player2 = player2GameObject.GetComponent<PlayerController>();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindAllUI();
            FindPlayers();
            UpdateAllUI();
            RestoreCollectedItems();

            // Make sure game is not paused when a new scene loads
            if (isPaused)
                TogglePause();

            // Set up blur effect again (in case the pause menu is recreated)
            SetupBlurEffect();
        }

        private void UpdateAllUI()
        {
            if (coinText != null)
                coinText.text = "Coins: " + coinsCounter;
            if (fuelText != null)
                fuelText.text = "Energy: " + fuelCounter;
            if (player1DeathText != null)
                player1DeathText.text = player1DeathCounter.ToString();
            if (player2DeathText != null)
                player2DeathText.text = player2DeathCounter.ToString();
        }

        // Pause Menu Methods
        public void TogglePause()
        {
            isPaused = !isPaused;

            if (isPaused)
            {
                Time.timeScale = 0f; // Freeze the game
                if (pauseMenuPanel != null)
                    pauseMenuPanel.SetActive(true);
            }
            else
            {
                Time.timeScale = 1f; // Resume normal time
                if (pauseMenuPanel != null)
                    pauseMenuPanel.SetActive(false);
            }
        }

        public void ResumeGame()
        {
            if (isPaused)
                TogglePause();
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        // Rest of your methods remain unchanged
        public void ResetLevelStats()
        {
            if (checkpointReached)
            {
                coinsCounter = checkpointCoins;
                fuelCounter = checkpointFuel;
                collectedCoins = new HashSet<string>(PlayerPrefs.GetString("CollectedCoins", "").Split(','));
                collectedFuel = new HashSet<string>(PlayerPrefs.GetString("CollectedFuel", "").Split(','));
            }
            else
            {
                coinsCounter = 0;
                fuelCounter = 0;
                collectedCoins.Clear();
                collectedFuel.Clear();
            }
        }

        public void SaveCheckpoint()
        {
            checkpointCoins = coinsCounter;
            checkpointFuel = fuelCounter;
            checkpointReached = true;

            if (player1GameObject != null)
                checkpointPlayer1Pos = player1GameObject.transform.position;
            if (player2GameObject != null)
                checkpointPlayer2Pos = player2GameObject.transform.position;

            PlayerPrefs.SetString("CollectedCoins", string.Join(",", collectedCoins));
            PlayerPrefs.SetString("CollectedFuel", string.Join(",", collectedFuel));
            PlayerPrefs.Save();
        }

        public void ClearCheckpoint()
        {
            checkpointReached = false;
            checkpointCoins = 0;
            checkpointFuel = 0;
        }

        private void HandlePlayerDeath(GameObject playerObject, GameObject deathPrefab, int playerNumber)
        {
            if (playerNumber == 1)
                player1DeathCounter++;
            else if (playerNumber == 2)
                player2DeathCounter++;

            playerObject.SetActive(false);

            if (deathPrefab != null)
            {
                GameObject deathPlayer = Instantiate(deathPrefab, playerObject.transform.position, playerObject.transform.rotation);
                deathPlayer.transform.localScale = playerObject.transform.localScale;
            }

            ResetLevelStats();
            Invoke(nameof(ReloadLevel), 3f);
        }

        private void ReloadLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            StartCoroutine(RespawnPlayersAtCheckpoint());
        }

        private IEnumerator RespawnPlayersAtCheckpoint()
        {
            yield return new WaitForSeconds(0.1f);
            FindPlayers();

            if (checkpointReached)
            {
                if (player1GameObject != null)
                {
                    player1GameObject.transform.position = checkpointPlayer1Pos;
                    player1GameObject.SetActive(true);
                }
                if (player2GameObject != null)
                {
                    player2GameObject.transform.position = checkpointPlayer2Pos;
                    player2GameObject.SetActive(true);
                }
            }
        }

        public void CollectCoin(string coinID)
        {
            coinsCounter++;
            collectedCoins.Add(coinID);
        }

        public void CollectFuel(string fuelID, int amount = 1)
        {
            fuelCounter += amount;
            collectedFuel.Add(fuelID);
        }

        public void AddFuel(int amount = 1)
        {
            fuelCounter += amount;
        }

        public void DamagePlayer(string playerName)
        {
            if (playerName == "Player1" && player1 != null)
                player1.deathState = true;
            if (playerName == "Player2" && player2 != null)
                player2.deathState = true;
        }

        public void DamageBothPlayers()
        {
            if (player1 != null)
                player1.deathState = true;
            if (player2 != null)
                player2.deathState = true;
        }

        private void RestoreCollectedItems()
        {
            foreach (Coin coin in FindObjectsByType<Coin>(FindObjectsSortMode.None))
            {
                if (collectedCoins.Contains(coin.coinID))
                    Destroy(coin.gameObject);
            }

            foreach (Fuel fuel in FindObjectsByType<Fuel>(FindObjectsSortMode.None))
            {
                if (collectedFuel.Contains(fuel.fuelID))
                    Destroy(fuel.gameObject);
            }
        }
    }
}