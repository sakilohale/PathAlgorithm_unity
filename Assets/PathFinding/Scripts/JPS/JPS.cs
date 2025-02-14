using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class JpsUtils{
    public static readonly Vector2Int left = Vector2Int.left;
    public static readonly Vector2Int right = Vector2Int.right;
    public static readonly Vector2Int up = Vector2Int.up;
    public static readonly Vector2Int down = Vector2Int.down;
    public static readonly Vector2Int upRight = Vector2Int.one;
    public static readonly Vector2Int upLeft = new Vector2Int(-1, 1);
    public static readonly Vector2Int downRight = new Vector2Int(1, -1);
    public static readonly Vector2Int downLeft = new Vector2Int(-1, -1);
    public static Dictionary<Vector2Int, Vector2Int[]> verticalDirLut;

    public static void init(){
        // 将常用的方向加入到一个字典中，通过方向来索引它的垂线方向
        
        verticalDirLut = new Dictionary<Vector2Int, Vector2Int[]>();
        Vector2Int[] horizontalLines = new Vector2Int[]{left, right};
        Vector2Int[] verticalLines = new Vector2Int[]{up, down};
        verticalDirLut.Add(left, verticalLines);
        verticalDirLut.Add(right, verticalLines);
        verticalDirLut.Add(up, horizontalLines);
        verticalDirLut.Add(down, horizontalLines);
    }

    /// <summary> 判断当前方向是否为一个直线方向 </summary>
    public static bool isLineDireaction(Vector2Int direaction){
        return direaction.x * direaction.y == 0;
    }
    public static int manhattan(Vector2Int p1, Vector2Int p2){
        /* 曼哈顿距离 */

        return Mathf.Abs(p1.x - p2.x) + Mathf.Abs(p1.y - p2.y);
    }
    public static int euler(Vector2Int p1, Vector2Int p2){
        /* 欧拉距离 */

        int dx = p1.x - p2.x;
        int dy = p1.y - p2.y;
        return dx * dx + dy * dy;
    }
}
public class JpsNode{
    /* 跳点, 跳点会记录当前自身需要查询的方向以及父跳点的位置 */

    public Vector2Int pos;
    public List<Vector2Int> parents;
    public Vector2Int[] direactions;
    public int cost;
    public JpsNode(Vector2Int parent, Vector2Int pos, Vector2Int[] dirs, int cost){

        this.pos = pos;
        this.direactions = dirs;
        this.cost = cost;
        this.parents = new List<Vector2Int>();
        this.parents.Add(parent);
    }
}

public class JPS
{
    private Dictionary<Vector2Int, JpsNode> lut = new();
    private SortedList<JpsNode,float> nodes = new();
    private Vector2Int start;
    private Vector2Int end;
    private GridBase env;
    public JPS(){
    }
    
    
    public Vector2Int[] find(GridBase env, Vector2Int s, Vector2Int e){
        
        Debug.Log("Begin PathFinding");
        this.env = env;
        this.start = s;
        this.end = e;

        this.lut.Add(s, new JpsNode(s, s, new Vector2Int[0], 0));            // 直接将起点加入到查找表

        // 起点是一个特殊的跳点，也是唯一一个全方向检测的跳点，其他跳点最多拥有三个方向
        Vector2Int[] dirs = new Vector2Int[]{
            JpsUtils.up,
            JpsUtils.down,
            JpsUtils.left,
            JpsUtils.right,
            JpsUtils.upLeft,
            JpsUtils.upRight,
            JpsUtils.downLeft,
            JpsUtils.downRight,
        };
        JpsNode S = new JpsNode(s, s, dirs, 0);
        nodes.Add(S, 0);

        while(nodes.Count>0){
            JpsNode node = nodes.Keys[0];
            nodes.Remove(node);
            if(node.pos == end)return completePath();
            foreach(Vector2Int d in node.direactions){
                if(JpsUtils.isLineDireaction(d)){
                    testLine(node.pos, d, node.cost);
                }else{
                    testDiagonal(node.pos, node.pos, d, node.cost);
                }
            }
        }
        Debug.Log("End PathFinding");

        return null;
    }

    
    
    private void addPoint(Vector2Int parent, Vector2Int p, Vector2Int[] dirs, int fcost){
        /* 追加一个新的跳点
        @parent: 跳点的父节点
        @p: 跳点的位置
        @dirs: 跳点应该扫描的方向
        @fcost: 从起点到跳点的消耗 */

        if(lut.ContainsKey(p)){
            lut[p].parents.Add(parent);
        }else{
            JpsNode node = new JpsNode(parent, p, dirs, fcost);
            lut.Add(p, node);
            nodes.Add(node, fcost + JpsUtils.euler(p, end));
            env.Grid[p.x, p.y].GetComponent<MeshRenderer>().material.color = Color.cyan;
        }
    }
    
    private List<Vector2Int> testForceNeighborsInLine(Vector2Int p, Vector2Int d){
        /* 检查给定的目标点和方向是否存在强制邻居, 该函数只适用于横纵搜索
        @p: 点X
        @d: 方向PX，P为X的父节点*/

        List<Vector2Int> directions = new List<Vector2Int>();
        foreach(Vector2Int vd in JpsUtils.verticalDirLut[d]){
            Vector2Int blockPt = vd + p;
            if(env.IsWall(blockPt) && env.IsAttachable(blockPt + d))directions.Add(vd + d);
        }
        return directions;
    }
    
