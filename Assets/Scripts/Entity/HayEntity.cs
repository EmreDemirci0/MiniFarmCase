using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class HayEntity : MonoBehaviour, IEntity
{
    private HayResource _hayResource;
    private ResourceCollector _resourceCollector;
    [SerializeField] private int productionTime = 5; // Inspectordan ayarlanabilir
    [SerializeField] private int maxCapacity = 5; // Inspectordan ayarlanabilir



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
    public async void Interact()
    {
        
        if (_hayResource != null)
        {
            await _hayResource.CollectResources();
        }
    }
}
