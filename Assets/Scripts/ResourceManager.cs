using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : MonoBehaviour
{


    public List<SCResources> _allResources;
    public ReactiveProperty<int> TotalHayCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalFlourCount { get; private set; } = new ReactiveProperty<int>(100);
    public ReactiveProperty<int> TotalBreadCount { get; private set; } = new ReactiveProperty<int>(0);


    private Camera _camera;
    private HashSet<ResourceDependentEntity> _resources = new HashSet<ResourceDependentEntity>();
    private ResourceDependentEntity _currentOpenResource;

    [Header("Total Counts UI's")]
    [SerializeField] private TextMeshProUGUI _totalHayCountText;
    [SerializeField] private TextMeshProUGUI _totalFlourCountText;
    [SerializeField] private TextMeshProUGUI _totalBreadCountText;

    [Header("Hay Entity UI's")]
    [SerializeField] private Slider _hayFactoryResourceSlider;
    [SerializeField] private Image _hayFactoryResourceImage;
    [SerializeField] private TextMeshProUGUI _hayResourceCapacityText;
    [SerializeField] private TextMeshProUGUI _hayProductionTimerText;

    [Header("Flour Entity UI's")]
    [SerializeField] private Slider _flourFactoryResourceSlider;
    [SerializeField] private Image _flourFactoryResourceImage;
    [SerializeField] private TextMeshProUGUI _flourResourceCapacityText;
    [SerializeField] private TextMeshProUGUI _flourProductionTimerText;

    [Header("BreadV1 Entity UI's")]
    [SerializeField] private Slider _breadV1FactoryResourceSlider;
    [SerializeField] private Image _breadV1FactoryResourceImage;
    [SerializeField] private TextMeshProUGUI _breadV1ResourceCapacityText;
    [SerializeField] private TextMeshProUGUI _breadV1ProductionTimerText;

    [Header("BreadV2 Entity UI's")]
    [SerializeField] private Slider _breadV2FactoryResourceSlider;
    [SerializeField] private Image _breadV2FactoryResourceImage;
    [SerializeField] private TextMeshProUGUI _breadV2ResourceCapacityText;
    [SerializeField] private TextMeshProUGUI _breadV2ProductionTimerText;

    private const string _totalHayCountKey = ConstantKeys.TotalHayCountKey;
    private const string _totalFlourCountKey =ConstantKeys.TotalFlourCountKey;
    private const string _totalBreadCountKey = ConstantKeys.TotalBreadCountKey;
    

    private void Start()
    {
        _camera = Camera.main;

        TotalHayCount.Value = PlayerPrefs.GetInt(_totalHayCountKey, 10);
        TotalFlourCount.Value = PlayerPrefs.GetInt(_totalFlourCountKey, 0);
        TotalBreadCount.Value = PlayerPrefs.GetInt(_totalBreadCountKey, 0);


        TotalHayCount.Subscribe(count =>
        {
            SetTotalText(ResourceType.Hay, count);
            PlayerPrefs.SetInt(_totalHayCountKey, count);
        }).AddTo(this);

        TotalFlourCount.Subscribe(count =>
        {
            SetTotalText(ResourceType.Flour, count);
            PlayerPrefs.SetInt(_totalFlourCountKey, count);
        }).AddTo(this);

        TotalBreadCount.Subscribe(count =>
        {
            SetTotalText(ResourceType.BreadV1, count);
            SetTotalText(ResourceType.BreadV2, count);
            PlayerPrefs.SetInt(_totalBreadCountKey, count);
        }).AddTo(this);

        PlayerPrefs.Save();
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
                        if (_currentOpenResource != null && _currentOpenResource != resourceDependentEntity)
                        {
                            _currentOpenResource.CloseProductionButton();
                        }

                        if (_currentOpenResource != resourceDependentEntity)
                        {
                            _currentOpenResource = resourceDependentEntity;
                            if (!_resources.Contains(resourceDependentEntity))
                                _resources.Add(resourceDependentEntity);
                        }

                        entity.Interact();
                    }
                    else
                    {
                        foreach (var item in _resources)
                            item.CloseProductionButton();
                        _currentOpenResource = null;
                        entity.Interact();
                    }
                }
            }
            else
            {
                foreach (var item in _resources)
                    item.CloseProductionButton();
                _currentOpenResource = null;
            }

        }
    }

    public void SetSliderValue(ResourceType type, float value)
    {
        switch (type)
        {
            case ResourceType.Hay:
                _hayFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.Flour:
                _flourFactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.BreadV1:
                _breadV1FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
                break;
            case ResourceType.BreadV2:
                _breadV2FactoryResourceSlider.DOValue(value, 1f).SetEase(Ease.Linear);
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
                _hayProductionTimerText.text = text;
                break;
            case ResourceType.Flour:
                _flourProductionTimerText.text = text;
                break;
            case ResourceType.BreadV1:
                _breadV1ProductionTimerText.text = text;
                break;
            case ResourceType.BreadV2:
                _breadV2ProductionTimerText.text = text;
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
                _hayResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.Flour:
                _flourResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.BreadV1:
                _breadV1ResourceCapacityText.text = stored.ToString();
                break;
            case ResourceType.BreadV2:
                _breadV2ResourceCapacityText.text = stored.ToString();
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
                _hayFactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.Flour:
                _flourFactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.BreadV1:
                _breadV1FactoryResourceImage.sprite = sprite;
                break;
            case ResourceType.BreadV2:
                _breadV2FactoryResourceImage.sprite = sprite;
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
                _hayFactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.Flour:
                _flourFactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.BreadV1:
                _breadV1FactoryResourceSlider.gameObject.SetActive(active);
                break;
            case ResourceType.BreadV2:
                _breadV2FactoryResourceSlider.gameObject.SetActive(active);
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }



    public void SetTotalText(ResourceType resourceType, int count)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                _totalHayCountText.text = count.ToString();
                break;
            case ResourceType.Flour:
                _totalFlourCountText.text = count.ToString();
                break;
            case ResourceType.BreadV1:
            case ResourceType.BreadV2:
                _totalBreadCountText.text = count.ToString();
                break;
            default:
                Debug.LogWarning("Unknown ResourceType");
                break;
        }
    }
    public int GetTotalResourceCount(ResourceType resourceType)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                return TotalHayCount.Value;
            case ResourceType.Flour:
                return TotalFlourCount.Value;
            case ResourceType.BreadV1:
                return TotalBreadCount.Value;
            case ResourceType.BreadV2:
                return TotalBreadCount.Value;
            default:
                return 0;
        }
    }
    public IObservable<int> GetTotalResourceObservable(ResourceType resourceType)
    {
        return resourceType switch
        {
            ResourceType.Hay => TotalHayCount,
            ResourceType.Flour => TotalFlourCount,
            ResourceType.BreadV1 => TotalBreadCount,
            ResourceType.BreadV2 => TotalBreadCount,
            _ => Observable.Return(0)
        };
    }


    public void AddResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                TotalHayCount.Value += quantity;
                break;
            case ResourceType.Flour:
                TotalFlourCount.Value += quantity;
                break;
            case ResourceType.BreadV1:
                TotalBreadCount.Value += quantity;
                break;
            case ResourceType.BreadV2:
                TotalBreadCount.Value += quantity;
                break;
        }
    }

    public void RemoveResource(ResourceType resourceType, int quantity)
    {
        switch (resourceType)
        {
            case ResourceType.Hay:
                if (TotalHayCount.Value >= quantity)
                {
                    TotalHayCount.Value -= quantity;
                }
                break;
            case ResourceType.Flour:
                if (TotalFlourCount.Value >= quantity)
                {
                    TotalFlourCount.Value -= quantity;
                }
                break;
            case ResourceType.BreadV1:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                }
                break;
            case ResourceType.BreadV2:
                if (TotalBreadCount.Value >= quantity)
                {
                    TotalBreadCount.Value -= quantity;
                }
                break;
        }
    }
}
