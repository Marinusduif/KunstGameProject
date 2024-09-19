using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject floorPrefab;
    [SerializeField] GameObject northFacingWallPrefab;
    [SerializeField] GameObject eastFacingWallPrefab;
    [SerializeField] float tileSize;
    [SerializeField] Vector2Int mazeSize;
    [SerializeField] int seed;
    [SerializeField] bool random;
    bool drawnMaze;
    Vector2Int cell = new Vector2Int(0, 0);
    GameObject[,] floors;
    bool[,] done;
    int doneCount;
    List<Vector2Int> route = new List<Vector2Int>();
    bool[,] northFacingWalls;
    GameObject[,] northFacingWallsGO;
    bool[,] eastFacingWalls;
    GameObject[,] eastFacingWallsGO;

    void Start()
    {
        // Initialize the random seed if random generation is enabled
        if (random) seed = Random.Range(int.MinValue, int.MaxValue);
        Random.InitState(seed);

        // Create the floor tiles
        floors = new GameObject[mazeSize.x, mazeSize.y];
        for (int x = 0; x < mazeSize.x; x++)
        {
            for (int y = 0; y < mazeSize.y; y++)
            {
                floors[x, y] = Instantiate(floorPrefab);
                floors[x, y].transform.position = new Vector3(x * tileSize, 0, y * tileSize);
                floors[x, y].transform.localScale = new Vector3(tileSize, 1, tileSize);
            }
        }

        // Initialize north-facing walls
        northFacingWalls = new bool[mazeSize.x, mazeSize.y + 1];
        for (int x = 0; x < northFacingWalls.GetLength(0); ++x)
        {
            for (int y = 0; y < northFacingWalls.GetLength(1); ++y)
            {
                northFacingWalls[x, y] = true; // Set all north walls as true (existing)
            }
        }

        // Initialize east-facing walls
        eastFacingWalls = new bool[mazeSize.x + 1, mazeSize.y];
        for (int x = 0; x < eastFacingWalls.GetLength(0); ++x)
        {
            for (int y = 0; y < eastFacingWalls.GetLength(1); ++y)
            {
                eastFacingWalls[x, y] = true; // Set all east walls as true (existing)
            }
        }

        done = new bool[mazeSize.x, mazeSize.y]; // Track completed cells

        UpdateMaze(); // Start the maze generation process
    }

    void UpdateMaze()
    {
        // Continue generating the maze until it is fully drawn
        while (!drawnMaze)
        {
            // Check for possible moves from the current cell
            List<Vector2Int> moves = CheckMoves();

            if (moves.Count != 0) // If there are valid moves
            {
                // Choose a random move from the available options
                Vector2Int chosenMove = moves[Random.Range(0, moves.Count)];

                // Update the wall arrays based on the chosen move
                if (chosenMove == Vector2Int.up)
                {
                    northFacingWalls[cell.x, cell.y + 1] = false; // Remove north wall
                }
                else if (chosenMove == Vector2Int.down)
                {
                    northFacingWalls[cell.x, cell.y] = false; // Remove south wall
                }
                else if (chosenMove == Vector2Int.right)
                {
                    eastFacingWalls[cell.x + 1, cell.y] = false; // Remove east wall
                }
                else if (chosenMove == Vector2Int.left)
                {
                    eastFacingWalls[cell.x, cell.y] = false; // Remove west wall
                }

                // Add the current cell to the route and mark it as done
                route.Add(cell);
                done[cell.x, cell.y] = true;
                cell += chosenMove; // Move to the chosen cell
                ++doneCount; // Increment the count of completed cells
            }
            else if (doneCount != done.Length - 1) // If there are no moves left and not all cells are done
            {
                done[cell.x, cell.y] = true; // Mark the current cell as done
                cell = route[route.Count - 1]; // Backtrack to the last cell in the route
                route.RemoveAt(route.Count - 1); // Remove the last cell from the route
            }
            else
            {
                break; // Exit the loop if all cells are done
            }
        }

        // Instantiate north-facing walls based on the wall array
        northFacingWallsGO = new GameObject[mazeSize.x, mazeSize.y + 1];
        for (int x = 0; x < northFacingWalls.GetLength(0); ++x)
        {
            for (int y = 0; y < northFacingWalls.GetLength(1); ++y)
            {
                if (northFacingWalls[x, y]) // If a north-facing wall exists
                {
                    northFacingWallsGO[x, y] = Instantiate(northFacingWallPrefab); // Create wall GameObject
                    northFacingWallsGO[x, y].transform.position = new Vector3(x * tileSize, 0, y * tileSize); // Set position
                    northFacingWallsGO[x, y].transform.localScale = new Vector3(tileSize, tileSize, tileSize); // Set scale
                    northFacingWallsGO[x, y].name = "north" + x + "," + y; // Name the GameObject
                }
            }
        }

        // Instantiate east-facing walls based on the wall array
        eastFacingWallsGO = new GameObject[mazeSize.x + 1, mazeSize.y];
        for (int x = 0; x < eastFacingWalls.GetLength(0); ++x)
        {
            for (int y = 0; y < eastFacingWalls.GetLength(1); ++y)
            {
                if (eastFacingWalls[x, y]) // If an east-facing wall exists
                {
                    eastFacingWallsGO[x, y] = Instantiate(eastFacingWallPrefab); // Create wall GameObject
                    eastFacingWallsGO[x, y].transform.position = new Vector3(x * tileSize, 0, y * tileSize); // Set position
                    eastFacingWallsGO[x, y].transform.localScale = new Vector3(tileSize, tileSize, tileSize); // Set scale
                    eastFacingWallsGO[x, y].name = "east" + x + "," + y; // Name the GameObject
                }
            }
        }
        done = null;
        eastFacingWalls = null;
        northFacingWalls = null;
    }

    // Method to check possible moves from the current cell
    List<Vector2Int> CheckMoves()
    {
        List<Vector2Int> toReturn = new List<Vector2Int>(); // List to hold valid moves

        // Check if moving up is within limits and the cell above is not done
        if (InLimits(done, cell + Vector2Int.up))
        {
            if (!done[cell.x, cell.y + 1]) toReturn.Add(Vector2Int.up);
        }

        // Check if moving right is within limits and the cell to the right is not done
        if (InLimits(done, cell + Vector2Int.right))
        {
            if (!done[cell.x + 1, cell.y]) toReturn.Add(Vector2Int.right);
        }

        // Check if moving down is within limits and the cell below is not done
        if (InLimits(done, cell + Vector2Int.down))
        {
            if (!done[cell.x, cell.y - 1]) toReturn.Add(Vector2Int.down);
        }

        // Check if moving left is within limits and the cell to the left is not done
        if (InLimits(done, cell + Vector2Int.left))
        {
            if (!done[cell.x - 1, cell.y]) toReturn.Add(Vector2Int.left);
        }

        return toReturn; // Return the list of valid moves
    }

    // Method to check if a point is within the limits of the array
    private bool InLimits(bool[,] toTest, Vector2Int point)
    {
        // Check if the point is within the bounds of the array
        return point.x >= 0 && point.y >= 0 && point.x < toTest.GetLength(0) && point.y < toTest.GetLength(1);
    }
}