using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        // This makes the label's "forward" direction parallel to the camera's "forward"
        // It ensures the text always faces the user.
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.forward);
        }
    }
}
