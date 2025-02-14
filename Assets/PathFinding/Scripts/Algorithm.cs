using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

// 节点类，表示图中一个顶点
public class GraphNode : IComparable<GraphNode>
{
    public int Id { get; set; } // 节点编号
    public int G { get; set; } // 从起点到该节点的实际代价
    public int H { get; set; } // 启发式代价（对于 Dijkstra，始终为 0）
    public int F => G + H; // 总评估值 f = g + h
    public GraphNode Parent { get; set; } // 父节点，用于回溯路径

    // 按照 f 值（若相等则按 Id）排序
    public int CompareTo(GraphNode other)
    {
        int cmp = this.F.CompareTo(other.F);
        if (cmp == 0)
            cmp = this.Id.CompareTo(other.Id);
        return cmp;
    }

    public override bool Equals(object obj)
    {
        if (obj is GraphNode other)
            return this.Id == other.Id;
        return false;
    }

    public override int GetHashCode()
    {
        return this.Id;
    }
}

public static class Algorithm
{
    public static void BFS(TreeNode root)
    {
        var squence = DOTween.Sequence();
        squence.Pause();
        
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
                    squence.Append(cur.val.GetComponent<MeshRenderer>().material.DOColor(Color.red, .2f));
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
        squence.Play();
    }
    
    public static void DFS(TreeNode root)
    {
        var squence = DOTween.Sequence();
        Stack<TreeNode> stack = new();
        int num = 1;
        stack.Push(root);
        squence.Pause();
        while (num > 0)
        {
            for (int i = 0; i < num; i++)
            {
                TreeNode cur = stack.Pop();
                if (cur.right != null)
                {
                    stack.Push(cur.right);
                }
                squence.Append(cur.val.GetComponent<MeshRenderer>().material.DOColor(Color.red, .2f));
                if (cur.left != null)
                {
                    stack.Push(cur.left);
                }
            }
            num = stack.Count;
        }

        squence.Play();
    }
    
    // public static void Dijkstra()
    // {
    //     // 根据图的顶点数，构建图的邻接矩阵
    //     const int INF = 100000;
    //     const int vertexCount = 6;
    //     int[,] graph = new int[vertexCount, vertexCount]
    //     {
    //         // 0    1    2    3    4    5
    //         { 0,   10,  INF, 30, 100, INF },   // 顶点0
    //         { INF, 0,   50,  INF, INF, INF },   // 顶点1
    //         { INF, INF, 0,   INF, 10, INF },     // 顶点2
    //         { INF, INF, 20,  0,   60, INF },     // 顶点3
    //         { INF, INF, INF, INF, 0,   10  },     // 顶点4
    //         { INF, INF, INF, INF, INF, 0   }      // 顶点5
    //     };
    //     
    //     // Dijkstra算法的三大关键
    //     // 1.dist[]记录每个点到起始点最短距离
    //     // 2.visited[]记录每个点是否已被访问，被访问的点到起始点最短距离以确定
    //     // 3.prev[]记录每个点在自己最短路径上的的前驱节点，用于回溯路径
    //     int[] dist = new int[vertexCount];
    //     bool[] visited = new bool[vertexCount];
    //     int[] prev = new int[vertexCount];
    //     
    //     // init
    //     for (int i = 0; i < vertexCount; i++)
    //     {
    //             dist[i] = INF;
    //             visited[i] = false;
    //             prev[i] = -1;
    //     }
    //
    //     // 假设source为起始节点
    //     int source = 0;
    //     dist[source] = 0;
    //     
    //     // 开始遍历graph，一共遍历vertexCount次，每次找到一个最短路径
    //     for (int i = 0; i < vertexCount; i++)
    //     {
    //         // 1.从未访问节点中 找到 一个与起点距离最近的点
    //         // 第一次找到的节点为起点本身
    //         int min = INF;
    //         int minIndex = -1;
    //         for (int j = 0; j < vertexCount; j++)
    //         {
    //             if (!visited[j] && dist[j] < min)
    //             {
    //                 min = dist[j];
    //                 minIndex = j;
    //             }
    //         }
    //         // 所有节点已被访问则不再遍历
    //         if (minIndex == -1)
    //             break;
    //
    //         // 该轮访问minIndex下标的节点
    //         visited[minIndex] = true;
    //         
    //         // 2.预计算该轮访问节点的邻接节点到起点距离
    //         for (int j = 0; j < vertexCount; j++)
    //         {
    //             if (!visited[j] && graph[minIndex, j] != INF && dist[j] > graph[minIndex, j] + dist[minIndex])
    //             {
    //                 dist[j] = graph[minIndex, j] + dist[minIndex];
    //                 prev[j] = minIndex;
    //             }
    //         }
    //     }
    //     
    //     
    //     Debug.Log("从源点 " + 0 + " 到各顶点的最短距离为：");
    //     for (int i = 0; i < vertexCount; i++)
    //     {
    //         Debug.Log("到顶点 " + i + " 的距离 = " + dist[i] +
    //                   " ，路径 = " + GetPath(prev, i));
    //     }
    //     
    //
    // }
    //
        
