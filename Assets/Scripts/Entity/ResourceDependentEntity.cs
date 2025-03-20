using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject.ReflectionBaking.Mono.Cecil;

public abstract class ResourceDependentEntity : EntityBase
{
    protected DependentResource _resourceDependentBase;
    protected ResourceManager _resourceManager;

    [Header("Production Buttons")]
    [SerializeField] protected SCRequireResources _requireResource;
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
        if (resourceProductionImage==null)
        {
            Debug.Log("EEEEEEEEEEEEE");
        }
        resourceProductionImage.sprite = _requireResource.RequireResourceSprite;
        resourceProductionQuantityText.text = "x" + _requireResource.RequireQuantity.ToString();

        //Debug.LogError("CONSTRUCT BASE CALISIYPOR" + this.gameObject.name);
        plusProductionOrderButton.onClick.AddListener(ProductionPlusButtonClicked);
        minusProductionOrderButton.onClick.AddListener(ProductionMinusButtonClicked);

        _resourceManager.GetTotalResourceObservable(_requireResource.RequireResourceType)
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
        Debug.Log("SIRAYA ALDIK");
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

        plusProductionOrderButton.interactable = currentResourceCount >= _requireResource.RequireQuantity && _resourceDependentBase.QueueCount.Value < resourceInfo.maxCapacity;
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
        //Debug.LogError("SetProductionOrderText:"+this.gameObject.name);
        productionOrderText.text = $"{count}/{resourceInfo.maxCapacity}";
    }
}
