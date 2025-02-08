using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;


class TreeNode
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
    
    private void Start()
    {
        CreateTree(nodeNums);
    }
}
