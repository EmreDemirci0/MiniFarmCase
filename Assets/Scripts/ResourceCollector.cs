using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Zenject;

public class ResourceCollector : MonoBehaviour
{

    private Camera _camera;
    private HayEntity _hayEntity;
    private FlourEntity _flourEntity;

    [SerializeField] private TextMeshProUGUI totalHayCountText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    [SerializeField] private Image hayFactoryResourceImage;
    [SerializeField] private TextMeshProUGUI hayResourceCapacityText;
    [SerializeField] private TextMeshProUGUI hayProductionTimerText;

    [SerializeField] private TextMeshProUGUI totalFlourCountText;
    [SerializeField] private Slider flourFactoryResourceSlider;
    [SerializeField] private Image flourFactoryResourceImage;
    [SerializeField] private TextMeshProUGUI flourResourceCapacityText;
    [SerializeField] private TextMeshProUGUI flourProductionTimerText;

    [SerializeField] private TextMeshProUGUI totalBreadCountText;
    [SerializeField] private Slider breadV1FactoryResourceSlider;
    [SerializeField] private Image breadV1FactoryResourceImage;
    [SerializeField] private TextMeshProUGUI breadV1ResourceCapacityText;
    [SerializeField] private TextMeshProUGUI breadV1ProductionTimerText;
    [SerializeField] private Slider breadV2FactoryResourceSlider;
    [SerializeField] private Image breadV2FactoryResourceImage;
    [SerializeField] private TextMeshProUGUI breadV2ResourceCapacityText;
    [SerializeField] private TextMeshProUGUI breadV2ProductionTimerText;

    private List<ResourceDependentEntity> _resources = new List<ResourceDependentEntity>();


    [Inject]
    public void Construct(HayEntity hayEntity, FlourEntity flourEntity)
    {
        //BURAYA EL AT BU REZÝÝLÝK NE
        Debug.Log(" //BURAYA EL AT BU REZÝÝLÝK NE");
        _hayEntity = hayEntity;
        _flourEntity = flourEntity;

        hayFactoryResourceImage.sprite = _hayEntity.resourceInfo.resourceSprite;
        flourFactoryResourceImage.sprite = _flourEntity.resourceInfo.resourceSprite;
        breadV1FactoryResourceImage.sprite = _flourEntity.resourceInfo.resourceSprite;//?
        breadV2FactoryResourceImage.sprite = _flourEntity.resourceInfo.resourceSprite;//?

    }
    private void Start()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            //burayý refactor !!!
            if (Physics.Raycast(ray, out hit))
            {
                IEntity entity = hit.collider.GetComponent<IEntity>();
                if (entity != null)
                {
                    //Debug.Log("hit game.name"+hit.transform.name);
                    if (entity is ResourceDependentEntity resourceDependentEntity)
                    {
                        if (!_resources.Contains(resourceDependentEntity))
                        {
                            _resources.Add(resourceDependentEntity);
                        }
                        entity.Interact();
                    }
                    else
                    {
                        entity.Interact();

                        foreach (var item in _resources)
                            item.CloseProductionButton();
                    }
                }
            }
            else
            {
                foreach (var item in _resources)
                    item.CloseProductionButton();
            }

        }
    }

    public void SetProductionTimerText(ResourceType resourceType, string text)
    {
        if (resourceType == ResourceType.Hay)
            hayProductionTimerText.text = text;
        else if (resourceType == ResourceType.Flour)
            flourProductionTimerText.text = text;
        else if (resourceType == ResourceType.BreadV1)
            breadV1ProductionTimerText.text = text;
        else if (resourceType == ResourceType.BreadV2)
            breadV2ProductionTimerText.text = text;
    }
    public void SetSlider(ResourceType resourceType, float targetValue)
    {
        if (resourceType == ResourceType.Hay)
        {
            hayFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
        }
        else if (resourceType == ResourceType.Flour)
        {
            flourFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
        }
        else if (resourceType == ResourceType.BreadV1)
        {
            breadV1FactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
        }
        else if (resourceType == ResourceType.BreadV2)
        {
            breadV2FactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
        }
    }
    public void SetTotalText(ResourceType resourceType, int count)
    {
        if (resourceType == ResourceType.Hay)
            totalHayCountText.text = count.ToString();
        else if (resourceType == ResourceType.Flour)
            totalFlourCountText.text = count.ToString();
        else if (resourceType == ResourceType.BreadV1)//altla ayný
            totalBreadCountText.text = count.ToString();
        else if (resourceType == ResourceType.BreadV2)
            totalBreadCountText.text = count.ToString();
    }
    public void SetStoredResourcesText(ResourceType resourceType, int stored)
    {
        if (resourceType == ResourceType.Hay)
            hayResourceCapacityText.text = stored.ToString();
        else if (resourceType == ResourceType.Flour)
            flourResourceCapacityText.text = stored.ToString();
        else if (resourceType == ResourceType.BreadV1)
            breadV1ResourceCapacityText.text = stored.ToString();
        else if (resourceType == ResourceType.BreadV2)
            breadV2ResourceCapacityText.text = stored.ToString();
    }
    public void SetSliderActive(ResourceType resourceType, bool isActive)
    {
        if (resourceType == ResourceType.Hay)
            hayFactoryResourceSlider.gameObject.SetActive(isActive);
        if (resourceType == ResourceType.Flour)
            flourFactoryResourceSlider.gameObject.SetActive(isActive);
        if (resourceType == ResourceType.BreadV1)
            breadV1FactoryResourceSlider.gameObject.SetActive(isActive);
        if (resourceType == ResourceType.BreadV2)
            breadV2FactoryResourceSlider.gameObject.SetActive(isActive);
    }
}