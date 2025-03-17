using Cysharp.Threading.Tasks;
using System;
using System.Resources;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public class FlourEntity : MonoBehaviour, IEntity
{

    private ResourceManager _resourceManager;
    private FlourResource _flourResource;  // FlourResource veya kendi kaynak s�n�f�n�z� burada kullanabilirsiniz.
    [SerializeField] private int productionTime = 5; // Inspectordan ayarlanabilir
    [SerializeField] private int maxCapacity = 5; // Inspectordan ayarlanabilir

    [SerializeField] bool isNeedResource;
    [SerializeField] ResourceType resourceType;
    [SerializeField] int resourceQuantity;
    [SerializeField] TextMeshProUGUI totalQuantityText;
    [SerializeField] GameObject productionButtons;
    [SerializeField] Button plusProductionOrderButton;
    [SerializeField] Button minusProductionOrderButton;
    [SerializeField] Image productionImage;
    [SerializeField] TextMeshProUGUI productionQuantityText;
    public/**/ bool isProductionButtonOpen=false;

    [SerializeField]
    Sprite[] productionSprites;//Hay,Flour 


    public/*?*/ int currentQueueCount = 0;  // Kuyruktaki item say�s�
    private int inventoryCount = 10;


    [Inject]
    public void Construct(ResourceManager resourceManager, FlourResource flourResource)
    {
        _flourResource = flourResource;
        _flourResource.SetProductionValues(productionTime, maxCapacity);
        _resourceManager = resourceManager;
        if (resourceType==ResourceType.Hay)
        {
            productionImage.sprite = productionSprites[0];
        }
        else if (resourceType == ResourceType.Flour)
        {
            productionImage.sprite = productionSprites[1];
        }
        productionQuantityText.text = "x"+resourceQuantity.ToString();

        plusProductionOrderButton.onClick.AddListener(OnAddButtonClicked);
        minusProductionOrderButton.onClick.AddListener(OnRemoveButtonClicked);

       // UpdateTotalQuantityText();

        _resourceManager.TotalHayCount
             .Subscribe(_ => UpdateButtonInteractivity())  // Hay miktar�ndaki de�i�iklikleri dinle
             .AddTo(this);

        _resourceManager.TotalFlourCount
            .Subscribe(_ => UpdateButtonInteractivity())  // Flour miktar�ndaki de�i�iklikleri dinle
            .AddTo(this);
    }
    private void UpdateButtonInteractivity()
    {
        int currentResourceCount = _resourceManager.GetTotalResourceCount(resourceType);

        // Butonlar� kaynak miktar�na g�re aktif/pasif yap�yoruz
        plusProductionOrderButton.interactable = currentResourceCount >= resourceQuantity;
        minusProductionOrderButton.interactable = currentQueueCount > 0;  // Kuyrukta kaynak varsa ��kar�labilir
    }
    public async void OnAddButtonClicked()
    {
        bool success =await _flourResource.AddToQueue(resourceType,resourceQuantity);
        if (!success)
        {
            Debug.Log("Kaynak eklenemedi!");
        }
    }

    // - butonuna bas�ld���nda kaynak ��karma
    public void OnRemoveButtonClicked()
    {
        bool success = _flourResource.RemoveFromQueue(resourceQuantity);
        if (!success)
        {
            Debug.Log("Kaynak ��kar�lamad�!");
        }
    }


    // Kuyruktaki ��eleri ve toplam kapasiteyi g�ncelleyen metot
    public void UpdateTotalQuantityText()
    {
        currentQueueCount++;
        totalQuantityText.text = $"{currentQueueCount}/{maxCapacity}";
    }
    public void ReduceTotalQuantityText()
    {
        if (currentQueueCount > 0)
        {
            currentQueueCount--;
            totalQuantityText.text = $"{currentQueueCount}/{maxCapacity}";
        }
        
    }
    public async void Interact()
    {
        if (!isProductionButtonOpen)
        {
            isProductionButtonOpen = true;
            productionButtons.SetActive(true);
        }
        else
        {
            Debug.Log("Zaten a��km�s");
            if (_flourResource != null)
            {
                //    ReduceTotalQuantityText();
                await _flourResource.CollectResources();
            }
        }



       
    }
    public void CloseProductionButton()
    {
        Debug.Log("Kapat");
        isProductionButtonOpen = false;
        productionButtons.SetActive(false);
    }
}
