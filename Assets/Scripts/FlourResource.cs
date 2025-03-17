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

    // Kaynak ��karma metodu
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
        DecreaseQueueCount();
        _resourceManager.AddResource(type, quantity);

      

    }
    public override async UniTask Produce()
    {
        // E�er zaten �retim yap�l�yorsa ��k
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("�retim yap�lm�yor veya kapasite dolmu�.");
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
            /*BU KISIM GEREKS�Z G�B�*/
            storedResources.Value++; // �retilen kaynak ekleniyor
            Debug.Log("Yeni kaynak �retildi!");

            // Kuyruktan bir eksilt
            DecreaseQueueCount();
            /*BU KISIM GEREKS�Z G�B�*/



            // E�er kuyruk tamamen bittiyse �retimi durdur
            if (_queueCount.Value <= 0)
            {
                Debug.Log("�retim kuyru�u bo�ald�, �retim durduruluyor.");
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
            // E�er hi� kaynak yoksa, toplama i�lemi yap�lmas�n
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
