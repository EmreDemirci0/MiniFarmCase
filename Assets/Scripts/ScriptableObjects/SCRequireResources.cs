using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RequireResource", menuName = "ScriptableObjects/RequireResources", order = 2)]
public class SCRequireResources : ScriptableObject
{
    public ResourceType RequireResourceType;
    public Sprite RequireResourceSprite;
    public int RequireQuantity;
}
