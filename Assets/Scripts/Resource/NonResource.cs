using Cysharp.Threading.Tasks;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public abstract class NonResource : ResourceBase
{ 

    // Kaydedilen zaman
    //private const string LastSavedTimeKey = "LastSavedTime";
    //private const string StoredResourceKey = "StoredResources";


    [Inject]
    public NonResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
    {
        var delay = UniTask.DelayFrame(1).ContinueWith(() =>
        {
            //IsSaveable = true;
            if (IsSaveable)
            {
                lastSavedTime = LoadSavedTime(PlayerPrefsKeys.HayLastSavedTimeKey);
                LoadStoredResources(PlayerPrefsKeys.HayStoredResourceKey);//burasý storedkey olmasý gerekmez mi?

                if (lastSavedTime == default)
                {
                    Debug.Log("Kaydedilen zaman bulunamadý. Yeni üretim baþlatýlýyor.");
                    SAVE(PlayerPrefsKeys.HayLastSavedTimeKey,PlayerPrefsKeys.HayStoredResourceKey);
                }
            }


            var task = UniTask.WhenAll(
                IsSaveable ? CalculateProductionOnLoad() : ProduceWithSlider(0)
            );
        });

    }
    public float curr;
    public override async UniTask ProduceWithSlider(float remainingTime)
    {
        // Eðer zaten üretim yapýlýyorsa veya depo doluysa, metottan çýk
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim zaten devam ediyor veya depo dolu.");
            return;
        }

        // Üretimi baþlat
        SetIsProducing(true);

        // Üretim sürecini ve slider'ý takip et
        while (StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;

            // Slider'ý ve geri sayýmý güncelle
            while (currentRemainingTime > 0)
            {
                float targetValue = (float)(currentRemainingTime / ProductionTime);
                _resourceCollector.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                //Debug.LogError("TARGETVALUE:" + targetValue + " currentRemainingTime:" + currentRemainingTime);
                curr = currentRemainingTime;
                _resourceCollector.SetSliderValue(ResourceType, targetValue);

                await UniTask.Delay(1000); // 1 saniye bekle
                currentRemainingTime--;
            }

            // Üretim tamamlandý, depoya ekle
            SetStoredResources(StoredResources.Value + 1);
            if (IsSaveable)
            {
                SAVE(PlayerPrefsKeys.HayLastSavedTimeKey, PlayerPrefsKeys.HayStoredResourceKey);
            }
            //    Debug.Log("Depoya 1 " + ResourceType + " eklendi.");
        }

        // Üretim tamamlandý, slider'ý ve metni güncelle
        _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        _resourceCollector.SetSliderValue(ResourceType, 1);

        // Üretimi durdur
        SetIsProducing(false);
        if (IsSaveable)
        {
            SAVE(PlayerPrefsKeys.HayLastSavedTimeKey, PlayerPrefsKeys.HayStoredResourceKey);
        }

    }

    public override void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceCollector.SetResourceCapacityText(ResourceType, stored));
        SetResourceImage();
    }
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0)
        {
            return 0;
        }

        int collected = StoredResources.Value;
        SetStoredResources(0);
        _resourceManager.AddResource(ResourceType, collected);
        if (IsSaveable)
        {
            SAVE(PlayerPrefsKeys.HayLastSavedTimeKey, PlayerPrefsKeys.HayStoredResourceKey,curr);
        }

        if (!IsProducing)
        {
            await ProduceWithSlider(0);
        }

        return collected;
    }


    




}
