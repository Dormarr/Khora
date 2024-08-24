using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This will combine all the generated perlin noise maps and generate a final world based on the return values for coords.
//Coords come in, passed over to perlinGenerator, a cluster of info comes back, is assessed and rendered.
//The rendering should probably be done elsewhere though, because that's a whole other task, especially with connected tiles.

public class WorldEngine : MonoBehaviour
{
    //this will probably need it's own cache, or assign a cache to the chunk it generate to?
    //Assign by coordinate values, and attach to the chunk gameobject to be drawn on later by other stuff.
    //Maybe create a scriptable object that contains the 4 values, and give each chunk their 32x32 share to store.
}
