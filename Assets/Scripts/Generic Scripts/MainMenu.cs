using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnPlayButton(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }

    public void OnExitButton()
    {
        Application.Quit();
    }

}
