using UnityEngine;

public class PlanetOrbit : MonoBehaviour
{
    public Transform orbitCenter;

    [Tooltip("Control the speed of all planets.")]
    [SerializeField] private float orbitSpeed = 10f;

    void Update()
    {
        // Safety check
        if (orbitCenter == null)
        {
            return;
        }

        // Rotate the planet around the orbitCenter (the star)
        transform.RotateAround(
            orbitCenter.position,  // The point to orbit
            Vector3.up,            // The axis to rotate around
            orbitSpeed * Time.deltaTime // The speed
        );
    }
}
