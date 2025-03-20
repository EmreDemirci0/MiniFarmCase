using Zenject;
 
public class HayEntity : NonResourceEntity
{  
    
    [Inject]
    public void Construct(HayResource hayResource)
    {
        _resourceNon = hayResource;

        _resourceNon.SetProductionValues(resourceInfo.productionTime, resourceInfo.maxCapacity);   
    }    
}
