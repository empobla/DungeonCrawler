using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    [Tooltip("Reference to the target location.")]
    public Transform Target;

    [Tooltip("Reference to the Game Manager.")]
    public GameObject GameManager;

    [Tooltip("Floor layer mask, to be able to calculate raycast for mouse click.")]
    public LayerMask FloorLayer;

    [Tooltip("Speed at which the Hero moves.")]
    [Min(0f)]
    public float HeroSpeed = 10f;

    // Target position calculated from the raycast from mouse
    Vector3 targetPosition;

    // Flag to know if the hero is moving
    bool moving = false;

    // Queue made from final path found by A*
    List<Node> pathQueue;

    // Animator component
    Animator animator;

    void Start() { 
        pathQueue = new List<Node>();   // Initialize path queue
        animator = GetComponent<Animator>();    // Initialize animator
    }

    void Update()
    {
        // If user clicks, find where he clicked by casting a ray. Set the moving flag to true.
        if (Input.GetMouseButton(0))
        {
            // Get a ray from the camera to the current mouse position
            Ray castPoint = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the ray hits a floor tile, and if it does, set the lastClickedPosition to the point where the ray hit
            RaycastHit hit;
            if (Physics.Raycast(castPoint, out hit, Mathf.Infinity, FloorLayer))
                targetPosition = new Vector3 (hit.point.x, 0.5f, hit.point.z);
            
            // Set the moving flag to true
            moving = true;

            // Move the target and start the A* algorithm
            Target.position = new Vector3 (targetPosition.x, Target.position.y, targetPosition.z);

            // Clear the path queue
            pathQueue.Clear();
        }

        // Set moving to false if the hero is not moving 
        // or if the distance from the last clicked position and the current position is less than 0.1f 
        // and there are no items on the pathQueue
        if (!moving || (transform.position - targetPosition).magnitude < 0.1f && pathQueue.Count == 0)
            moving = false;

        // If path queue is empty, stop moving
        // if (pathQueue.Count == 0)
        //     moving = false;

        // If hero is moving
        if (moving)
        {
            // Get the finalPath list calculated by A* from the MapGrid component
            List<Node> finalPath = GameManager.GetComponent<MapGrid>().finalPath;
            // Check if it has been initialized to prevent NullReference error
            if (finalPath == null) return;

            // If pathQueue is empty, copy finalPath list to pathQueue
            if (pathQueue.Count == 0)
                foreach (Node node in finalPath)
                    pathQueue.Add(node);
            
            // If there are Nodes in the pathQueue, set the first node of the pathQueue as the target position for the hero
            if (pathQueue.Count > 0)
                targetPosition = pathQueue[0].position;
            else
            {
                moving = false;
                return;
            }

            // If the hero reached the target node and there are still nodes to go, remove the first node of the pathqueue to
            // allow the hero to move to the next node
            if ((transform.position - pathQueue[0].position).magnitude < 0.1f && pathQueue.Count > 0)
                pathQueue.RemoveAt(0);

            // Move the hero to the target node
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, HeroSpeed * Time.deltaTime);

            // Rotate the hero to look at the target node
            Quaternion toRotation = GetManhattanLookRotation(transform.position, targetPosition);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1000 * Time.deltaTime);
        }

        // Animate the hero to walk
        animator.SetFloat("HeroSpeed", moving ? HeroSpeed : 0);
    }

    /// <summary>Private helper method that gets the look rotation of the hero, knowing that it'll move following a Manhattan
    /// Heuristic.</summary>
    /// <param name="currentLocation">The current location of the hero.</param>
    /// <param name="targetLocation">The target location of the hero.</param>
    /// <returns>A rotation quaternion with the specified forward and upwards direction.</returns>
    Quaternion GetManhattanLookRotation(Vector3 currentLocation, Vector3 targetLocation)
    {
        // Add a unit vector in each cardinal location to the current direction. This will be used to calculate the least distance from
        // the target location and decide the look rotation from this.    
        Vector3[] directionalTests = new Vector3[]{
            currentLocation + Vector3.forward,  // North side test
            currentLocation + Vector3.right,    // East side test
            currentLocation + Vector3.back, // South side test
            currentLocation + Vector3.left  // West side test
        };

        // Find the directionalTest that has the least magnitude from the target location
        int leastDistanceIdx = 0;
        for (int i = 0; i < directionalTests.Length; i++)
            if ((targetLocation - directionalTests[i]).magnitude < (targetLocation - directionalTests[leastDistanceIdx]).magnitude)
                leastDistanceIdx = i;
        
        // Return the appropriate quaternion look rotation depending on which cardinal direction is closest to the target location
        if (leastDistanceIdx == 0)
            return Quaternion.LookRotation(new Vector3(0.01f, 0, 0), Vector3.up);
        else if (leastDistanceIdx == 1)
            return Quaternion.LookRotation(new Vector3(90, 0, 0), Vector3.up);
        else if (leastDistanceIdx == 2)
            return Quaternion.LookRotation(new Vector3(0, 0, -90), Vector3.up);
        else
            return Quaternion.LookRotation(new Vector3(-90, 0, 0), Vector3.up);
    }
}
