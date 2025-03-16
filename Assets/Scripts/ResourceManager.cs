using Zenject;
using UniRx;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private HayFactory _hayFactory;
    private HayResource _currentResource;

    [Inject]
    public void Construct(HayFactory hayFactory)
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
