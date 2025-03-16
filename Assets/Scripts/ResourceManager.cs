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
        // HayFactory �zerinden bir kaynak �ret
        //_currentResource = _hayFactory.Create() as HayResource;
       //await _currentResource.Produce(); // �retim ba�lat
    }

    void Update()
    {
        // E�er T tu�una bas�l�rsa, �retimi ba�lat
        if (Input.GetKeyDown(KeyCode.T))
        {
            //_currentResource.Produce(); // �retim ba�lat
        }
    }
}
