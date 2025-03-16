using Zenject;
using UniRx;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private GeneralResourceFactory _hayFactory;
    private HayResource _currentResource;

    [Inject]
    public void Construct(GeneralResourceFactory hayFactory)
    {
        _hayFactory = hayFactory;
    }

    void Start()
    {
        // HayFactory üzerinden bir kaynak üret
        //_currentResource = _hayFactory.Create() as HayResource;
       //await _currentResource.Produce(); // Üretim baþlat
    }

    void Update()
    {
        // Eðer T tuþuna basýlýrsa, üretimi baþlat
        if (Input.GetKeyDown(KeyCode.T))
        {
            //_currentResource.Produce(); // Üretim baþlat
        }
    }
}
