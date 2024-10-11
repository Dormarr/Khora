using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FeatureSettings
{
    public List<NaturalFeature> naturalFeatures = new List<NaturalFeature>();
    //public List<UnnaturalFeatures> unnaturalFeatures;

    public FeatureSettings(List<NaturalFeature> naturalFeature){
        this.naturalFeatures = naturalFeature;
    }

    public class Build{
        private List<NaturalFeature> naturalFeatures = new List<NaturalFeature>();

        public FeatureSettings.Build AddNaturalFeature(NaturalFeature naturalFeature){
            this.naturalFeatures.Add(naturalFeature);
            return this;
        }

        public FeatureSettings BuildFeatureSettings(){
            return new FeatureSettings(this.naturalFeatures);

        }

    }
}
