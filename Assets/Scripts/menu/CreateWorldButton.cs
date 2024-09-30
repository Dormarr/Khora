using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CreateWorldButton : MonoBehaviour
{
    public TMP_InputField worldNameInput;
    public TMP_InputField seedInput;
    private int seed;

    public void OnCreateWorldButtonClick(){
        string worldName = worldNameInput.text;

        if(!int.TryParse(seedInput.text, out seed)){
            Debug.LogError("No seed input. Creating new.");
            seed = Seed.GenerateSeed(seedInput.text);
        }

        WorldDataTransfer.worldName = worldName;
        WorldDataTransfer.worldSeed = seed;
        WorldDataTransfer.worldDate = Utility.GetDateTimeString();


        //Redo scene management to increase to singleplayer scene. Stanmdardise the process.
        SceneManager.LoadScene("TerrainGen");
    }
}
