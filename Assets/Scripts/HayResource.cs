using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Resources;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;

public class HayResource : NonResourceBase
{
    public HayResource(ResourceManager resourceManager, ResourceCollector resourceCollector) : base(resourceManager, resourceCollector,ResourceType.Hay)
    {
        Debug.Log("SORUN VAR B�R ANDA 5.sn ���nlan�oyr");
        //resourceType = ResourceType.Hay;
    }
}
