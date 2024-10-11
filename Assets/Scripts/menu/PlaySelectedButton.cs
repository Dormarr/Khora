using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySelectedButton : MonoBehaviour
{
    public void OnPlaySelectedWorldButtonClick(){
        SceneManager.LoadScene("Singleplayer");
    }
}
