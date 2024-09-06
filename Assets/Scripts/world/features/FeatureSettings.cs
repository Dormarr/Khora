using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FeatureSettings
{
    //this needs a build class to put it all together, same as the biome.
    //This doesn't need to be serializable though, because it won't be saved.
    //The loaded feature on the independent tiles needs to be saved though.

    //This just sends across the probabilities of generation.

    public float treeChanceSolo; //chance of a tree generating alone
    public float treeChanceNeighbour; //chance of a tree generating next to another tree, motivated clusters
    public float grassChance;
    public float flowerChanceSolo;
    public float flowerChanceNeighbour;
    //there'll be more to add, but stick with this for now.


    public FeatureSettings(float treeChanceSolo, float treeChanceNeighbour, float grassChance, float flowerChanceSolo, float flowerChanceNeighbour){
        this.treeChanceSolo = treeChanceSolo;
        this.treeChanceNeighbour = treeChanceNeighbour;
        this.grassChance = grassChance;
        this.flowerChanceSolo = flowerChanceSolo;
        this.flowerChanceNeighbour = flowerChanceNeighbour;
    }

    public class Build{

        //Each set to zero as default, meaning they can be instantiated when necessary.

        private float treeChanceSolo = 0;
        private float treeChanceNeighbour = 0;
        private float grassChance = 0;
        private float flowerChanceSolo = 0;
        private float flowerChanceNeighbour = 0;


        public FeatureSettings.Build Flowers(float flowerChanceSolo, float flowerChanceNeighbour){
            this.flowerChanceSolo = flowerChanceSolo;
            this.flowerChanceNeighbour = flowerChanceNeighbour;
            return this;
        }

        public FeatureSettings.Build Trees(float treeChanceSolo, float treeChanceNeighbour){
            this.treeChanceSolo = treeChanceSolo;
            this.treeChanceNeighbour = treeChanceNeighbour;
            return this;
        }

        public FeatureSettings.Build Grass(float grassChance){
            this.grassChance = grassChance;
            return this;
        }

        public FeatureSettings BuildFeatureSettings(){
            return new FeatureSettings(this.treeChanceSolo, this.treeChanceNeighbour, this.grassChance, this.flowerChanceSolo, this.flowerChanceNeighbour);

        }

    }
}
