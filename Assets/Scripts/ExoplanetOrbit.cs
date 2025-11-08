using UnityEngine;

public class ExoplanetOrbit : MonoBehaviour
{
    // --- Public variables ---
    // These will be set by the ExoplanetDataFetcher script when the planet is spawned.
    public Transform orbitCenter; // The star
    public float orbitalPeriodInDays; // e.g., "3.5" (days)

    [Tooltip("Global speed multiplier for the entire simulation.")]
    [SerializeField] private float orbitSpeedScale = 50f;

    void Update()
    {
        // Safety check: Don't do anything if we don't have a star or orbital period.
        if (orbitCenter == null || orbitalPeriodInDays <= 0)
        {
            return;
        }


        // Calculate the speed.
        // We divide 1 by the period. A shorter period (like 2 days)
        // will result in a FASTER speed than a longer period (like 100 days).
        // This is Kepler's Third Law
        float orbitSpeed = (1.0f / orbitalPeriodInDays) * orbitSpeedScale;

        // Rotate the planet around the star (orbitCenter).
        // We use Vector3.up to orbit along the X-Z plane (a "top-down" view).
        transform.RotateAround(
            orbitCenter.position,  // The point to orbit
            Vector3.up,            // The axis to rotate around
            orbitSpeed * Time.deltaTime // The speed
        );
    }
}
