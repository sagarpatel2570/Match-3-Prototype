using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour {

    public static InputController Instance;

    public LayerMask itemLayermask;
    public float radius = 2;
    public bool debug;

    Vector3 initialPos;
    [HideInInspector]
    public bool canDrag = true;

	void Awake ()
    {
		if(Instance == null)
        {
            Instance = this;
        }
	}
	
	void Update ()
    {

        if(Input.GetMouseButtonDown(1))
        {
            if (debug)
            {
                Debug.Break();
            }
            Vector3 p = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            p.z = 0;
            RaycastHit2D[] hits = Physics2D.CircleCastAll(p,radius,Vector2.up,0,itemLayermask);
            foreach (RaycastHit2D h in hits)
            {
                Destroy(h.transform.gameObject);
            }
        }

        if(!canDrag)
        {
            return;
        }

		if(Input.GetMouseButtonDown(0))
        {
            initialPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            initialPos.z = 0;
        }
        
        if(Input.GetMouseButtonUp(0))
        {
            Vector3 finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            finalPos.z = 0;

            RaycastHit2D hit = Physics2D.Raycast(initialPos, Vector3.down, 10, itemLayermask);
            if(hit.transform != null)
            {
                Vector3 dir = (finalPos - initialPos).normalized;
                float lengthInX = Mathf.Abs(finalPos.x - initialPos.x);
                float lengthInY = Mathf.Abs(finalPos.y - initialPos.y);
                if(lengthInX > lengthInY)
                {
                    if(dir.x > 0)
                    {
                        Move(hit.transform, Vector3.right);
                    }
                    else
                    {
                        Move(hit.transform, Vector3.left);
                    }
                }
                else
                {
                    if(dir.y > 0)
                    {
                        Move(hit.transform, Vector3.up);
                    }
                    else
                    {
                        Move(hit.transform, Vector3.down);
                    }
                }
                
            }
        }
	}

    void Move (Transform itemTransform,Vector3 direction)
    {
        Item item = itemTransform.GetComponent<Item>();
        Grid targetGrid = null;
        if(direction == Vector3.left)
        {
            targetGrid = item.grid.leftGrid;
        }
        else if (direction == Vector3.right)
        {
            targetGrid = item.grid.rightGrid;
        }
        else if (direction == Vector3.up)
        {
            targetGrid = item.grid.upGrid;
        }
        else if (direction == Vector3.down)
        {
            targetGrid = item.grid.downGrid;
        }

        if(targetGrid == null)
        {
            return;
        }
        Item adjacentItem = targetGrid.node.item;
        if(adjacentItem == null)
        {
            return;
        }

        canDrag = false;

        item.transform.DOMove(adjacentItem.transform.position, 0.5f);
        adjacentItem.transform.DOMove(item.transform.position, 0.5f).OnComplete(()=> { canDrag = true; });

        Swap(item, adjacentItem);

    }

    void Swap (Item a,Item b)
    {
        Item tempItem = a;
        a.grid.node.item = b;
        b.grid.node.item = tempItem;

        Grid tempGrid = a.grid;
        a.grid = b.grid;
        b.grid = tempGrid;

        Transform tempParent = a.transform.parent;
        a.transform.parent = b.transform.parent;
        b.transform.parent = tempParent;
    }
}
