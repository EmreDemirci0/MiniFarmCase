using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Test : MonoBehaviour
{
    [Inject] HayFactory resourceBase;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var hayFactory= resourceBase.Create();
            //hayFactory.Kaynak();
        }
    }
}
