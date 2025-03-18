using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceDependentBase
{
    [Inject]
    
    public FlourResource(ResourceManager resourceManager, ResourceCollector resourceCollector) 
        : base(resourceManager, resourceCollector, ResourceType.Flour)
    {
        //resourceType=ResourceType.Flour;
    }
}
