using Cysharp.Threading.Tasks;
using UniRx;
using Zenject; 
public abstract class NonResource : ResourceBase
{
    
    [Inject]
    public NonResource(ResourceManager resourceManager , EntityBase entityBase)
         : base(resourceManager, entityBase)
    {
     
        var delay = UniTask.DelayFrame(1).ContinueWith(() => 
         {
             var task = UniTask.WhenAll(
                 Produce(),
                 SliderTask()
             );
         });
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
        _resourceManager.AddResource(resourceType, collected);

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
                _entityBase.SetProductionTimerText("FULL");
                _entityBase.SetSliderValue(1);
            }
            else
            {
                int remainingTime = ProductionTime;

                while (remainingTime > 0)
                {
                    _entityBase.SetProductionTimerText(remainingTime + "s");
                    float targetValue = (float)remainingTime / ProductionTime;

                    _entityBase.SetSliderValue(targetValue);


                    await UniTask.Delay(1000);
                    remainingTime--;
                }
            }
            await UniTask.Yield();
        }
    }
}
