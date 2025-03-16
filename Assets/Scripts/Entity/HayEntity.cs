using UnityEngine;
using Zenject;

public class HayEntity : MonoBehaviour, IEntity
{
    private HayResource _hayResource;
    [SerializeField] private int productionTime = 5; // Inspectordan ayarlanabilir
    [SerializeField] private int maxCapacity = 5; // Inspectordan ayarlanabilir



    [Inject]
    public void Construct(HayResource hayResource)
    {
        _hayResource = hayResource;
        _hayResource.SetProductionValues(productionTime, maxCapacity);

      

    }
    private async void Start()
    {
        await _hayResource.Produce(); // Üretimi baþlat
    }
    public async void Interact()
    {
        
        if (_hayResource != null)
        {
            await _hayResource.CollectResources();
        }
    }
}
