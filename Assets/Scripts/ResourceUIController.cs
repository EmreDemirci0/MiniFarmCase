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
    //    // HayFactory �zerinden bir kaynak �ret
    //    _currentResource = hayFactory.Create() as HayResource;
    //    _currentResource.OnProductionTimeChanged += UpdateProductionTimer; // Event dinlemeye ba�la
    //    //await _currentResource.Produce(); // �retimi ba�lat

    //    hayFactoryResourceSlider.maxValue = _currentResource.ProductionTime;
    //    hayFactoryResourceSlider.value = _currentResource.ProductionTime;
    //}

    private void Start()
    {
        // Depodaki kaynak say�s�n� UI'ya ba�la
        //_currentResource.StoredResources.Subscribe(UpdateResourceCount).AddTo(this);

        // Butona t�klan�nca kaynaklar� topla
        //collectButton.onClick.AddListener(CollectResources);
    }
   
    // Kaynak say�s�n� g�ncelle
    public void UpdateResourceCount(int value)
    {
        Debug.Log("G�NCELLENDI"+value);
        resourceCountText.text = $"_{value}";
        //collectButton.interactable = value > 0; // Depoda �r�n varsa butonu a�
    }

    // Kaynak toplama i�lemi ()
    private void CollectResources()
    {
        // Kaynaklar� toplama i�lemi ve ard�ndan �retimi ba�latma
        var collected = _currentResource.CollectResources();
       // HayCount += collected;
        hayText.text = HayCount.ToString();
        // await _currentResource.Produce(); // �retime ba�la
    }

    // Event tetiklendi�inde UI'yi g�ncelle
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
    //    //    productionTimerText.text = "�retildi!";
    //    //}
    //}

    // Obje silindi�inde event'i temizle
    private void OnDestroy()
    {
        //_currentResource.OnProductionTimeChanged -= UpdateProductionTimer; // Event'i iptal et
    }
}
