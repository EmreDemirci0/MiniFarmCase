using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ResourceCollector : MonoBehaviour
{
    private Camera _camera;
    private HayResource _hayResource;
    private FlourResource _flourResource;

    [SerializeField] private TextMeshProUGUI totalHayCountText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI hayResourceCapacityText;
    [SerializeField] private TextMeshProUGUI hayProductionTimerText;

    [SerializeField] private TextMeshProUGUI totalFlourCountText;
    [SerializeField] private Slider flourFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI flourResourceCapacityText;
    [SerializeField] private TextMeshProUGUI flourProductionTimerText;

    private FlourEntity _currentFlourEntity;

    private ResourceManager _resourceManager;

    [Inject]
    public void Construct(HayResource hayResource, FlourResource flourResource, ResourceManager resourceManager)
    {
        _hayResource = hayResource;
        _flourResource = flourResource;
        _resourceManager = resourceManager;
    }
    private void Start()
    {
        _hayResource.StoredResources.Subscribe(UpdateHayStoredResources).AddTo(this);
        _flourResource.StoredResources.Subscribe(UpdateFlourStoredResources).AddTo(this);

        // Hay count'ý ve Flour count'ý güncellemeleri için dinliyoruz
        _resourceManager.TotalHayCount
            .Subscribe(hayCount => totalHayCountText.text = hayCount.ToString())
            .AddTo(this);

        _resourceManager.TotalFlourCount
            .Subscribe(flourCount => totalFlourCountText.text = flourCount.ToString())
            .AddTo(this);
    }
    private async void Awake()
    {
        _camera = Camera.main;
       await UniTask.WhenAll(
        StartProgressUpdateLoop()
    );
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
                    if (entity is FlourEntity flourEntity)
                    {
                        // FlourEntity'ye týklanýrsa butonu aktif et
                        _currentFlourEntity = flourEntity;
                        _currentFlourEntity.Interact();
                    }

                    else
                    {
                        entity.Interact();
                        // Baþka bir yere týklanýrsa aktif olan butonu kapat
                        if (_currentFlourEntity != null)
                        {
                            _currentFlourEntity.CloseProductionButton();
                        }
                    }
                }
            }
            else
            {
                // Eðer baþka bir yere týklanmýþsa tüm butonlarý kapat
                if (_currentFlourEntity != null)
                {
                    _currentFlourEntity.CloseProductionButton();
                }
            }

        }
    }


    private async UniTask StartProgressUpdateLoop()
    {
        while (true)
        {

            // Kapasiteyi kontrol et
            if (_hayResource.StoredResources.Value >= _hayResource.MaxCapacity)
            {
                hayProductionTimerText.text = "FULL"; // FULL yazýsý göster
                hayFactoryResourceSlider.DOValue(1f, 0.5f).SetEase(Ease.OutQuad); // Slider'ý FULL yap
            }
            else
            {

                int remainingTime = _hayResource.ProductionTime;
                // Yeni hasat baþladýðýnda slider'ý anýnda sýfýrla (animasyonsuz)
                hayFactoryResourceSlider.value = 0f;

                while (remainingTime > 0)
                {
                    //Debug.Log("Burada4:" + remainingTime);
                    hayProductionTimerText.text = remainingTime + "s";

                    float targetValue = (float)remainingTime / _hayResource.ProductionTime;

                    // Animasyonu 5-4-3-2-1 sýrasýyla yap
                    if (remainingTime < _hayResource.ProductionTime)
                    {
                        hayFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }
                    else
                    {
                        // Eðer yeni hasat baþladýysa, direkt deðeri ata (animasyonsuz)
                        hayFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }

                    await UniTask.Delay(1000); // 1 saniye bekle
                    remainingTime--;
                }
            }
            await UniTask.Yield(); // Frame kaçýrmamak için bekleme
        }
    }
    public async UniTask StartProgressUpdateLoop2()
    {
        while (true)
        {

            // Kapasiteyi kontrol et
            if (_flourResource.StoredResources.Value >= _flourResource.MaxCapacity)
            {
                flourProductionTimerText.text = "FULL"; // FULL yazýsý göster
                flourFactoryResourceSlider.DOValue(1f, 0.5f).SetEase(Ease.OutQuad); // Slider'ý FULL yap
            }
            else
            {

                int remainingTime = _flourResource.ProductionTime;
                // Yeni hasat baþladýðýnda slider'ý anýnda sýfýrla (animasyonsuz)
                flourFactoryResourceSlider.value = 0f;

                while (remainingTime > 0)
                {
                    //Debug.Log("Burada4:" + remainingTime);
                    flourProductionTimerText.text = remainingTime + "s";

                    float targetValue = (float)remainingTime / _flourResource.ProductionTime;

                    // Animasyonu 5-4-3-2-1 sýrasýyla yap
                    if (remainingTime < _flourResource.ProductionTime)
                    {
                        flourFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }
                    else
                    {
                        // Eðer yeni hasat baþladýysa, direkt deðeri ata (animasyonsuz)
                        flourFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }

                    await UniTask.Delay(1000); // 1 saniye bekle
                    remainingTime--;
                }
            }
            await UniTask.Yield(); // Frame kaçýrmamak için bekleme
        }
    }

    private void UpdateHayStoredResources(int stored)
    {
        hayResourceCapacityText.text = stored.ToString();  // Hay kapasitesini göster
    }
    private void UpdateFlourStoredResources(int stored)
    {
        flourResourceCapacityText.text = stored.ToString();  // Hay kapasitesini göster
    }



}