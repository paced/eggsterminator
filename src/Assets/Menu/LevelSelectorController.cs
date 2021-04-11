using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// References:
// https://docs.unity3d.com/Manual/LoadingResourcesatRuntime.html
// https://docs.unity3d.com/Manual/JSONSerialization.html
// https://stackoverflow.com/questions/3309188/how-to-sort-a-listt-by-a-property-in-the-object
// http://answers.unity3d.com/questions/829335/46-adding-onclicklistener-to-instatiated-buttons-o.html
public class LevelSelectorController : MonoBehaviour {
    // Button prefab
    public GameObject buttonPrefab;
    // Maximum number of level buttons per row
    public int levelsPerRow = 2;
    // Gap between buttons
    public int gap = 5;

    // The heading (used to set location of heading)
    public Text levelSelectionHeading;

    // Level Information
    System.Collections.Generic.List<LevelInformation> levels = new System.Collections.Generic.List<LevelInformation>();

    void Start() {
        // Load levels and their information into a list of LevelInformation
        foreach (UnityEngine.Object assetObject in Resources.LoadAll("Levels")) {
            TextAsset levelFile = assetObject as TextAsset;
            levels.Add(JsonUtility.FromJson<LevelInformation>(levelFile.text));
        }
        // Sort LevelInformation using level numbers
        levels.Sort((x, y) => x.LevelNumber.CompareTo(y.LevelNumber));

        // Find button prefab width/height
        Vector2 dimensions = buttonPrefab.GetComponent<RectTransform>().sizeDelta;
        
        // Find number of rows/columns
        int col = levelsPerRow;
        int row = (int)Math.Ceiling((float)levels.Count / col);

        // Find length/width of button area
        int totalLength = col * (int)dimensions.x + gap * (col - 1);
        int totalWidth = row * (int)dimensions.y + gap * (row - 1);
        // Find top left corner coordinates
        float topLeftXOffset = totalLength / 2;
        float topLeftYOffset = totalWidth / 2 - levelSelectionHeading.GetComponent<RectTransform>().sizeDelta.y * 3 / 4;

        // Set location of heading
        levelSelectionHeading.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, (float)(topLeftYOffset + dimensions.y * 0.5));

        // Initalise Level 'Buttons' and set their locations
        int current_row = 0;
        int current_col = 0;
        foreach (LevelInformation level in levels) {
            // Instatiate button from Prefab
            GameObject levelObject = Instantiate(buttonPrefab);

            // Set position of button
            levelObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            float xLoc = -topLeftXOffset + current_col * dimensions.x + (current_col > 0 ? current_col * gap : 0);
            float yLoc = -topLeftYOffset + current_row * dimensions.y + (current_row > 0 ? current_row * gap : 0);
            levelObject.GetComponent<RectTransform>().pivot = new Vector2(0, 1);
            levelObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(xLoc, -yLoc);

            // Attach method to button click
            Button btn = levelObject.GetComponent<Button>();
            btn.onClick.AddListener(() => SelectLevel(level.LevelNumber));

            // Set Text
            if (level.LevelName != null) {
                // Name for level exists, so use it
                levelObject.GetComponentInChildren<Text>().text = level.LevelName;
            } else {
                // Name for level does not exist, so use its number
                levelObject.GetComponentInChildren<Text>().text = level.LevelNumber.ToString();
            }

            // If previous level has not been finished and level is not an initially unlocked level, disable button
            if (!GameController.gameProgress.LevelFinished(level.LevelNumber - 1) && level.LevelNumber > 1) {
                btn.interactable = false;
            }

            // Increment counters to keep track of grid location
            current_col++;
            if (current_col == levelsPerRow) {
                current_col = 0;
                current_row++;
            }
        }
    }

    // Handles selection of levels
    public void SelectLevel(int levelNumber) {
        // Set Level in GameController
        foreach (LevelInformation level in levels) {
            if (level.LevelNumber == levelNumber) {
                GameController.QueuedLevel = level;
            }
        }
        // Switch to Main Scene
        SceneManager.LoadScene("MainScene");
    }

    // Go back to menu
    public void Back() {
        SceneManager.LoadScene("Menu Scene");
    }
}
