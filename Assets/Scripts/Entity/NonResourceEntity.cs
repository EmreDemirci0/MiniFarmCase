using Cysharp.Threading.Tasks;
using Zenject;
public abstract class NonResourceEntity<TResource> : EntityBase where TResource : NonResource
{
    protected TResource _resourceNon;

    [Inject]
    public void ConstructBase(TResource resource)
    {
        _resourceNon = resource;
        InitializeResource(_resourceNon);
    }
    public async override UniTaskVoid Interact()
    {
        if (_resourceNon != null)
        {
            await _resourceNon.CollectResources();
        }
    }
}