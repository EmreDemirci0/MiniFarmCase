using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resources", order = 1)]
public class SCResources : ScriptableObject
{
    [Header("Resource")]
    public ResourceType resourceType;
    public Sprite resourceSprite;
    [Header("Production")]
    public int productionTime = 5;
    public int maxCapacity = 5;
    public bool isSaveable=true;
}
