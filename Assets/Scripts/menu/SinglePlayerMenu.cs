using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SinglePlayerMenu : MonoBehaviour
{
    //get all versions of the world data files
    //and list the worlds as a set format that you can interact with.

    public GameObject content;
    public TMP_FontAsset fontAsset;

    private void OnEnable() {
        ListWorlds();    
    }

    private void OnDisable() {
        ClearChildren();    
    }

    void ClearChildren(){
        foreach(Transform child in content.transform){
            GameObject.Destroy(child.gameObject);
        }
    }

    void ListWorlds(){
        //populate the content gameobject with world widgets for each world data file as children.

        WorldSaveData[] wsds = GetWorldDataFiles();

        foreach(WorldSaveData wsd in wsds){
            CreateWorldWidget(wsd);
        }
    }

    void CreateWorldWidget(WorldSaveData wsd){
        //this is where you actually put together the game object and it's components.

        GameObject worldWidget = new GameObject($"World_{wsd.name}");
        worldWidget.transform.SetParent(content.transform);

        RectTransform rectTransform = worldWidget.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(586,64);
        rectTransform.localScale = new Vector3(1,1,1);

        VerticalLayoutGroup layoutGroup = worldWidget.AddComponent<VerticalLayoutGroup>();

        CreateTextElement(worldWidget, $"Name: {wsd.name}");
        CreateTextElement(worldWidget, $"Seed: {wsd.seed}");
        //add directly to content.
    }

    void CreateTextElement(GameObject parent, string textContent){
        GameObject textObject = new GameObject("TextElement");
        textObject.transform.SetParent(parent.transform, false);

        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.text = textContent;

        textMesh.font = fontAsset;//Resources.Load<TMP_FontAsset>("Pizel");
        textMesh.fontSize = 16;
        textMesh.color = Color.white;

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 32);
    }

    WorldSaveData[] GetWorldDataFiles(){
        //get the world data files from what already exists.
        //Synthesis it all into an array and return.
        WorldSaveData[] wsds = new WorldSaveData[1];
        WorldSaveData wsd = Utility.LoadWorldSaveData();
        //This will only retrieve one because the current setup doesn't support multiple worlds.
        wsds[0] = wsd;
        return wsds;
    }


    //Create new world. -> go to new world gen menu.
    //Delete selected world.
    //Play selected world.
}
