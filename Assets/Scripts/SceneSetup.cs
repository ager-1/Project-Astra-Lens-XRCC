using UnityEngine;
using UnityEngine.SceneManagement;

// This script goes on the root of your XR_Rig_Prefab.
// It checks the scene name and configures the camera and menu.
public class SceneSetup : MonoBehaviour
{
    [Header("Configuration")]
    public string mainSceneName = "MR_Solar_System";

    [Header("Object Links")]
    public GameObject menuCanvas; // Your 'Menu1' Canvas
    public Camera xrCamera;       // Your 'Main Camera'

    void Awake()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == mainSceneName)
        {
            // --- This is the MAIN MR SCENE ---
            menuCanvas.SetActive(true);
            xrCamera.clearFlags = CameraClearFlags.SolidColor;
            xrCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent for MR
        }
        else
        {
            // --- This is a PLANET VR SCENE ---
            menuCanvas.SetActive(false);
            xrCamera.clearFlags = CameraClearFlags.Skybox;
        }
    }
}