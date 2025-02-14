using System;
using UnityEngine;

public class GridCell : MonoBehaviour, IComparable<GridCell>
{
    public int PosX;
    public int PosY;
    public bool Attachable = true;

    public int G = 0;
    public int H = 0;
    public int F => G + H;
    public GridCell Parent = null;


    public int CompareTo(GridCell other)
    {
        // 按F值排序
        int cmp = this.F.CompareTo(other.F);
        // 若F值相同则按位置排序
        if (cmp == 0)
        {
            cmp = this.PosX.CompareTo(other.PosX);
        }
        return cmp;
    }

    public override bool Equals(object obj)
    {
        // 按Id来判断是否相同
        if (obj is GridCell other)
            return this.PosX == other.PosX && this.PosY == other.PosY ;
        return false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

