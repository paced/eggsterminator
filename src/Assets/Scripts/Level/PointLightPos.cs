using UnityEngine;

/**
 * Gets the light source color for terrain
 * updates terrain with location of light source
 */
public class PointLightPos : MonoBehaviour {
    // Terrain
    Terrain terrain;

	// Use this for initialization
	void Start () {
        // get Terrain and light source colour
        terrain = GetComponent<Terrain>();
        terrain.materialTemplate.SetColor("_PointLightColor", GameObject.FindGameObjectWithTag("Sun").GetComponent<Sun>().sunColor);
    }

    // Update is called once per frame
    void Update () {
        // Update light source position
        terrain.materialTemplate.SetVector("_PointLightPosition", GameObject.FindGameObjectWithTag("Sun").transform.localPosition);
    }
}
