using UnityEngine;

/// <summary>
/// Continuously rotates the GameObject this script is attached to around its Y-axis.
/// </summary>
public class SimpleRotate : MonoBehaviour
{
    // The speed of the rotation in degrees per second.
    // You can change this value in the Unity Inspector.
    [Tooltip("The speed of rotation in degrees per second.")]
    public float rotationSpeed = 30.0f;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        // Get the axis to rotate around. Vector3.up is the Y-axis (0, 1, 0).
        Vector3 axis = Vector3.forward;

        // Calculate the amount to rotate this frame.
        // We multiply by Time.deltaTime to make the rotation smooth and 
        // independent of the frame rate (so it rotates at degrees per second, 
        // not degrees per frame).
        float degreesToRotate = rotationSpeed * Time.deltaTime;

        // Apply the rotation to the GameObject's transform.
        // We use Space.Self to rotate around the object's own local Y-axis.
        // Change this to Space.World to rotate around the world's Y-axis.
        transform.Rotate(axis, degreesToRotate, Space.Self);
    }
}