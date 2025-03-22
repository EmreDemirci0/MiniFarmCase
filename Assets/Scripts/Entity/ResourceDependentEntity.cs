using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public abstract class ResourceDependentEntity : EntityBase
{
    [Header("Production Buttons")]
    [SerializeField] protected SCRequireResources _requireResource;
    [SerializeField] protected TextMeshProUGUI _productionOrderText;
    [SerializeField] protected GameObject _productionButtonsParent;
    [SerializeField] protected Button _plusProductionOrderButton;
    [SerializeField] protected Button _minusProductionOrderButton;
    [SerializeField] protected Image _resourceProductionImage;
    [SerializeField] protected TextMeshProUGUI _resourceProductionQuantityText;
    protected bool _isProductionButtonOpen = false;

    public abstract void CloseProductionButton();
    public override abstract UniTaskVoid Interact();
    protected void SetProductionOrderText(int count)
    {
        _productionOrderText.text = $"{count}/{resourceInfo.maxCapacity}";
    }
}
