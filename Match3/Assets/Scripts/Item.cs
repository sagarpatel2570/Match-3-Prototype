﻿using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Item : MonoBehaviour {

    public ItemType type;
    public LayerMask itemLayerMask;
    public ItemState itemState;
    public Grid grid;
    public Color[] colors;

    public float speed = 3;
    public AnimationCurve curve;

    Map map;
    bool isMovingDiagonal;
    Vector3 targetPos;
    float T;

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
        GetComponentInChildren<SpriteRenderer>().color = colors[Random.Range(0, colors.Length)];
    }

    public void Update()
    {
        if (!InputController.Instance.canDrag)
        {
            return;
        }
        
        if (itemState == ItemState.STABLE)
        {
            T -= Time.deltaTime; 
            MoveTowardsEmptyGrid();
        }
        else
        {
            T += Time.deltaTime;
        }

        T = Mathf.Clamp(T,0.5f, 1);
        

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
                Grid targetGrid = map.nodeGridDictionary[new Vector2(Mathf.FloorToInt(finalPos.x), Mathf.FloorToInt(finalPos.y))];
                MoveTowardsGrid(targetGrid);
            }
        }

        if (hit.transform == null && !isMovingDiagonal)
        {
            transform.position += Time.deltaTime * speed * curve.Evaluate(T) * dir;
            itemState = ItemState.FALLING;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed * curve.Evaluate(T));
        }

        if((transform.position - targetPos).sqrMagnitude <= 0.01f)
        {
            transform.position = targetPos;
            OnReachedGrid();
        }
        UpdateGrid(transform.position);
    }

    void UpdateGrid(Vector3 position)
    {
        if (isMovingDiagonal)
        {
            return;
        }
        if (itemState == ItemState.STABLE)
        {
            return;
        }

        Vector3 dir = Vector3.down;
        position += dir * (transform.localScale.x / 2 + 0.01f);
        position = new Vector2(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

        if (map.nodeGridDictionary.ContainsKey(position))
        {
            Grid g = map.nodeGridDictionary[position];
            if (g.node.item != null && g.node.item.type == ItemType.WALL)
            {
                Debug.Break();
                return;
            }
            MoveTowardsGrid(g);
        }
    }

    bool IsAllDownGridFilled(Grid downGrid)
    {
        if(downGrid == null)
        {
            return true;
        }

        Grid g = downGrid.downGrid;

        if (g != null)
        {
            if (g.node.item != null)
            {
                return IsAllDownGridFilled(g);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
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

        if(!IsAllDownGridFilled(grid.downGrid))
        {
            return false;
        }

        Grid rightGrid = grid.rightGrid;
        if (rightGrid != null)
        {
            Item rightItem = rightGrid.node.item;

            if (rightItem != null)
            {
                if (rightItem.type == ItemType.WALL)
                {
                    Grid downGrid = rightGrid.downGrid;
                    if(MoveTowardsGrid(downGrid,true))
                    {
                        return true;
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
                if (MoveTowardsGrid(downGrid,true))
                {
                    return true;
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
                        if (MoveTowardsGrid(downGrid,true))
                        {
                            return true;
                        }
                    }
                }
                if (leftGrid != null)
                {
                    Item leftItem = leftGrid.node.item;
                    if (leftItem == null)
                    {
                        Grid downGrid = leftGrid.downGrid;
                        if (MoveTowardsGrid(downGrid,true))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    bool MoveTowardsGrid (Grid g,bool isDiagnonal = false)
    {
        if (g != null)
        {
            Item i = g.node.item;
            if (i == null)
            {
                if (grid != null && grid.node.item != null)
                {
                    grid.node.item = null;
                }

                targetPos = g.transform.position;
                itemState = ItemState.FALLING;
                
                grid = g;
                g.node.item = this;
                transform.parent = grid.transform;
                isMovingDiagonal = isDiagnonal;
                return true;
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
        if (MoveTowardsEmptyGrid() || (grid.downGrid != null && grid.downGrid.node.item  == null))
        {
            itemState = ItemState.FALLING;
        }
    }

    public void UpdateTargetPos()
    {
        targetPos = grid.transform.position;
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
