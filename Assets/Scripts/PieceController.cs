using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Sprite KeyUp, KeyDown;
    public float moveSpeed;

    private Vector3 zOffset = new Vector3 (0,0,10);
    private Vector3Int originalPos;
    private BoxCollider2D myCollider;
    private List<Vector3Int> neighborPoses;
    private List<GameObject> neighbors;
    private List<Vector3Int> destinations;

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

        // Find positions of all neighbor pieces
        neighborPoses = new List<Vector3Int>();
        if (originalPos.x < BoardController.board.boardWidth - 1)
        {
            Vector3Int neighborRight = originalPos + Vector3Int.right;
            neighborPoses.Add(neighborRight);
        }

        if (originalPos.x > 0)
        {
            Vector3Int neighborLeft = originalPos + Vector3Int.left;
            neighborPoses.Add(neighborLeft);
        }

        if (originalPos.y < BoardController.board.boardHeight - 1)
        {
            Vector3Int neighborUp = originalPos + Vector3Int.up;
            neighborPoses.Add(neighborUp);
        }

        if (originalPos.y > 0)
        {
            Vector3Int neighborDown = originalPos + Vector3Int.down;
            neighborPoses.Add(neighborDown);
        }

        
    }

    private void OnMouseDown()
    {
        // Press buttons down on mouse down
        GetComponent<SpriteRenderer>().sprite = KeyDown;

        // Set possible distinations for selected piece
        destinations = new List<Vector3Int>
        {
            originalPos
        };
        destinations.AddRange(neighborPoses);

        // Find GameObjects of all neighbor pieces
        neighbors = new List<GameObject>();
        foreach (Vector3Int neighborPos in neighborPoses)
        {
            GameObject neighborPiece = BoardController.board.pieces[Vector3Int.RoundToInt(neighborPos).x, Vector3Int.RoundToInt(neighborPos).y];
            neighbors.Add(neighborPiece);
        }
    }

    private void OnMouseDrag()
    {
        // Get mouse position on the board
        Vector3 mousePosOnBoard = Camera.main.ScreenToWorldPoint(Input.mousePosition) + zOffset;

        // Find the closest destination to the mouse position on board
        List<float> distances = new List<float>();
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
        
        // Get closest desination
        Vector3 destination = destinations[indexOfMinDis];

        // If the mouse is at a diagonal angle, keep the draging piece where it is.
        Vector3 deltaMouse = mousePosOnBoard - originalPos;
        float deltaX = Mathf.Abs(deltaMouse.x);
        float deltaY = Mathf.Abs(deltaMouse.y);
        if (deltaX/deltaY > 0.5f && deltaX / deltaY < 2f)
        {
            destination = originalPos;
        }

        // Move it to the closest destination
        Vector3 delta = destination - transform.position;
        transform.position = transform.position + delta * moveSpeed;

        // Find the piece at the closest destination
        GameObject swapPiece = BoardController.board.pieces[Vector3Int.RoundToInt(destination).x, Vector3Int.RoundToInt(destination).y];

        // Move that piece to the start position of draging piece
        swapPiece.GetComponent<PieceController>().MoveTo(originalPos, moveSpeed);

        // Move all other neibor pieces back to their position
        List<GameObject> noneSwapPieces = new List<GameObject>();
        noneSwapPieces.AddRange(neighbors);
        noneSwapPieces.Remove(swapPiece);
        foreach (GameObject noneSwapPiece in noneSwapPieces)
        {
            noneSwapPiece.GetComponent<PieceController>().MoveBack(moveSpeed);
        }

    }

    // This method will be called while swaping
    public void MoveTo(Vector3 moveDestination, float speed)
    {
        Vector3 delta = moveDestination - transform.position;
        transform.position = transform.position + delta * speed;
    }

    // This method will be called while cancle swaping
    public void MoveBack(float speed)
    {
        Vector3 delta = originalPos - transform.position;
        transform.position = transform.position + delta * speed;
    }



    private void OnMouseUp()
    {
        // Left buttons up on mouse up
        GetComponent<SpriteRenderer>().sprite = KeyUp;

        // Try swap if it's in a swap position
        foreach (Vector3Int neighborPos in neighborPoses)
        {
            if ( myCollider.bounds.Contains(neighborPos))
            {
                // Simulate the swap and see if it forms a match or matches
                bool willMatch = BoardController.board.TryMatch(originalPos.x, originalPos.y, neighborPos.x, neighborPos.y);

                // Comfirm the swap if it will form a match or matches
                if (willMatch)
                {
                    BoardController.board.Swap(originalPos.x, originalPos.y, neighborPos.x, neighborPos.y);
                }
                
            }
        }

        // Sort the board
        BoardController.board.SortBoard();

        // Move everything back if swap is not confirmed or mouse up while pieces are still moving
        MoveBack(1);
        foreach (GameObject neighbor in neighbors)
        {
            neighbor.GetComponent<PieceController>().MoveBack(1);
        }
    }
}
