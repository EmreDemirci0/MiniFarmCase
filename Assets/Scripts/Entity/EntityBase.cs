using Cysharp.Threading.Tasks;
using UnityEngine;
public abstract class EntityBase : MonoBehaviour,IEntity
{
    public SCResources resourceInfo;
    public abstract UniTaskVoid Interact();
    protected abstract ResourceType GetResourceType();
    protected void InitializeResource<TResource>(TResource resource) where TResource : ResourceBase
    {
        resource.ResourceType = GetResourceType();
        resource.IsSaveable = resourceInfo.isSaveable;
        resource.SetSubscribes();
        resource.SetProductionValues(resourceInfo.productionTime, resourceInfo.maxCapacity);
    }

}
