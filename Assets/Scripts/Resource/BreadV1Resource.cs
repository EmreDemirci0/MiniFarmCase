public class BreadV1Resource : DependentResource
{   
    public BreadV1Resource(ResourceManager resourceManager)
        : base(resourceManager)
    {
        _lastSavedTimeKey = ConstantKeys.BreadV1LastSavedTimeKey;
        _storedResourceKey = ConstantKeys.BreadV1StoredResourceKey;
        _queueCountKey = ConstantKeys.BreadV1QueueCountKey;
    }
}
