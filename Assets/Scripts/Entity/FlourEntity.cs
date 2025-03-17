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
        _flourResource.OnQueueCountChanged.Subscribe(count => UpdateProductionOrderText(count)).AddTo(this);
    }
    
    
    public override void UpdateButtonInteractivity()
    {
        if (_flourResource == null || _resourceManager == null) 
            return; 

        int currentResourceCount = _resourceManager.GetTotalResourceCount(requireResource.resourceType);

        plusProductionOrderButton.interactable = currentResourceCount >= requireResource.resourceQuantity && _flourResource.QueueCount.Value < maxCapacity;
        minusProductionOrderButton.interactable = _flourResource.QueueCount.Value > 1;
    }
    public override async void ProductionPlusButtonClicked()
    {
        await _flourResource.AddToQueue(requireResource.resourceType, requireResource.resourceQuantity);
    }

    public override void ProductionMinusButtonClicked()
    {
        _flourResource.RemoveFromQueue(requireResource.resourceType, requireResource.resourceQuantity);
    }
    public void UpdateProductionOrderText(int count)
    { 
        productionOrderText.text = $"{count}/{maxCapacity}";
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
    public void CloseProductionButton()
    {
        isProductionButtonOpen = false;
        productionButtonsParent.SetActive(false);
    }
}
