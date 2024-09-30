using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

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

        WorldSaveData[] wsds = Utility.GetWorldDataFiles();

        if(wsds.Length <= 0){
            return;
        }

        foreach(WorldSaveData wsd in wsds){
            CreateWorldWidget(wsd);
        }
    }

    void CreateWorldWidget(WorldSaveData wsd){
        if(wsd == null) return;

        GameObject worldWidget = new GameObject($"World_{wsd.name}");
        worldWidget.transform.SetParent(content.transform);

        RectTransform rectTransform = worldWidget.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(586,64);
        rectTransform.localScale = new Vector3(1,1,1);

        VerticalLayoutGroup layoutGroup = worldWidget.AddComponent<VerticalLayoutGroup>();

        CreateTextElement(worldWidget, $"Name: {wsd.name}");
        CreateTextElement(worldWidget, $"Last Played: {wsd.date}");
        CreateTextElement(worldWidget, $"Seed: {wsd.seed}");
        //add directly to content.

        Button button = worldWidget.AddComponent<Button>();
        button.onClick.AddListener(() => SelectWorld(wsd));

        ColorBlock cb = button.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = Color.grey;
        button.colors = cb;

        //Selectable functionality in place, need to sort out UI.

    }

    void SelectWorld(WorldSaveData wsd){
        WorldDataTransfer.worldName = wsd.name;
        WorldDataTransfer.worldSeed = wsd.seed;
        WorldDataTransfer.worldDate = Utility.GetDateTimeString();
        //assign world transfer stuff.
        Debug.Log($"Selected world: {wsd.name}");
    }

    void CreateTextElement(GameObject parent, string textContent){
        GameObject textObject = new GameObject("TextElement");
        textObject.transform.SetParent(parent.transform, false);

        TextMeshProUGUI textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.text = textContent;

        textMesh.font = fontAsset;//Resources.Load<TMP_FontAsset>("Pizel");
        textMesh.fontSize = 12;
        textMesh.color = Color.white;//Redo colour.

        RectTransform rectTransform = textObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 32);
    }
}