    // static string GetPath(int[] prev, int vertex)
    // {
    //     // 使用递归方式或循环方式构造路径
    //     if (prev[vertex] == -1)
    //     {
    //         // 如果无前驱，则返回顶点本身（通常是源点）
    //         return vertex.ToString();
    //     }
    //     else
    //     {
    //         // 递归调用，构造从源点到前驱节点的路径，再加上当前顶点
    //         return GetPath(prev, prev[vertex]) + "->" + vertex;
    //     }
    // }
    public static GraphNode Dijkstra(int[,] graph, int source,int target)
    {
        // 根据图的顶点数，构建图的邻接矩阵
        int INF = 100000;
        int vertexCount = graph.GetLength(0);        
        // 假设source为起始节点
        // int source = 0;
        // int[,] graph = new int[vertexCount, vertexCount]
        // {
        //     // 0    1    2    3    4    5
        //     { 0,   10,  INF, 30, 100, INF },   // 顶点0
        //     { INF, 0,   50,  INF, INF, INF },   // 顶点1
        //     { INF, INF, 0,   INF, 10, INF },     // 顶点2
        //     { INF, INF, 20,  0,   60, INF },     // 顶点3
        //     { INF, INF, INF, INF, 0,   10  },     // 顶点4
        //     { INF, INF, INF, INF, INF, 0   }      // 顶点5
        // };
        
        // OpenSet：使用 SortedSet，根据 f 值排序（对于 Dijkstra，f = g）
        SortedSet<GraphNode> openSet = new SortedSet<GraphNode>();
        // ClosedSet：已处理节点编号集合
        HashSet<int> closedSet = new HashSet<int>();

        // 初始化起点节点（h 设为 0）
        GraphNode startNode = new GraphNode { Id = source, G = 0, H = 0, Parent = null };
        openSet.Add(startNode);
        
        // 不断扩展遍历直到openSet没有节点
        while (openSet.Count > 0)
        {
            //1.从openSet中找到最小g值节点
            var current = openSet.Min();
            
            // 移出openSet加入closeSet
            openSet.Remove(current);
            if (current.Id == target)
                return current; // 找到目标
            closedSet.Add(current.Id);
            
            
            //2.将该节点邻接点放入openSet或更新
            for (int i = 0; i < vertexCount; i++)
            {
                // 若非邻接或已经在closedSet则跳过
                if (graph[current.Id,i] == INF || closedSet.Contains(i))
                {
                    continue;
                }
                
                int tentativeG = current.G + graph[current.Id, i];
                // 构造邻居节点，Dijkstra 中 h=0
                GraphNode neighbor = new GraphNode { Id = i, H = 0 };
                // 若在openSet里则判断是否更新
                if (openSet.TryGetValue(neighbor, out GraphNode existing))
                {
                    if (tentativeG < existing.G)
                    {
                        openSet.Remove(existing);
                        existing.G = tentativeG;
                        existing.Parent = current;
                        openSet.Add(existing);
                    }                
                }
                else
                {
                    neighbor.G = tentativeG;
                    neighbor.Parent = current;
                    openSet.Add(neighbor);
                }

            }

        }

        return null;

    }
    
