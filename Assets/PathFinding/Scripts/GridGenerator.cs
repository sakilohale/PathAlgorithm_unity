using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class GridGenerator : MonoBehaviour
{
    public int nums;
    public float sizeX => transform.localScale.x*10/nums;
    public float sizeZ => transform.localScale.z*10/nums;

    public int[,] Grid = new int[6, 6]
    {
        // 0    1    2    3    4    5
        { 1, 1, -1, 1, 1, -1},   // 顶点0
        { -1, 1, 1, 1, -1, -1},   // 顶点1
        { 1, -1, 1, -1, 1, -1},     // 顶点2
        { -1, -1, 1, 1, 1, -1},     // 顶点3
        { 1, -1, 1, -1, 1, 1},     // 顶点4
        { 1, 1, 1, -1, -1, 1 }      // 顶点5
    };

    public List<GridCell> gridList;
    public GameObject GridCellPrefab;

    public Vector2Int Begin;
    public Vector2Int End;

    
    // Start is called before the first frame update
    void Start()
    {
        gridList = new();
        nums = Grid.GetLength(0);
        for (int i = 0; i < nums; i++)
        {
            for (int j = 0; j < nums; j++)
            {
                var go = Instantiate(GridCellPrefab,transform.position+new Vector3((i+0.5f) * sizeX,.5f,(j+0.5f) * sizeZ)-new Vector3(nums*sizeX/2,0,nums*sizeZ/2),Quaternion.identity);
                var g = go.AddComponent<GridCell>();
                g.PosX = i;
                g.PosY = j;
                if (Grid[i, j] == -1)
                {
                    g.Attachable = false;
                    go.GetComponent<MeshRenderer>().material.color = Color.red;
                }
                gridList.Add(g);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            StartCoroutine(Algorithm.Astar_Grid(Grid, gridList, Begin, End));
            // Debug.Log($"从(0,0)到(5,5)的最低代价为:{gridCell.F}");
            // Debug.Log(Algorithm.ReconstructPath_Grid(gridCell));
        }
    }
    
    private void OnDrawGizmos()
    {
        for (int i = 0; i < nums; i++)
        {
            for (int j = 0; j < nums; j++)
            {
                if (Grid[i, j] == -1)
                {
                    Gizmos.color = Color.red;
                }
                else if(i==Begin.x&&j==Begin.y)
                {
                    Gizmos.color = Color.white;
                }
                else if(i==End.x&&j==End.y)
                {
                    Gizmos.color = Color.black;
                }else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawWireCube(transform.position+new Vector3((i+0.5f) * sizeX,0,(j+0.5f) * sizeZ)-new Vector3(nums*sizeX/2,0,nums*sizeZ/2),new Vector3(sizeX*.8f,1,sizeZ*.8f));
            }
        }
    }
}
