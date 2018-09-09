using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    public int x;
    public int y;
    public Item item;
    public Grid leftGrid;
    public Grid rightGrid;
    public Grid upGrid;
    public Grid downGrid;

    [HideInInspector]
    public bool isChecked;
   
    
    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

