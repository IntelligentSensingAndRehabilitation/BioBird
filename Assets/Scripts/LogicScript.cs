using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class LogicScript : MonoBehaviour
{
    [Serializable]
    public class Event : UnityEvent { };
    public Event SetDefaultPipeWidthEvent;

    public int playerScore; // Holds the player's score
    public int countdownTimer; // Holds the countdown timer value
    public Text scoreText; // Reference to the score text in the UI
    public Text countdownText; // Reference to the countdown text in the UI
    public GameObject gameOverScreen; // Reference to the game-over screen
    public AudioSource ding; // Reference to the audio source for sound effects
    private bool isPausedGameOver = false;
    private Coroutine restartCoroutine; // Reference to the coroutine for restarting

    public void addScore(int scoreToAdd)
    {
        playerScore += scoreToAdd; // Increase the score
        updateScoreText(); // Update the score in the UI
        ding.Play(); // Play the sound effect
        
        AndroidBinding.Instance.SetPlayerNormalScore(playerScore);

        Scene scene = SceneManager.GetActiveScene();
        AndroidBinding.Instance.SetPlayerLevel(scene.name);
    }

    private void updateScoreText()
    {
        scoreText.text = playerScore.ToString(); // Update the score text
    }

    public void setCountdownTimer(int seconds)
    {
        countdownTimer = seconds; // Set the countdown timer
        updateCountdownText(); // Update the countdown text
    }

    private void updateCountdownText()
    {
        countdownText.text = $"Restarting in: {countdownTimer}"; // Update the countdown text in the UI
    }

    public void restartGame()
    {
        if (restartCoroutine != null)
        {
            StopCoroutine(restartCoroutine); // Stop the existing coroutine if running
        }

        Scene currentScene = SceneManager.GetActiveScene(); // Get the current scene
        playerScore = 0; // Reset the player score
        setCountdownTimer(0); // Reset the countdown timer
        AndroidBinding.Instance.SetPlayerNormalScore(playerScore);
        Time.timeScale = 1; // Ensure the game time is resumed

        StartCoroutine(SwitchToScene(currentScene.buildIndex, currentScene.buildIndex)); // Reload the scene
        SetDefaultPipeWidthEvent.Invoke(); // Reset the default pipe width
    }

    public void gameOver()
    {
        gameOverScreen.SetActive(true); // Display the game-over screen
        PauseOnLoss(); // Pause the game

        setCountdownTimer(10); // Set the countdown timer to 10 seconds
        restartCoroutine = StartCoroutine(RestartGameAfterDelay(10)); // Start the countdown coroutine
    }

    private IEnumerator RestartGameAfterDelay(int delaySeconds)
    {
        for (int i = delaySeconds; i > 0; i--)
        {
            setCountdownTimer(i); // Update the countdown timer
            yield return new WaitForSecondsRealtime(1); // Wait for 1 second in real-time
        }

        restartGame(); // Restart the game after the countdown
    }

    public void playGame()
    {
        SceneManager.LoadScene(1); // Load the first level
    }

    public void levelTwo()
    {
        StartCoroutine(SwitchToScene(1, 2)); // Switch to level two
    }

    public void levelThree()
    {
        StartCoroutine(SwitchToScene(2, 3)); // Switch to level three
    }

    public void quitGame()
    {
        Application.Quit(); // Exit the game
    }

    private IEnumerator SwitchToScene(int sceneToUnload, int sceneToLoad)
    {
        yield return SceneManager.UnloadSceneAsync(sceneToUnload); // Unload the current scene
        SceneManager.LoadScene(sceneToLoad); // Load the new scene
    }

    public void PauseOnLoss()
    {
        isPausedGameOver = !isPausedGameOver;

        if (isPausedGameOver)
        {
            Time.timeScale = 0; // Pause the game
        }
        else
        {
            Time.timeScale = 1; // Resume the game
        }
    }
}
