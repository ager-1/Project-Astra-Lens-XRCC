using UnityEngine;
using UnityEngine.SceneManagement; // We need this to read the scene name

public class MenuVisibilityManager : MonoBehaviour
{
    // Set this in the Inspector to your main scene's name
    public string mainSceneName = "MR_Solar_System";

    // This function runs every time a new scene finishes loading
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Check if the new scene's name is our main scene
        if (scene.name == mainSceneName)
        {
            // If it is, show this GameObject (the menu)
            gameObject.SetActive(true);
        }
        else
        {
            // If it's *any other scene*, hide this GameObject
            gameObject.SetActive(false);
        }
    }

    // Subscribe to the event when this object is enabled
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Unsubscribe when it's disabled
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This handles the very first scene you load
    void Start()
    {
        // Check the scene we're in right now, just in case
        if (SceneManager.GetActiveScene().name != mainSceneName)
            
        {
            gameObject.SetActive(false);
        }
    }
}