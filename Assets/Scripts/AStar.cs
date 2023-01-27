using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
    [Tooltip("Start position for the A* Pathfinding Algorithm")]
    public Transform startPosition;

    [Tooltip("Target position for the A* Pathfinding Algorithm")]
    public Transform targetPosition;

    // The previous target position, to start A*
    Vector3 previousTargetPosition;

    // Reference to the map grid
    MapGrid grid;

    // Flag that specifies if the path has been found
    bool foundPath = true;

    // To play the ding sound when destination is unreachable
    AudioSource audioData;

    void Awake() {  grid = GetComponent<MapGrid>(); }

    void Start() { 
        previousTargetPosition =  targetPosition.position; 
        audioData = GetComponent<AudioSource>();
    }

    void Update() 
    {
        // If target changed position
        if ((targetPosition.position - previousTargetPosition).magnitude >= 0.1f)
            foundPath = false;

        // If the path has not been found, find a path.
        if (!foundPath)
            FindPath(startPosition.position, targetPosition.position);
    }

    /// <summary>Runs the A* algorithm and finds the shortest path between the start position and the target position.</summary>
    /// <param name="startPosition">The starting position (in Vector3) for the A* pathfinding algorithm.</param>
    /// <param name="targetPosition">The target position (in Vector3) for the A* pathfinding algorithm.</param>
    void FindPath(Vector3 startPosition, Vector3 targetPosition) 
    {
        // Get the closest node to the starting and target position from the grid
        Node startNode = grid.GetNodeFromWorldSpace(startPosition);
        Node targetNode = grid.GetNodeFromWorldSpace(targetPosition);

        // Create A* open and closed lists
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>(); // HashSet in order to prevent duplicates

        // Start A* Algorithm
        openList.Add(startNode);

        // Flag to know if a destination is unreachable
        bool flagUnreachable = true;
        // While the open list is not empty
        while (openList.Count > 0)
        {
            // Get the lowest FCost node from the open list and make it the current node
            Node currentNode = GetLowestFCostNode(openList);

            // Remove the current node from the open list and add it to the closed list
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // (Guard Clause) If the current node is equal to the target node, the target node has 
            // been found. Break out of the loop.
            if (currentNode == targetNode)
            {
                flagUnreachable = false;
                break;
            }
            
            // For each neighbor of the current node
            foreach (Node neighborNode in grid.GetNeighborNodes(currentNode))
            {
                // (Guard Clause) If the neighbor is a wall or is in the closed list, skip it
                if (neighborNode.isWall || closedList.Contains(neighborNode))
                    continue;

                // Calculate new gCost (cost of getting to the neighbor node from the current node)
                int newGCost = currentNode.gCost + CalculateManhattanDistance(currentNode, neighborNode);

                // If the neighbor is not in the open list or its gCost is higher than the new gCost
                if (!openList.Contains(neighborNode) || newGCost < neighborNode.gCost)
                {
                    neighborNode.Parent = currentNode;  // Set the parent (past node) of the neighbor node as the current node
                    neighborNode.gCost = newGCost;  // Set the neighbor's gCost equal to the new gCost
                    neighborNode.hCost = CalculateManhattanDistance(neighborNode, targetNode);  // Re-calculate the neighbor's hCost (distance from the target)

                    // If the neighbor node is not in the open list, add it to the open list
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }

            }
        }

        // If destination is unreachable, reset A* and play sound
        if (flagUnreachable)
        {
            Debug.Log("Destination is unreachable!");
            if (grid.finalPath != null)
                grid.finalPath.Clear();
            foundPath = true;
            this.targetPosition.position = previousTargetPosition;
            audioData.Play(0);
            return;
        }

        // Once the loop has ended, the final path has been found. Construct the final path.
        GetFinalPath(startNode, targetNode);

        // Set the foundPath flag as true to not run the A* algorithm again on the next frame
        foundPath = true;
        previousTargetPosition = targetPosition;
    }

    /// <summary>Private helper method that linearly finds the lowest FCost node from a list and returns it.</summary>
    /// <param name="nodeList">List of nodes to find the lowest FCost node from.</param>
    /// <returns>The node from the <c>nodeList</c> with the lowest FCost.</returns>
    Node GetLowestFCostNode (List<Node> nodeList)
    {
        // Assume the first node is the one with the lowest FCost
        int leastFCost = 0;
        // For each node in the node list, check if it's FCost is lower than the current one (index).
        // If it is, replace the leastFCost variable with the index of the new least FCost node. 
        for (int i = 0; i < nodeList.Count; i++)
            if (nodeList[i].FCost < nodeList[leastFCost].FCost)
                leastFCost = i;

        // Return the node with the least FCost from the list.
        return nodeList[leastFCost];
    }

    /// <summary>Constructs a list with the final path found from the A* algorithm from the startNode to the targetNode.</summary>
    /// <param name="startNode">The starting node for the A* pathfinding algorithm.</param>
    /// <param name="targetNode">The target node for the A* pathfinding algorithm.</param>
    void GetFinalPath (Node startNode, Node targetNode)
    {
        List<Node> finalPath = new List<Node>();    // Make a list for the final path to be stored in
        Node currentNode = targetNode;  // Set the current node as the final node, to trace back from it

        // While the current node is not the starting node
        while (currentNode != startNode)
        {
            if (currentNode.Parent == null) continue;   // Prevent null reference error
            finalPath.Add(currentNode); // Add the current node to the final path list
            currentNode = currentNode.Parent;   // Set the current node as the current node's parent (preceding node)
        }

        // Reverse the list to get a path from the starting node to the target node
        finalPath.Reverse();
        // Set the finalPath variable of the grid class as the calculated final path from A*
        grid.finalPath = finalPath;
    }

    /// <summary>Private helper method to calculate the Manhattan distance from a node A to a node B.</summary>
    /// <param name="nodeA">A node to calculate the Manhattan distance from.</param>
    /// <param name="nodeB">A node to calculate the Manhattan distance to.</param>
    /// <returns>An integer with the Manhattan distance from node A to node B.</returns>
    int CalculateManhattanDistance (Node nodeA, Node nodeB)
    {
        // Subtract the gridX and the gridY from both nodes and return the sum of them
        int deltaX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int deltaY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return deltaX + deltaY;
    }
}
