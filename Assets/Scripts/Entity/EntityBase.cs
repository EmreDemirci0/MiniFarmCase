using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour,IEntity
{
    [SerializeField] protected int productionTime = 5; 
    [SerializeField] protected int maxCapacity = 5; 

    public abstract void Interact();
   
}
