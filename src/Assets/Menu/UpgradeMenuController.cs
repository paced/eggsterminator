using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeMenuController : MonoBehaviour {
    // List of weapon information
    private List<WeaponInformation> weaponInformation;

    // Index of weapon that is currently being displayed
    private int displayedWeaponIndex = 0;

    // List of text objects which need to be modified
    public Text weaponName;
    public Text currentLevel;
    public Text nextUpgrade;
    public Text upgradeCost;
    public Text funds;

    // Left/Right navigation arrows
    public Button leftArrow;
    public Button rightArrow;

    // Upgrade button
    public Button upgradeButton;

    // Use this for initialization
    void Start() {
        // Get weapon JSON files and load their information into a list of WeaponInformation
        weaponInformation = new List<WeaponInformation>();
        foreach (UnityEngine.Object assetObject in Resources.LoadAll("Weapons")) {
            TextAsset weaponFile = assetObject as TextAsset;
            weaponInformation.Add(JsonUtility.FromJson<WeaponInformation>(weaponFile.text));
        }
        // Sort WeaponInformation using WeaponID
        weaponInformation.Sort((x, y) => x.WeaponID.CompareTo(y.WeaponID));

        // Load first weapon and refresh UI
        LoadWeapon(0);
        RefreshFunds();

        // Add listeners to buttons
        leftArrow.onClick.AddListener(LoadPrevWeapon);
        rightArrow.onClick.AddListener(LoadNextWeapon);
        upgradeButton.onClick.AddListener(Upgrade);
    }

    // Load the previous weapon
    void LoadPrevWeapon() {
        LoadWeapon(--displayedWeaponIndex);
    }
    // Load the next weapon
    void LoadNextWeapon() {
        LoadWeapon(++displayedWeaponIndex);
    }

    // Upgrade the currently displayed weapon
    void Upgrade() {
        // Get weapon ID and upgrade cost
        int weaponID = weaponInformation[displayedWeaponIndex].WeaponID;

        // There are no more upgrades, do not upgrade...
        if (NextLevel(weaponID) >= 4) return;

        // Reduce credits and perform upgrade
        int upgradeCost = GetUpgradeCost(weaponID, NextLevel(weaponID));
        if (GameController.gameProgress.Credits >= upgradeCost) {
            GameController.gameProgress.ChangeCreditStandingBy(-upgradeCost);
            GameController.gameProgress.IncreaseWeaponLevel(weaponID);
        }

        // Refresh UI
        LoadWeapon(displayedWeaponIndex);
        RefreshFunds();
    }

    // Update funds text
    void RefreshFunds() {
        funds.text = "Available Funds: " + GameController.gameProgress.Credits.ToString();
    }

    // Load weapon at specified index of weaponInformation
    void LoadWeapon(int weaponIndex) {
        // Reset upgrade button
        upgradeButton.interactable = true;
        upgradeButton.GetComponentInChildren<Text>().text = "Upgrade";

        // Disable left/right arrows where applicable
        leftArrow.interactable = !(weaponIndex == 0);
        rightArrow.interactable = !(weaponIndex == weaponInformation.Count - 1);

        // Get WeaponInformation at index weaponIndex
        WeaponInformation currentWeaponInformation = weaponInformation[weaponIndex];
        weaponName.text = currentWeaponInformation.WeaponName;

        // Display current level
        int level = GameController.gameProgress.GetWeaponLevel(currentWeaponInformation.WeaponID);
        switch (level) {
            case 0:
                currentLevel.text = "Initial";
                break;
            case 3:
                // At level 3, no more upgrades can be performed
                currentLevel.text = "Max Level";
                upgradeButton.interactable = false;
                break;
            default:
                currentLevel.text = level.ToString();
                break;
        }

        // Load and display information about next upgrade/level
        int nextLevel = NextLevel(currentWeaponInformation.WeaponID);
        nextUpgrade.text = GetUpgradeInformation(currentWeaponInformation.WeaponID, nextLevel);
        int nextUpgradeCost = GetUpgradeCost(currentWeaponInformation.WeaponID, nextLevel);
        upgradeCost.text = nextUpgradeCost.ToString();
        if (nextLevel == 4) upgradeCost.text = "N/A";

        // Disable upgrade button when upgrade cannot be performed (due to insufficient funds)
        if (GameController.gameProgress.Credits < nextUpgradeCost) {
            upgradeButton.GetComponentInChildren<Text>().text = "Insufficient Funds";
            upgradeButton.interactable = false;
        }
    }

    // Get information (name) of next upgrade
    string GetUpgradeInformation(int weaponID, int level) {
        WeaponInformation weaponInfo = weaponInformation.Find(x => x.WeaponID == weaponID);
        switch (level) {
            case 1:
                return weaponInfo.Level1;
            case 2:
                return weaponInfo.Level2;
            case 3:
                return weaponInfo.Level3;
            case 4:
                return "N/A";
        }
        return null;
    }

    // Get upgrade cost of next upgrade
    int GetUpgradeCost(int weaponID, int level) {
        WeaponInformation weaponInfo = weaponInformation.Find(x => x.WeaponID == weaponID);
        switch (level) {
            case 1:
                return weaponInfo.Level1Cost;
            case 2:
                return weaponInfo.Level2Cost;
            case 3:
                return weaponInfo.Level3Cost;
        }
        return 0;
    }

    // Get the next level of the weapon
    int NextLevel(int weaponID) {
        return GameController.gameProgress.GetWeaponLevel(weaponID) + 1;
    }

    // Return to main menu
    public void MainMenu() {
        SceneManager.LoadScene("Menu Scene");
    }
}
