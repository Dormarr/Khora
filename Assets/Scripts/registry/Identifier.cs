using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Identifier
{
    //codec stuff maybe, I don't know. Need to serialise and deserialise stuff when it comes to jsons.
    public static char _nameSeparator = ':';
    public static string _defaultName = "dormarrs_sandbox"; //change once you've chosen a name.
    private string name;
    private string path;

    private Identifier(string name, string path){
        //check namespace and path are valid.
        if(isNameValid(name) && isPathValid(path)){
            this.name = name;
            this.path = path;
        }
    }

    public static Identifier of(string name, string path){
        //validate and return.
        return ofValidated(name, path);
    }

    public static Identifier of(string id){
        //split to find id.
        return splitOn(id, _nameSeparator);
    }

    public static Identifier ofOriginal(string path){
        return new Identifier(_defaultName, validatePath(_defaultName, path));
    }

    public static Identifier ofValidated(string name, string path){
        return new Identifier(validateName(name, path), validatePath(name, path));
    }

    public string getPath(){return this.path;}
    public string getName(){return this.name;}

    public static bool isPathValid(string path){
        //validate path, return true if valid.
        for(int i = 0; i < path.Length; i++){
            if(!isPathCharacterValid(path[i])){
                return false;
            }
        }

        return true;
    }

    public static bool isPathCharacterValid(char c){
        //validate character by path criteria, return true if valid.

        return c == '_' || c == '-' || c >= 'a' && c <= 'z' || c >= 0 && c <= 9 || c == '.';
    }

    public static string validateName(string name, string path){
        if(!isNameValid(name)){
            throw new Exception("Non [a-z0-9_.-] character in name of " + path + _nameSeparator + name);
        }else{
            return name;
        }
    }

        public static string validatePath(string name, string path){
        if(!isPathValid(name)){
            throw new Exception("Non [a-z0-9_.-] character in path of " + path + _nameSeparator + name);
        }else{
            return path;
        }
    }

    public static bool isNameValid(string name){
        //validate namespace, return true if valid.
        for(int i = 0; i < name.Length; i++){
            if(!isNameCharacterValid(name[i])){
                return false;
            }
        }

        return true;
    }

    public static bool isNameCharacterValid(char c){
        //validate character by namespace criteria, return true if valid.

        return c == '_' || c == '-' || c >= 'a' && c <= 'z' || c >= 0 && c <= 9 || c == '.';
    }

    public static Identifier splitOn(string id, char delimiter){
        //return name and path.

        string[] returns = id.Split(':');
        return new Identifier(returns[0], returns[1]);
    }

    public bool equals(object o){
        if(this == o){
            return true;
        }else{
            return false;
        }
    }

}
