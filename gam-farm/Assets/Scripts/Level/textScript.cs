using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class textScript : MonoBehaviour {

    private LevelController levelController;
    int TEXT_LEN = 9;
    Text text;
    int scene = 0;
    string[] lines;
    int i;
    int currentLevel;
    bool isOuttro;


	// Use this for initialization
	void Start () {
        i = 0;
        text = GetComponentInChildren<Text>();
        levelController = GameObject.FindGameObjectWithTag("lvlController").GetComponent<LevelController>();
        currentLevel = levelController.GetLevelNumber();
        currentLevelFinished = GameController.gameProgress.LevelFinished(currentLevel);
        if (lines == null) EndCutScene();
    }

    public void LoadDialogue(string filePath, bool isOuttro)
    {
        scene = 0;
        this.isOuttro = isOuttro;
        if (filePath == "") return;
        try
        {
            TextAsset bindata = Resources.Load(filePath) as TextAsset;
            //Debug.Log(bindata.text);
            lines = bindata.text.Split('\n');
            TEXT_LEN = lines.Length;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    
    // Whether current level has been finished
    private bool currentLevelFinished;
    // Update is called once per frame
    void Update()
    {
        if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED && !LevelController.DialogActive) return;

        // If current level has been completed before, pressing enter will skip dialogue completely
        if (currentLevelFinished && Input.GetKeyDown(KeyCode.Return))
        {
            EndCutScene();
        }

        if (Input.GetKeyDown(KeyCode.Space) && scene < TEXT_LEN)
        {
            if (i < lines[scene].Length) {
                i = lines[scene].Length;
                text.text = lines[scene];
                return;
            }

            scene++;
            i = 0;
            text.text = "";
            if (scene >= TEXT_LEN - 1)
            {
                EndCutScene();
                return;
            }
        }
        displayText(lines[scene]);
    }

    void displayText(string line)
    {
        if (i < line.Length)
        {
            text.text = text.text + line[i];
            i++;
        }
    }

    private void EndCutScene()
    {
        this.gameObject.SetActive(false);
        levelController.EndCutScene(isOuttro);
    }
}
