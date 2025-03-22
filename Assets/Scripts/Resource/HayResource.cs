public class HayResource : NonResource
{
    public HayResource(ResourceManager resourceManager, ResourceCollector resourceCollector)
        : base(resourceManager, resourceCollector)
    {
        _lastSavedTimeKey = ConstantKeys.HayLastSavedTimeKey;
        _storedResourceKey = ConstantKeys.HayStoredResourceKey;
    }
}