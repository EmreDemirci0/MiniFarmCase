using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "ScriptableObjects/Resources", order = 1)]

public class SCResources : ScriptableObject
{
    public ResourceType resourceType;
    public int resourceQuantity;
    public Sprite resourceSprite;
}
