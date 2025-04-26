using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("Fuel UI")]
    public Slider fuelSlider; // Assign your Fuel Slider here
    public int startingFuel = 100; // Starting fuel you want

    [Header("Votes UI")]
    public Slider votesSlider; // Assign your Votes Slider here
    public int maxVotes = 100; // Set based on your game rules

    [Header("Level Timer UI")]
    public Image timerRadialFill; // Timer radial image
    public float levelDuration = 300f; // 5 minutes = 300 seconds
    private float timer;

    public GameObject LevelCompletePanel;
    public GameObject GameOverPanel;
    private BoatController boatController;
    public string NextLevel;

    private void Start()
    {
        boatController = FindObjectOfType<BoatController>();

        if (boatController != null)
        {
            boatController.maxFuel = startingFuel; // Setting boat's max fuel if needed

        }

        if (fuelSlider != null && boatController != null)
        {
            boatController.currentFuel = startingFuel;
            fuelSlider.maxValue = boatController.maxFuel; // Set slider's max value
            fuelSlider.value = startingFuel;    // Start full
        }

        if (votesSlider != null)
        {
            votesSlider.maxValue = maxVotes;
            votesSlider.value = 0;
        }

        timer = levelDuration;

        Debug.Log("Game Started");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private void Update()
    {
        UpdateFuelUI();
        UpdateVotesUI();
        UpdateTimerUI();
    }

    void UpdateFuelUI()
    {
        if (boatController != null && fuelSlider != null)
        {
            fuelSlider.value = boatController.currentFuel; // Directly assign the current fuel
        }
    }

    void UpdateVotesUI()
    {
        if (boatController != null && votesSlider != null)
        {
            votesSlider.value = Mathf.Clamp(HouseManager.VoteCount, 0, maxVotes); // Direct votes value
        }
        if (votesSlider.value == votesSlider.maxValue)
        {
            LevelCompleted();
        }
    }

    void UpdateTimerUI()
    {
        if (timerRadialFill != null)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Max(timer, 0f);

            timerRadialFill.fillAmount = timer / levelDuration;
        }

        if (timer <= 0f)
        {
            EndGame();
        }
    }

   public  void EndGame()
    {
        Debug.Log("Level Timer Ended!");
        GameOver();
        // You can add endgame screen or logic here
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void LevelCompleted()
    {
        EnemyMover[] enemies = FindObjectsByType<EnemyMover>(FindObjectsSortMode.None);
        foreach (EnemyMover enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }
        //LevelCompletePanel.SetActive(true);
        Nextlevel();
    }
    void GameOver()
    {
        MainMenuButton();
    }
    public void Nextlevel()
    {
        SceneManager.LoadScene(NextLevel);
    }
}