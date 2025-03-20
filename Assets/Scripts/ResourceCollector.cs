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
    private List<ResourceDependentEntity> _resources = new List<ResourceDependentEntity>();
    private ResourceDependentEntity currentOpenResource;

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
                    if (entity is ResourceDependentEntity resourceDependentEntity)
                    {
                        if (currentOpenResource != null && currentOpenResource != resourceDependentEntity)
                        {
                            currentOpenResource.CloseProductionButton();
                        }

                        if (currentOpenResource != resourceDependentEntity)
                        {
                            currentOpenResource = resourceDependentEntity;
                            if (!_resources.Contains(resourceDependentEntity))
                                _resources.Add(resourceDependentEntity);
                        }

                        entity.Interact();
                    }
                    else
                    {
                        foreach (var item in _resources)
                            item.CloseProductionButton();
                        currentOpenResource = null;
                        entity.Interact();
                    }
                }
            }
            else
            {
                foreach (var item in _resources)
                    item.CloseProductionButton();
                currentOpenResource = null;
            }

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
    public void SetSliderValue(ResourceType type,float value)
    {
        if (type==ResourceType.Hay)
            hayFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
        else if (type == ResourceType.Flour)
            flourFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
        else if (type == ResourceType.BreadV1)
            breadV1FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
        else if (type == ResourceType.BreadV2)
            breadV2FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
        //factoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
    }
    public void SetProductionTimerText(ResourceType type,string text)
    {
        if (type == ResourceType.Hay)
            hayProductionTimerText.text = text;
        else if (type == ResourceType.Flour)
            flourProductionTimerText.text = text;
        else if (type == ResourceType.BreadV1)
            breadV1ProductionTimerText.text = text;
        else if (type == ResourceType.BreadV2)
            breadV2ProductionTimerText.text = text;
        //productionTimerText.text = text;
    }
    public void SetResourceCapacityText(ResourceType type,int stored)
    {
        if (type == ResourceType.Hay)
            hayResourceCapacityText.text = stored.ToString();
        else if (type == ResourceType.Flour)
            flourResourceCapacityText.text = stored.ToString();
        else if (type == ResourceType.BreadV1)
            breadV1ResourceCapacityText.text = stored.ToString();
        else if (type == ResourceType.BreadV2)
            breadV2ResourceCapacityText.text = stored.ToString();
        //resourceCapacityText.text = stored.ToString();
    }
    public void SetResourceImage(ResourceType type, Sprite sprite)
    {
        if (type == ResourceType.Hay)
            hayFactoryResourceImage.sprite= sprite;
        else if (type == ResourceType.Flour)
            flourFactoryResourceImage.sprite = sprite;
        else if (type == ResourceType.BreadV1)
            breadV1FactoryResourceImage.sprite = sprite;
        else if (type == ResourceType.BreadV2)
            breadV2FactoryResourceImage.sprite = sprite;
        //resourceCapacityText.text = stored.ToString();
    }
    public void UpdateSliderSetActive(ResourceType type,bool active)
    {
        Debug.LogError("SETSLÝDERACTÝVE:"+type+active);
        if (type == ResourceType.Hay)
            hayFactoryResourceSlider.gameObject.SetActive(active);
        else if (type == ResourceType.Flour)
            flourFactoryResourceSlider.gameObject.SetActive(active);
        else if (type == ResourceType.BreadV1)
            breadV1FactoryResourceSlider.gameObject.SetActive(active);
        else if (type == ResourceType.BreadV2)
            breadV2FactoryResourceSlider.gameObject.SetActive(active);
        //factoryResourceSlider.gameObject.SetActive(active);
    }
}