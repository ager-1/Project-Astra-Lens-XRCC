using UnityEngine;
using UnityEngine.SceneManagement;

public class StartExperienceButton : MonoBehaviour
{
    public void LoadMenu()
    {
        // 2. This line loads the scene by its exact name
        SceneManager.LoadScene("Menu1");
    }
}
