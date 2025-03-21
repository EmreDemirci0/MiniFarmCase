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
            Debug.Log("Kaydedilen zaman bulunamad�. Yeni �retim ba�lat�l�yor.");
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
            // Kaydedilen zaman varsa, ge�en s�reyi hesapla ve �retimi devam ettir
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
            SAVE();
            //    Debug.Log("Depoya 1 " + ResourceType + " eklendi.");
        }

        // �retim tamamland�, slider'� ve metni g�ncelle
        _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        _resourceCollector.SetSliderValue(ResourceType, 1);

        // �retimi durdur
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



   

    private async UniTask CalculateProductionOnLoad()//E�er kay�tl� bir zaman var ise, kald�g� yerden devam ettirir
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
                await ProduceWithSlider((float)(remainingTime)); // �retimi kald�g� yerden devam ettir
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
    //private DateTime LoadSavedTime()//Kay�tl� bir zaman varsa onu y�kle
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

    //private void LoadStoredResources()//Depodaki �r�n adedini y�kle
    //{
    //    if (PlayerPrefs.HasKey(StoredResourceKey))
    //    {
    //        int storedResources = PlayerPrefs.GetInt(StoredResourceKey);
    //        SetStoredResources(storedResources); // Depo durumunu y�kle
    //    }
    //}
    private void LoadStoredResources()
    {
        int storedResources = PlayerPrefsHelper.LoadInt(PlayerPrefsKeys.StoredResourceKey);
        SetStoredResources(storedResources); // Depo durumunu y�kle
    }

    //private void SaveCurrentTime(float curr)
    //{
    //    if (curr > 0)
    //    {
    //        lastSavedTime = DateTime.Now - (TimeSpan.FromSeconds(ProductionTime-curr));
    //    }
    //    else
    //    {
    //        lastSavedTime = DateTime.Now; // curr de�eri yoksa �u anki zaman� kaydet
    //    }
    //    //lastSavedTime = DateTime.Now; // Mevcut zaman� kaydet
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
            lastSavedTime = DateTime.Now; // curr de�eri yoksa �u anki zaman� kaydet
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
