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
    [Tooltip("The GameObject prefab to use for the planets.")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Scene References")]
    [Tooltip("An empty GameObject to parent all the spawned planets and star to.")]
    [SerializeField] private Transform systemParent;

    [Header("Procedural Materials")]
    [Tooltip("e.g., A 'Hot Jupiter' texture")]
    [SerializeField] private Material matGasHot;
    [Tooltip("e.g., A 'Neptune' texture")]
    [SerializeField] private Material matGasCold;
    [Tooltip("e.g., A 'Lava' texture")]
    [SerializeField] private Material matRockyLava;
    [Tooltip("e.g., An 'Earth-like' texture")]
    [SerializeField] private Material matRockyHabitable;
    [Tooltip("e.g., An 'Ice/Europa' texture")]
    [SerializeField] private Material matRockyIce;

    [Header("Planet Logic")]
    [Tooltip("A planet with a radius (in Jupiter radii) larger than this will be a 'Gas Giant'. Neptune's radius is ~0.35")]
    [SerializeField] private float gasGiantThreshold = 0.35f;

    [Header("Data Scaling")]
    [Tooltip("A final multiplier to scale the entire system (planets, star, and distances) up or down.")]
    [SerializeField] private float overallSystemScale = 1.0f;

    [Tooltip("The constant spacing (gap) BETWEEN the edges of the planets.")]
    [SerializeField] private float planetSpacing = 20.0f;

    [Tooltip("How much to multiply the Jupiter radius by (e.g., 1 Jupiter radius = 2 Unity units)")]
    [SerializeField] private float radiusScale = 1.0f;

    // Private Variables
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
        // This variable will track the 'cursor' of where the next planet can start
        float currentXPosition = 0f;

        for (int i = 0; i < planets.Length; i++)
        {
            PlanetData planet = planets[i];

            GameObject planetGO = Instantiate(planetPrefab, Vector3.zero, Quaternion.identity);
            planetGO.name = planet.pl_name;
            planetGO.transform.SetParent(systemParent);


            // Calculate the planet's visual scale and its actual radius in Unity units.
            // (A default Unity sphere has a 0.5 radius at scale 1, so radius is scale / 2)
            float finalScale = (float)planet.pl_radj * radiusScale;
            float currentRadius = finalScale / 2.0f;

            // Calculate the center position
            // The center is the current "cursor" position + its own radius
            float planetCenter = currentXPosition + currentRadius;

            // Set position and scale
            planetGO.transform.position = new Vector3(planetCenter, 0, 0);
            planetGO.transform.localScale = new Vector3(finalScale, finalScale, finalScale);

            // Update the "cursor" for the *next* planet
            // The new position is the center of this planet + its radius + the spacing gap
            currentXPosition = planetCenter + currentRadius + planetSpacing;


            // Apply material
            MeshRenderer renderer = planetGO.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = GetProceduralMaterial(planet);
            }

            // Set the label text
            // Find the TextMeshPro component in the prefab's children
            TextMeshPro label = planetGO.GetComponentInChildren<TextMeshPro>();
            if (label != null)
            {
                label.text = planet.pl_name;
            }

            Debug.Log($"Created {planet.pl_name} at {planetCenter} units.");
        }

        // Apply the final overall scale
        systemParent.localScale = new Vector3(overallSystemScale, overallSystemScale, overallSystemScale);
    }

    Material GetProceduralMaterial(PlanetData planet)
    {
        float radius = (float)planet.pl_radj;
        float temp = (float)planet.pl_eqt;

        if (radius > gasGiantThreshold)
        {
            if (temp > 373) { return matGasHot; }
            else { return matGasCold; }
        }
        else
        {
            if (temp > 373) { return matRockyLava; }
            else if (temp >= 273) { return matRockyHabitable; }
            else { return matRockyIce; }
        }
    }
}
