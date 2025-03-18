using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class BreadV1Resource : ResourceDependentBase
{
    [Inject]
    public BreadV1Resource(ResourceManager resourceManager, ResourceCollector resourceCollector)
        : base(resourceManager, resourceCollector, ResourceType.BreadV1)
    {
        //resourceType = ResourceType.BreadV1;
    }
}
