using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegistryKey<T>
{
    private Identifier registry;
    private Identifier value;


    private RegistryKey(Identifier registry, Identifier value){
        this.registry = registry;
        this.value = value;
    }

    public static RegistryKey<T> of(RegistryKey<Registry<T>> registry, Identifier value){
        //return of(registry.value, value);
        return new RegistryKey<T>(registry.GetValue(), value);
    }

    // public static RegistryKey<T> of(Identifier registry, Identifier value){
    //     return new RegistryKey<T>(registry, value);
    // }

    public static RegistryKey<Registry<T>> of(Identifier registry, Identifier value){
        return new RegistryKey<Registry<T>>(registry, value);
    }

    public bool isOf(RegistryKey<Registry<T>> registry){
        return this.registry.Equals(registry.GetValue());}

    public Identifier GetValue(){
        return this.value;
    }

    public Identifier GetRegistry(){
        return this.registry;
    }

    public static RegistryKey<Registry<T>> ofRegistry(Identifier registry){
        return of(RegistryKeys.ROOT, registry);
    }

}
