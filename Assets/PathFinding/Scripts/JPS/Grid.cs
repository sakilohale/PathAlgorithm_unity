using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class GridBase : MonoBehaviour
{
    public GridCellJPS[,] Grid;
    public abstract void InitGrid(int sideNums, List<Vector2Int> blankList, List<Vector2Int> wallList);
    public abstract bool IsAttachable(Vector2Int pos);
    public abstract bool IsWall(Vector2Int pos);
}

public class Grid : GridBase
{
    public int sideNums;
    public GameObject gridPrefab;
    public List<Vector2Int> blankList;
    public List<Vector2Int> wallList;
    public JPS JpsSearch;
    private void Start()
    {
        InitGrid(sideNums, blankList, wallList);
        JpsSearch = new JPS();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            JpsSearch.find(this, new Vector2Int(0, 0), new Vector2Int(7, 3));
        }
    }

    public override void InitGrid(int sideNums, List<Vector2Int> blankList, List<Vector2Int> wallList)
    {
        Grid = new GridCellJPS[sideNums, sideNums];
        for (int i = 0; i < sideNums; i++)
        {
            for (int j = 0; j < sideNums; j++)
            {
                var go = Instantiate(gridPrefab, transform.position + Vector3.right * i * 1.2f + Vector3.forward * j * 1.2f, Quaternion.identity);
                go.name = $"GridCell({i},{j})";
                go.transform.parent = transform;
                var gcj = go.AddComponent<GridCellJPS>();
                gcj.pos = new Vector2Int(i, j);
                Grid[i, j] = gcj;

                if (wallList.Contains(new Vector2Int(i, j)))
                {
                    go.GetComponent<MeshRenderer>().material.color = Color.red;
                    gcj.isWall=true;
                }else if (blankList.Contains(new Vector2Int(i, j)))
                {
                    go.GetComponent<MeshRenderer>().material.color = Color.black;
                    go.AddComponent<GridCellJPS>();
                }
                else
                {
                    go.GetComponent<MeshRenderer>().material.color = Color.white;
                    go.AddComponent<GridCellJPS>();
                }
            }
        }
    }
    
    public override bool IsAttachable(Vector2Int pos)
    {
        if (pos.x >= Grid.GetLength(0) || pos.y >= Grid.GetLength(0) || pos.x < 0 || pos.y < 0)
        {
            return false;
        }

        return Grid[pos.x, pos.y].isBlank;
    }

    public override bool IsWall(Vector2Int pos)
    {
        if (pos.x >= Grid.GetLength(0) || pos.y >= Grid.GetLength(0) || pos.x < 0 || pos.y < 0)
        {
            return false;
        }

        return Grid[pos.x, pos.y].isWall;
    }
}
