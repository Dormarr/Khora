using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NaturalFeature : Feature
{
    //this is for the natural features, like trees, grass, and flowers.
    public NaturalFeatureEnum type {get; private set;}

    public NaturalFeature(string name, NaturalFeatureEnum type)
        : base(name){
            this.type = type;
       }
}