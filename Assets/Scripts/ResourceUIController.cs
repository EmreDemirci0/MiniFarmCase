using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Cysharp.Threading.Tasks;
using System;
using DG.Tweening;

public class ResourceUIController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI resourceCountText;
    [SerializeField] private TextMeshProUGUI productionTimerText;
    [SerializeField] private Slider hayFactoryResourceSlider;
    [SerializeField] private Button collectButton;

    private ResourceFactory factory;
    private int remainingTime; // Geriye kalan �retim s�resi

    [Inject]
    public void Construct(ResourceFactory factory)
    {
        this.factory = factory;
        hayFactoryResourceSlider.maxValue = factory.ProductionTime;
        hayFactoryResourceSlider.value = factory.ProductionTime;
    }


    private void Start()
    {
        // Depodaki kaynak say�s�n� UI'ya ba�la
        factory.StoredResources.Subscribe(value =>
        {
            //resourceCountText.text = $"{value}/{factory.MaxCapacity}";
            resourceCountText.text = $"{value}";
            collectButton.interactable = value > 0; // Depoda �r�n varsa butonu a�
        }).AddTo(this);

        // �retim zaman�n� dinle ve g�ncelle
        factory.OnProductionTimeChanged += UpdateProductionTimer;

        // Butona t�klan�nca kaynaklar� topla
        collectButton.onClick.AddListener(() =>
        {
            factory.CollectResources();
        });
    }

    // Event tetiklendi�inde UI'yi g�ncelle
    private void UpdateProductionTimer(int remainingTime, bool isFull)
    {
        if (isFull)
        {
            hayFactoryResourceSlider.DOValue(0, 1f).SetEase(Ease.Linear);
            productionTimerText.text = "Depo Dolu!";
        }
        else if (remainingTime > 0)
        {
            //hayFactoryResourceSlider.value = remainingTime;
            hayFactoryResourceSlider.DOValue(remainingTime, 1f).SetEase(Ease.Linear);
            productionTimerText.text = $"{remainingTime} sn";
        }
        else
        {
            productionTimerText.text = "�retildi!";
        }
    }


    // Obje silindi�inde event'i temizle
    private void OnDestroy()
    {
        factory.OnProductionTimeChanged -= UpdateProductionTimer;
    }


}
