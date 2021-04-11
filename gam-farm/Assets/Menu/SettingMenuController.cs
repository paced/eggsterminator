using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

// Reference: https://docs.unity3d.com/ScriptReference/QualitySettings.SetQualityLevel.html
public class SettingMenuController : MonoBehaviour {
    // Dropdown for graphic quality settings
    public Dropdown graphicsQualityDropdown;
    // Game progress reset button
    public Button resetButton;

    void Start() {
        // Get graphics quality settings and add them to dropdown
        graphicsQualityDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        foreach (string name in QualitySettings.names) {
            Dropdown.OptionData option = new Dropdown.OptionData {
                text = name
            };
            options.Add(option);
        }
        graphicsQualityDropdown.AddOptions(options);

        // Set selected item of dropdown to current quality
        graphicsQualityDropdown.value = QualitySettings.GetQualityLevel();
    }

    // Update quality to selected quality
    public void ChangeQuality() {
        if (graphicsQualityDropdown.value == -1) return;
        QualitySettings.SetQualityLevel(graphicsQualityDropdown.value);
    }

    // Go back to Main Menu
    public void Back() {
        SceneManager.LoadScene("Menu Scene");
    }

    // Reset game progress and disable button
    public void ResetProgress() {
        GameController.gameProgress.ResetProgress();
        resetButton.interactable = false;
    }
}
