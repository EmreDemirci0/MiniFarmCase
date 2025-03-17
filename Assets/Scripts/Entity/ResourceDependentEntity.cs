using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public abstract class ResourceDependentEntity : EntityBase
{
    protected ResourceManager _resourceManager;

    [SerializeField] protected SCResources requireResource;
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
        resourceProductionQuantityText.text = "x" + requireResource.resourceQuantity.ToString();

        plusProductionOrderButton.onClick.AddListener(ProductionPlusButtonClicked);
        minusProductionOrderButton.onClick.AddListener(ProductionMinusButtonClicked);

        if (requireResource.resourceType == ResourceType.Hay)
        {
            _resourceManager.TotalHayCount
           .Subscribe(_ => UpdateButtonInteractivity())
           .AddTo(this);
        }
        else if (requireResource.resourceType == ResourceType.Flour)
        {
            _resourceManager.TotalFlourCount
         .Subscribe(_ => UpdateButtonInteractivity())
         .AddTo(this);
        }
    }

   
    public abstract void ProductionPlusButtonClicked();

    public abstract void ProductionMinusButtonClicked();

    public abstract void UpdateButtonInteractivity();
}
