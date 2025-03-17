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
    private HayResource _hayResource;
    private FlourResource _flourResource;

    [SerializeField] private TextMeshProUGUI totalHayCountText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI hayResourceCapacityText;
    [SerializeField] private TextMeshProUGUI hayProductionTimerText;

    [SerializeField] private TextMeshProUGUI totalFlourCountText;
    [SerializeField] private  /*public*/ Slider flourFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI flourResourceCapacityText;
    [SerializeField] private/*public*/ TextMeshProUGUI flourProductionTimerText;

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


        _flourResource.StoredResources.Subscribe(_ => UpdateFlourSlider()).AddTo(this);
        _flourResource.QueueCount.Subscribe(_ => UpdateFlourSlider()).AddTo(this);


        // Hay count'� ve Flour count'� g�ncellemeleri i�in dinliyoruz
        _resourceManager.TotalHayCount
            .Subscribe(hayCount => totalHayCountText.text = hayCount.ToString())
            .AddTo(this);

        _resourceManager.TotalFlourCount
            .Subscribe(flourCount => totalFlourCountText.text = flourCount.ToString())
            .AddTo(this);
    }
    private void Awake()
    {
        _camera = Camera.main;


    }
    private void Update()
    {
        //Debug.Log("isproduct:" + _flourResource.IsProducing);
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
                    //Debug.Log("hit game.name"+hit.transform.name);
                    if (entity is FlourEntity flourEntity)
                    {
                        // FlourEntity'ye t�klan�rsa butonu aktif et
                        _currentFlourEntity = flourEntity;
                        _currentFlourEntity.Interact();
                    }

                    else
                    {
                        entity.Interact();
                        // Ba�ka bir yere t�klan�rsa aktif olan butonu kapat
                        if (_currentFlourEntity != null)
                        {
                            _currentFlourEntity.CloseProductionButton();
                        }
                    }
                }
            }
            else
            {
                // E�er ba�ka bir yere t�klanm��sa t�m butonlar� kapat
                if (_currentFlourEntity != null)
                {
                    _currentFlourEntity.CloseProductionButton();
                }
            }

        }
    }


    public async UniTask StartProgressUpdateLoop()
    {
        while (true)
        {

            // Kapasiteyi kontrol et
            if (_hayResource.StoredResources.Value >= _hayResource.MaxCapacity)
            {
                hayProductionTimerText.text = "FULL"; // FULL yaz�s� g�ster
                hayFactoryResourceSlider.DOValue(1f, 0.5f).SetEase(Ease.OutQuad); // Slider'� FULL yap
            }
            else
            {

                int remainingTime = _hayResource.ProductionTime;
                // Yeni hasat ba�lad���nda slider'� an�nda s�f�rla (animasyonsuz)
                hayFactoryResourceSlider.value = 0f;

                while (remainingTime > 0)
                {
                    //Debug.Log("Burada4:" + remainingTime);
                    hayProductionTimerText.text = remainingTime + "s";

                    float targetValue = (float)remainingTime / _hayResource.ProductionTime;

                    // Animasyonu 5-4-3-2-1 s�ras�yla yap
                    if (remainingTime < _hayResource.ProductionTime)
                    {
                        hayFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }
                    else
                    {
                        // E�er yeni hasat ba�lad�ysa, direkt de�eri ata (animasyonsuz)
                        hayFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);
                    }

                    await UniTask.Delay(1000); // 1 saniye bekle
                    remainingTime--;
                }
            }
            await UniTask.Yield(); // Frame ka��rmamak i�in bekleme
        }
    }
    public async UniTask FlourSliderTask()
    {
        if (!_flourResource.IsProducing)
        {
            Debug.Log("�retim ba�lamad�, slider duruyor.");
            return;
        }
        while (_flourResource.IsProducing)
        {
            int remainingTime = _flourResource.ProductionTime;
            flourFactoryResourceSlider.DOValue(0, flourFactoryResourceSlider.value);

            while (remainingTime > 0 && _flourResource.IsProducing)
            {
                flourProductionTimerText.text = remainingTime + "s";

                float targetValue = (float)remainingTime / _flourResource.ProductionTime;

                flourFactoryResourceSlider.DOKill();
                flourFactoryResourceSlider.DOValue(targetValue, 1f).SetEase(Ease.Linear);

                await UniTask.WhenAny(
                  UniTask.Delay(1000), // 1 saniye bekleme
                  UniTask.WaitUntil(() => !_flourResource.IsProducing) // E�er �retim biterse hemen ��k
                );

                if (!_flourResource.IsProducing)
                {
                    if (_flourResource.StoredResources.Value >= _flourResource.MaxCapacity)
                    {
                        //Debug.Log("�retim bitti, depo FULL oldu.");
                        flourFactoryResourceSlider.DOKill();
                        flourFactoryResourceSlider.DOValue(1f, 0.3f).SetEase(Ease.Linear); 
                        flourProductionTimerText.text = "FULL";
                    }
                    else
                    {
                        //Debug.Log("�retim bitti, slider s�f�rland�.");
                        flourFactoryResourceSlider.DOKill();
                        flourFactoryResourceSlider.DOValue(1, .3f).SetEase(Ease.Linear);
                        flourProductionTimerText.text = "FINISH"; 
                    }
                    return;
                }
                remainingTime--;
            }

        }
    }

    private void UpdateHayStoredResources(int stored)
    {
        hayResourceCapacityText.text = stored.ToString();  // Hay kapasitesini g�ster
    }
    private void UpdateFlourStoredResources(int stored)
    {
        flourResourceCapacityText.text = stored.ToString();  // Flour kapasitesini g�ster
    }
    public void UpdateFlourSlider()
    {
        //Debug.Log("G�RD�KK: " + _flourResource.IsProducing + " STORED" + _flourResource.StoredResources.Value);
        bool active = !(!(_flourResource.IsProducing) && _flourResource.StoredResources.Value <= 0 && _flourResource.QueueCount.Value <= 0);
        flourFactoryResourceSlider.gameObject.SetActive(active);
    }



}