using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    // X and Y position in the Node Array
    public int gridX, gridY;

    // Tells program if this node is being obstructed
    public bool isWall;

    // The world position of the node
    public Vector3 position;

    // For A*, will store what node it previously came from so it can trace the shortest path
    public Node Parent;
    
    // The cost of moving to the next square (g) and the distance to the goal from the node (h)
    public int gCost, hCost;

    // Get function to get the f cost, which is the sum of the g cost and the h cost
    public int FCost { get { return gCost + hCost; } }

    public Node(bool isWall, Vector3 position, int gridX, int gridY) {
        this.isWall = isWall;
        this.position = position;
        this.gridX = gridX;
        this.gridY = gridY;
    }

}
