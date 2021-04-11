using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelEndCanvasController : MonoBehaviour {
    // Text objects
    public Text completionMessageText;
    public Text completionTimeText;
    public Text scoreText;
    public Text creditsEarnedText;

    void Start() {
        // Make canvas visible and add transparent background
        GetComponent<CanvasGroup>().alpha = 1f;
        GetComponent<Canvas>().gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.95f);
    }

    // Updates information which is displayed on the canvas
    public void UpdateInfo(int levelNumber, string levelName, bool success, int score, int completionTime) {
        Cursor.visible = true;
        
        // Update completion message
        string completionMessage;
        completionMessage = levelName ?? "Level " + levelNumber.ToString();
        if (success) {
            completionMessageText.text = completionMessage + " Complete";
        } else {
            completionMessageText.text = completionMessage + " Failed";
        }

        // Update text of time
        // Reference: https://stackoverflow.com/questions/894461/how-can-i-convert-int-90-minutes-to-datetime-130
        completionTimeText.text = new DateTime(TimeSpan.FromSeconds(completionTime).Ticks).ToString("mm:ss");

        // Update text of credits/score
        creditsEarnedText.text = GameController.ScoreToCredits(success, score).ToString();
        scoreText.text = score.ToString();
    }

    // Load upgrades scene
    public void LoadUpgrades() {
        Time.timeScale = 1;
        SceneManager.LoadScene("Upgrade");
    }
    // Load level selection scene
    public void ReturnToMenu() {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelector");
    }
}
