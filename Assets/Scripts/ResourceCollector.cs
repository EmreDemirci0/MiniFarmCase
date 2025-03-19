using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Zenject;

public class ResourceCollector : MonoBehaviour
{

    private Camera _camera;
    private List<ResourceDependentEntity> _resources = new List<ResourceDependentEntity>();
    private ResourceDependentEntity currentOpenResource; 


    private void Start()
    {
        _camera = Camera.main;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;
            //burayý refactor !!!
            if (Physics.Raycast(ray, out hit))
            {
                IEntity entity = hit.collider.GetComponent<IEntity>();
                if (entity != null)
                {
                    if (entity is ResourceDependentEntity resourceDependentEntity)
                    {
                        if (currentOpenResource != null && currentOpenResource != resourceDependentEntity)
                        {
                            currentOpenResource.CloseProductionButton();
                        }

                        if (currentOpenResource != resourceDependentEntity)
                        {
                            currentOpenResource = resourceDependentEntity;
                            if (!_resources.Contains(resourceDependentEntity))
                                _resources.Add(resourceDependentEntity);
                        }

                        entity.Interact();
                    }
                    else
                    {
                        foreach (var item in _resources)
                            item.CloseProductionButton();
                        currentOpenResource = null;
                        entity.Interact();
                    }
                }
            }
            else
            {
                foreach (var item in _resources)
                    item.CloseProductionButton();
                currentOpenResource = null;
            }

        }
    }
    
}