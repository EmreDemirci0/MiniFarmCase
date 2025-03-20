using Cysharp.Threading.Tasks;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject; 
public abstract class NonResource : ResourceBase
{
    
    [Inject]
    public NonResource(ResourceManager resourceManager , ResourceCollector resourceCollector)
         : base(resourceManager, resourceCollector)
    { 
     
        var delay = UniTask.DelayFrame(1).ContinueWith(() => 
         {
             var task = UniTask.WhenAll(
                 Produce(),
                 SliderTask()
             );
         });
    }
    public override void SetSubscribes()
    {
        StoredResources.Subscribe(stored => _resourceCollector.SetResourceCapacityText(_resourceType, stored));
        SetResourceImage();
    }
    public override async UniTask Produce()
    {
        if (IsProducing)
        {
            return;
        }

        SetIsProducing(true);

        while (StoredResources.Value < MaxCapacity)
        {
            for (int i = ProductionTime; i > 0; i--)
            {
                await UniTask.Delay(1000);
            }

            SetStoredResources(StoredResources.Value+1);
        }

        SetIsProducing(false);
    }

    public override async UniTask<int> CollectResources()
    {
        if (StoredResources.Value == 0)
        {
            return 0;
        }

        int collected = StoredResources.Value;
        SetStoredResources(0);
        _resourceManager.AddResource(_resourceType, collected);

        if (!IsProducing)
        {
            await Produce();
        }

        return collected;
    }

    public async UniTask SliderTask()
    {
        while (true)
        {

            if (StoredResources.Value >= MaxCapacity)
            {
                _resourceCollector.SetProductionTimerText(_resourceType,"FULL");
                _resourceCollector.SetSliderValue(_resourceType,1);
                //_entityBase.SetProductionTimerText("FULL");
                //_entityBase.SetSliderValue(1);
            }
            else
            {
                int remainingTime = ProductionTime;

                while (remainingTime > 0)
                {
                    //_entityBase.SetProductionTimerText(remainingTime + "s");
                    float targetValue = (float)remainingTime / ProductionTime;
                    _resourceCollector.SetProductionTimerText(_resourceType, remainingTime + "s");
                    _resourceCollector.SetSliderValue(_resourceType, targetValue);
                    //_entityBase.SetSliderValue(targetValue);


                    await UniTask.Delay(1000);
                    remainingTime--;
                }
            }
            await UniTask.Yield();
        }
    }
}
