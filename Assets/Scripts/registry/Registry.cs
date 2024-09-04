using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Registry<T>
{
    private readonly Dictionary<string, T> _registry = new Dictionary<string, T>();
}
