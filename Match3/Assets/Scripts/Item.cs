using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Item : MonoBehaviour {

    public ItemType type;
    public LayerMask itemLayerMask;
    public ItemState itemState;
    public Grid grid;

    public float speed = 3;

    Map map;
    bool isMovingDiagonal;
    Vector3 diagDir;
    Vector3 targetPos;

    private void OnEnable()
    {
        map = GameObject.FindObjectOfType<Map>();
    }

    private void Start()
    {
        if (grid != null)
        {
            targetPos = grid.transform.position;
        }
    }

    void UpdateGrid (Vector3 position)
    {
        Vector3 dir = Vector3.down;
        position += dir * (transform.localScale.x / 2 + 0.01f) ;

        if (isMovingDiagonal)
        {
            position += diagDir * (transform.localScale.x / 2 + 0.01f);
        }
        if (itemState == ItemState.FALLING )
        {
            if (grid != null)
            {
                Vector3 test = position;
                position = new Vector2(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
               
                if (map.nodeGridDictionary.ContainsKey(position))
                {
                    Grid g = map.nodeGridDictionary[position];
                    if(g.node.item != null && g.node.item.type == ItemType.WALL)
                    {
                        Debug.LogError(test.x + " " + test.y + " " + position + " "  + transform.position);
                        Debug.Break();
                        return;
                    }
                    grid.node.item = null;
                    grid = map.nodeGridDictionary[position];
                }
                grid.node.item = this;
                targetPos = grid.transform.position;
                transform.parent = grid.transform;
            }
        }
    }

    public void Update()
    {
        if (!InputController.Instance.canDrag)
        {
            return;
        }

        UpdateGrid(transform.position );
        
        if (itemState == ItemState.STABLE)
        {
            if (MoveTowardsEmptyGrid())
            {
               // return;
            }
        }
        

        Vector3 raycastPos = transform.position;
        Vector3 dir = Vector3.down;
        raycastPos += dir * (transform.localScale.y/2f  + 0.5f);
        RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector3.forward, 10, itemLayerMask);
        
        if (hit.transform != null && itemState == ItemState.FALLING && !isMovingDiagonal)
        {
            Item i = hit.transform.GetComponent<Item>();
            if (i.itemState != ItemState.FALLING)
            {
                Vector3 finalPos = hit.transform.position + Vector3.up;

                grid = map.nodeGridDictionary[new Vector2(Mathf.FloorToInt(finalPos.x), Mathf.FloorToInt(finalPos.y))];
                grid.node.item = this;
                transform.parent = grid.transform;
                float d = Vector3.Distance(grid.transform.position, transform.position);
                float s = speed;
                targetPos = grid.transform.position;
                itemState = ItemState.FALLING;
            }
        }

        if (hit.transform == null && !isMovingDiagonal)
        {
            transform.position += Time.deltaTime * speed  * dir;
            itemState = ItemState.FALLING;
        }
        else
        {

            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
        }

        if((transform.position - targetPos).sqrMagnitude <= 0.01f)
        {
            transform.position = targetPos;
            OnReachedGrid();
        }
    }

    bool MoveTowardsEmptyGrid()
    {
        if(isMovingDiagonal)
        {
            return false;
        }

        if (grid == null)
        {
            return false;
        }

        if(grid.downGrid != null && grid.downGrid.node.item == null)
        {
            return false;
        }

        //check for left or right grid weather it has wall item or not 
        // if it has then check for its down tile if it is empty if so then we can move towards that tile 
        // but make that that grid is marked as having some tile so that tile from other direction will not be able to move

        Grid rightGrid = grid.rightGrid;
        if (rightGrid != null)
        {
            Item rightItem = rightGrid.node.item;

            if (rightItem != null)
            {
                if (rightItem.type == ItemType.WALL)
                {
                    Grid downGrid = rightGrid.downGrid;
                    if (downGrid != null)
                    {
                        Item downItem = downGrid.node.item;
                        if (downItem == null )
                        {
                            float d = Vector3.Distance(downGrid.transform.position, transform.position);
                           
                            float s =  speed;
                            targetPos = downGrid.transform.position;
                            itemState = ItemState.FALLING;
                            if (grid != null && grid.node.item != null)
                            {
                                grid.node.item = null;
                            }
                            grid = downGrid;
                            downGrid.node.item = this;
                            transform.parent = grid.transform;
                            isMovingDiagonal = true;
                            diagDir = Vector3.right;
                            return true;
                        }
                    }
                }
            }
        }

        Grid leftGrid = grid.leftGrid;
        if (leftGrid != null)
        {
            Item leftItem = leftGrid.node.item;

            if (leftItem != null && leftItem.type == ItemType.WALL)
            {
                Grid downGrid = leftGrid.downGrid;
                if (downGrid != null)
                {
                    Item downItem = downGrid.node.item;
                    if (downItem == null )
                    {
                        float d = Vector3.Distance(downGrid.transform.position, transform.position);
                       
                        float s = speed;
                        targetPos = downGrid.transform.position;
                        itemState = ItemState.FALLING;
                        if (grid != null && grid.node.item != null)
                        {
                            grid.node.item = null;
                        }
                        grid = downGrid;
                        downGrid.node.item = this;
                        transform.parent = grid.transform;
                        isMovingDiagonal = true;
                        diagDir = Vector3.left;
                        return true;
                    }
                }
            }
        }

        Grid upGrid = grid.upGrid;
        if (upGrid != null)
        {
            Item upItem = IsWallAvailableVertically(upGrid);
            if(upItem != null && upItem.type == ItemType.WALL)
            {
                if(rightGrid != null)
                {
                    Item rightItem = rightGrid.node.item;
                    if(rightItem == null)
                    {
                        Grid downGrid = rightGrid.downGrid;
                        if(downGrid != null)
                        {
                            if(downGrid.node.item == null)
                            {
                                float d = Vector3.Distance(downGrid.transform.position, transform.position);
                                
                                float s =  speed;
                                targetPos = downGrid.transform.position;
                                itemState = ItemState.FALLING;
                                if (grid != null && grid.node.item != null)
                                {
                                    grid.node.item = null;
                                }
                                grid = downGrid;
                                downGrid.node.item = this;
                                transform.parent = grid.transform;
                                isMovingDiagonal = true;
                                diagDir = Vector3.right;
                                return true;
                            }
                        }
                    }
                }
                if (leftGrid != null)
                {
                    Item leftItem = leftGrid.node.item;
                    if (leftItem == null)
                    {
                        Grid downGrid = leftGrid.downGrid;
                        if (downGrid != null)
                        {
                            if (downGrid.node.item == null)
                            {
                                float d = Vector3.Distance(downGrid.transform.position, transform.position);
                                
                                float s =  speed;
                                targetPos = downGrid.transform.position;
                                itemState = ItemState.FALLING;
                                if (grid != null && grid.node.item != null)
                                {
                                    grid.node.item = null;
                                }
                                grid = downGrid;
                                downGrid.node.item = this;
                                transform.parent = grid.transform;
                                isMovingDiagonal = true;
                                diagDir = Vector3.left;
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }


    Item IsWallAvailableVertically (Grid g)
    {
        if (g == null)
        {
            return null;
        }

        if (g.node.item != null)
        {
            if (g.node.item.type == ItemType.WALL)
            {
                return g.node.item;
            }
            else
            {
                return null;
            }
        }
       
        return IsWallAvailableVertically(g.upGrid);
    }

    void OnReachedGrid ()
    {
        itemState = ItemState.STABLE;
        isMovingDiagonal = false;
        if (!MoveTowardsEmptyGrid())
        {
        }
        else
        {
            itemState = ItemState.FALLING;
        }
        
    }
}

public enum ItemType
{
    CHAIR,
    WALL
}

public enum ItemState
{
    REACHING,
    FALLING,
    STABLE
}
