using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // 1. We need this namespace

public class PlanetSwitcher : MonoBehaviour
{
    [Header("Scene To Load")]
    // 2. Set this in the Inspector to your main scene's name
    public string solarSystemSceneName = "MR_Solar_System";

    [Header("Controller Input")]
    // 3. Link your controller button action here
    public InputActionReference backButtonAction;

    // We need to make sure the action is enabled
    private void OnEnable()
    {
        if (backButtonAction != null)
        {
            backButtonAction.action.Enable();
        }
    }

    // 4. Update is called every frame
    private void Update()
    {
        // 5. Check if the button was pressed down this frame
        if (backButtonAction != null && backButtonAction.action.WasPressedThisFrame())
        {
            BackToSolarSystem();
        }
    }

    // 6. Your BackToSolarSystem function is now filled in
    public void BackToSolarSystem()
    {
        SceneManager.LoadScene(solarSystemSceneName);
    }

    // --- All your other planet functions ---
    public void Mercury()
    {
        SceneManager.LoadScene("Mercury_Scene");
    }
    public void Venus()
    {
        SceneManager.LoadScene("Venus_Scene");
    }
    public void Earth()
    {
        SceneManager.LoadScene("Earth_Scene");
    }
    public void Mars()
    {
        SceneManager.LoadScene("Mars_Scene");
    }
    public void Jupiter()
    {
        SceneManager.LoadScene("Jupiter_Scene");
    }
    public void Saturn()
    {
        SceneManager.LoadScene("Saturn_Scene");
    }
    public void Uranus()
    {
        SceneManager.LoadScene("Uranus_Scene");
    }
    public void Neptune()
    {
        SceneManager.LoadScene("Neptune_Scene");
    }
}