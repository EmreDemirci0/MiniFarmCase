using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class ResourceUI
{
    public Slider ResourceSlider;
    public TextMeshProUGUI ResourceCapacityText;
    public TextMeshProUGUI ProductionTimerText;
    public TextMeshProUGUI TotalResourceText;

    public ResourceUI(Slider slider, TextMeshProUGUI capacityText, TextMeshProUGUI timerText, TextMeshProUGUI totalText)
    {
        ResourceSlider = slider;
        ResourceCapacityText = capacityText;
        ProductionTimerText = timerText;
        TotalResourceText = totalText;
    }

    public void UpdateUI(int currentResource, int maxCapacity, int productionTime)
    {
        // Kaynaðýn kapasitesini ve üretim süresini UI'ye aktar
        ResourceCapacityText.text = currentResource.ToString();
        TotalResourceText.text = ResourceBase.TotalResourcesCount.Value.ToString();

        if (currentResource >= maxCapacity)
        {
            ResourceSlider.DOValue(1f, 0.5f).SetEase(Ease.OutQuad); // Slider'ý dolu yap
            ProductionTimerText.text = "FULL";
        }
        else
        {
            ResourceSlider.DOValue((float)currentResource / maxCapacity, 1f).SetEase(Ease.Linear); // Kaynak ilerlemesini göster
            ProductionTimerText.text = productionTime + "s"; // Üretim zamaný
        }
    }
}
