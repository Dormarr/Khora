using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MenuUtility
{
    public static PauseMenu pauseMenu;

    public static void Initialize(){
        pauseMenu = GameObject.Find("GameManager").GetComponent<PauseMenu>();
    }

    public static void Pause(){
        pauseMenu.Pause();
    }

    public static void Resume(){
        pauseMenu.Resume();
    }

    public static void QuitToMain(){
        pauseMenu.QuitToMain();
    }
}
