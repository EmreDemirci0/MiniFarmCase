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

    [SerializeField] private TextMeshProUGUI totalHayCountText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI hayResourceCapacityText;
    [SerializeField] private TextMeshProUGUI hayProductionTimerText;

    [SerializeField] private TextMeshProUGUI totalFlourCountText;
    [SerializeField] private Slider flourFactoryResourceSlider;
    [SerializeField] private TextMeshProUGUI flourResourceCapacityText;
    [SerializeField] private TextMeshProUGUI flourProductionTimerText;



    [Inject]
    public void Construct(HayResource hayResource)
    {
        _hayResource = hayResource;

        _hayResource.StoredResources.Subscribe(UpdateStoredResources).AddTo(this);
        ResourceBase.TotalResourcesCount.Subscribe(UpdateTotalCount).AddTo(this);

        //if (_currentResource is HayResource)
        //{
        //    hayUI = new ResourceUI(hayFactoryResourceSlider, hayResourceCapacityText, hayProductionTimerText, totalHayCountText);
        //    _currentResource.StoredResources.Subscribe(value => hayUI.UpdateUI(value, _currentResource.MaxCapacity, _currentResource.ProductionTime)).AddTo(this);
        //    // Hay i�in toplam kaynak say�s�n� g�nceller
        //    HayResource.TotalResourcesCount.Subscribe(UpdateTotalHayCount).AddTo(this);
        //}
        //else if (_currentResource is FlourResource)
        //{
        //    flourUI = new ResourceUI(flourFactoryResourceSlider, flourResourceCapacityText, flourProductionTimerText, totalFlourCountText);
        //    _currentResource.StoredResources.Subscribe(value => flourUI.UpdateUI(value, _currentResource.MaxCapacity, _currentResource.ProductionTime)).AddTo(this);
        //    // Flour i�in toplam kaynak say�s�n� g�nceller
        //    FlourResource.TotalResourcesCount.Subscribe(UpdateTotalFlourCount).AddTo(this);
        //}
    }

    private async void Awake()
    {
        _camera = Camera.main;
        await StartProgressUpdateLoop();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                IEntity entity = hit.collider.GetComponent<IEntity>();
                if (entity != null)
                {
                    entity.CollectResources();
                }
            }
        }
    }

    private void UpdateTotalCount(int totalCount)
    {
        
            totalHayCountText.text = totalCount.ToString();  // Hay toplam�n� g�ster
        
        //else if (_currentResource is FlourResource)
        //{
        //    totalFlourCountText.text = totalCount.ToString();  // Flour toplam�n� g�ster
        //}
    }

    private void UpdateStoredResources(int stored)
    {
        Debug.Log("Updated");
        // Hay kapasitesini g�ncelleme
        
            hayResourceCapacityText.text = stored.ToString();  // Hay i�in kapasiteyi g�ncelle
        
        //else if (_currentResource is FlourResource flourResource)
        //{
        //    flourResourceCapacityText.text = stored.ToString();  // Flour i�in kapasiteyi g�ncelle
        //}
    }




    private async UniTask StartProgressUpdateLoop()
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





}