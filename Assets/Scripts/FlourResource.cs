using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceBase
{
    private FlourEntity _flourEntity;  // FlourEntity referansý

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

    // Kaynak ekleme metodu
    public async UniTask<bool> AddToQueue(ResourceType type, int quantity)
    {
        
        _resourceManager.RemoveResource(type, quantity);
        _flourEntity.UpdateTotalQuantityText();

        if (_flourEntity.currentQueueCount <= 0 || isProducing)
            return false;
        //await Produce(); // Üretimi baþlat
        //StartProgressUpdateLoop2
        await UniTask.WhenAll(
            Produce(),
            _resourceCollector.StartProgressUpdateLoop2()
        );
        //burada döngü oluyor. onu engelle, eðer entity.currentQueueObject>0 ise yap, iþ bitince bir azalt eðer 0 ise slider vb. iþlemler dursun,
        //+ butonuna üst üste basýnca bug oluyor sýraya alsýn,
        return true;
        
    }

    // Kaynak çýkarma metodu
    public bool RemoveFromQueue(int quantity)
    {
        return false;
        //if (currentQueueCount >= quantity)
        //{
        //    currentQueueCount -= quantity;
        //    inventoryCount += quantity;
        //    //UpdateTotalQuantityText();
        //    return true;  // Baþarýlý
        //}
        //else
        //{
        //    Debug.Log("Kuyrukta yeterli kaynak yok.");
        //    return false;  // Baþarýsýz
        //}
    }
    public override async UniTask Produce()
    {
        // Eðer zaten üretim yapýlýyorsa çýk
        if (isProducing || _flourEntity.currentQueueCount <= 0)
            return;

        isProducing = true;

        while (_flourEntity.currentQueueCount > 0 && storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {
                _resourceCollector.flourProductionTimerText.text = $"{i}s";
                await UniTask.Delay(1000);
            }

            storedResources.Value++; // Üretilen kaynak ekleniyor
            Debug.Log("Yeni kaynak üretildi!");

            // Kuyruktan bir eksilt
            _flourEntity.ReduceTotalQuantityText();

            // Eðer kuyruk tamamen bittiyse üretimi durdur
            if (_flourEntity.currentQueueCount <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                isProducing = false;
                _resourceCollector.flourProductionTimerText.text = "Finish";
                //_resourceCollector.flourFactoryResourceSlider.value = 0;
                // Slider'ý sýfýrla
                _resourceCollector.flourFactoryResourceSlider.DOValue(0f, 0.5f).SetEase(Ease.OutQuad);
                return;
            }
        }

        isProducing = false;
    }



    //public override async UniTask Produce()
    //{
    //    //OnStoredResourcesChanged?.Invoke(0);
    //    if (isProducing)
    //    {
    //        return;
    //    }

    //    isProducing = true;

    //    while (storedResources.Value < maxCapacity)
    //    {
    //        for (int i = productionTime; i > 0; i--)
    //        {
    //            //OnProductionTimeChanged?.Invoke(i);
    //            await UniTask.Delay(1000);
    //        }

    //        storedResources.Value++;
    //        Debug.Log("Value ARttý mý ?");
    //        //OnStoredResourcesChanged?.Invoke(storedResources.Value);
    //    }

    //    isProducing = false;
    //}

    public async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            // Eðer hiç kaynak yoksa, toplama iþlemi yapýlmasýn
            return 0;
        }

        int collected = storedResources.Value;
        Debug.Log("Collect" + collected);
        storedResources.Value = 0;
        //OnStoredResourcesChanged?.Invoke(0);
        _resourceManager.AddResource(ResourceType.Flour, collected);
        //TotalResourcesCount.Value += collected;
        // Only call Produce() if it's not already running
        if (!isProducing)
        {
            await Produce(); // Start production if it's not already running
        }

        return collected;
    }
}
