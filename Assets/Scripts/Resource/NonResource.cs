using Cysharp.Threading.Tasks;
using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using Zenject;
public abstract class NonResource : ResourceBase
{

    private DateTime lastSavedTime; // Kaydedilen zaman
    //private const string LastSavedTimeKey = "LastSavedTime";
    //private const string StoredResourceKey = "StoredResources";

    [Inject]
    public NonResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
    {
        lastSavedTime= LoadSavedTime();
        LoadStoredResources();

        if (lastSavedTime == default)
        {
            Debug.Log("Kaydedilen zaman bulunamadý. Yeni üretim baþlatýlýyor.");
            SAVE();
            var delay = UniTask.DelayFrame(1).ContinueWith(() =>
            {
                var task = UniTask.WhenAll(
                 ProduceWithSlider(0)
                );
            });
        }
        else
        {
            // Kaydedilen zaman varsa, geçen süreyi hesapla ve üretimi devam ettir
            //UniTask.DelayFrame(1).ContinueWith(() => CalculateProductionOnLoad()).Forget();
            var delay = UniTask.DelayFrame(1).ContinueWith(() =>
            {
                var task = UniTask.WhenAll(
                  CalculateProductionOnLoad()
                //StartAutoSave()
                );
            });
        }

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
            SAVE();
            //    Debug.Log("Depoya 1 " + ResourceType + " eklendi.");
        }

        // Üretim tamamlandý, slider'ý ve metni güncelle
        _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        _resourceCollector.SetSliderValue(ResourceType, 1);

        // Üretimi durdur
        SetIsProducing(false);
        SAVE();
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
        SAVE(curr);
        if (!IsProducing)
        {
            await ProduceWithSlider(0);
        }

        return collected;
    }



   

    private async UniTask CalculateProductionOnLoad()//Eðer kayýtlý bir zaman var ise, kaldýgý yerden devam ettirir
    {
        if (lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - lastSavedTime;


            int producedResources = (int)(timeDifference.TotalSeconds / ProductionTime);
            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);


            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);
            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {

                double remainingTime = ProductionTime - remainingResources;
                _resourceCollector.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                await ProduceWithSlider((float)(remainingTime)); // Üretimi kaldýgý yerden devam ettir
            }
            else
            {
                _resourceCollector.SetSliderValue(ResourceType, 1); // Depo dolu, slider tam dolu
                _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
            }
        }
    }
    public void SAVE(float curr=0)
    {
        Debug.Log("SAVED");
        SaveCurrentTime(curr);
        SaveStoredResources();
    }
    //private DateTime LoadSavedTime()//Kayýtlý bir zaman varsa onu yükle
    //{
    //    if (PlayerPrefs.HasKey(LastSavedTimeKey))
    //    {
    //        string savedTimeString = PlayerPrefs.GetString(LastSavedTimeKey);
    //        if (DateTime.TryParse(savedTimeString, out DateTime savedTime))
    //        {
    //            return savedTime;
    //        }
    //    }
    //    return default;
    //}
    private DateTime LoadSavedTime()
    {
        return PlayerPrefsHelper.LoadDateTime(PlayerPrefsKeys.LastSavedTimeKey);
    }

    //private void LoadStoredResources()//Depodaki ürün adedini yükle
    //{
    //    if (PlayerPrefs.HasKey(StoredResourceKey))
    //    {
    //        int storedResources = PlayerPrefs.GetInt(StoredResourceKey);
    //        SetStoredResources(storedResources); // Depo durumunu yükle
    //    }
    //}
    private void LoadStoredResources()
    {
        int storedResources = PlayerPrefsHelper.LoadInt(PlayerPrefsKeys.StoredResourceKey);
        SetStoredResources(storedResources); // Depo durumunu yükle
    }

    //private void SaveCurrentTime(float curr)
    //{
    //    if (curr > 0)
    //    {
    //        lastSavedTime = DateTime.Now - (TimeSpan.FromSeconds(ProductionTime-curr));
    //    }
    //    else
    //    {
    //        lastSavedTime = DateTime.Now; // curr deðeri yoksa þu anki zamaný kaydet
    //    }
    //    //lastSavedTime = DateTime.Now; // Mevcut zamaný kaydet
    //    PlayerPrefs.SetString(LastSavedTimeKey, lastSavedTime.ToString()); // PlayerPrefs'e kaydet
    //    PlayerPrefs.Save(); // Kaydetmeyi unutma!
    //    Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    //}
    private void SaveCurrentTime(float curr)
    {
        if (curr > 0)
        {
            lastSavedTime = DateTime.Now - TimeSpan.FromSeconds(ProductionTime - curr);
        }
        else
        {
            lastSavedTime = DateTime.Now; // curr deðeri yoksa þu anki zamaný kaydet
        }
        PlayerPrefsHelper.SaveDateTime(PlayerPrefsKeys.LastSavedTimeKey, lastSavedTime);
        Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    }

    //private void SaveStoredResources()
    //{
    //    PlayerPrefs.SetInt(StoredResourceKey, StoredResources.Value); // Depo durumunu kaydet
    //    PlayerPrefs.Save(); // Kaydetmeyi unutma!
    //                        // Debug.Log("Depo Durumu Kaydedildi: " + StoredResources.Value);
    //}
    private void SaveStoredResources()
    {
        PlayerPrefsHelper.SaveInt(PlayerPrefsKeys.StoredResourceKey, StoredResources.Value);
    }




}
