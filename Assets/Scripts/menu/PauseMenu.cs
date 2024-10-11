using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Config.isPaused = false;
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Config.isPaused = true;
    }

    public void Settings(){
        //Open a settings menu.
    }

    public void QuitToMain(){
        //save the game.
        ChunkManager cm = GameObject.Find("ChunkManager").GetComponent<ChunkManager>();
        cm.ExitProcedure();
        SceneManager.LoadScene("Title");
        Resume();
    }
}
