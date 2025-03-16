using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class FlourResource : ResourceBase
{
    private FlourEntity _flourEntity;  // FlourEntity referans�



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
        //OnStoredResourcesChanged?.Invoke(0);
        if (isProducing)
        {
            return;
        }

        isProducing = true;

        while (storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {
                //OnProductionTimeChanged?.Invoke(i);
                await UniTask.Delay(1000);
            }

            storedResources.Value++;
            Debug.Log("Value ARtt� m� ?");
            //OnStoredResourcesChanged?.Invoke(storedResources.Value);
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
