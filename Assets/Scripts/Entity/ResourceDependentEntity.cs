using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;

public abstract class ResourceDependentEntity : EntityBase
{
    protected DependentResource _resourceDependentBase;
    protected ResourceManager _resourceManager;

    [Header("Production Buttons")]
    [SerializeField] protected SCResources requireResource;
    [SerializeField] protected int resourceQuantity;
    [SerializeField] protected TextMeshProUGUI productionOrderText;
    [SerializeField] protected GameObject productionButtonsParent;
    [SerializeField] protected Button plusProductionOrderButton;
    [SerializeField] protected Button minusProductionOrderButton;
    [SerializeField] protected Image resourceProductionImage;
    [SerializeField] protected TextMeshProUGUI resourceProductionQuantityText;
    protected bool isProductionButtonOpen = false;

    [Inject]
    public void ConstructBase(ResourceManager resourceManager)
    {
        _resourceManager = resourceManager;

        resourceProductionImage.sprite = requireResource.resourceSprite;
        resourceProductionQuantityText.text = "x" + resourceQuantity.ToString();

        plusProductionOrderButton.onClick.AddListener(ProductionPlusButtonClicked);
        minusProductionOrderButton.onClick.AddListener(ProductionMinusButtonClicked);

        _resourceManager.GetTotalResourceObservable(requireResource.resourceType)
            .Subscribe(_ => PlusMinusButtonInteractivity())
            .AddTo(this);
    }


    public async override UniTaskVoid Interact()
    {
        if (!isProductionButtonOpen)
        {
            isProductionButtonOpen = true;
            productionButtonsParent.SetActive(true);
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
        await _resourceDependentBase.AddToQueue(requireResource.resourceType, resourceQuantity);
    }
    public void ProductionMinusButtonClicked()
    {
        _resourceDependentBase.RemoveFromQueue(requireResource.resourceType, resourceQuantity);
    }

    public void PlusMinusButtonInteractivity()
    {
        if (_resourceDependentBase == null || _resourceManager == null)
            return;

        int currentResourceCount = _resourceManager.GetTotalResourceCount(requireResource.resourceType);

        plusProductionOrderButton.interactable = currentResourceCount >= resourceQuantity && _resourceDependentBase.QueueCount.Value < maxCapacity;
        minusProductionOrderButton.interactable = _resourceDependentBase.QueueCount.Value > 1;
    }

    public void CloseProductionButton()
    {
        if (isProductionButtonOpen)
        {
            isProductionButtonOpen = false;
            productionButtonsParent.SetActive(false);
        }
    }
    protected void SetQueueSubscribes()
    {
        _resourceDependentBase.QueueCount.Subscribe(_ => PlusMinusButtonInteractivity()).AddTo(this);
        _resourceDependentBase.QueueCount.Subscribe(count => SetProductionOrderText(count)).AddTo(this);

    }
    protected void SetProductionOrderText(int count)
    {
        productionOrderText.text = $"{count}/{maxCapacity}";
    }
}
