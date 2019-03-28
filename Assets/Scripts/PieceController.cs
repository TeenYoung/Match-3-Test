using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour
{
    public Sprite KeyUp, KeyDown;
    public float moveSpeed;
    public Vector3Int myPosition;
    public Animator animator;
    public AudioClip swapBack;


    private Vector3 zOffset = new Vector3 (0,0,9);
    private BoxCollider2D myCollider;
    private List<Vector3Int> neighborPoses;
    private List<GameObject> neighbors;
    private List<Vector3Int> destinations;
    private BoardController board = BoardController.board;
    private bool isMouseDown = false;


    // Start is called before the first frame update
    void Start()
    {   
        myCollider = GetComponent<BoxCollider2D>();

        UpdatePoses((int)transform.position.x,(int)transform.position.y);
    }

    // Record current position of this piece and find all neighbor positions
    public void UpdatePoses(int x, int y)
    {
        myPosition = new Vector3Int(x,y,0);

        // Find positions of all neighbor pieces
        neighborPoses = new List<Vector3Int>();
        if (myPosition.x < BoardController.board.boardWidth - 1)
        {
            Vector3Int neighborRight = myPosition + Vector3Int.right;
            neighborPoses.Add(neighborRight);
        }

        if (myPosition.x > 0)
        {
            Vector3Int neighborLeft = myPosition + Vector3Int.left;
            neighborPoses.Add(neighborLeft);
        }

        if (myPosition.y < BoardController.board.boardHeight - 1)
        {
            Vector3Int neighborUp = myPosition + Vector3Int.up;
            neighborPoses.Add(neighborUp);
        }

        if (myPosition.y > 0)
        {
            Vector3Int neighborDown = myPosition + Vector3Int.down;
            neighborPoses.Add(neighborDown);
        }

        
    }

    private void OnMouseDown()
    {
        // Only accept user inputs while the board is stable
        if (board.boardState == State.stable)
        {
            // use this to fix a weired case that a mouse down before boardState switchs to stable and a mouse up after
            isMouseDown = true;

            // Press buttons down on mouse down
            GetComponent<SpriteRenderer>().sprite = KeyDown;

            // Set possible distinations for selected piece
            destinations = new List<Vector3Int>
            {
                myPosition
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
    }

    private void OnMouseDrag()
    {
        // Only accept user inputs while the board is stable
        if (board.boardState == State.stable && isMouseDown)
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
            Vector3 deltaMouse = mousePosOnBoard - myPosition;
            float deltaX = Mathf.Abs(deltaMouse.x);
            float deltaY = Mathf.Abs(deltaMouse.y);
            if (deltaX / deltaY > 0.5f && deltaX / deltaY < 2f)
            {
                destination = myPosition;
            }

            // Move it to the closest destination
            MoveTo(destination,moveSpeed);

            // Find the piece at the closest destination
            GameObject swapPiece = BoardController.board.pieces[Vector3Int.RoundToInt(destination).x, Vector3Int.RoundToInt(destination).y];

            // Move that piece to the start position of draging piece
            swapPiece.GetComponent<PieceController>().MoveTo(myPosition, moveSpeed);

            // Move all other neibor pieces back to their position
            List<GameObject> noneSwapPieces = new List<GameObject>();
            noneSwapPieces.AddRange(neighbors);
            noneSwapPieces.Remove(swapPiece);
            foreach (GameObject noneSwapPiece in noneSwapPieces)
            {
                noneSwapPiece.GetComponent<PieceController>().MoveTo(noneSwapPiece.GetComponent<PieceController>().myPosition, moveSpeed);
            }
        }
    }

    private void OnMouseUp()
    {
        // Only accept user inputs while the board is stable
        if (board.boardState == State.stable && isMouseDown)
        {
            isMouseDown = false;

            // Left buttons up on mouse up
            GetComponent<SpriteRenderer>().sprite = KeyUp;

            // Try swap if it's in a swap position
            foreach (Vector3Int neighborPos in neighborPoses)
            {
                if (myCollider.bounds.Contains(neighborPos))
                {
                    // Simulate the swap and see if it forms a match or matches
                    bool willMatch = BoardController.board.TryMatch(myPosition.x, myPosition.y, neighborPos.x, neighborPos.y);

                    // Comfirm the swap if it will form a match or matches, then sort the board
                    if (willMatch)
                    {
                        board.moveCount++;
                        board.moveCountText.text = (board.moveLimit - board.moveCount).ToString();
                        Swap(neighborPos);
                        board.StartShifting();
                    }

                    // Play sound effect of swap back if will not match
                    else
                    {
                        AudioController.audioController.PlaySoungEffect(swapBack);
                    }
                }
            }

            // Move everything back if swap is not confirmed or mouse up while pieces are still moving
            StartCoroutine(Return());
            foreach (GameObject neighbor in neighbors)
            {
                StartCoroutine(neighbor.GetComponent<PieceController>().Return()); ;
            }
        }
    }

    // This method will be called while swaping
    public void MoveTo(Vector3 moveDestination, float speed)
    {
        Vector3 delta = moveDestination - transform.position;
        transform.position += delta * speed;
    }

    // Retrun a piece to it's position
    public IEnumerator Return()
    {
        Vector3 delta = myPosition - transform.position;
        for (float f = 1f; f >= 0; f-=0.1f)
        {
            transform.position += (delta * 0.1f);
            yield return null;
        }
    }

    // Swap position with another piece at incoming position, then update both positions.
    public void Swap(Vector3Int otherPos)
    {
        
        GameObject temp = gameObject;

        board.pieces[myPosition.x, myPosition.y] = board.pieces[otherPos.x, otherPos.y];
        board.pieces[myPosition.x, myPosition.y].GetComponent<PieceController>().UpdatePoses(myPosition.x, myPosition.y);

        board.pieces[otherPos.x, otherPos.y] = temp;
        UpdatePoses(otherPos.x, otherPos.y);
    }

}
