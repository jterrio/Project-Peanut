using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    Grid GridReference;//For referencing the grid class
    public Transform StartPosition;//Starting position to pathfind from
    public Transform TargetPosition;//Starting position to pathfind to
    private Node StartNode;
    private Node TargetNode;
    private Coroutine PathCoroutine;
    public int maxNodesPath = 500;

    private void Awake()//When the program starts
    {
        GridReference = GetComponent<Grid>();//Get a reference to the game manager
    }

    public List<Vector3> FindPath(Vector3 a_StartPos, Vector3 a_TargetPos) {
        List<Node> a = new List<Node>();
        int times = 0;
        List<List<Node>> Paths = new List<List<Node>>();

        while (times < 3) {
            times++;
            StartNode = GridReference.NodeFromWorldPoint(a_StartPos);//Gets the node closest to the starting position
            TargetNode = GridReference.NodeFromWorldPoint(a_TargetPos);//Gets the node closest to the target position
            int count = 0;

            List<Node> OpenList = new List<Node>();//List of nodes for the open list
            HashSet<Node> ClosedList = new HashSet<Node>();//Hashset of nodes for the closed list

            OpenList.Add(StartNode);//Add the starting node to the open list to begin the program

            while (OpenList.Count > 0)//Whilst there is something in the open list
            {
                Node CurrentNode = OpenList[0];//Create a node and set it to the first item in the open list
                for (int i = 1; i < OpenList.Count; i++)//Loop through the open list starting from the second object
                {
                    if (OpenList[i].FCost < CurrentNode.FCost || OpenList[i].FCost == CurrentNode.FCost && OpenList[i].ihCost < CurrentNode.ihCost)//If the f cost of that object is less than or equal to the f cost of the current node
                    {
                        CurrentNode = OpenList[i];//Set the current node to that object
                    }
                }
                OpenList.Remove(CurrentNode);//Remove that from the open list
                ClosedList.Add(CurrentNode);//And add it to the closed list

                if (CurrentNode == TargetNode)//If the current node is the same as the target node
                {
                    a = GetFinalPathObstructed(StartNode, TargetNode);//Calculate the final path
                    if (!IsPathBlocked(a)) {
                        ClearLOSNodes();
                        return NodesToPoints(a);
                    } else {
                        break;
                    }
                }

                foreach (Node NeighborNode in GridReference.GetNeighboringNodes(CurrentNode))//Loop through each neighbor of the current node
                {
                    count++;
                    //SetNodesInSight(NeighborNode);
                    if (!NeighborNode.bIsWall || ClosedList.Contains(NeighborNode) || GridReference.BlockedPath.Contains(NeighborNode))//If the neighbor is a wall or has already been checked
                    {
                        continue;//Skip it
                    }
                    int MoveCost = CurrentNode.igCost + GetManhattenDistance(CurrentNode, NeighborNode);//Get the F cost of that neighbor

                    if (MoveCost < NeighborNode.igCost || !OpenList.Contains(NeighborNode))//If the f cost is greater than the g cost or it is not in the open list
                    {
                        NeighborNode.igCost = MoveCost;//Set the g cost to the f cost
                        NeighborNode.ihCost = GetManhattenDistance(NeighborNode, TargetNode);//Set the h cost
                        NeighborNode.ParentNode = CurrentNode;//Set the parent of the node for retracing steps

                        if (!OpenList.Contains(NeighborNode))//If the neighbor is not in the openlist
                        {
                            OpenList.Add(NeighborNode);//Add it to the list
                        }
                    }
                }
            }
            Paths.Add(a);
        }

        float distanceFromTarget = 999999;
        int t = 0;
        int r = 0;
        foreach(List<Node> p in Paths) {
            if(p.Count == 0) {
                continue;
            }
            Node endNode = p[p.Count - 1];
            float checkDistance = Vector3.Distance(endNode.vPosition, GameManager.gm.player.transform.position);
            if(checkDistance < distanceFromTarget) {
                r = t;
                distanceFromTarget = checkDistance;
            }
            t++;
        }
        ClearLOSNodes();
        return NodesToPoints(Paths[r]);
    }

    List<Vector3> NodesToPoints(List<Node> nodes) {
        List<Vector3> a = new List<Vector3>();
        foreach(Node n in nodes) {
            a.Add(n.vPosition);
        }
        return a;
    }

    bool IsPathBlocked(List<Node> path) {
        if (GameManager.gm.blink.isBlinking) {
            return false;
        }
        bool isBlocked = false;
        foreach(Node n in new List<Node>(path)) {
            if (SetNodesInSight(n)) {
                isBlocked = true;
            }
            if (isBlocked) {
                path.Remove(n);
                if (!GridReference.BlockedPath.Contains(n)) {
                    GridReference.BlockedPath.Add(n);
                }
            }
        }


        return isBlocked;
    }


    List<Node> GetFinalPath(Node a_StartingNode, Node a_EndNode) {
        int i = 0;
        List<Node> FinalPath = new List<Node>();//List to hold the path sequentially 
        Node CurrentNode = a_EndNode;//Node to store the current node being checked

        while (CurrentNode != a_StartingNode && CurrentNode.ParentNode != null)//While loop to work through each node going through the parents to the beginning of the path
        {
            if(i++ > maxNodesPath) {
                break;
            }
            FinalPath.Add(CurrentNode);//Add that node to the final path
            CurrentNode = CurrentNode.ParentNode;//Move onto its parent node
        }

        FinalPath.Reverse();//Reverse the path to get the correct order

        return FinalPath;
    }

    List<Node> GetFinalPathObstructed(Node a_StartingNode, Node a_EndNode) {
        List<Node> FinalPath = GetFinalPath(a_StartingNode, a_EndNode);
        List<Node> ObsPath = new List<Node>();

        foreach(Node n in FinalPath) {
            if (n.inSight) {
                break;
            } else {
                ObsPath.Add(n);
            }
        }

        return ObsPath;
    }

    int GetManhattenDistance(Node a_nodeA, Node a_nodeB) {
        int ix = Mathf.Abs(a_nodeA.iGridX - a_nodeB.iGridX);//x1-x2
        int iy = Mathf.Abs(a_nodeA.iGridY - a_nodeB.iGridY);//y1-y2

        return ix + iy;//Return the sum
    }

    bool SetNodesInSight(Node n) {

        //Check if node we are checking is our start or end position
        if (!(n.iGridX == StartNode.iGridX && n.iGridX == StartNode.iGridY) || !(n.iGridX == TargetNode.iGridX && n.iGridX == TargetNode.iGridY)) {
            //Points to check
            List<Vector3> pointsToCheck = new List<Vector3>();
            //pointsToCheck.Add(n.vPosition);
            pointsToCheck.Add(new Vector3(n.vPosition.x + GridReference.fNodeRadius, 0, n.vPosition.z + GridReference.fNodeRadius));
            pointsToCheck.Add(new Vector3(n.vPosition.x - GridReference.fNodeRadius, 0, n.vPosition.z + GridReference.fNodeRadius));
            pointsToCheck.Add(new Vector3(n.vPosition.x + GridReference.fNodeRadius, 0, n.vPosition.z - GridReference.fNodeRadius));
            pointsToCheck.Add(new Vector3(n.vPosition.x - GridReference.fNodeRadius, 0, n.vPosition.z - GridReference.fNodeRadius));

            foreach (Vector3 corner in pointsToCheck) {
                //Check to see if the point is inside the camera view
                Vector3 screenPoint = Camera.main.WorldToViewportPoint(corner);
                if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1) {
                    Ray ray = Camera.main.ViewportPointToRay(screenPoint);
                    RaycastHit hit;
                    //Debug.DrawRay(GameManager.gm.player.transform.position, corner);
                    if (!Physics.Raycast(ray, out hit, Vector3.Distance(GameManager.gm.player.transform.position, corner), GridReference.WallMask)) {
                        n.inSight = true;
                        return true;
                    }
                }
            }
        }
        n.inSight = false;
        return false;
    }

    void ClearLOSNodes() {
        foreach(Node n in GridReference.BlockedPath) {
            n.inSight = false;
        }
        GridReference.BlockedPath.Clear();
    }
    /*
    public Vector3[] GetCornerPath() {
        List<Vector3> corners = new List<Vector3>();
        if (GridReference.FinalPath == null) {
            return corners.ToArray();
        }
        foreach(Node n in GridReference.FinalPath) {
            corners.Add(n.vPosition);
        }
        return corners.ToArray();
    }*/




}