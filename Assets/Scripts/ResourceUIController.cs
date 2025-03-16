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
        // HayFactory �zerinden bir kaynak �ret
        _currentResource = hayFactory.Create() as HayResource;
        _currentResource.OnProductionTimeChanged += UpdateProductionTimer; // Event dinlemeye ba�la
      await  _currentResource.Produce(); // �retimi ba�lat

        hayFactoryResourceSlider.maxValue = _currentResource.ProductionTime;
        hayFactoryResourceSlider.value = _currentResource.ProductionTime;
    }

    private void Start()
    {
        // Depodaki kaynak say�s�n� UI'ya ba�la
        _currentResource.StoredResources.Subscribe(UpdateResourceCount).AddTo(this);

        // Butona t�klan�nca kaynaklar� topla
        //collectButton.onClick.AddListener(CollectResources);
    }
    private void Update()
    {
        // 3D sahnede bina objesine t�klama i�lemi
        if (Input.GetMouseButtonDown(0)) // Sol fare tu�u
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // Fare konumundan bir ray olu�tur

            if (Physics.Raycast(ray, out hit)) // Ray objeye �arpt�ysa
            {
                if (hit.collider != null)
                {
                    //Tag vb. ile kontrol et
                    Debug.Log("Bina t�kland�!");
                    CollectResources(); // Kaynaklar� topla
                }
            }
        }
    }
    // Kaynak say�s�n� g�ncelle
    private void UpdateResourceCount(int value)
    {
        resourceCountText.text = $"{value}";
        //collectButton.interactable = value > 0; // Depoda �r�n varsa butonu a�
    }

    // Kaynak toplama i�lemi ()
    private void CollectResources()
    {
        // Kaynaklar� toplama i�lemi ve ard�ndan �retimi ba�latma
        _currentResource.CollectResources();
        // await _currentResource.Produce(); // �retime ba�la
    }

    // Event tetiklendi�inde UI'yi g�ncelle
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
        //    productionTimerText.text = "�retildi!";
        //}
    }

    // Obje silindi�inde event'i temizle
    private void OnDestroy()
    {
        _currentResource.OnProductionTimeChanged -= UpdateProductionTimer; // Event'i iptal et
    }
}
