using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour {

    public int width;
    public int height;

    public Grid gridPrefab;
    public Item[] itemPrefabs;
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
                Vector3 position = new Vector3(x + 0.5f, y + 0.5f, 0);
                Grid grid = Instantiate(gridPrefab, position, Quaternion.identity) as Grid;
                grid.name = "Grid_" + x + "_" + y;
                grid.Init(x, y);
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
                    int randomIndex = Random.Range(0, itemPrefabs.Length);
                    itemPrefab = itemPrefabs[randomIndex];
                }
                
                Item item = Instantiate(itemPrefab, grid.transform.position, Quaternion.identity) as Item;
                item.transform.parent = grid.transform;
                item.grid = grid;
                grid.item = item;

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


