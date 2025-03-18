using Cysharp.Threading.Tasks;
using UnityEngine;
public abstract class EntityBase : MonoBehaviour,IEntity
{
    public SCResources resourceInfo;
    [SerializeField] protected int productionTime = 5; 
    [SerializeField] protected int maxCapacity = 5;
    public abstract UniTaskVoid Interact();
   
}
