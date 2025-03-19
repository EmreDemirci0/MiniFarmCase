using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
public abstract class EntityBase : MonoBehaviour,IEntity
{
    public SCResources resourceInfo;

    [Header("UI")]
    [SerializeField] private Slider factoryResourceSlider;
    [SerializeField] private Image factoryResourceImage;
    [SerializeField] private TextMeshProUGUI resourceCapacityText;
    [SerializeField] private TextMeshProUGUI productionTimerText;
    
    [Header("Production")]
    [SerializeField] protected int productionTime = 5; 
    [SerializeField] protected int maxCapacity = 5;
    public abstract UniTaskVoid Interact();

    private void Start()
    {
        factoryResourceImage.sprite = resourceInfo.resourceSprite;
    }
    public void SetSliderValue(float value)
    {
        factoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
    }
    public void SetProductionTimerText(string text)
    {
        productionTimerText.text = text;
    }
    public void SetResourceCapacityText(int stored)
    {
        resourceCapacityText.text = stored.ToString();
    }
    public void UpdateSliderSetActive(bool active)
    {
        factoryResourceSlider.gameObject.SetActive(active);
    }
}
