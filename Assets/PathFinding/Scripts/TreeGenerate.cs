using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;


public class TreeNode
{
    public GameObject val;
    public TreeNode left;
    public TreeNode right;

    public TreeNode(GameObject val, TreeNode left, TreeNode right)
    {
        this.val = val;
        this.left = left;
        this.right = right;
    }

}





public class TreeGenerate : MonoBehaviour
{
    public GameObject treeNodeGo;
    [Range(1, 10)] public int nodeNums;
    private TreeNode _root;
    void CreateTree(int nodeNums)
    {
        List<TreeNode> treeNodeList = new();
        int layer;
        if (nodeNums <= 0) return;

        for (int i = 0; i < nodeNums; i++)
        {
            layer = Mathf.CeilToInt((Mathf.Log(i + 2)/Mathf.Log(2)) / (Mathf.Log(2)/Mathf.Log(2)));
            Debug.Log(layer);
            if (_root == null)
            {   
                var node = new TreeNode(Instantiate(treeNodeGo, transform), null, null);
                node.val.transform.position = Vector3.zero;
                node.val.name = "Root";
                _root = node;
                treeNodeList.Add(node);
            }
            else if (treeNodeList[0].left == null)
            {
                var node = new TreeNode(Instantiate(treeNodeGo, treeNodeList[0].val.transform), null, null);
                node.val.transform.position += Vector3.left * 8 * 1/layer;
                node.val.transform.position += Vector3.down * 8 * 1/layer;
                node.val.name = $"Node{i+1}";
                treeNodeList[0].left = node;            
                treeNodeList.Add(node);
            }
            else
            {
                var node = new TreeNode(Instantiate(treeNodeGo, treeNodeList[0].val.transform), null, null);
                node.val.transform.position += Vector3.right * 8 * 1/layer;
                node.val.transform.position += Vector3.down * 8 * 1/layer;
                treeNodeList[0].right = node;
                node.val.name = $"Node{i+1}";
                treeNodeList.RemoveAt(0);            
                treeNodeList.Add(node);
            }


        }

    }

    void ResetColor(TreeNode root)
    {
        Queue<TreeNode> queue = new();
        queue.Enqueue(root);
        int num = 1;

        while (num > 0)
        {
            num = queue.Count;
            for (int i = 0; i < num; i++)
            {
                TreeNode cur = queue.Dequeue();
                if (cur != null)
                {
                    cur.val.GetComponent<MeshRenderer>().material.color=Color.white;
                    if (cur.left != null)
                    {
                        queue.Enqueue(cur.left);
                    }

                    if (cur.left != null)
                    {
                        queue.Enqueue(cur.right);
                    }   
                }
            }
        }
    }
    private void Start()
    {
        CreateTree(nodeNums);
    }

    private void Update()
    {
        int source = 1;
        int target = 5;

        int INF = 100000;
        int[,] graph = new int[6, 6]
        {
            // 0    1    2    3    4    5
            { 0,   10,  INF, 30, 100, INF },   // 顶点0
            { INF, 0,   50,  INF, INF, INF },   // 顶点1
            { INF, INF, 0,   INF, 10, INF },     // 顶点2
            { INF, INF, 20,  0,   60, INF },     // 顶点3
            { INF, INF, INF, INF, 0,   10  },     // 顶点4
            { INF, INF, INF, INF, INF, 0   }      // 顶点5
        };
        
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            Algorithm.BFS(_root);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Algorithm.DFS(_root);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            var n = Algorithm.Dijkstra(graph,source,target);
            Debug.Log($"从{source}到{target}的最低代价为:{n.F}");
            Debug.Log(Algorithm.ReconstructPath(n));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            var n = Algorithm.Astar(graph,source,target);
            Debug.Log($"从{source}到{target}的最低代价为:{n.F}");
            Debug.Log(Algorithm.ReconstructPath(n));
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            ResetColor(_root);
        }
    }
}