    /// 启发式函数 h(n)；本例中简单使用绝对值距离（假设节点编号代表某种位置关系）
    static int Manhattan(int node, int target)
    {
        return Math.Abs(node - target);
    }
    public static GraphNode Astar(int[,] graph, int source,int target)
    {
        // 根据图的顶点数，构建图的邻接矩阵
        int INF = 100000;
        int vertexCount = graph.GetLength(0);        
        // 假设source为起始节点
        // int source = 0;
        // int[,] graph = new int[vertexCount, vertexCount]
        // {
        //     // 0    1    2    3    4    5
        //     { 0,   10,  INF, 30, 100, INF },   // 顶点0
        //     { INF, 0,   50,  INF, INF, INF },   // 顶点1
        //     { INF, INF, 0,   INF, 10, INF },     // 顶点2
        //     { INF, INF, 20,  0,   60, INF },     // 顶点3
        //     { INF, INF, INF, INF, 0,   10  },     // 顶点4
        //     { INF, INF, INF, INF, INF, 0   }      // 顶点5
        // };
        
        // OpenSet：使用 SortedSet，根据 f 值排序（对于 Dijkstra，f = g）
        SortedSet<GraphNode> openSet = new SortedSet<GraphNode>();
        // ClosedSet：已处理节点编号集合
        HashSet<int> closedSet = new HashSet<int>();

        // 初始化起点节点（h 设为 0）
        GraphNode startNode = new GraphNode { Id = source, G = 0,H = Manhattan(source, target), Parent = null };
        openSet.Add(startNode);
        
        // 不断扩展遍历直到openSet没有节点
        while (openSet.Count > 0)
        {
            //1.从openSet中找到最小g值节点
            var current = openSet.Min();
            
            // 移出openSet加入closeSet
            openSet.Remove(current);
            if (current.Id == target)
                return current; // 找到目标
            closedSet.Add(current.Id);
            
            
            //2.将该节点邻接点放入openSet或更新
            for (int i = 0; i < vertexCount; i++)
            {
                // 若非邻接或已经在closedSet则跳过
                if (graph[current.Id,i] == INF || closedSet.Contains(i))
                {
                    continue;
                }
                
                int tentativeG = current.G + graph[current.Id, i];
                // 构造邻居节点，Dijkstra 中 h=0
                GraphNode neighbor = new GraphNode { Id = i, H = Manhattan(i, target)};
                // 若在openSet里则判断是否更新
                if (openSet.TryGetValue(neighbor, out GraphNode existing))
                {
                    if (tentativeG < existing.G)
                    {
                        openSet.Remove(existing);
                        existing.G = tentativeG;
                        existing.Parent = current;
                        openSet.Add(existing);
                    }                
                }
                else
                {
                    neighbor.G = tentativeG;
                    neighbor.Parent = current;
                    openSet.Add(neighbor);
                }

            }

        }

        return null;

    }
    public static string ReconstructPath(GraphNode targetNode)
    {
        List<int> path = new List<int>();
        GraphNode current = targetNode;
        while (current != null)
        {
            path.Add(current.Id);
            current = current.Parent;
        }
        path.Reverse();
        return string.Join(" -> ", path);
    }
    static int Manhattan_Grid(GridCell node, GridCell target)
    {
        return (Math.Abs(node.PosX - target.PosX) + Math.Abs(node.PosY - target.PosY))*10;
    }
    public static IEnumerator Astar_Grid(int[,]grid, List<GridCell> gridList, Vector2Int source,Vector2Int target)
    {
        Debug.Log("Begin Path Finding");
        // 根据图的顶点数，构建图的邻接矩阵
        int sideNums = grid.GetLength(0);
        
        // OpenSet：使用 SortedSet，根据 f 值排序（对于 Dijkstra，f = g）
        SortedSet<GridCell> openSet = new SortedSet<GridCell>();
        // ClosedSet：已处理节点编号集合
        HashSet<GridCell> closedSet = new HashSet<GridCell>();

        // 初始化起点节点（h 设为 0）
        GridCell startNode = gridList.Find((g) => { return g.PosX == source.x && g.PosY == source.y;});
        GridCell endNode = gridList.Find((g) => { return g.PosX == target.x && g.PosY == target.y;});

        openSet.Add(startNode);
        
        // 不断扩展遍历直到openSet没有节点
        while (openSet.Count > 0)
        {
            //1.从openSet中找到最小f值节点
            var current = openSet.Min();
            current.gameObject.GetComponent<MeshRenderer>().material.color = Color.green;

            // 移出openSet加入closeSet
            openSet.Remove(current);
            if (current == endNode)
            {
                Debug.Log("Find Target");
                yield return current; // 找到目标
                break;
            }
            closedSet.Add(current);

            
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    int currPosX = current.PosX + i;
                    int currPosY = current.PosY + j;

                    if (currPosX < 0 || currPosY < 0 || currPosX >= sideNums || currPosY >= sideNums || (currPosX==current.PosX&&currPosY==current.PosY))
                    {
                        // 超出地图边界
                        continue;
                    }
                    
                    var neighbor = gridList.Find((g) => { return g.PosX == currPosX && g.PosY == currPosY; });

                    if (!neighbor.Attachable || closedSet.Contains(neighbor))
                    {
                        // 若是障碍物或在CloseSet里
                        continue;
                    }
                    
                    neighbor.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                    int tentativeG = current.G + (Mathf.Abs(current.PosX-neighbor.PosX+currPosY-neighbor.PosY)==1?14:10);
                    neighbor.H = Manhattan_Grid(neighbor, endNode);
                    if (openSet.TryGetValue(neighbor, out GridCell existing))
                    {
                        if (tentativeG < existing.G)
                        {
                            openSet.Remove(existing);
                            existing.G = tentativeG;
                            existing.Parent = current;
                            openSet.Add(existing);
                        }
                    }
                    else
                    {
                        neighbor.G = tentativeG;
                        neighbor.Parent = current;
                        openSet.Add(neighbor);
                    }

                }
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;

    }

    public static string ReconstructPath_Grid(GridCell targetNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        GridCell current = targetNode;
        while (current != null)
        {
            path.Add(new Vector2Int(current.PosX,current.PosY));
            current = current.Parent;
        }
        path.Reverse();
        return string.Join(" -> ", path);
    }
    
    // public static IEnumerator JumpPointSearch_Grid(GridCellJPS[,]grid, List<GridCellJPS> gridList, Vector2Int source,Vector2Int target)
    // {
    //     Debug.Log("Begin Path Finding");
    //     // 根据图的顶点数，构建图的邻接矩阵
    //     int sideNums = grid.GetLength(0);
    //     
    //     // OpenSet：使用 SortedSet，根据 f 值排序（对于 Dijkstra，f = g）
    //     SortedSet<GridCellJPS> jumpPointSet = new SortedSet<GridCellJPS>();
    //
    //     // 初始化起点节点（h 设为 0）
    //     GridCellJPS startNode = gridList.Find((g) => { return g.pos == source;});
    //     jumpPointSet.Add(startNode);
    //
    //     while (jumpPointSet.Count>0)
    //     {
    //         var curr = jumpPointSet.Min();
    //         
    //     }
    //
    // }
    
    
}


