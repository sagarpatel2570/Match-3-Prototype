using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemData : ScriptableObject {

    public ItemType type;
    public LayerMask itemLayerMask;
    public float speed = 3;
    public AnimationCurve curve;
    public Sprite sprite;
}
