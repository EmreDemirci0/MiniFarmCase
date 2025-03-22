public class BreadV2Resource : DependentResource
{
    public BreadV2Resource(ResourceManager resourceManager)
        : base(resourceManager)
    {
        _lastSavedTimeKey = ConstantKeys.BreadV2LastSavedTimeKey;
        _storedResourceKey = ConstantKeys.BreadV2StoredResourceKey;
        _queueCountKey = ConstantKeys.BreadV2QueueCountKey;
    }
}
