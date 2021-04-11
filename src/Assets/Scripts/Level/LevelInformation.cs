using System;

[Serializable]
public class LevelInformation {
    // Constructor for LevelInformation
    public LevelInformation(int levelNumber, int levelLength, int normalEnemyCount) {
        this.LevelNumber = levelNumber;
        this.LevelLength = levelLength;
        this.NormalEnemyCount = normalEnemyCount;
    }

    // Level Number
    public int LevelNumber;

    // Level Name
    public string LevelName;

    // Length of level (in seconds)
    public int LevelLength;

    // Number of enemies
    public int NormalEnemyCount;

    // Wave Information
    public string Wave;
}
