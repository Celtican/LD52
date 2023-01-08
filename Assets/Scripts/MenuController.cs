using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController instance { get; private set; }
    
    public bool canPause = true;
    private bool isPaused = false;

    public GameObject pauseScreen;

    public AudioController.Audio clickSound;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && canPause)
        {
            if (isPaused) Unpause();
            else Pause();
        }
    }

    public void LoadScene(string scene)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(scene);
    }

    public void PlayClickSound()
    {
        AudioController.instance.PlaySound(clickSound);
    }

    public void Pause()
    {
        if (canPause)
        {
            isPaused = true;
            Time.timeScale = 0;
            pauseScreen.SetActive(true);
        }
    }

    public void Unpause()
    {
        if (canPause)
        {
            isPaused = false;
            Time.timeScale = 1;
            pauseScreen.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        instance = null;
    }

    public void SetCanPause(bool canPause)
    {
        this.canPause = canPause;
    }
}
