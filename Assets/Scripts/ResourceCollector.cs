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
    private HashSet<ResourceDependentEntity> _resources = new HashSet<ResourceDependentEntity>();
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

    public void SetSliderValue(ResourceType type, float value)
    {
        switch (type)
        {
            case ResourceType.Hay:
                hayFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.Flour:
                flourFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.BreadV1:
                breadV1FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.BreadV2:
                breadV2FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }
    public void SetProductionTimerText(ResourceType type, string text)
    {
        switch (type)
        {
            case ResourceType.Hay:
                hayProductionTimerText.text = text;
                break;
            case ResourceType.Flour:
                flourProductionTimerText.text = text;
                break;
            case ResourceType.BreadV1:
                breadV1ProductionTimerText.text = text;
                break;
            case ResourceType.BreadV2:
                breadV2ProductionTimerText.text = text;
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }
    public void SetResourceCapacityText(ResourceType type, int stored)
    {
        switch (type)
        {
            case ResourceType.Hay:
                hayResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.Flour:
                flourResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.BreadV1:
                breadV1ResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.BreadV2:
                breadV2ResourceCapacityText.text = stored.ToString();
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }
    public void SetResourceImage(ResourceType type, Sprite sprite)
    {
        switch (type)
        {
            case ResourceType.Hay:
                hayFactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.Flour:
                flourFactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.BreadV1:
                breadV1FactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.BreadV2:
                breadV2FactoryResourceImage.sprite = sprite;
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }

    public void UpdateSliderSetActive(ResourceType type, bool active)
    {
        switch (type)
        {
            case ResourceType.Hay:
                hayFactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.Flour:
                flourFactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.BreadV1:
                breadV1FactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.BreadV2:
                breadV2FactoryResourceSlider.gameObject.SetActive(active);
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }
}