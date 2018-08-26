using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public Node node;
	public Grid leftGrid;
    public Grid rightGrid;
    public Grid upGrid;
    public Grid downGrid;
}

[System.Serializable]
public class Node
{
    public int x;
    public int y;
    public Vector3 position;
    public Item item;

    public Node(int x, int y)
    {
        this.x = x;
        this.y = y;
        position = new Vector3(x + 0.5f, y + 0.5f, 0);
    }
}
