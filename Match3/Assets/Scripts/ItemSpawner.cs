using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public Item chairPrefab;
    public LayerMask itemLayerMask;

	void Start () {
		
	}

    void Update()
    {
        Vector3 raycastPos = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 1, itemLayerMask);
        Debug.DrawLine(transform.position, hit.point, Color.red);
        if (hit.transform == null)
        {
            Item chair = Instantiate(chairPrefab, transform.position, Quaternion.identity) as Item;
        }
    }
}
