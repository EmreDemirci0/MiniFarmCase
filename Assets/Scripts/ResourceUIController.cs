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
    [SerializeField] private TextMeshProUGUI resourceCountText;
    [SerializeField] private TextMeshProUGUI productionTimerText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    //[SerializeField] private Button collectButton;

    private HayResource _currentResource;

    [Inject]
    public  async void Construct(HayFactory hayFactory)
    {
        // HayFactory üzerinden bir kaynak üret
        _currentResource = hayFactory.Create() as HayResource;
        _currentResource.OnProductionTimeChanged += UpdateProductionTimer; // Event dinlemeye baþla
      await  _currentResource.Produce(); // Üretimi baþlat

        hayFactoryResourceSlider.maxValue = _currentResource.ProductionTime;
        hayFactoryResourceSlider.value = _currentResource.ProductionTime;
    }

    private void Start()
    {
        // Depodaki kaynak sayýsýný UI'ya baðla
        _currentResource.StoredResources.Subscribe(UpdateResourceCount).AddTo(this);

        // Butona týklanýnca kaynaklarý topla
        //collectButton.onClick.AddListener(CollectResources);
    }
    private void Update()
    {
        // 3D sahnede bina objesine týklama iþlemi
        if (Input.GetMouseButtonDown(0)) // Sol fare tuþu
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Fare konumundan bir ray oluþtur

            if (Physics.Raycast(ray, out hit)) // Ray objeye çarptýysa
            {
                if (hit.collider != null)
                {
                    //Tag vb. ile kontrol et
                    Debug.Log("Bina týklandý!");
                    CollectResources(); // Kaynaklarý topla
                }
            }
        }
    }
    // Kaynak sayýsýný güncelle
    private void UpdateResourceCount(int value)
    {
        resourceCountText.text = $"{value}";
        //collectButton.interactable = value > 0; // Depoda ürün varsa butonu aç
    }

    // Kaynak toplama iþlemi ()
    private void CollectResources()
    {
        // Kaynaklarý toplama iþlemi ve ardýndan üretimi baþlatma
        _currentResource.CollectResources();
        // await _currentResource.Produce(); // Üretime baþla
    }

    // Event tetiklendiðinde UI'yi güncelle
    private void UpdateProductionTimer(int remainingTime, bool isFull)
    {
        if (isFull)
        {
            hayFactoryResourceSlider.value = hayFactoryResourceSlider.maxValue;
            productionTimerText.text = "Depo Dolu!";
        }
        else if (remainingTime > 0)
        {
            hayFactoryResourceSlider.DOValue(remainingTime, 1f).SetEase(Ease.Linear);
            productionTimerText.text = $"{remainingTime} sn";
        }
        //else
        //{
        //    productionTimerText.text = "Üretildi!";
        //}
    }

    // Obje silindiðinde event'i temizle
    private void OnDestroy()
    {
        _currentResource.OnProductionTimeChanged -= UpdateProductionTimer; // Event'i iptal et
    }
}
