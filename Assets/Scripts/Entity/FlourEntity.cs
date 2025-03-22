public class FlourEntity : ResourceDependentEntityGeneric<FlourResource>
{

    protected override ResourceType GetResourceType()
    {
        return ResourceType.Flour;
    }
}
