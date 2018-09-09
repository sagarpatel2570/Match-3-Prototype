using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineManager : MonoBehaviour {

    public Map map;

    void Start () {
		
	}

    void Update()
    {

        List<List<Grid>> combinedGridList = CombineItems();
        List<List<Grid>> newCombinedGridList = new List<List<Grid>>();
        foreach (List<Grid> combineGrid in combinedGridList)
        {
            List<Grid> newCombinedGrid = RemoveUnwanedGrids(combineGrid);
            if (newCombinedGrid != null)
            {
                newCombinedGridList.Add(newCombinedGrid);
            }
        }

        foreach (List<Grid> combineGrid in newCombinedGridList)
        {
            foreach (Grid g in combineGrid)
            {
                Destroy(g.item.gameObject);
            }
        }
    }

    List<Grid> RemoveUnwanedGrids(List<Grid> combinedGrids)
    {
        List<Grid> newCombinedGrid = new List<Grid>();
        foreach (Grid g in combinedGrids)
        {
            int horizontalCount = GetNumberOfMatchesToRight(combinedGrids, g) + GetNumberOfMatchesToLeft(combinedGrids, g) - 1;
            if(horizontalCount >= 3)
            {
                newCombinedGrid.Add(g);
            }
            else
            {
                int verticalCount = GetNumberOfMatchesToUp(combinedGrids, g) + GetNumberOfMatchesToDown(combinedGrids, g) - 1;
                if (verticalCount >= 3)
                {
                    newCombinedGrid.Add(g);
                }
            }
        }
        return newCombinedGrid.Count > 0 ? newCombinedGrid : null;
    }

    int GetNumberOfMatchesToRight (List<Grid> combinedGrids,Grid currentGrid)
    {
        int count = 1;
        while(currentGrid != null)
        {
            if(currentGrid.rightGrid != null && combinedGrids.Contains(currentGrid.rightGrid))
            {
                count++;
                currentGrid = currentGrid.rightGrid;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    int GetNumberOfMatchesToLeft(List<Grid> combinedGrids, Grid currentGrid)
    {
        int count = 1;
        while (currentGrid != null)
        {
            if (currentGrid.leftGrid != null && combinedGrids.Contains(currentGrid.leftGrid))
            {
                count++;
                currentGrid = currentGrid.leftGrid;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    int GetNumberOfMatchesToUp(List<Grid> combinedGrids, Grid currentGrid)
    {
        int count = 1;
        while (currentGrid != null)
        {
            if (currentGrid.upGrid != null && combinedGrids.Contains(currentGrid.upGrid))
            {
                count++;
                currentGrid = currentGrid.upGrid;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    int GetNumberOfMatchesToDown(List<Grid> combinedGrids, Grid currentGrid)
    {
        int count = 1;
        while (currentGrid != null)
        {
            if (currentGrid.downGrid != null && combinedGrids.Contains(currentGrid.downGrid))
            {
                count++;
                currentGrid = currentGrid.downGrid;
            }
            else
            {
                break;
            }
        }
        return count;
    }

    int GetNumberOfVerticalMatches(List<Grid> combinedGrids, Grid currentGrid, int count = 1)
    {
        if (currentGrid.upGrid != null && combinedGrids.Contains(currentGrid.upGrid))
        {
            return GetNumberOfVerticalMatches(combinedGrids, currentGrid.upGrid, count + 1);
        }

        if (currentGrid.downGrid != null && combinedGrids.Contains(currentGrid.downGrid))
        {
            return GetNumberOfVerticalMatches(combinedGrids, currentGrid.downGrid, count + 1);
        }
        return count;
    }

    List<List<Grid>> CombineItems()
    {
        foreach (Grid g in map.nodeGridDictionary.Values)
        {
            g.isChecked = false;
        }

        List<List<Grid>> commbinedItemList = new List<List<Grid>>();
        Queue<Grid> gridQueue = new Queue<Grid>();
        for (int y = -map.height; y < map.height; y++)
        {
            for (int x = -map.width; x < map.width; x++)
            {
                Grid g = map.nodeGridDictionary[new Vector2(x, y)];
                if(g.item != null && g.item.itemState == ItemState.STABLE && !g.isChecked)
                {
                    List<Grid> combinedGrid = new List<Grid>();
                    gridQueue.Enqueue(g);
                    combinedGrid.Add(g);
                    g.isChecked = true;
                    while(gridQueue.Count > 0)
                    {
                        Grid currentGrid = gridQueue.Dequeue();

                        if(currentGrid.leftGrid != null  && !currentGrid.leftGrid.isChecked )
                        {
                            if (currentGrid.leftGrid.item == null || currentGrid.leftGrid.item.itemState == ItemState.FALLING)
                            {
                                currentGrid.leftGrid.isChecked = true;
                            }
                            else
                            {
                                if(currentGrid.leftGrid.item.data.type == currentGrid.item.data.type)
                                {
                                    currentGrid.leftGrid.isChecked = true;
                                    gridQueue.Enqueue(currentGrid.leftGrid);
                                    combinedGrid.Add(currentGrid.leftGrid);
                                }
                            }
                        }

                        if (currentGrid.rightGrid != null && !currentGrid.rightGrid.isChecked )
                        {
                            if (currentGrid.rightGrid.item == null || currentGrid.rightGrid.item.itemState == ItemState.FALLING)
                            {
                                currentGrid.rightGrid.isChecked = true;
                            }
                            else
                            {
                                if (currentGrid.rightGrid.item.data.type == currentGrid.item.data.type)
                                {
                                    currentGrid.rightGrid.isChecked = true;
                                    gridQueue.Enqueue(currentGrid.rightGrid);
                                    combinedGrid.Add(currentGrid.rightGrid);
                                }
                            }
                        }

                        if (currentGrid.upGrid != null && !currentGrid.upGrid.isChecked )
                        {
                            if (currentGrid.upGrid.item == null || currentGrid.upGrid.item.itemState == ItemState.FALLING)
                            {
                                currentGrid.upGrid.isChecked = true;
                            }
                            else
                            {
                                if (currentGrid.upGrid.item.data.type == currentGrid.item.data.type)
                                {
                                    currentGrid.upGrid.isChecked = true;
                                    gridQueue.Enqueue(currentGrid.upGrid);
                                    combinedGrid.Add(currentGrid.upGrid);
                                }
                            }
                        }

                        if (currentGrid.downGrid != null && !currentGrid.downGrid.isChecked )
                        {
                            if (currentGrid.downGrid.item == null || currentGrid.downGrid.item.itemState == ItemState.FALLING)
                            {
                                currentGrid.downGrid.isChecked = true;
                            }
                            else
                            {
                                if (currentGrid.downGrid.item.data.type == currentGrid.item.data.type)
                                {
                                    currentGrid.downGrid.isChecked = true;
                                    gridQueue.Enqueue(currentGrid.downGrid);
                                    combinedGrid.Add(currentGrid.downGrid);
                                }
                            }
                        }
                    }
                    if (combinedGrid.Count >= 3)
                    {
                        commbinedItemList.Add(combinedGrid);
                    }
                }
            }
        }
        return commbinedItemList;
    }
}
