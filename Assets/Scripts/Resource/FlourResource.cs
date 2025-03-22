public class FlourResource : DependentResource
{
    public FlourResource(ResourceManager resourceManager ) 
        : base(resourceManager)
    {
        _lastSavedTimeKey = ConstantKeys.FlourLastSavedTimeKey;
        _storedResourceKey = ConstantKeys.FlourStoredResourceKey;
        _queueCountKey = ConstantKeys.FlourQueueCountKey;
    }
}
