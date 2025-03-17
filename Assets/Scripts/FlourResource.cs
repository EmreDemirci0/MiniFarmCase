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
    /*private */public FlourEntity _flourEntity;  // FlourEntity referansý

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
        //await Produce(); // Üretimi baþlat
        //StartProgressUpdateLoop2
        await UniTask.WhenAll(
            Produce(),
            _resourceCollector.StartProgressUpdateLoop2()
        );
        //burada döngü oluyor. onu engelle, eðer entity.currentQueueObject>0 ise yap, iþ bitince bir azalt eðer 0 ise slider vb. iþlemler dursun,
        //+ butonuna üst üste basýnca bug oluyor sýraya alsýn,
        Debug.Log("Addtoqueuue2");
        return true;
        
    }

    // Kaynak çýkarma metodu
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
        //    _resourceCollector.UpdateFlourSlider(); // Slider'ý güncelle
        //}
        Debug.Log("uc");
        return true;

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

        while (_flourEntity.currentQueueCount.Value > 0 && storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {                
                //_resourceCollector.flourProductionTimerText.text = $"{i}s";
                await UniTask.Delay(1000);
            }

            storedResources.Value++; // Üretilen kaynak ekleniyor
            Debug.Log("Yeni kaynak üretildi!");

            // Kuyruktan bir eksilt
            _flourEntity.ReduceTotalQuantityText();

            // Eðer kuyruk tamamen bittiyse üretimi durdur
            if (_flourEntity.currentQueueCount.Value <= 0)
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
            _resourceCollector.StartProgressUpdateLoop2()
            );
        }

        return collected;
    }
}
