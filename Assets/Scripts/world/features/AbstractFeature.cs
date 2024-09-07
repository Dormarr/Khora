using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFeature
{
    //this is where we define the basic foundation for a feature to be registered.
    //Not sure if this is really necessary, but I'd rather set the foundations now just in case.

    public string name {get; private set;}


    public AbstractFeature(string name){
        this.name = name;
    }

    //This may need a builder after all.
}
