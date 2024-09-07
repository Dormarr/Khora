using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Feature
{
    //this is where we define the basic foundation for a feature to be registered.

    public string name {get; private set;}


    public Feature(string name){
        this.name = name;
    }

    //I don't think this needs a builder.
}
