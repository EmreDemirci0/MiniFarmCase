using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HayEntity : NonResourceEntity
{
    private HayResource _hayResource;
    private ResourceCollector _resourceCollector;
   



    [Inject]
    public void Construct(HayResource hayResource, ResourceCollector resourceCollector)
    {
        _hayResource = hayResource;
        _hayResource.SetProductionValues(productionTime, maxCapacity);


        _resourceCollector = resourceCollector;
    }
    private async void Start()
    {
        await UniTask.WhenAll(
            _hayResource.Produce(),
            _resourceCollector.StartProgressUpdateLoop()
        );
    }
    public async override void Interact()
    {
        
        if (_hayResource != null)
        {
            await _hayResource.CollectResources();
        }
    }
}
