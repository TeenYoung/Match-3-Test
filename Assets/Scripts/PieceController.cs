using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Sprite KeyUp, KeyDown;

    private Vector3 zOffset = new Vector3 (0,0,10);
    private Vector3Int originalPos;
    private BoxCollider2D myCollider;
    private List<Vector3Int> neighbors;

    // Start is called before the first frame update
    void Start()
    {   
        myCollider = GetComponent<BoxCollider2D>();

        UpdatePoses();
    }

    // Record current position of this piece and find all neighbor positions
    public void UpdatePoses()
    {
        originalPos = Vector3Int.RoundToInt(transform.position);

        neighbors = new List<Vector3Int>();

        if (originalPos.x < BoardController.board.boardWidth - 1)
        {
            Vector3Int neighborRight = originalPos + Vector3Int.right;
            neighbors.Add(neighborRight);
        }

        if (originalPos.x > 0)
        {
            Vector3Int neighborLeft = originalPos + Vector3Int.left;
            neighbors.Add(neighborLeft);
        }

        if (originalPos.y < BoardController.board.boardHeight - 1)
        {
            Vector3Int neighborUp = originalPos + Vector3Int.up;
            neighbors.Add(neighborUp);
        }

        if (originalPos.y > 0)
        {
            Vector3Int neighborDown = originalPos + Vector3Int.down;
            neighbors.Add(neighborDown);
        }
    }

    private void OnMouseDown()
    {
        // Press buttons down on mouse down
        GetComponent<SpriteRenderer>().sprite = KeyDown;
    }

    private void OnMouseDrag()
    {
        // Get mouse position on the board
        Vector3 mousePosOnBoard = Camera.main.ScreenToWorldPoint(Input.mousePosition) + zOffset;

        // Set possible distinations for selected piece
        List<float> distances = new List<float>();
        List<Vector3Int> destinations = new List<Vector3Int>
        {
            originalPos
        };
        destinations.AddRange(neighbors);

        // Find the closest destination to the mouse position on board
        foreach (Vector3Int position in destinations)
        {
            float distance = Vector3.Distance(position, mousePosOnBoard);
            distances.Add(distance);
        }
        float minDistance = distances[0];
        int indexOfMinDis = 0;
        for (int i = 0; i < distances.Count; i++)
        {
            if (minDistance > distances[i]) 
            {
                minDistance = distances[i];
                indexOfMinDis = i;
            }
        }

        // Move it to the closest destination
        Vector3 destination = destinations[indexOfMinDis];
        Vector3 delta = destination - transform.position;
        transform.position = transform.position + delta / 5;
    }

    
    private void OnMouseUp()
    {
        // Left buttons up on mouse up
        GetComponent<SpriteRenderer>().sprite = KeyUp;

        // Confirm the swap if it's in a swap position
        foreach (Vector3Int neighborPos in neighbors)
        {
            if ( myCollider.bounds.Contains(neighborPos))
            {
                BoardController.board.Swap(originalPos.x, originalPos.y, neighborPos.x, neighborPos.y);
            }
        }
    }
}
