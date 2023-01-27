using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGrid : MonoBehaviour
{
    [Tooltip("Bottom left wall of the map")]
    public Transform bottomLeft;    // (2,2,2)
    
    [Tooltip("Top right wall of the map")]
    public Transform topRight;  // (102,2,78)

    // Size of the nodes
    Vector2 nodeSize;

    // Grid representation of the map
    Node[,] grid;

    // Amount of nodes in the grid on the x and y axes
    int gridSizeX, gridSizeY;

    // Final Path
    public List<Node> finalPath;

    void Start() {
        // Initialize nodeSize with the scale of the bottomLeft wall (assuming all walls and nodes are the same size)
        nodeSize = new Vector2(bottomLeft.localScale.x, bottomLeft.localScale.y);
        CreateGrid();
    }

    /// <summary>Creates the map array grid representation for pathfinding.</summary>
    void CreateGrid() 
    {
        // Calculate the grid array size based on the bottom left and top right nodes of the map
        gridSizeX = Mathf.RoundToInt((topRight.position.x - bottomLeft.position.x) / 4 + 1); // 26
        gridSizeY = Mathf.RoundToInt((topRight.position.z - bottomLeft.position.z) / 4 + 1); // 20

        // Initialize grid array
        grid = new Node[gridSizeX, gridSizeY];

        // Get all floor and wall objects from map
        GameObject[] floorObjects = GameObject.FindGameObjectsWithTag("Floor");
        GameObject[] wallObjects = GameObject.FindGameObjectsWithTag("Wall");

        // Add all floor objects to correct positions in grid array
        foreach (GameObject floor in floorObjects)
        {
            int x = Mathf.RoundToInt((floor.transform.position.x - bottomLeft.position.x) / nodeSize.x);
            int y = Mathf.RoundToInt((floor.transform.position.z - bottomLeft.position.z) /  nodeSize.y);
            grid[x, y] = new Node(false, floor.transform.position, x, y);
        }

        // Add all wall objects to correct positions in grid array
        foreach (GameObject wall in wallObjects)
        {
            int x = Mathf.RoundToInt((wall.transform.position.x - bottomLeft.position.x) / nodeSize.x);
            int y = Mathf.RoundToInt((wall.transform.position.z - bottomLeft.position.z) /  nodeSize.y);
            grid[x, y] = new Node(true, wall.transform.position, x, y);
        }
    }

    /// <summary>Gets closest node in the grid to the specified world space coordinates.</summary>
    /// <param name="worldPosition">World position to find a node from the grid for.</param>
    /// <returns>The closest node from the grid to the specified world position.</returns>
    public Node GetNodeFromWorldSpace (Vector3 worldPosition)
    {
        // Calculate the size of the grid in world space
        Vector2 gridSizeWorld = new Vector2(topRight.position.x + nodeSize.x / 2, topRight.position.z + nodeSize.y / 2);

        // Find a percentage value of the coordinates with respect to the max grid size in order to calculate
        // where in the grid the worldPosition is
        float xPercentage = worldPosition.x / gridSizeWorld.x;
        float yPercentage = worldPosition.z / gridSizeWorld.y;

        // Clamp the calculated values between 0 (grid min value) and 1 (grid max value) to prevent 
        // an error if values are outside the gride
        xPercentage = Mathf.Clamp01(xPercentage);
        yPercentage = Mathf.Clamp01(yPercentage);

        // Find the actual position in the node array
        int x = Mathf.RoundToInt((gridSizeX - 1) * xPercentage);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPercentage);

        return grid[x, y];
    }

    /// <summary>Private helper function that returns a boolean specifying if the neighbor to be checked is within the grid.</summary>
    /// <param name="xDirection">Direction (index) in the grid to check, on the x-axis, relative to the current node.</param>
    /// <param name="yDirection">Direction (index) in the grid to check, on the y-axis, relative to the current node.</param>
    /// <returns>True if the node is inside of the grid, false if the node is not inside of the grid.</returns>
    bool CheckNeighbor (int xDirection, int yDirection)
    {
        return (xDirection >= 0 && xDirection < gridSizeX) && (yDirection >= 0 && yDirection < gridSizeY);
    }

    /// <summary>Gets the neighboring nodes to the specified node in the north, east, south, and west directions relative to the node.</summary>
    /// <param name="node">Node to find the neighbors for.</param>
    /// <returns>A list with the neighbors of the specified node.</returns>
    public List<Node> GetNeighborNodes (Node node)
    {
        List<Node> neighborNodes = new List<Node>();

        int[,] directions = new int[,]{
            { node.gridX, node.gridY + 1 },  // North side neighbor
            { node.gridX + 1, node.gridY },  // East side neighbor
            { node.gridX, node.gridY - 1 },  // South side neighbor
            { node.gridX - 1, node.gridY },  // West side neighbor
            // { node.gridX + 1, node.gridY + 1 },  // North east side neighbor
            // { node.gridX + 1, node.gridY - 1 },  // South east side neighbor
            // { node.gridX - 1, node.gridY - 1 },  // South west side neighbor
            // { node.gridX - 1, node.gridY + 1 }  // North west side neighbor
        };

        // For each direction pair, check if neighbors are in the grid. If they are, add them to the neighbor list
        for (int i = 0; i < directions.Length / 2; i++)
            if (CheckNeighbor(directions[i, 0], directions[i, 1]))
                neighborNodes.Add(grid[directions[i, 0], directions[i, 1]]);
        
        return neighborNodes;
    }

    void OnDrawGizmos() 
    {
        // Check if grid has been initialized to avoid NullReference errors
        if (grid == null) return;

        // Loop through each node in the array and check if they're a wall or not
        foreach (Node node in grid)
        {
            // If they're a wall, set them to blue. If they're not a wall, set them to white
            Gizmos.color = node.isWall ? Color.blue : Color.white;

            // Check if the FinalPath list is empty and if it contains the current searched node.
            // If it does, set the node to red color
            if (finalPath != null && finalPath.Contains(node))
                Gizmos.color = Color.red;

            // Draw the cubes using the DrawCube function
            Gizmos.DrawCube(new Vector3(node.position.x, 0, node.position.z), new Vector3(nodeSize.x, 1, nodeSize.y));
        }
    }
}
