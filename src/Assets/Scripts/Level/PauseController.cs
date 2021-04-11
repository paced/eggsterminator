using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// References
// http://answers.unity3d.com/questions/818450/how-to-visible-on-and-off-canvas-in-new-ui.html
// http://answers.unity3d.com/questions/1100026/canvas-with-mask-and-transparent-image.html
// http://answers.unity3d.com/questions/197948/how-to-speed-up-my-game-to-2x.html

public class PauseController : MonoBehaviour {
    // Level Controller
    LevelController levelController;

    void Start() {
        // Get Level Controller
        levelController = GameObject.FindGameObjectWithTag("lvlController").GetComponent<LevelController>();

        // Add transparent background
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<Canvas>().gameObject.AddComponent<Image>().color = new Color(0, 0, 0, 0.95f);
        this.GetComponent<CanvasGroup>().alpha = 0f;
    }

    void Update() {
        // Pause game when Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape) && LevelController.CurrentLevelState != LevelController.LevelState.PAUSED) {
            Pause();
        }
    }

    // Pause game
    public void Pause() {
        Cursor.visible = true;
        this.GetComponent<CanvasGroup>().interactable = true;
        this.GetComponent<CanvasGroup>().alpha = 1f;
        levelController.Pause();
    }

    // Resume game on button click
    public void Resume() {
        Cursor.visible = false;
        this.GetComponent<CanvasGroup>().interactable = false;
        this.GetComponent<CanvasGroup>().alpha = 0f;
        levelController.Resume();
    }

    // Call level finish and trigger menu scene (when update button is pressed)
    public void Exit() {
        GameController.FinishLevel(levelController.GetLevelNumber(), false, 0, 0);
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu Scene");
    }
}
