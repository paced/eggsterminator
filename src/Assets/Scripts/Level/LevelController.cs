using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
	// The dialogue box
	private GameObject dialogueBox;
	private GameObject HUDCanvas;
	private GameObject levelEndCanvas;
	private GameObject pauseCanvas;

	// The player
	private GameObject player;

	// The state of the level
	public enum LevelState {
		NORMAL,
		WAVE,
		PAUSED
	}

	public static LevelState CurrentLevelState {
		get;
		private set;
	}

	// LevelInformation of current level
	private LevelInformation levelInformation;
	// Returns level number
	public int GetLevelNumber() {
		return levelInformation.LevelNumber;
	}

	// Total elapsed time
	public int TotalTime {
		get;
		private set;
	}

	public void IncrementTotalTime() {
		TotalTime++;
	}

	// The internally used spawn time
	// Note that spawn time is paused when a wave comes
	public int SpawnTime {
		get;
		private set;
	}

	private void IncrementSpawnTime() {
		SpawnTime++;
	}

	// The last spawn time
	private int LastSpawnTime {
		get;
		set;
	}

	// Score
	public int LevelScore {
		get;
		private set;
	}

	public void IncreaseScore(int score) {
		LevelScore += score;
	}

	// The number of onscreen enemies
	public int NumberOfOnScreenEnemies {
		get;
		private set;
	}

	// The number of remaining enemies
	public int RemainingEnemies {
		get;
		private set;
	}

	// This method is called when an enemy is killed
	public void EnemyKilled() {
		// Reduce enemy counts and add score
		RemainingEnemies--;
		NumberOfOnScreenEnemies--;
		LevelScore += 100;

		// Level has finished, trigger finish screen
		if (RemainingEnemies == 0) {
			// TODO @Albangalo
			string dialogLocation = GameController.GetOuttroDialogue(levelInformation.LevelNumber);
			if (dialogLocation == "") {
				FinishLevel(true);
				return;
			}
			dialogueBox.GetComponent<textScript>().LoadDialogue(GameController.GetOuttroDialogue(levelInformation.LevelNumber), true);
			HUDCanvas.SetActive(false);
			dialogueBox.SetActive(true);
			//WinLevel();
			return;
		}

		// Finish wave if necessary
		if (NumberOfOnScreenEnemies == 0 && CurrentLevelState == LevelState.WAVE) {
			CurrentLevelState = LevelState.NORMAL;
		}
	}

	// Show level completion canvas
	public void FinishLevel(bool completionStatus) {
		// Pause game
		Pause();
		pauseCanvas.SetActive(false);

		// Save progress
		GameController.FinishLevel(GetLevelNumber(), completionStatus, LevelScore, TotalTime);

		// Show levelEndCanvas
		levelEndCanvas.GetComponent<CanvasGroup>().interactable = true;
		levelEndCanvas.GetComponent<LevelEndCanvasController>().UpdateInfo(levelInformation.LevelNumber, levelInformation.LevelName, completionStatus, LevelScore, TotalTime);
		levelEndCanvas.SetActive(true);
	}

	// When enemies spawn and the amount of enemies that spawn at that time
	private Dictionary<int, int> SpawnInformation {
		get;
		set;
	}
	// Spawn Time of waves
	private List<int> WaveTimes;
	// Add enemies to a time
	private void AddSpawn(int time, int amount) {
		RemainingEnemies += amount;
		if (!SpawnInformation.ContainsKey(time))
			SpawnInformation[time] = 0;
		SpawnInformation[time] += amount;
	}

	// Parses normal spawn information from LevelInformation to a dictionary of spawn time/amount
	private void AddNormalSpawns(int normalEnemiesCount, int levelEndTime) {
		for (int time = 0; time < levelEndTime; time++) {
			float normalisedTime = (float)time / (float)levelEndTime;
			// TODO: fix normal enemies count acting as a multiplier
			float count = Mathf.Log(normalisedTime + 1) * normalEnemiesCount;
			AddSpawn(time, (int)Mathf.Ceil(count));
		}
	}

	// Parses wave information from LevelInformation and adds wave spawns
	private void AddWaveSpawns(string waveInformation) {
		if (waveInformation == null)
			return;
		foreach (string wave in waveInformation.Split(',')) {
			if (wave == "")
				continue;
			int spawnTime = int.Parse(wave.Split(':')[0]);
			int spawnAmount = int.Parse(wave.Split(':')[1]);
			AddSpawn(spawnTime, spawnAmount);
			WaveTimes.Add(spawnTime);
		}
	}

	// Use this for initialization
	void Start() {
		// Set initial attributes
		TotalTime = 0;
		LevelScore = 0;
		SpawnTime = -1;
		RemainingEnemies = 0;
		Timer = MaxTime;

		// Set state
		CurrentLevelState = LevelState.NORMAL;

		// Get and store level information
		LevelInformation levelInformation = GameController.QueuedLevel;
		this.levelInformation = levelInformation;
		LastSpawnTime = levelInformation.LevelLength;

		// Determine enemies and their spawn times
		SpawnInformation = new Dictionary<int, int>();
		WaveTimes = new List<int>();
		AddWaveSpawns(levelInformation.Wave);
		AddNormalSpawns(levelInformation.NormalEnemyCount, levelInformation.LevelLength);

		// Get all canvas
		dialogueBox = GameObject.FindGameObjectWithTag("DialogueBox");
		HUDCanvas = GameObject.FindGameObjectWithTag("HUD");
		levelEndCanvas = GameObject.FindGameObjectWithTag("LevelEndCanvas");
		pauseCanvas = GameObject.FindGameObjectWithTag("PauseCanvas");
		levelEndCanvas.SetActive(false);
		dialogueBox.SetActive(false);
		Cursor.visible = false;

		// Should be a switch case for each cut scene, so as to load text into dialogue box
		// The dialogue box will deactivate itself and then tell LevelController to activate HUD
		if (GameController.GetIntroDialogue(levelInformation.LevelNumber) != null) {
			Pause();
			Time.timeScale = 1;
			DialogActive = true;
			dialogueBox.GetComponent<textScript>().LoadDialogue(GameController.GetIntroDialogue(levelInformation.LevelNumber), false);
			HUDCanvas.SetActive(false);
			dialogueBox.SetActive(true);
		}

		Debug.Log("Total enemies: " + RemainingEnemies);
		player = GameObject.FindGameObjectWithTag("Player");
	}

	// Shows the HUD when dialogue finishes, or shows level complete screen if level won
	public void EndCutScene(bool isLevelEnd) {
		if (!isLevelEnd) {
			Resume();
			dialogueBox.SetActive(false);
			HUDCanvas.SetActive(true);
		} else
			FinishLevel(true);
	}

	// Whether the dialog is active
	public static bool DialogActive {
		get;
		private set;
	}

	// The level state before pausing
	private LevelState previousLevelState;
	// Pause level
	public void Pause() {
		DialogActive = false;
		Time.timeScale = 0;
		GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().Pause();
		if (CurrentLevelState == LevelState.PAUSED)
			return;
		previousLevelState = CurrentLevelState;
		CurrentLevelState = LevelState.PAUSED;
	}
	// Resume level
	public void Resume() {
		GameObject.FindGameObjectWithTag("Player").GetComponent<AudioSource>().UnPause();
		Time.timeScale = 1;
		CurrentLevelState = previousLevelState;
	}

	public float MaxTime = 1;
	private float Timer;
	// Update is called once per frame
	void Update() {
		// Do not execute when game is paused
		if (LevelController.CurrentLevelState == LevelController.LevelState.PAUSED)
			return;

		Timer -= Time.deltaTime;
		if (Timer < 0f) {
			Timer += MaxTime;

			// Increment total time
			IncrementTotalTime();

			// In wave, do not take further actions
			if (CurrentLevelState == LevelState.WAVE) {
				return;
			}
            
			// Level has ended, waiting for last enemies to be defeated
			if (SpawnTime > LastSpawnTime) {
				return;
			}

			// Increment spawn time and spawn enemies
			IncrementSpawnTime();
			int enemiesToSpawn = 0;
			if (SpawnInformation.ContainsKey(SpawnTime))
				enemiesToSpawn = SpawnInformation[SpawnTime];
			NumberOfOnScreenEnemies += enemiesToSpawn;
			this.GetComponent<InvestorSpawnerScript>().Spawn(enemiesToSpawn);

			// Into wave status if wave triggered
			if (WaveTimes.Contains(SpawnTime)) {

				player.GetComponent<weaponScript>().importantText.text = "NEW WAVE SPAWNED!";
				StartCoroutine(player.GetComponent<weaponScript>().importantFadeout(1500));
				CurrentLevelState = LevelState.WAVE;
			}
		}
	}
}
