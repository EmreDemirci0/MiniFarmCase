using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GeneralResourceFactory : PlaceholderFactory<ResourceBase>
{
    public T CreateResource<T>() where T : ResourceBase
    {
        return base.Create() as T;
    }
}