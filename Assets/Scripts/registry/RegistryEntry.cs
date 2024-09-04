using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IRegisterEntry<T>{
    T Value();
    bool MatchesId(Identifier id);
}

public class RegistryEntry<T> : IRegisterEntry<T>
{
    #nullable enable
    private RegistryKey<T>? registryKey;
    private T value;
    #nullable disable

    protected RegistryEntry(RegistryKey<T> registryKey, T value){
        this.registryKey = registryKey;
        this.value = value;
    }

    public RegistryKey<T> RegistryKey(){
        
        if(this.registryKey == null){
            throw new Exception("Trying to access unbound registry key.");
        }else{
            return this.registryKey;
        }
    }

    public T Value(){
        if(this.value == null){
            throw new Exception("Trying to access unbound value.");
        }else{
            return this.value;
        }
    }

    public bool MatchesId(Identifier id){
        return this.RegistryKey().GetValue().Equals(id);
    }
}
