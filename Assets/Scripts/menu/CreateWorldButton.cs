using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateWorldButton : MonoBehaviour
{
    public TMP_InputField worldNameInput;
    public TMP_InputField seedInput;

    public void OnCreateWorldButtonClick(){
        string worldName = worldNameInput.text;
        int seed = Seed.GenerateSeed();

        if(!int.TryParse(seedInput.text, out seed)){
            Debug.LogError("No seed input. Creating new.");
        }

        WorldDataTransfer.worldName = worldName;
        WorldDataTransfer.worldSeed = seed;

        SceneManager.LoadScene("TerrainGen");
    }
}
