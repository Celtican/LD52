using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public AudioController.Audio clickSound;

    public void PlayClickSound()
    {
        AudioController.instance.PlaySound(clickSound);
    }
    
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void Quit()
    {
        Application.Quit();
    }
}