    private bool testLine(Vector2Int parent, Vector2Int d, int fcost){
        /* 从当前点p开始沿着直线方向d进行循环移动, 遇到跳点、墙体、地图边缘时退出循环，如果该函数是因为
        跳点退出函数的，那么返回true，否则为false
        @parent：从parent的下一个位置开始检测（这表示当前节点正处于parent）
        @d：前进的方向
        @fcost：从起点到parent节点的总消耗*/

        Vector2Int p = parent + d;
        while(env.IsAttachable(p)){
            if(p == end){
                /* 找到终点时将终点加入openset */
                addPoint(parent, p, new Vector2Int[0], 0);
                return true;
            }
            fcost ++;
            List<Vector2Int> directions = testForceNeighborsInLine(p, d);
            if(directions.Count > 0){
                directions.Add(d);
                addPoint(parent, p, directions.ToArray(), fcost);
                return true;
            }
            p += d;
        }
        return false;
    }
    
    private bool diagonalExplore(Vector2Int p, Vector2Int d, int cost){
        /* 朝着角点的分量方向进行探索
        @p: 斜向移动后的点位p
        @d: 斜向方向
        @cost: 走到点p时所消耗的成本 */
        bool _1 = testLine(p, new Vector2Int(d.x, 0), cost);
        bool _2 = testLine(p, new Vector2Int(0, d.y), cost);
        return _1 || _2;
    }
    
    private List<Vector2Int> testForceNeighborsInDiagonal(Vector2Int X, Vector2Int B, Vector2Int D, Vector2Int mask){
        /* 检查给定地目标点和方向是否存在强制邻居, 该函数只适用于斜向搜索
        只要检测到一边就可以退出函数了，因为只可能存在一边
        @X: 移动到的点X，
        @B：X点侧边的障碍物
        @D: X - parent
        @mask: 方向遮罩 */

        List<Vector2Int> directions = new List<Vector2Int>();
        B += D * mask;
        if(env.IsAttachable(B)){
            directions.Add(B - X);
        }
        return directions;
    }
    
    private void testDiagonal(Vector2Int parent, Vector2Int p, Vector2Int d, int fcost){
        /* 斜向移动，每次斜向移动一格，并检查斜向分量是否存在跳点。
        @parent: 斜向移动最初的起点，parent必是一个跳点，否则它不会触发testDiagonal函数
        @p: 当前移动到的点p 
        @d: 斜向的方向
        @fcost: 消耗 */

        // 计算障碍物1和障碍物2的位置
        Vector2Int b1 = new Vector2Int(p.x + d.x, p.y);
        Vector2Int b2 = new Vector2Int(p.x, p.y + d.y);
        if(env.IsAttachable(b1)){
            if(env.IsAttachable(b2)){
                /* 情况1，B1和B2均为空，可以移动且本次移动不需要检测斜向的跳点 */
                p += d;
                if(env.IsAttachable(p)){
                    //新的位置不是障碍物
                    fcost ++;
                    if(p == end){
                        addPoint(parent, p, null, fcost);
                        return;
                    }
                    if(diagonalExplore(p, d, fcost)){
                        addPoint(parent, p, new Vector2Int[]{d}, fcost);
                        return;
                    }
                    testDiagonal(parent, p, d, fcost);          // 递归该函数
                }
            }else{
                // 情况3，b1可以移动，而b2不可移动
                p += d;
                if(env.IsAttachable(p)){
                    fcost ++;
                    if(p == end){
                        addPoint(parent, p, null, fcost);
                        return;
                    }
                    List<Vector2Int> dirs = testForceNeighborsInDiagonal(p, b2, d, Vector2Int.up);
                    if(diagonalExplore(p, d, fcost) || dirs.Count > 0){
                        dirs.Add(d);
                        addPoint(parent, p, dirs.ToArray(), fcost);
                        return;
                    }
                    testDiagonal(parent, p, d, fcost);
                }
            }
        }else{
            if(env.IsAttachable(b2)){
                // 情况4，b2可以移动，而b1不可移动

                p += d;
                if(env.IsAttachable(p)){
                    fcost ++;
                    if(p == end){
                        addPoint(parent, p, null, fcost);
                        return;
                    }
                    List<Vector2Int> dirs = testForceNeighborsInDiagonal(p, b1, d, Vector2Int.right);
                    if(diagonalExplore(p, d, fcost) || dirs.Count > 0){
                        dirs.Add(d);
                        addPoint(parent, p, dirs.ToArray(), fcost);
                        return;
                    }
                    testDiagonal(parent, p, d, fcost);
                }
            }else{
                // 情况2，两者均不可移动，什么都不做
                // code..
            }
        }
    }
        

    
    public Vector2Int[] completePath(){

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Queue<JpsNode> openSet = new Queue<JpsNode>();
        openSet.Enqueue(lut[end]);
        while(openSet.Count > 0){
            JpsNode node = openSet.Dequeue();
            closedSet.Add(node.pos);
            foreach(Vector2Int pos in node.parents){
                if(closedSet.Contains(pos))continue;
                cameFrom.Add(node.pos, pos);
                if(pos == start)return _trace(cameFrom);
                openSet.Enqueue(lut[pos]);
            }
        }
        return null;
    }
    private Vector2Int[] _trace(Dictionary<Vector2Int, Vector2Int> cameFrom){
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int current = end;
        while(current != start){
            path.Add(current);
            current = cameFrom[current];
        }
        path.Add(start);
        return path.ToArray();
    }
    
    
}


