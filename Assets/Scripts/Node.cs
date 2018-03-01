using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node> {
    public Vector3 worldPos;
    public int gridX;
    public int gridY;

    public int hCost;
    public int gCost;

    public int FCost {
        get { return hCost + gCost; }
    }

    public bool walkable;
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY) {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int CompareTo(Node other) {
        int compare = FCost.CompareTo(other.FCost);
        if (compare == 0) {
            compare = hCost.CompareTo(other.hCost);
        }

        return -compare;
    }

    public int HeapIndex { get; set; }
}
