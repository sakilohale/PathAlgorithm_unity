using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCellJPS : MonoBehaviour, IComparable<GridCellJPS>
{
    // 一个单元需要有的属性
    public Vector2Int pos;
    public bool isWall;
    public bool isBlank;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public int CompareTo(GridCellJPS other)
    {
        return this.pos==other.pos?1:0;
    }

    public override bool Equals(object obj)
    {
        // 按Id来判断是否相同
        if (obj is GridCellJPS other)
            return this.pos == other.pos ;
        return false;
    }
}
