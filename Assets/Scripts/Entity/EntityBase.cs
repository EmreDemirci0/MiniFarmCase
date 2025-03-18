using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour,IEntity
{
    public SCResources resourceInfo;
    [SerializeField] protected int productionTime = 5; 
    [SerializeField] protected int maxCapacity = 5; 

    public abstract void Interact();
   
}
