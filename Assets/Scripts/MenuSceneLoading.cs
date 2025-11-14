using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSceneLoading : MonoBehaviour
{
    public void LoadSolarSystem()
    {
        SceneManager.LoadScene("MR_Solar_System");
    }
    public void LoadTrappistSystem()
    {
        SceneManager.LoadScene("MR_Trappist_System");
    }
}
