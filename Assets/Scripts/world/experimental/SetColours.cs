using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class SetColours : MonoBehaviour
{
    //get tilemap, render different colours to all 16 colour variables.

    [SerializeField] private Gradient gradient;
    public Tilemap tilemap;
    public TilemapRenderer tilemapRenderer;

    public void SetColour(){
        //TilemapRenderer tmr = tilemap.GetComponent<TilemapRenderer>();
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();

        mpb.SetColor("_Color1", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color2", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color3", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color4", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color5", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color6", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color7", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color8", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color9", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color10", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color11", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color12", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color13", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color14", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color15", gradient.Evaluate(Random.Range(0.0f, 1.0f)));
        mpb.SetColor("_Color16", gradient.Evaluate(Random.Range(0.0f, 1.0f)));

        tilemapRenderer.SetPropertyBlock(mpb);

        Debug.Log($"Set all the colours. Example: {gradient.Evaluate(Random.Range(0.0f, 1.0f))}");
    }
}

[CustomEditor(typeof(SetColours))]
public class SetColoursEditor : Editor{

    public override void OnInspectorGUI(){

        SetColours setColours = (SetColours)target;

        DrawDefaultInspector();

        if(GUILayout.Button("Set Colours")){
            setColours.SetColour();
        }
    }
}
