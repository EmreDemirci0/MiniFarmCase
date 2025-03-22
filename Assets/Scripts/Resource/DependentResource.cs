using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;

public class DependentResource : ResourceBase
{
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;

    public DependentResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
    {
        lastSavedTime = LoadSavedTime(PlayerPrefsKeys.FlourLastSavedTimeKey);
        LoadStoredResources(PlayerPrefsKeys.FlourStoredResourceKey);
        LoadQueueCount(PlayerPrefsKeys.FlourQueueCountKey);
        Debug.Log("GETGETGETSAVE FOR FLOUR:LoadedTime:" + LoadSavedTime(PlayerPrefsKeys.FlourLastSavedTimeKey) + " storedValue:" + PlayerPrefsHelper.LoadInt(PlayerPrefsKeys.FlourStoredResourceKey) + " queuecount:" + PlayerPrefsHelper.LoadInt(PlayerPrefsKeys.FlourQueueCountKey));
        if (lastSavedTime != default)
        {
            Debug.Log("Kay�tl� zaman varm�i�");
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
    protected override async UniTask CalculateProductionOnLoad()//E�er kay�tl� bir zaman var ise, kald�g� yerden devam ettirir
    {
        //base olabilir
        if (lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - lastSavedTime;
            //if (_queueCount.Value <= 0)
            //{
            //    Debug.Log("QueueCount 0, �retim yap�lm�yor.");
            //    return;
            //}
            int maxProducedResources = (int)(timeDifference.TotalSeconds / ProductionTime);

            int producedResources = Mathf.Min(_queueCount.Value, maxProducedResources);
            //if (producedResources <= 0)
            //{
            //    Debug.Log("QueueCount 0, �retim yap�lm�yor.");
            //    return;
            //}

            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);
            Debug.Log("Ge�enS�re: " + timeDifference.TotalSeconds + "\n" +
                " maxProducedResources: " + maxProducedResources + "\n" +
            " producedResources: " + producedResources + "\n");


            _queueCount.Value -= producedResources;



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
            SaveQueueCount(PlayerPrefsKeys.FlourQueueCountKey);
        }
    }
    protected override void SAVE(string lastSavedKey, string storedKey, float curr = 0)
    {

        base.SAVE(lastSavedKey, storedKey, curr);
        SaveQueueCount(PlayerPrefsKeys.FlourQueueCountKey);
        Debug.Log("<color=ff0077 > SAVE FOR FLOUR:LoadedTime:" + LoadSavedTime(lastSavedKey) + " storedValue:" + PlayerPrefsHelper.LoadInt(storedKey) + " queuecount:" + PlayerPrefsHelper.LoadInt(PlayerPrefsKeys.FlourQueueCountKey) + "</color>");

    }
    protected override DateTime LoadSavedTime(string lastSavedKey)
    {
        return base.LoadSavedTime(lastSavedKey);
    }
    protected override void LoadStoredResources(string lastSavedKey)
    {
        base.LoadStoredResources(lastSavedKey);
        Debug.Log("Loaded Stored Resource Count" + PlayerPrefsHelper.LoadInt(lastSavedKey));
    }
    private void LoadQueueCount(string queueKey)
    {
        int productResources = PlayerPrefsHelper.LoadInt(queueKey);
        Debug.Log("Loaded Queue count: " + productResources);
        _queueCount.Value = productResources;
    }
    protected override void SaveCurrentTime(string lastSavedKey, float curr)
    {
        base.SaveCurrentTime(lastSavedKey, curr);
        Debug.Log("Zaman kaydedildi: " + lastSavedTime);
    }
    protected override void SaveStoredResources(string key)
    {
        PlayerPrefsHelper.SaveInt(key, StoredResources.Value);
    }
    private void SaveQueueCount(string queueKey)
    {
        PlayerPrefsHelper.SaveInt(queueKey, QueueCount.Value);
        Debug.Log("queue count saved:" + QueueCount.Value);
    }





    public override void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceCollector.SetResourceCapacityText(ResourceType, stored));
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
        SetResourceImage();
    }

    //Set
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        Debug.Log("ProduceWithSlider:1");
        // E�er zaten �retim yap�l�yorsa veya depo doluysa, metottan ��k
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("�retim yap�lm�yor veya kapasite dolmu�.");
            return;
        }

        // �retimi ba�lat
        SetIsProducing(true);

        // �retim s�recini ve slider'� takip et
        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            Debug.Log("ProduceWithSlider:2");
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;

            // Slider'� ve geri say�m� g�ncelle
            while (currentRemainingTime > 0)
            {
                //Debug.Log("ProduceWithSlider:3+++"+currentRemainingTime+ProductionTime);
                float targetValue = (float)currentRemainingTime / ProductionTime;
                _resourceCollector.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                curr = currentRemainingTime;
                _resourceCollector.SetSliderValue(ResourceType, targetValue);



                await UniTask.Delay(1000); // 1 saniye bekle
                currentRemainingTime--;
            }

            // �retim tamamland�, depoya ekle ve kuyruktan ��kar
            SetStoredResources(StoredResources.Value + 1);
            DecreaseQueueCount();

            if (IsSaveable)
            {
                SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);
            }

            // E�er kuyruk bo�ald�ysa, �retimi durdur
            if (_queueCount.Value <= 0)
            {
                Debug.Log("�retim kuyru�u bo�ald�, �retim durduruluyor.");
                break;
            }
        }
        Debug.Log("ProduceWithSlider:4");
        // �retim tamamland�, slider'� ve metni g�ncelle
        if (StoredResources.Value >= MaxCapacity)
        {
            _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
        }
        else
        {
            _resourceCollector.SetProductionTimerText(ResourceType, "FINISH");
        }
        _resourceCollector.SetSliderValue(ResourceType, 1);
        if (IsSaveable)
        {
            SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);
        }
        // �retimi durdur
        SetIsProducing(false);
    }
    public float curr;//degiskeni d�zelt
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0)
        {
            return 0;
        }

        int collected = StoredResources.Value;
        SetStoredResources(0);
        if (IsSaveable)
        {
            SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey, curr);
        }
        _resourceManager.AddResource(ResourceType, collected);

        if (!IsProducing)
        {
            await UniTask.WhenAll(
            //Produce(),
            //SliderTask()
            ProduceWithSlider()
            );
        }
        return collected;
    }
    public async UniTask AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing)
            return;

        //await UniTask.WhenAll(
        //    //Produce(),
        //    //SliderTask()
        //    ProduceWithSlider()
        //);
        if (lastSavedTime == default)
        {
            Debug.Log("Kaydedilen zaman bulunamad�. Yeni �retim ba�lat�l�yor.");
            SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);

            await UniTask.WhenAll(
             ProduceWithSlider(0)
            );

        }
        if (IsSaveable)
        {
            SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);
            var task2 = UniTask.WhenAll(
                CalculateProductionOnLoad()
            );
           
        }
        else
        {
            var task2 = UniTask.WhenAll(
              ProduceWithSlider(0)
          );
            //SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);
        }
            //var task = UniTask.WhenAll(

            //         IsSaveable ? CalculateProductionOnLoad() : ProduceWithSlider(0)
            //     );

    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {
        if (_queueCount.Value <= 0)
            return;
       
        DecreaseQueueCount();
         //SAVE(PlayerPrefsKeys.FlourLastSavedTimeKey, PlayerPrefsKeys.FlourStoredResourceKey);

        _resourceManager.AddResource(type, quantity);
    }
    public void IncreaseQueueCount()
    {
        Debug.Log("SAVE EDIYORUM1");

        _queueCount.Value++;
        SaveQueueCount(PlayerPrefsKeys.FlourQueueCountKey);

    }
    public void DecreaseQueueCount()
    {
        Debug.Log("SAVE EDIYORUM2");
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--;
        }
        SaveQueueCount(PlayerPrefsKeys.FlourQueueCountKey);
    }
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.UpdateSliderSetActive(ResourceType, active);
    }
}
