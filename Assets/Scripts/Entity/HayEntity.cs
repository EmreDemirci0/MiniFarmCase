using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
 
public class HayEntity : NonResourceEntity
{
    private HayResource _hayResource;

    [Inject]
    public void Construct(HayResource hayResource)
    {
        _hayResource = hayResource;
        
        _hayResource.SetProductionValues(productionTime, maxCapacity);   
    }
   
    private void Update()
    {
        //Debug.Log("ProductionTime:"+ _hayResource.ProductionTimeLeft);
    }
    public async override void Interact()
    {   
        if (_hayResource != null)
        {
            await _hayResource.CollectResources();
        }
    }
}
