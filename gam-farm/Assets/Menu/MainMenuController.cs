using UnityEngine;
using UnityEngine.SceneManagement;

// Controller for Main Menu
public class MainMenuController : MonoBehaviour {
    // Opens Level Selector
    public void PlayGame() {
        SceneManager.LoadScene("LevelSelector");
    }

    // Opens Settings Scene
    public void OpenSettings() {
        SceneManager.LoadScene("Settings");
    }

    // Open Upgrades Scene
    public void OpenUpgrades() {
        SceneManager.LoadScene("Upgrade");
    }

    // Exit Game
    public void ExitGame() {
        Application.Quit();
    }
}
