using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR.WSA;

public class Grid : MonoBehaviour
{
    public bool showGrid;
    public LayerMask obstacles;
    public Vector2 gridWorldSize;

    public float nodeRadius;

    private Node[,] grid;

    private float nodeDiameter;

    private int gridSizeX, gridSizeY;

    public int MaxSize
    {
        get { return gridSizeX * gridSizeY; }
    }


    // Use this for initialization
    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        Debug.Log(string.Format("{0}:{1}", gridSizeX, gridSizeY));
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        var worldBotomLeft =
            transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (var x = 0; x < gridSizeX; x++)
        {
            for (var y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPosition = worldBotomLeft + Vector3.right * (x * nodeDiameter + nodeRadius)
                                                       + Vector3.forward * (y * nodeDiameter + nodeRadius);
                var walkable = !Physics.CheckSphere(worldPosition, nodeRadius, layerMask: obstacles);
                grid[x, y] = new Node(walkable, worldPosition, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        for (int y = -1; y <= 1; y++)
        {
            if (x == 0 && y == 0)
                continue;
            var checkX = node.gridX + x;
            var checkY = node.gridY + y;

            if (checkX >= 0 && checkY >= 0 && checkX < gridSizeX && checkY < gridSizeY)
                neighbours.Add(grid[checkX, checkY]);
        }

        return neighbours;
    }

    public Node NodeFromWorldPoint(Vector3 positon)
    {
        var percentX = (positon.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        var percentY = (positon.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        var x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        var y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && showGrid)
        {
            foreach (var node in grid)
            {
                Gizmos.color = node.walkable ? Color.white : Color.red;
                Gizmos.DrawWireCube(node.worldPos, Vector3.one * nodeDiameter * 0.95f);
                if (node.FCost != 0)
                {
                    //Gizmos.color = Color.green;
                    //Gizmos.DrawCube(node.worldPos, Vector3.one * nodeDiameter);
                    //Handles.Label(node.worldPos, node.FCost.ToString());
                }
            }
        }
    }
}