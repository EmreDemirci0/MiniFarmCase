using Cysharp.Threading.Tasks;
using Zenject;
using UniRx;
using UnityEditor;
using UnityEngine; 

public class FlourEntity : ResourceDependentEntity 
{
    private FlourResource _flourResource;
    
    [Inject]
    public void Construct(FlourResource flourResource)
    {
        // Base class'taki ConstructBase metodunu çaðýr
        //ConstructBase(resourceManager);

        _flourResource = flourResource;
        _flourResource.SetProductionValues(productionTime, maxCapacity);

       
        _flourResource.QueueCount.Subscribe(_ => UpdateButtonInteractivity()).AddTo(this);
        _flourResource.QueueCount.Subscribe(count => SetProductionOrderText(count)).AddTo(this);

    }
    public async override void Interact()
    {
        if (!isProductionButtonOpen)
        {
            isProductionButtonOpen = true;
            productionButtonsParent.SetActive(true);
        }
        else
        {
            if (_flourResource != null)
            {
                await _flourResource.CollectResources();
            }
        }
    }

    public override void UpdateButtonInteractivity()
    {
        if (_flourResource == null || _resourceManager == null) 
            return; 

        int currentResourceCount = _resourceManager.GetTotalResourceCount(requireResource.resourceType);

        plusProductionOrderButton.interactable = currentResourceCount >= resourceQuantity && _flourResource.QueueCount.Value < maxCapacity;
        minusProductionOrderButton.interactable = _flourResource.QueueCount.Value > 1;
    }
    public override async void ProductionPlusButtonClicked()
    {
        await _flourResource.AddToQueue(requireResource.resourceType, resourceQuantity);
    }

    public override void ProductionMinusButtonClicked()
    {
        _flourResource.RemoveFromQueue(requireResource.resourceType, resourceQuantity);
    }
    public void SetProductionOrderText(int count)
    { 
        productionOrderText.text = $"{count}/{maxCapacity}";
    }
    public void CloseProductionButton()
    {
        isProductionButtonOpen = false;
        productionButtonsParent.SetActive(false);
    }
}
