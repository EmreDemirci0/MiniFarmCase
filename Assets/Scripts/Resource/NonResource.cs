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
                LoadStoredResources(PlayerPrefsKeys.HayStoredResourceKey);//buras� storedkey olmas� gerekmez mi?

                if (lastSavedTime == default)
                {
                    Debug.Log("Kaydedilen zaman bulunamad�. Yeni �retim ba�lat�l�yor.");
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
        // E�er zaten �retim yap�l�yorsa veya depo doluysa, metottan ��k
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("�retim zaten devam ediyor veya depo dolu.");
            return;
        }

        // �retimi ba�lat
        SetIsProducing(true);

        // �retim s�recini ve slider'� takip et
        while (StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;

            // Slider'� ve geri say�m� g�ncelle
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

            // �retim tamamland�, depoya ekle
            SetStoredResources(StoredResources.Value + 1);
            if (IsSaveable)
            {
                SAVE(PlayerPrefsKeys.HayLastSavedTimeKey, PlayerPrefsKeys.HayStoredResourceKey);
            }
            //    Debug.Log("Depoya 1 " + ResourceType + " eklendi.");
        }

        // �retim tamamland�, slider'� ve metni g�ncelle
        _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        _resourceCollector.SetSliderValue(ResourceType, 1);

        // �retimi durdur
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
