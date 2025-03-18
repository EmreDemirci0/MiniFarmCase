using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;

public class BreadV1Entity : ResourceDependentEntity
{
    private BreadV1Resource _breadV1Resource;
    [Inject]
    public void Construct(BreadV1Resource breadV1Resource)
    {
        // Base class'taki ConstructBase metodunu çaðýr
        //ConstructBase(resourceManager);

        _breadV1Resource = breadV1Resource;
        _breadV1Resource.SetProductionValues(productionTime, maxCapacity);


        _breadV1Resource.QueueCount.Subscribe(_ => UpdateButtonInteractivity()).AddTo(this);
        _breadV1Resource.QueueCount.Subscribe(count => SetProductionOrderText(count)).AddTo(this);

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
            if (_breadV1Resource != null)
            {
                await _breadV1Resource.CollectResources();
            }
        }
    }
    public async override void ProductionPlusButtonClicked()
    {
        Debug.Log("Butona bastýk");
        await _breadV1Resource.AddToQueue(requireResource.resourceType, resourceQuantity);
    }
    public  override void ProductionMinusButtonClicked()
    {
       
        _breadV1Resource.RemoveFromQueue(requireResource.resourceType, resourceQuantity);
    }

   

    public override void UpdateButtonInteractivity()
    {
        if (_breadV1Resource == null || _resourceManager == null)
            return;

        int currentResourceCount = _resourceManager.GetTotalResourceCount(requireResource.resourceType);
        plusProductionOrderButton.interactable = currentResourceCount >= resourceQuantity && _breadV1Resource.QueueCount.Value < maxCapacity;
        minusProductionOrderButton.interactable = _breadV1Resource.QueueCount.Value > 1;
    }

    public void CloseProductionButton()
    {
        isProductionButtonOpen = false;
        productionButtonsParent.SetActive(false);
    }
    public void SetProductionOrderText(int count)
    {
        productionOrderText.text = $"{count}/{maxCapacity}";
    }
}
