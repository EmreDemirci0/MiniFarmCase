using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceBase
{
   

    private ResourceManager _resourceManager;
    private ResourceCollector _resourceCollector;
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);

    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;
    public IObservable<int> OnQueueCountChanged => _queueCount.AsObservable();
    public bool IsProducing => isProducing;

    [Inject]
    public void Construct(ResourceCollector resourceCollector, ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;
    }
    public void IncreaseQueueCount()
    {
        _queueCount.Value++;
    }

    public void DecreaseQueueCount()
    {
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--;
        }
    }
    public async UniTask AddToQueue(ResourceType type, int quantity)
    {      
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing)
            return;

        await UniTask.WhenAll(
            Produce(),
            _resourceCollector.FlourSliderTask()
        );
      
        
    }

    // Kaynak çýkarma metodu
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
        DecreaseQueueCount();
        _resourceManager.AddResource(type, quantity);

      

    }
    public override async UniTask Produce()
    {
        // Eðer zaten üretim yapýlýyorsa çýk
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
            return;
        }

        isProducing = true;

        while (_queueCount.Value > 0 && storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {                
                //_resourceCollector.flourProductionTimerText.text = $"{i}s";
                await UniTask.Delay(1000);
            }
            /*BU KISIM GEREKSÝZ GÝBÝ*/
            storedResources.Value++; // Üretilen kaynak ekleniyor
            Debug.Log("Yeni kaynak üretildi!");

            // Kuyruktan bir eksilt
            DecreaseQueueCount();
            /*BU KISIM GEREKSÝZ GÝBÝ*/



            // Eðer kuyruk tamamen bittiyse üretimi durdur
            if (_queueCount.Value <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                isProducing = false;
                return;
            }
        }

        isProducing = false;
    }



   
    
    public async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            // Eðer hiç kaynak yoksa, toplama iþlemi yapýlmasýn
            return 0;
        }

        int collected = storedResources.Value;
        storedResources.Value = 0;
        //OnStoredResourcesChanged?.Invoke(0);
        _resourceManager.AddResource(ResourceType.Flour, collected);

        if (storedResources.Value == 0)
        {
            //_resourceCollector.flourFactoryResourceSlider.gameObject.SetActive(false);
        }

        //TotalResourcesCount.Value += collected;
        // Only call Produce() if it's not already running
        if (!isProducing)
        {
            await UniTask.WhenAll(
            Produce(),
            _resourceCollector.FlourSliderTask()
            );
        }

        return collected;
    }
}
