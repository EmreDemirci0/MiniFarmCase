using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceBase
{
    private FlourEntity _flourEntity;  // FlourEntity referans�

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
        //await Produce(); // �retimi ba�lat
        //StartProgressUpdateLoop2
        await UniTask.WhenAll(
            Produce(),
            _resourceCollector.StartProgressUpdateLoop2()
        );
        //burada d�ng� oluyor. onu engelle, e�er entity.currentQueueObject>0 ise yap, i� bitince bir azalt e�er 0 ise slider vb. i�lemler dursun,
        //+ butonuna �st �ste bas�nca bug oluyor s�raya als�n,
        return true;
        
    }

    // Kaynak ��karma metodu
    public bool RemoveFromQueue(int quantity)
    {
        return false;
        //if (currentQueueCount >= quantity)
        //{
        //    currentQueueCount -= quantity;
        //    inventoryCount += quantity;
        //    //UpdateTotalQuantityText();
        //    return true;  // Ba�ar�l�
        //}
        //else
        //{
        //    Debug.Log("Kuyrukta yeterli kaynak yok.");
        //    return false;  // Ba�ar�s�z
        //}
    }
    public override async UniTask Produce()
    {
        // E�er zaten �retim yap�l�yorsa ��k
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

            storedResources.Value++; // �retilen kaynak ekleniyor
            Debug.Log("Yeni kaynak �retildi!");

            // Kuyruktan bir eksilt
            _flourEntity.ReduceTotalQuantityText();

            // E�er kuyruk tamamen bittiyse �retimi durdur
            if (_flourEntity.currentQueueCount <= 0)
            {
                Debug.Log("�retim kuyru�u bo�ald�, �retim durduruluyor.");
                isProducing = false;
                _resourceCollector.flourProductionTimerText.text = "Finish";
                //_resourceCollector.flourFactoryResourceSlider.value = 0;
                // Slider'� s�f�rla
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
    //        Debug.Log("Value ARtt� m� ?");
    //        //OnStoredResourcesChanged?.Invoke(storedResources.Value);
    //    }

    //    isProducing = false;
    //}

    public async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            // E�er hi� kaynak yoksa, toplama i�lemi yap�lmas�n
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
