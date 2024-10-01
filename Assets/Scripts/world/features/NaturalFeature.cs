using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NaturalFeature : AbstractFeature
{
    //this is for the natural features, like trees, grass, and flowers.
    //Configure properties like breakability(strength) and loot tables.
    //Different enums maybe should have their own default configurations.
    public NaturalFeatureEnum type {get; private set;}
    public float strength {get; private set;}
    public float frequency {get; private set;}

    public NaturalFeature(string name, NaturalFeatureEnum type, float strength, float frequency)
        : base(name){
            this.type = type;
            this.strength = strength;
            this.frequency = frequency;
       }


    public class Build{
        private string name;
        private NaturalFeatureEnum type;
        private float strength = 0.0f;
        private float frequency = 0.0f;//I need to think about this, because this will need adjustment per biome.


        public NaturalFeature.Build Name(string name){
            this.name = name;
            return this;
        }

        public NaturalFeature.Build Type(NaturalFeatureEnum type){
            this.type = type;
            return this;
        }

        public NaturalFeature.Build Strength(float strength){
            this.strength = strength;
            return this;
        }

        public NaturalFeature.Build Frequency(float frequency){
            this.frequency = frequency;
            return this;
        }

        public NaturalFeature BuildNaturalFeature(){
            if(this.name != null){
                return new NaturalFeature(name, type, strength, frequency);
            }else{
                throw new Exception($"Missing NaturalFeature parameters in builder\n{this}");
            }
        }
    }
}