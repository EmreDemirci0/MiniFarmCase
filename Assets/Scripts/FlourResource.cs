using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceBase
{
    /*private */public FlourEntity _flourEntity;  // FlourEntity referans�

    public bool IsProducing => isProducing;

    private ResourceManager _resourceManager;
    private ResourceCollector _resourceCollector;

    [Inject]
    public void Construct(FlourEntity flourEntity, ResourceCollector resourceCollector, ResourceManager resourceManager)
    {
        _flourEntity = flourEntity;
        _resourceManager = resourceManager;
        _resourceCollector = resourceCollector;

        
    }
    
    public async UniTask<bool> AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        _flourEntity.UpdateTotalQuantityText();

        if (_flourEntity.currentQueueCount.Value <= 0 || IsProducing)
            return false;
        //await Produce(); // �retimi ba�lat
        //StartProgressUpdateLoop2
        await UniTask.WhenAll(
            Produce(),
            _resourceCollector.StartProgressUpdateLoop2()
        );
        //burada d�ng� oluyor. onu engelle, e�er entity.currentQueueObject>0 ise yap, i� bitince bir azalt e�er 0 ise slider vb. i�lemler dursun,
        //+ butonuna �st �ste bas�nca bug oluyor s�raya als�n,
        Debug.Log("Addtoqueuue2");
        return true;
        
    }

    // Kaynak ��karma metodu
    public bool RemoveFromQueue(ResourceType type, int quantity)
    {
        Debug.Log("Bir");
        if (_flourEntity.currentQueueCount.Value <= 0)
            return false;
        Debug.Log("iki");
        _flourEntity.ReduceTotalQuantityText();
        _resourceManager.AddResource(type, quantity);

        //if (_flourEntity.currentQueueCount.Value <= 0)
        //{
        //    isProducing = false;
        //    _resourceCollector.UpdateFlourSlider(); // Slider'� g�ncelle
        //}
        Debug.Log("uc");
        return true;

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

        while (_flourEntity.currentQueueCount.Value > 0 && storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {                
                //_resourceCollector.flourProductionTimerText.text = $"{i}s";
                await UniTask.Delay(1000);
            }

            storedResources.Value++; // �retilen kaynak ekleniyor
            Debug.Log("Yeni kaynak �retildi!");

            // Kuyruktan bir eksilt
            _flourEntity.ReduceTotalQuantityText();

            // E�er kuyruk tamamen bittiyse �retimi durdur
            if (_flourEntity.currentQueueCount.Value <= 0)
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
            _resourceCollector.StartProgressUpdateLoop2()
            );
        }

        return collected;
    }
}
