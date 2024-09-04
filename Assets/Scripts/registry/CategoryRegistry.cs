using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CategoryRegistry
{
    private readonly Dictionary<string, object> _categories = new Dictionary<string, object>();

    public void RegisterCategory<T>(string categoryKey){
        if(!_categories.ContainsKey(categoryKey)){
            _categories.Add(categoryKey, new Registry<T>());
            Debug.Log($"Registered category: {categoryKey}");
        }else{
            throw new Exception($"Category '{categoryKey}' is already registered");
        }
    }

    public Registry<T> GetCategoryRegistry<T>(string categoryKey){
        if(_categories.TryGetValue(categoryKey, out object registry)){
            return registry as Registry<T>;
        }
        throw new Exception($"Category '{categoryKey}' is not found.");
    }
}
