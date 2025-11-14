using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

// Classes MUST match the JSON data
// System Serialize so Unity's JSON utility can read them

[System.Serializable]
public class PlanetData
{
    // These variable names MUST match the API's JSON keys exactly.
    // (e.g., "pl_name", "pl_orbsmax")
    public string pl_name;
    public double pl_radj;
    public double pl_bmassj;
    public double pl_eqt;
    public string pl_pubdate;
}

// We will manually add "{\"items\": ... }" to the JSON string.
[System.Serializable]
public class PlanetList
{
    public PlanetData[] items;
}

public class ExoplanetDataFetcher : MonoBehaviour
{

    [Header("Prefabs")]
    [Tooltip("The GameObject prefab to use for the star.")]
    [SerializeField] private GameObject starPrefab;
    [Tooltip("The GameObject prefab to use for the planets.")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Scene References")]
    [Tooltip("An empty GameObject to parent all the spawned planets and star to.")]
    [SerializeField] private Transform systemParent;

    [Header("Planet Materials")]
    [Tooltip("Material for TRAPPIST-1 b")]
    [SerializeField] private Material mat_Trappist_b;
    [Tooltip("Material for TRAPPIST-1 c")]
    [SerializeField] private Material mat_Trappist_c;
    [Tooltip("Material for TRAPPIST-1 d")]
    [SerializeField] private Material mat_Trappist_d;
    [Tooltip("Material for TRAPPIST-1 e")]
    [SerializeField] private Material mat_Trappist_e;
    [Tooltip("Material for TRAPPIST-1 f")]
    [SerializeField] private Material mat_Trappist_f;
    [Tooltip("Material for TRAPPIST-1 g")]
    [SerializeField] private Material mat_Trappist_g;
    [Tooltip("Material for TRAPPIST-1 h")]
    [SerializeField] private Material mat_Trappist_h;

    [Header("Planet Logic")]
    [Tooltip("A planet with a radius (in Jupiter radii) larger than this will be a 'Gas Giant'. Neptune's radius is ~0.35")]
    [SerializeField] private float gasGiantThreshold = 0.35f;

    [Header("Data Scaling")]
    [Tooltip("A final multiplier to scale the entire system (planets, star, and distances) up or down.")]
    [SerializeField] private float overallSystemScale = 1.0f;

    [Tooltip("The constant spacing *between* orbital rings.")]
    [SerializeField] private float orbitalSpacing = 100.0f;

    [Tooltip("How much to multiply the Jupiter radius by (e.g., 1 Jupiter radius = 2 Unity units)")]
    [SerializeField] private float radiusScale = 1.0f;

    // Private Variables
    private Transform starTransform;

    private string apiUrl = "https://exoplanetarchive.ipac.caltech.edu/TAP/sync?query=select+pl_name,pl_radj,pl_bmassj,pl_eqt,pl_pubdate+from+ps+where+hostname='TRAPPIST-1'+and+(pl_name='TRAPPIST-1 h'+or+pl_name='TRAPPIST-1 b'+or+pl_name='TRAPPIST-1 f'+or+pl_name='TRAPPIST-1 e'+or+pl_name='TRAPPIST-1 g'+or+pl_name='TRAPPIST-1 c'+or+pl_name='TRAPPIST-1 d')+and+pl_radj+is+not+null+and+pl_eqt+is+not+null&format=json";

    void Start()
    {
        StartCoroutine(GetExoplanetData());
    }

    IEnumerator GetExoplanetData()
    {
        Debug.Log("Fetching exoplanet data...");

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + request.error);
            }
            else
            {
                Debug.Log("Success! Received data.");
                string jsonResponse = request.downloadHandler.text;
                ParseAndGenerateSystem(jsonResponse);
            }
        }
    }

    void ParseAndGenerateSystem(string jsonData)
    {
        string wrappedJson = "{\"items\":" + jsonData + "}";
        PlanetList planetList = JsonUtility.FromJson<PlanetList>(wrappedJson);

        if (planetList == null || planetList.items == null)
        {
            Debug.LogError("Could not parse JSON data.");
            return;
        }

        // Get the latest, unique data for each planet
        Dictionary<string, PlanetData> planetMap = planetList.items
            .GroupBy(p => p.pl_name)
            .ToDictionary(
                g => g.Key,
                g => g.OrderByDescending(p => p.pl_pubdate).First()
            );

        // Get all the planets from the map and sort them by name
        PlanetData[] sortedPlanets = planetMap.Values
            .OrderBy(p => p.pl_name)
            .ToArray();


        Debug.Log($"--- Total planets received: {planetList.items.Length}. Displaying: {sortedPlanets.Length} (sorted alphabetically) ---");

        // Pass the new, sorted list to the generator.
        GenerateStarSystem(sortedPlanets);
    }

    void GenerateStarSystem(PlanetData[] planets)
    {
        // 1. Create the Star
        GameObject star = Instantiate(starPrefab, Vector3.zero, Quaternion.identity);
        star.name = "TRAPPIST-1";
        star.transform.SetParent(systemParent);
        // Store the star's transform so planets can find it
        this.starTransform = star.transform;

        // 2. Loop through and create each planet
        for (int i = 0; i < planets.Length; i++)
        {
            PlanetData planet = planets[i];

            GameObject planetGO = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
            planetGO.name = planet.pl_name;
            planetGO.transform.SetParent(systemParent);

            // 1. Calculate the orbital radius (this is the same as before)
            float orbitalRadius = (i + 1) * orbitalSpacing;

            // 2. Get a random angle in radians
            float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;

            // 3. Calculate the X and Z positions on the circle
            float xPos = orbitalRadius * Mathf.Cos(randomAngle);
            float zPos = orbitalRadius * Mathf.Sin(randomAngle);

            // 4. Set the planet's starting position
            planetGO.transform.position = new Vector3(xPos, 0, zPos);

            // --- Set Scale ---
            float finalScale = (float)planet.pl_radj * radiusScale;
            planetGO.transform.localScale = new Vector3(finalScale, finalScale, finalScale);

            // --- Set Material ---
            MeshRenderer renderer = planetGO.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                // Call our new function to get the right material
                renderer.material = GetMaterialForPlanet(planet.pl_name);
            }

            // --- Set Label ---
            TextMeshPro label = planetGO.GetComponentInChildren<TextMeshPro>();
            if (label != null)
            {
                label.text = planet.pl_name;
            }

            // --- >> NEW: Set up the orbit ---
            PlanetOrbit orbitScript = planetGO.GetComponent<PlanetOrbit>();
            if (orbitScript != null)
            {
                // Tell the planet what to orbit
                orbitScript.orbitCenter = this.starTransform;
            }

            Debug.Log($"Created {planet.pl_name} at {orbitalRadius} units.");
        }

        // 3. Apply the final overall scale
        systemParent.localScale = new Vector3(overallSystemScale, overallSystemScale, overallSystemScale);
    }

    Material GetMaterialForPlanet(string planetName)
    {
        switch (planetName)
        {
            case "TRAPPIST-1 b":
                return mat_Trappist_b;
            case "TRAPPIST-1 c":
                return mat_Trappist_c;
            case "TRAPPIST-1 d":
                return mat_Trappist_d;
            case "TRAPPIST-1 e":
                return mat_Trappist_e;
            case "TRAPPIST-1 f":
                return mat_Trappist_f;
            case "TRAPPIST-1 g":
                return mat_Trappist_g;
            case "TRAPPIST-1 h":
                return mat_Trappist_h;
            default:
                Debug.LogWarning($"No material specified for planet: {planetName}.");
                return null; // Or return a default material
        }
    }
}
