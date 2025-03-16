using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;
using Zenject;


public class ResourceUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hayText;
    [SerializeField] private TextMeshProUGUI flourText;
    [SerializeField] private TextMeshProUGUI breadText;
    [SerializeField] private TextMeshProUGUI resourceCountText;
    [SerializeField] private TextMeshProUGUI productionTimerText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    //[SerializeField] private Button collectButton;

    private HayResource _currentResource;

    public float HayCount = 10;
    public float FlourCount = 0;
    public float BreadCount = 0;

    //[Inject]
    //public async void Construct(GeneralResourceFactory hayFactory)
    //{
    //    // HayFactory üzerinden bir kaynak üret
    //    _currentResource = hayFactory.Create() as HayResource;
    //    _currentResource.OnProductionTimeChanged += UpdateProductionTimer; // Event dinlemeye baþla
    //    //await _currentResource.Produce(); // Üretimi baþlat

    //    hayFactoryResourceSlider.maxValue = _currentResource.ProductionTime;
    //    hayFactoryResourceSlider.value = _currentResource.ProductionTime;
    //}

    private void Start()
    {
        // Depodaki kaynak sayýsýný UI'ya baðla
        //_currentResource.StoredResources.Subscribe(UpdateResourceCount).AddTo(this);

        // Butona týklanýnca kaynaklarý topla
        //collectButton.onClick.AddListener(CollectResources);
    }
   
    // Kaynak sayýsýný güncelle
    public void UpdateResourceCount(int value)
    {
        Debug.Log("GÜNCELLENDI"+value);
        resourceCountText.text = $"_{value}";
        //collectButton.interactable = value > 0; // Depoda ürün varsa butonu aç
    }

    // Kaynak toplama iþlemi ()
    private void CollectResources()
    {
        // Kaynaklarý toplama iþlemi ve ardýndan üretimi baþlatma
        var collected = _currentResource.CollectResources();
       // HayCount += collected;
        hayText.text = HayCount.ToString();
        // await _currentResource.Produce(); // Üretime baþla
    }

    // Event tetiklendiðinde UI'yi güncelle
    //public void UpdateProductionTimer(int remainingTime, bool isFull)
    //{
    //    if (isFull)
    //    {
    //        hayFactoryResourceSlider.value = hayFactoryResourceSlider.maxValue;
    //        productionTimerText.text = "Depo Dolu!";
    //    }
    //    else if (remainingTime > 0)
    //    {
    //        hayFactoryResourceSlider.DOValue(remainingTime, 1f).SetEase(Ease.Linear);
    //        productionTimerText.text = $"{remainingTime} sn";
    //    }
    //    //else
    //    //{
    //    //    productionTimerText.text = "Üretildi!";
    //    //}
    //}

    // Obje silindiðinde event'i temizle
    private void OnDestroy()
    {
        //_currentResource.OnProductionTimeChanged -= UpdateProductionTimer; // Event'i iptal et
    }
}
