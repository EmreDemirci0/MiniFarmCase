public class FlourResource : DependentResource
{
    public FlourResource(ResourceManager resourceManager, ResourceCollector resourceCollector ) 
        : base(resourceManager, resourceCollector)
    {
        _lastSavedTimeKey = ConstantKeys.FlourLastSavedTimeKey;
        _storedResourceKey = ConstantKeys.FlourStoredResourceKey;
        _queueCountKey = ConstantKeys.FlourQueueCountKey;
    }
}
