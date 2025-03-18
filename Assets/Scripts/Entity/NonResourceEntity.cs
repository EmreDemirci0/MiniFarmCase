using Cysharp.Threading.Tasks;
public abstract class NonResourceEntity : EntityBase
{
    protected NonResource _resourceNon;
    public async override UniTaskVoid Interact()
    {
        if (_resourceNon != null)
        {
            await _resourceNon.CollectResources();
        }
    }
}
