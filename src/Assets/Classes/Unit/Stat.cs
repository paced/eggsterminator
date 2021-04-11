public class Stat {
	public string Name {
		get { return name; }
		set { name = value; } 
	}

	private string name;

	private int maxVal;

	public int MaxVal {
		get { return maxVal; }
		set { maxVal = value; }
	}

	private int defaultVal;

	public int DefaultVal {
		get { return defaultVal; }
		set { defaultVal = value; }
	}

	private int val;

	public int Val {
		get { return val; }
		set { val = value; }
	}

	public Stat(string name, int maxVal, int defaultVal) {
		/* Instantiate this abstract class. */
		this.name = name;
		this.maxVal = maxVal;
		this.defaultVal = defaultVal;
		this.val = defaultVal;
	}

	public void modVal(int amount) {
		/* Increment by an integer (can be negative) amount. */
		val += amount;
	}
}
