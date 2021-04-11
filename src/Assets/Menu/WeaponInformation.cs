using System;

[Serializable]
class WeaponInformation {
    // Constructor for WeaponInformation
    public WeaponInformation(int weaponID, string weaponName, string level1, int level1Cost, string level2, int level2Cost, string level3, int level3Cost) {
        this.WeaponID = weaponID;
        this.WeaponName = weaponName;
        this.Level1 = level1;
        this.Level1Cost = level1Cost;
        this.Level2 = level2;
        this.Level2Cost = level2Cost;
        this.Level3 = level3;
        this.Level3Cost = level3Cost;
    }

    // Internal ID of weapon
    public int WeaponID;
    // Name of weapon
    public string WeaponName;

    // Name of level 1 upgrade
    public string Level1;
    // Level 1 upgrade cost
    public int Level1Cost;
    // Name of level 2 upgrade
    public string Level2;
    // Level 2 upgrade cost
    public int Level2Cost;
    // Name of level 3 upgrade
    public string Level3;
    // Level 1 upgrade cost
    public int Level3Cost;
}
