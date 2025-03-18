using Cysharp.Threading.Tasks;
using DG.Tweening;
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

    private FlourEntity _currentFlourEntity;
    private BreadV1Entity _currentBreadV1Entity;


    [Inject]
    public void Construct(HayEntity hayEntity, FlourEntity flourEntity)
    {

        _hayEntity = hayEntity;
        _flourEntity = flourEntity;

        hayFactoryResourceImage.sprite = _hayEntity.resourceInfo.resourceSprite;
        flourFactoryResourceImage.sprite = _flourEntity.resourceInfo.resourceSprite;

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
                    if (entity is FlourEntity flourEntity)
                    {
                        // FlourEntity'ye týklanýrsa butonu aktif et
                        _currentFlourEntity = flourEntity;
                        _currentFlourEntity.Interact();
                    }
                    else if (entity is BreadV1Entity breadEntity)
                    {
                        _currentBreadV1Entity = breadEntity;
                        _currentBreadV1Entity.Interact();
                    }

                    else
                    {
                        entity.Interact();
                        // Baþka bir yere týklanýrsa aktif olan butonu kapat
                        if (_currentFlourEntity != null) _currentFlourEntity.CloseProductionButton();
                        if (_currentBreadV1Entity != null) _currentBreadV1Entity.CloseProductionButton();
                    }
                }
            }
            else
            {
                // Eðer baþka bir yere týklanmýþsa tüm butonlarý kapat
                if (_currentFlourEntity != null) _currentFlourEntity.CloseProductionButton();
                if (_currentBreadV1Entity != null) _currentBreadV1Entity.CloseProductionButton();
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
    }


    public void SetTotalHayText(int hayCount)
    {
        totalHayCountText.text = hayCount.ToString();
    }
    public void SetTotalFlourText(int flourCount)
    {
        totalFlourCountText.text = flourCount.ToString();
    }
    public void SetTotalBreadText(int flourCount)
    {
        totalBreadCountText.text = flourCount.ToString();
    }
    //public void SetHayStoredResourcesText(int stored)
    //{
    //    hayResourceCapacityText.text = stored.ToString();
    //}
    //public void SetFlourStoredResourcesText(int stored)
    //{
    //    flourResourceCapacityText.text = stored.ToString();
    //}
    //public void SetBreadV1StoredResourcesText(int stored)
    //{
    //    breadV1ResourceCapacityText.text = stored.ToString();
    //}
    public void SetStoredResourcesText(ResourceType resourceType,int stored)
    {
        if(resourceType==ResourceType.Hay)
        hayResourceCapacityText.text = stored.ToString();
        else if (resourceType == ResourceType.Flour)
            flourResourceCapacityText.text = stored.ToString();
        else if (resourceType == ResourceType.BreadV1)
            breadV1ResourceCapacityText.text = stored.ToString();
    }
    //public void SetFlourSliderActive(bool isActive)
    //{
    //    flourFactoryResourceSlider.gameObject.SetActive(isActive);
    //}
    //public void SetBreadV1SliderActive(bool isActive)
    //{
    //    breadV1FactoryResourceSlider.gameObject.SetActive(isActive);
    //}
    public void SetSliderActive(ResourceType resourceType,bool isActive)
    {
        if (resourceType==ResourceType.Hay)
            hayFactoryResourceSlider.gameObject.SetActive(isActive);
        if (resourceType == ResourceType.Flour)
            flourFactoryResourceSlider.gameObject.SetActive(isActive);
        if (resourceType == ResourceType.BreadV1)
            breadV1FactoryResourceSlider.gameObject.SetActive(isActive);
    }
}