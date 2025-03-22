using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class DependentResource : ResourceBase
{
    private ReactiveProperty<int> _queueCount = new ReactiveProperty<int>(0);
    public IReadOnlyReactiveProperty<int> QueueCount => _queueCount;
    protected string _queueCountKey;
    public DependentResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
      : base(resourceManager, resourceCollector)
    {
        InitializeAsync().Forget();
    }

    public override async UniTaskVoid InitializeAsync()
    {
        await UniTask.DelayFrame(1);

        if (IsSaveable)
        {
            _lastSavedTime = LoadSavedTime(_lastSavedTimeKey);
            LoadStoredResources(_storedResourceKey);
            LoadQueueCount(_queueCountKey);

            if (_lastSavedTime == default)
            {
                Debug.Log("Kaydedilen zaman bulunamadý. Yeni üretim baþlatýlýyor.");
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }
        }


        await StartProductionAsync();
    }
    public override async UniTask StartProductionAsync()
    {
        await CalculateProductionOnLoad();
    }
    public override async UniTask CalculateProductionOnLoad()
    {
        if (_lastSavedTime != default)
        {
            TimeSpan timeDifference = DateTime.Now - _lastSavedTime;

            int maxProducedResources = (int)(timeDifference.TotalSeconds / ProductionTime);
            int producedResources = Mathf.Min(_queueCount.Value, maxProducedResources);

            var remainingResources = (timeDifference.TotalSeconds % ProductionTime);

            if (StoredResources.Value < MaxCapacity)
            {
                _queueCount.Value -= producedResources;
                Debug.Log(StoredResources.Value +"._."+ MaxCapacity + "Queue " + producedResources + " kadar azaldý");
            }

            int newStoredResources = Mathf.Min(MaxCapacity, StoredResources.Value + producedResources);
            Debug.Log("MaxCapacity"+ MaxCapacity+ " StoredResources.Value:"+ StoredResources.Value + " producedResources" + producedResources+ "    newStoredResources"+ newStoredResources+ " maxProducedResources "+ maxProducedResources+ " _queueCount.Value "+ _queueCount.Value);
            SetStoredResources(newStoredResources);

            if (newStoredResources < MaxCapacity)
            {

                double remainingTime = ProductionTime - remainingResources;
                _resourceCollector.SetSliderValue(ResourceType, (float)(remainingTime / ProductionTime));
                Debug.Log("ÜRETÝMEDEVAMEDÝYORUZ");
                await ProduceWithSlider((float)(remainingTime)); // Üretimi kaldýgý yerden devam ettir
            }
            else
            {
                _resourceCollector.SetSliderValue(ResourceType, 1);
                _resourceCollector.SetProductionTimerText(ResourceType, "FULL");
            }
            SaveQueueCount(_queueCountKey);
        }
    }
    public override async UniTask ProduceWithSlider(float remainingTime = 0)
    {
        // Eðer zaten üretim yapýlýyorsa veya depo doluysa, metottan çýk
        if (IsProducing || StoredResources.Value >= MaxCapacity)
        {
            Debug.Log("Üretim yapýlmýyor veya kapasite dolmuþ.");
            return;
        }

        SetIsProducing(true);

        while (_queueCount.Value > 0 && StoredResources.Value < MaxCapacity)
        {
            float currentRemainingTime = remainingTime > 0 ? remainingTime : ProductionTime;
            remainingTime = 0;
            while (currentRemainingTime > 0)
            {
                float targetValue = (float)currentRemainingTime / ProductionTime;
                _resourceCollector.SetProductionTimerText(ResourceType, Mathf.RoundToInt(currentRemainingTime) + "s");
                _resourceCollector.SetSliderValue(ResourceType, targetValue);

                currentRemainingTimeForSave = currentRemainingTime;

                Debug.Log("EKSÝLÝYOR"+ currentRemainingTime);
                await UniTask.Delay(1000); // 1 saniye bekle
                currentRemainingTime--;
            }

            SetStoredResources(StoredResources.Value + 1);//Depoya ekle
            DecreaseQueueCount();//kuyruktan çýkar

            if (IsSaveable)
            {
                SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            }

            if (_queueCount.Value <= 0)
            {
                Debug.Log("Üretim kuyruðu boþaldý, üretim durduruluyor.");
                break;
            }
        }

        _resourceCollector.SetProductionTimerText(ResourceType, StoredResources.Value >= MaxCapacity ? ConstantKeys.ProductionTimeFullText : ConstantKeys.ProductionTimeFinishText);
        _resourceCollector.SetSliderValue(ResourceType, 1);

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey);
        }

        SetIsProducing(false);
    }
    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0) return 0;

        int collected = StoredResources.Value;

        SetStoredResources(0);//toplandýðý için sýfýrla

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey, currentRemainingTimeForSave);
        }
        _resourceManager.AddResource(ResourceType, collected);

        if (!IsProducing)
        {
            await ProduceWithSlider();
        }
        return collected;
    }
    public async UniTask AddToQueue(ResourceType type, int quantity)
    {
        _resourceManager.RemoveResource(type, quantity);
        IncreaseQueueCount();

        if (_queueCount.Value <= 0 || IsProducing) return;

        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey);
            if (_lastSavedTime == default)
            {
                Debug.Log("1");
                Debug.Log("Kaydedilen zaman bulunamadý. Yeni üretim baþlatýlýyor.");
                //SaveDatas(_lastSavedTimeKey, _storedResourceKey);
                await ProduceWithSlider();
            }
            else
            {
                Debug.Log("2");
                await CalculateProductionOnLoad();
            }

           
        }
        else
        {
            Debug.Log("3");
            await ProduceWithSlider();
        }
    }
    public void RemoveFromQueue(ResourceType type, int quantity)
    {//eksi kullanýnca sapýtýyor.
        if (_queueCount.Value <= 0) {
            Debug.Log("Value<0");
            return; 
        }
        if (quantity<= 0)
        {
            Debug.Log("quantity<0");
            return;
        }
        if (IsSaveable)
        {
            SaveDatas(_lastSavedTimeKey, _storedResourceKey);
        }

            int amountToRemove = Mathf.Min(quantity, _queueCount.Value);
        _queueCount.Value -= amountToRemove;

        // Kaynaklarý geri ekle
        _resourceManager.AddResource(type, amountToRemove);

        // QueueCount deðerini kaydet
        SaveQueueCount(_queueCountKey);


        //_resourceManager.AddResource(type, quantity);
        //DecreaseQueueCount();     
    }





    public override void SetSubscribes()
    {
        base.SetSubscribes();
        StoredResources.Subscribe(_ => SetSliderActive());
        QueueCount.Subscribe(_ => SetSliderActive());
    }

    public void IncreaseQueueCount()
    {
        _queueCount.Value ++;
        SaveQueueCount(_queueCountKey);

    }
    public void DecreaseQueueCount()
    {
        if (_queueCount.Value > 0)
        {
            _queueCount.Value--;
        }
        SaveQueueCount(_queueCountKey);
    }
    protected void SetSliderActive()
    {
        bool active = !(!IsProducing && StoredResources.Value <= 0 && QueueCount.Value <= 0);
        _resourceCollector.UpdateSliderSetActive(ResourceType, active);
    }

    protected override void SaveDatas(string lastSavedKey, string storedKey, float curr = 0)
    {
        Debug.Log("SAVED");
        base.SaveDatas(lastSavedKey, storedKey, curr);
        SaveQueueCount(_queueCountKey);    
    }

    private void LoadQueueCount(string queueKey)
    {
        int productResources = PlayerPrefsHelper.LoadInt(queueKey);
        //Debug.Log("Loaded Queue count: " + productResources);
        _queueCount.Value = productResources;
    }
    private void SaveQueueCount(string queueKey)
    {
        PlayerPrefsHelper.SaveInt(queueKey, QueueCount.Value);
        //Debug.Log("queue count saved:" + QueueCount.Value);
    }

}
