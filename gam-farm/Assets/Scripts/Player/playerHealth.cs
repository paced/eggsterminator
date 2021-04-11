using UnityEngine;
using System.Collections;

public class playerHealth : MonoBehaviour {
	public Vector2 pos = new Vector2(16, 16);
	public Vector2 size = new Vector2(128, 32);
	public float health;

	/* Add a health instance. */
	public Stat hitpoints;
	public GUIStyle styleBackground;
	public GUIStyle styleForeground;

	void Start() {
		hitpoints = new Stat("Health", 1000, 1000);
	}

    // Whether level finish has been called
    bool FinishLevelCalled = false;

	void Update() {
		/* Display health value as a percentage (max = 1.0f). */
		health = 1.0f * hitpoints.Val / hitpoints.MaxVal;

        /* Player has lost all health */
        if (health <= 0.0f) {
            if (FinishLevelCalled) return;
            FinishLevelCalled = true;
            GameObject.FindGameObjectWithTag("lvlController").GetComponent<LevelController>().FinishLevel(false);
        }

		/* Set textures for GUI styles. */
		styleBackground.normal.background = new Texture2D(1, 1);
		styleForeground.normal.background = new Texture2D(1, 1);

		/* Set them to the right colour and wrap. */
		styleBackground.normal.background.SetPixel(0, 0, Color.red);
		styleForeground.normal.background.SetPixel(0, 0, Color.green);
		styleBackground.normal.background.wrapMode = TextureWrapMode.Repeat;
		styleForeground.normal.background.wrapMode = TextureWrapMode.Repeat;
		styleBackground.normal.background.Apply();
		styleForeground.normal.background.Apply();

	}

	void OnGUI() {
		/* Draw healthbar background. */
		GUI.BeginGroup(new Rect(pos.x, pos.y, size.x, size.y));
		GUI.Label(new Rect(0, 0, size.x, size.y), hitpoints.Name, styleBackground);

		/* Draw actual health in foreground. */
		GUI.BeginGroup(new Rect(0, 0, size.x * health, size.y));
		GUI.Label(new Rect(0, 0, size.x, size.y), hitpoints.Name, styleForeground);
		GUI.EndGroup();

		GUI.EndGroup();
	}
}

