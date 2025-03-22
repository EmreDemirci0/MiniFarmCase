public class HayResource : NonResource
{
    public HayResource(ResourceManager resourceManager)
        : base(resourceManager)
    {
        _lastSavedTimeKey = ConstantKeys.HayLastSavedTimeKey;
        _storedResourceKey = ConstantKeys.HayStoredResourceKey;
    }
}