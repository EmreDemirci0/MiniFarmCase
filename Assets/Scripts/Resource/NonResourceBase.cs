using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class NonResourceBase : ResourceBase
{


    [Inject]
    public NonResourceBase(ResourceManager resourceManager, ResourceCollector resourceCollector, ResourceType resourceType)
         : base(resourceManager, resourceCollector, resourceType)
    {


        var delay = UniTask.DelayFrame(1).ContinueWith(() => //
         {
             var task = UniTask.WhenAll(
                 Produce(),
                 SliderTask()
             );
         });
    }


    public override async UniTask Produce()
    {
        if (isProducing)
        {
            return;
        }

        isProducing = true;

        while (storedResources.Value < maxCapacity)
        {
            for (int i = productionTime; i > 0; i--)
            {
                await UniTask.Delay(1000);
            }

            storedResources.Value++;
        }

        isProducing = false;
    }

    public override async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            return 0;
        }

        int collected = storedResources.Value;
        storedResources.Value = 0;

        _resourceManager.AddResource(resourceType, collected);

        if (!isProducing)
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
                _resourceCollector.SetProductionTimerText(resourceType, "FULL");
                _resourceCollector.SetSlider(resourceType, 1);
            }
            else
            {
                int remainingTime = ProductionTime;

                while (remainingTime > 0)
                {
                    _resourceCollector.SetProductionTimerText(resourceType, remainingTime + "s");
                    float targetValue = (float)remainingTime / ProductionTime;

                    _resourceCollector.SetSlider(resourceType, targetValue);

                    await UniTask.Delay(1000);
                    remainingTime--;
                }
            }
            await UniTask.Yield();
        }
    }
}
