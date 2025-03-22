using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;

public abstract class ResourceDependentEntityGeneric<TResource> : ResourceDependentEntity where TResource : DependentResource 
{
    protected TResource _resourceDependentBase;
    protected ResourceManager _resourceManager;

    [Inject]
    public void ConstructBase(ResourceManager resourceManager, TResource resource)
    {     
        _resourceManager = resourceManager;

        _resourceDependentBase = resource;
        InitializeResource(_resourceDependentBase);

        SetQueueSubscribes();
        
        _resourceProductionImage.sprite = _requireResource.RequireResourceSprite;
        _resourceProductionQuantityText.text = "x" + _requireResource.RequireQuantity.ToString();

        _plusProductionOrderButton.onClick.RemoveAllListeners();
        _minusProductionOrderButton.onClick.RemoveAllListeners();

        _plusProductionOrderButton.onClick.AddListener(ProductionPlusButtonClicked);
        _minusProductionOrderButton.onClick.AddListener(ProductionMinusButtonClicked);

        _resourceManager.GetTotalResourceObservable(_requireResource.RequireResourceType)
            .Subscribe(_ => PlusMinusButtonInteractivity())
            .AddTo(this);
    }
    
    public async override UniTaskVoid Interact()
    {
        if (!_isProductionButtonOpen)
        {
            _isProductionButtonOpen = true;
            _productionButtonsParent.SetActive(true);
        }
        else
        {
            if (_resourceDependentBase != null)
            {
                await _resourceDependentBase.CollectResources();
            }
        }
    }
    public async void ProductionPlusButtonClicked()
    {
        await _resourceDependentBase.AddToQueue(_requireResource.RequireResourceType, _requireResource.RequireQuantity);
    }
    public void ProductionMinusButtonClicked()
    {
        _resourceDependentBase.RemoveFromQueue(_requireResource.RequireResourceType, _requireResource.RequireQuantity);
    }
    public void PlusMinusButtonInteractivity()
    {
        if (_resourceDependentBase == null || _resourceManager == null)
            return;

        int currentResourceCount = _resourceManager.GetTotalResourceCount(_requireResource.RequireResourceType);

        _plusProductionOrderButton.interactable = currentResourceCount >= _requireResource.RequireQuantity && _resourceDependentBase.QueueCount.Value < resourceInfo.maxCapacity;
        _minusProductionOrderButton.interactable = _resourceDependentBase.QueueCount.Value > 1;
    }

    public override void CloseProductionButton()
    {
        if (_isProductionButtonOpen)
        {
            _isProductionButtonOpen = false;
            _productionButtonsParent.SetActive(false);
        }
    }
    protected void SetQueueSubscribes()
    {
        _resourceDependentBase.QueueCount.Subscribe(_ => PlusMinusButtonInteractivity()).AddTo(this);
        _resourceDependentBase.QueueCount.Subscribe(count => SetProductionOrderText(count)).AddTo(this);       
    }
    
}
