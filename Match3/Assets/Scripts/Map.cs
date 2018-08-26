using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public int width;
    public int height;

    public Grid gridPrefab;
    public Item chairPrefab;
    public Item wallPrefab;

    public Dictionary<Vector2, Grid> nodeGridDictionary = new Dictionary<Vector2, Grid>();

    private void Awake()
    {
    }

    void Start () {
        GenerateMap();
    }


    void GenerateMap()
    {
        for (int y = -height; y < height; y++)
        {
            for (int x = -width; x < width; x++)
            {
                Node node = new Node(x, y);
                Grid grid = Instantiate(gridPrefab, node.position, Quaternion.identity) as Grid;
                grid.name = "Grid_" + x + "_" + y;
                grid.node = node;
                grid.transform.parent = this.transform;
                Item itemPrefab = null;
                if (x == -5 && y == 0)
                {
                    itemPrefab = wallPrefab;
                }else if (x == -4 && y == 0)
                {
                    itemPrefab = wallPrefab;
                }
                /*else if (x == -3 && y == 0)
                {
                    itemPrefab = wallPrefab;
                }*/
                else if (x == -2 && y == 0)
                {
                    itemPrefab = wallPrefab;
                }
                else
                {
                    itemPrefab = chairPrefab;
                }
                Item item = Instantiate(itemPrefab, node.position, Quaternion.identity) as Item;
                item.transform.parent = grid.transform;
                item.grid = grid;
                node.item = item;

                nodeGridDictionary.Add(new Vector2(x, y), grid);
            }
        }

        //fill neighour

        for (int y = -height; y < height; y++)
        {
            for (int x = -width; x < width; x++)
            {
                Grid currentGrid = nodeGridDictionary[new Vector2(x, y)];

                Vector2 left = new Vector2(x - 1, y);
                if(nodeGridDictionary.ContainsKey(left))
                {
                    currentGrid.leftGrid = nodeGridDictionary[left];
                }

                Vector2 right = new Vector2(x + 1, y);
                if (nodeGridDictionary.ContainsKey(right))
                {
                    currentGrid.rightGrid = nodeGridDictionary[right];
                }

                Vector2 up = new Vector2(x, y + 1);
                if (nodeGridDictionary.ContainsKey(up))
                {
                    currentGrid.upGrid = nodeGridDictionary[up];
                }

                Vector2 down = new Vector2(x, y-1);
                if (nodeGridDictionary.ContainsKey(down))
                {
                    currentGrid.downGrid = nodeGridDictionary[down];
                }
            }
        }
    }
}


