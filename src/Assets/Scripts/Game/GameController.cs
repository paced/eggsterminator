using System;

public static class GameController {
    // Game Progress (persistent information that is shared across sessions)
    public static GameProgress gameProgress = new GameProgress();

    // State of Game
    private enum GameState { InMenu, WaitingForLevelInit, InGame }
    private static GameState state = GameState.InMenu;

    // Level information of next queued level
    private static LevelInformation _QueuedLevel;
    public static LevelInformation QueuedLevel {
        get {
            // If state is not WaitingForLevelInit, we're entering MainScene directly (most likely
            // through Unity), so give a long level for testing purposes
            if (state != GameState.WaitingForLevelInit) {
                _QueuedLevel = new LevelInformation(100, 1000, 100);
                // throw new Exception("Error: Game State Incorrect.");
            }
            // Since the level information is retrieved by LevelController, we're now InGame
            state = GameState.InGame;
            return _QueuedLevel;
        }
        set {
            // Next level is set by LevelSelectorController
            _QueuedLevel = value;
            state = GameState.WaitingForLevelInit;
        }
    }

    // Returns filepath of intro dialogue for a level
    public static string GetIntroDialogue(int levelNumber) {
        switch (levelNumber) {
            case 0: return "Dialogue/Tute";
            case 1: return "Dialogue/Intro1";
            default: return null;
        }
    }

    // Returns filpath of outtro dialogue for a level
    public static string GetOuttroDialogue(int levelNumber) {
        switch (levelNumber) {
            case 0: return "Dialogue/OuttroTute";
            // case 1: return "Dialogue/Outtro1";
            default: return "";
        }
    }

    // Called when level has finished
    public static void FinishLevel(int levelNumber, bool success, int score, int totalTime) {
        if (state != GameState.InGame) {
            throw new Exception("Error: Game State Incorrect.");
        }

        // Set level as finished if applicable and update score/credit standing
        if (success) gameProgress.SetLevelFinished(levelNumber);
        gameProgress.AddScore(score);
        gameProgress.ChangeCreditStandingBy(ScoreToCredits(success, score));

        // Set state
        state = GameState.InMenu;
    }

    // Converts score to credits
    public static int ScoreToCredits(bool success, int score) {
        // Calculation depends on whether the level has been successfuly completed
        return success ? score / 100 : score / 1000;
    }
}
