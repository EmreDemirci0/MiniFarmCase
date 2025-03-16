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
        //    // Hay için toplam kaynak sayýsýný günceller
        //    HayResource.TotalResourcesCount.Subscribe(UpdateTotalHayCount).AddTo(this);
        //}
        //else if (_currentResource is FlourResource)
        //{
        //    flourUI = new ResourceUI(flourFactoryResourceSlider, flourResourceCapacityText, flourProductionTimerText, totalFlourCountText);
        //    _currentResource.StoredResources.Subscribe(value => flourUI.UpdateUI(value, _currentResource.MaxCapacity, _currentResource.ProductionTime)).AddTo(this);
        //    // Flour için toplam kaynak sayýsýný günceller
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
        
            totalHayCountText.text = totalCount.ToString();  // Hay toplamýný göster
        
        //else if (_currentResource is FlourResource)
        //{
        //    totalFlourCountText.text = totalCount.ToString();  // Flour toplamýný göster
        //}
    }

    private void UpdateStoredResources(int stored)
    {
        Debug.Log("Updated");
        // Hay kapasitesini güncelleme
        
            hayResourceCapacityText.text = stored.ToString();  // Hay için kapasiteyi güncelle
        
        //else if (_currentResource is FlourResource flourResource)
        //{
        //    flourResourceCapacityText.text = stored.ToString();  // Flour için kapasiteyi güncelle
        //}
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





}