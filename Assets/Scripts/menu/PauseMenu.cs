using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pauseMenuUI;

    //new input system do thingy.

    public void Resume(){
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause(){
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    void Settings(){
        //Open a settings menu.
    }

    public void QuitToMain(){
        //save the game.
        ChunkManager cm = GameObject.Find("ChunkManager").GetComponent<ChunkManager>();
        cm.SaveModifications();
        Resume();
        SceneManager.LoadScene("Title");
    }
}
