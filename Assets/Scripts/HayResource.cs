using Cysharp.Threading.Tasks;
using System;
using UniRx;
using UnityEngine;
using Zenject;

public class HayResource : ResourceBase
{
    //public event Action<int> OnProductionTimeChanged;
    //public event Action<int> OnStoredResourcesChanged;

    

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
            Debug.Log("Value ARttý mý ?");
            //OnStoredResourcesChanged?.Invoke(storedResources.Value);
        }

        isProducing = false;
    }

    public async UniTask<int> CollectResources()
    {
        if (storedResources.Value == 0)
        {
            // Eðer hiç kaynak yoksa, toplama iþlemi yapýlmasýn
            return 0;
        }
       
        int collected = storedResources.Value;
        Debug.Log("Collect"+ collected);
        storedResources.Value = 0;
        //OnStoredResourcesChanged?.Invoke(0);

        TotalResourcesCount.Value += collected;
        // Only call Produce() if it's not already running
        if (!isProducing)
        {
            await Produce(); // Start production if it's not already running
        }

        return collected;
    }
}
