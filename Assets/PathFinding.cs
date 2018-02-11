using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    private Grid grid;

    private PathReqestManager pathRequestManager;

    // Use this for initialization
    void Awake()
    {
        pathRequestManager = GetComponent<PathReqestManager>();
        grid = GetComponent<Grid>();
    }

    IEnumerator FindPath(Vector3 seekerPos, Vector3 targetPos)
    {
        Vector3[] wayPoints = new Vector3[0];
        bool success = false;
        
        Node seekerNode = grid.NodeFromWorldPoint(seekerPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        if (seekerNode.walkable && targetNode.walkable)
        {
            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();

            openSet.Add(seekerNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet.RemoveFirst();

                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    success = true;
                    break;
                }

                foreach (var neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                        continue;
                    int newMovementCost = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCost;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                            openSet.Add(neighbour);
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        yield return null;
        if (success)
        {
            wayPoints = RetracePath(seekerNode, targetNode);
        }

        pathRequestManager.FinishedProcessingPath(wayPoints, success);
    }

    private Vector3[] RetracePath(Node seekerNode, Node targetNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != seekerNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] wayPoints = SimplifyPath(path);
        Array.Reverse(wayPoints);
        return wayPoints;
    }

    private Vector3[] SimplifyPath(List<Node> path)
    {
        //return path.Select(x => x.worldPos).ToArray();
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 oldDirection = Vector2.zero;
        for (int i = 1; i < path.Count; i++)
        {
            var newDirection = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (oldDirection != newDirection)
            {
                waypoints.Add(path[i].worldPos);
            }
            oldDirection = newDirection;
        }

        return waypoints.ToArray();
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
            return 14 * distY + 10 * (distX - distY);
        return 14 * distX + 10 * (distY - distX);
    }


    public void StartPathFind(Vector3 start, Vector3 target)
    {
        StartCoroutine(FindPath(start, target));
    }
}