using UnityEngine;
using System.Collections.Generic;
using System;

// References:
// https://docs.unity3d.com/ScriptReference/PlayerPrefs.html
// https://stackoverflow.com/questions/14051257/conversion-from-int-array-to-string-array
public class GameProgress {
    // Property Names
    private string FINISHED_LEVELS = "finished_levels";
    private string WEAPON_LEVELS = "weapon_levels";
    private string TOTAL_SCORE = "total_score";
    private string CREDITS = "credits";
    private int INITIAL_WEAPON_LEVEL = 0;

    // List of finished levels
    private List<int> FinishedLevels = new List<int>();
    // Load level numbers of finished levels
    public void LoadFinishedLevels() {
        FinishedLevels.Clear();
        foreach (string finishedLevelNumber in PlayerPrefs.GetString(FINISHED_LEVELS, "").Split(',')) {
            if (finishedLevelNumber == "") continue;
            FinishedLevels.Add(int.Parse(finishedLevelNumber));
        }
    }
    // Returns boolean indicating whether a level has been finished before
    public bool LevelFinished(int levelNumber) {
        LoadFinishedLevels();
        return FinishedLevels.Contains(levelNumber);
    }
    // Sets a level as finished
    public void SetLevelFinished(int levelNumber) {
        LoadFinishedLevels();
        if (FinishedLevels.Contains(levelNumber)) return;
        // Add a level to list of finished levels and save
        FinishedLevels.Add(levelNumber);
        PlayerPrefs.SetString(FINISHED_LEVELS, string.Join(",", Array.ConvertAll(FinishedLevels.ToArray(), x => x.ToString())));
        PlayerPrefs.Save();
    }

    // Weapon levels
    private Dictionary<int, int> WeaponLevel = new Dictionary<int, int>();
    // Load levels of all weapons
    public void LoadWeaponLevels() {
        WeaponLevel.Clear();
        foreach (string weapon in PlayerPrefs.GetString(WEAPON_LEVELS, "").Trim().Split(',')) {
            if (weapon == "") continue;
            int weaponId = int.Parse(weapon.Split(':')[0]);
            int weaponLevel = int.Parse(weapon.Split(':')[1]);
            WeaponLevel[weaponId] = weaponLevel;
        }
    }
    // Gets level of a specific weapon
    public int GetWeaponLevel(int weaponId) {
        LoadWeaponLevels();
        if (WeaponLevel.ContainsKey(weaponId)) {
            return WeaponLevel[weaponId];
        }
        // Weapon has not been upgraded before, return initial level
        return INITIAL_WEAPON_LEVEL;
    }
    // Increases level of particular weapon
    public void IncreaseWeaponLevel(int weaponId) {
        LoadWeaponLevels();

        // If the weapon is not in the dictionary, it has not been upgraded and should be at initial level
        if (!WeaponLevel.ContainsKey(weaponId)) {
            WeaponLevel[weaponId] = INITIAL_WEAPON_LEVEL;
        }
        // Increase level
        WeaponLevel[weaponId]++;

        // Save WeaponInformation
        int i = 0;
        string[] weaponLevelSaveString = new string[WeaponLevel.Keys.Count];
        foreach (int key in WeaponLevel.Keys) {
            weaponLevelSaveString[i++] = key + ":" + WeaponLevel[key].ToString();
        }
        PlayerPrefs.SetString(WEAPON_LEVELS, string.Join(",", weaponLevelSaveString));
        PlayerPrefs.Save();
    }

    // Total accumulated score
    public int Score {
        get {
            return PlayerPrefs.GetInt(TOTAL_SCORE, 0);
        }
        private set {
            PlayerPrefs.SetInt(TOTAL_SCORE, value);
            PlayerPrefs.Save();
        }
    }
    // Increase score
    public void AddScore(int value) {
        Score += value;
    }

    // Available credits
    public int Credits {
        get {
            return PlayerPrefs.GetInt(CREDITS, 0);
        }
        private set {
            PlayerPrefs.SetInt(CREDITS, value);
            PlayerPrefs.Save();
        }
    }
    // Increase/decrease credits
    public void ChangeCreditStandingBy(int amount) {
        Credits += amount;
    }

    // Reset All Progress
    public void ResetProgress() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }
}
