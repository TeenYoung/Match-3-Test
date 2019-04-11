using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BoardState
{
    initializing,
    stable,
    shifting,
    shuffling,
    idle
}


public class BoardController : MonoBehaviour
{
  //match board info
    public int boardWidth, boardHeight;
    public int moveLimit, moveCount, tileClearGoal;
    public Text moveCountText, scoreText, levelEndText;
    public GameObject levelEndPanel;
    public GameObject[] piecePrefabs;
    public GameObject[,] pieces;
    public float clearDelay, collapseDelay, refillDelay, resortDelay,
        acceptInputDelay, findHintDelay, showHintDelay, shuffleDelay;
    public static BoardController board = null;
    public BoardState boardState = BoardState.initializing;
    public AudioClip clearPieces;

    private int tileClearCount;

    private void Awake()
    {
        if (board == null)
        {
            board = this;
        }
        else if (board != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();
        //BattlefieldController.battlefield.SetupBattlefield();

        //StartShifting();
    }

    // Clear the board then start again
    public void Restart()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        BattlefieldController.battlefield.DestoryCreatures();

        SetupBoard();
        BattlefieldController.battlefield.SetupBattlefield();

        //StartShifting();
        
    }

    // Create a board (without any initial match)
    void SetupBoard()
    {
        // Reset Counts & Scores
        moveCount = 0;
        moveCountText.text = (moveLimit-moveCount).ToString();
        tileClearCount = 0;
        scoreText.text = string.Format("{0} / {1}", tileClearCount.ToString(), tileClearGoal.ToString());

        // Create a 2D array to store pieces and determine its size.
        pieces = new GameObject[boardWidth, boardHeight];

        // Layout the board
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                Vector2 coordinate = new Vector2(i, j);

                // Pick a random piece prefab
                GameObject randomPiecePrefab = piecePrefabs[Random.Range(0, piecePrefabs.Length)];

                // If the current prefab will result in an initial match, repeat the random process until it will not.
                while (CheckInitialMatch(i, j, randomPiecePrefab))
                {
                    randomPiecePrefab = piecePrefabs[Random.Range(0, piecePrefabs.Length)];
                };

                // Instantiate the piece prefab as a child of the board. 
                GameObject piece = Instantiate(randomPiecePrefab, coordinate, Quaternion.identity, this.transform);

                // Store the piece into the 2D array
                pieces[i, j] = piece;
            }
        }
    }

    // Shuffle the board using Fisher-Yates shuffle algorithm
    void ShuffleBoard()
    {
        boardState = BoardState.shuffling;

        int pieceNum = boardWidth * boardHeight;

        for (int i = 0; i < pieceNum-1; i++) //i < pieceNum-1 because shuffle the last piece is meaningless
        {
            int j = Random.Range(i, pieceNum);

            // Conver i & j to (x,y) coordinates
            int xOfI = i / boardHeight;
            int yOfI = i % boardHeight;
            int xOfJ = j / boardHeight;
            int yOfJ = j % boardHeight;

            // Swap the two
            pieces[xOfI, yOfI].GetComponent<PieceController>().Swap(new Vector3Int(xOfJ, yOfJ, 0));
        }

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                StartCoroutine(pieces[i, j].GetComponent<PieceController>().Return());
            }
        }

        boardState = BoardState.idle;
    }

    // Stop accepting user inputs and sort the board
    public void StartShifting()
    {
        boardState = BoardState.shifting;

        // Stop hint animation
        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                pieces[i, j].GetComponent<Animator>().SetBool("isHint", false);
            }
        }

        StartCoroutine(SortBoardCoroutine());
    }

    // Sort the board then enable user inputs again
    public IEnumerator SortBoardCoroutine()
    {
        // Find all matches
        List<GameObject> matchedPieces = CheckAllMatches();

        // Clean matches, refill empty tiles, repeat till no matched on board.       
        while (matchedPieces.Count > 0)
        {
            // Record and show score
            tileClearCount += matchedPieces.Count;
            scoreText.text = string.Format("{0} / {1}", tileClearCount.ToString(), tileClearGoal.ToString());
            

            // Highlight pieces to clear
            foreach (GameObject matchedPiece in matchedPieces)
            {
                matchedPiece.GetComponent<Animator>().SetTrigger("isMatched");
            }

            // Play sound effect for clear pieces
            AudioController.audioController.PlaySoungEffect(clearPieces);

            // Distory them and clear them from the 2D array after a delay
            yield return new WaitForSeconds(clearDelay);

            foreach (GameObject piece in matchedPieces)
            {
                if (piece != null)
                {
                    BattlefieldController.battlefield.AddScore(piece);

                    int x = piece.GetComponent<PieceController>().myPosition.x;
                    int y = piece.GetComponent<PieceController>().myPosition.y;
                    pieces[x, y] = null;
                    Destroy(piece);
                }
            }

            BattlefieldController.battlefield.UpdateScores();

            // Collapse existing pieces on board after a delay
            yield return new WaitForSeconds(collapseDelay);
            for (int i = 0; i < boardWidth; i++)
            {
                int columnEmptyTilesCount = 0;

                for (int j = 0; j < boardHeight; j++)
                {
                    if (pieces[i, j] == null)
                    {
                        columnEmptyTilesCount++;
                    }
                    else if (columnEmptyTilesCount > 0)
                    {
                        pieces[i, j - columnEmptyTilesCount] = pieces[i, j];
                        pieces[i, j] = null;
                        pieces[i, j - columnEmptyTilesCount].GetComponent<PieceController>().UpdatePoses(i, j - columnEmptyTilesCount);
                        StartCoroutine(pieces[i, j - columnEmptyTilesCount].GetComponent<PieceController>().Return());
                    }
                }
            } 

            // Refill empty tiles after a delay
            yield return new WaitForSeconds(refillDelay);
            for (int i = 0; i < boardWidth; i++)
            {
                for (int j = 0; j < boardHeight; j++)
                {
                    if (pieces[i, j] == null)
                    {
                        GameObject randomPiecePrefab = piecePrefabs[Random.Range(0, piecePrefabs.Length)];
                        GameObject piece = Instantiate(randomPiecePrefab, new Vector2(i, j), Quaternion.identity, this.transform);
                        pieces[i, j] = piece;
                        piece.GetComponent<PieceController>().UpdatePoses(i, j);
                    }
                }
            }
            
            // Check for matches again after a delay
            yield return new WaitForSeconds(resortDelay);
            matchedPieces.Clear();
            matchedPieces.AddRange(CheckAllMatches());
        };

        BattlefieldController.battlefield.Battle();

        //player lose when HP not more than 0 or move count come to end but monster still alive
        if ((moveCount >= moveLimit && BattlefieldController.battlefield.monster.CurrentHP > 0) || BattlefieldController.battlefield.player.CurrentHP <= 0)
        {
            levelEndText.text = "FAILED";
            levelEndPanel.SetActive(true);
        }
        //player win when monster HP not more than 0
        else if (BattlefieldController.battlefield.monster.CurrentHP <= 0)
        {
            levelEndText.text = "COMPLETED";
            levelEndPanel.SetActive(true);
        }
        else
        {
            //    Start to accept user inputs again after a small delay
            yield return new WaitForSeconds(acceptInputDelay);
            boardState = BoardState.stable;

            //    Check for hints and suffle the board if nothing found
            StartCoroutine(ShowHintCoroutine());
        }        
    }

    // Create an alterative 2D array with certain two pieces swaped
    GameObject[,] SimulateSwap(int x1, int y1, int x2, int y2)
    {
        GameObject[,] piecesAfterSwap = (GameObject[,])pieces.Clone();
        piecesAfterSwap[x1, y1] = pieces[x2, y2];
        piecesAfterSwap[x2, y2] = pieces[x1, y1];

        return piecesAfterSwap;
    }

    // Check all matches in the board, return a list of all matched pieces on the board.
    List<GameObject> CheckAllMatches()
    {
        List<GameObject> allMatchedPieces = new List<GameObject>();

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                List<GameObject> piecesToAdd = CheckMatch(i, j, pieces);
                if (piecesToAdd.Count != 0)
                {
                    // Prevent duplication
                    foreach (GameObject piece in piecesToAdd)
                    {
                        if (!allMatchedPieces.Contains(piece))
                        {
                            allMatchedPieces.Add(piece);
                        }
                    }
                }
            }
        }

        return allMatchedPieces;
    }

    // Check all potential matches in the board, return all pieces of the first potential match
    List<GameObject> CheckAllPotentialMatches()
    {
        List<GameObject> firstPotentialMatch = new List<GameObject>();

        for (int i = 0; i < boardWidth; i++)
        {
            for (int j = 0; j < boardHeight; j++)
            {
                List<GameObject> piecesToAdd = CheckPotentialMatch(i, j);
                if (piecesToAdd.Count != 0)
                {
                    firstPotentialMatch.AddRange(piecesToAdd);
                    return firstPotentialMatch;
                }
            }
        }

        return null;
    }

    // Check if the prefab will result in an initial match should it be instantiated at (x,y).
    bool CheckInitialMatch(int x, int y, GameObject piecePrefab)
    {
        // Check columns
        if (y > 1)
        {
            if (pieces[x, y - 1].tag == piecePrefab.tag && pieces[x, y - 2].tag == piecePrefab.tag)
            {
                return true;
            }
        }

        // Check rows
        if (x > 1)
        {
            if (pieces[x - 1, y].tag == piecePrefab.tag && pieces[x - 2, y].tag == piecePrefab.tag)
            {
                return true;
            }

        }

        return false;
    }

    // Simulate a swap between (x1,y1) and (x2,y2), check if any match will be formed.
    public bool TryMatch (int x1, int y1, int x2, int y2)
    {
        bool isMatch;

        // Simulate a swap
        GameObject[,] piecesAfterSwap = SimulateSwap(x1, y1, x2, y2);

        // Check match at x1,y1
        List<GameObject> matchedPieces = CheckMatch(x1, y1, piecesAfterSwap, true);

        if (matchedPieces.Count > 0)
        {
            isMatch = true;
        }
        else
        {
            // Check match at x2,y2
            matchedPieces.AddRange(CheckMatch(x2, y2, piecesAfterSwap, true));

            if (matchedPieces.Count > 0)
            {
                isMatch = true;
            }
            else
            {
                isMatch = false;
            }
        }

        return isMatch;
    }

    // Check if the piece at (x,y) is in a match, return all matched pieces of this match. Only check upwards and rightwards if allCases is false.
    List<GameObject> CheckMatch(int x, int y, GameObject[,] pieces, bool allCases = false)
    {
        List<GameObject> matchedPieces = new List<GameObject>();

        //Check column match upwards
        if (y < boardHeight - 2)
        {
            if (pieces[x, y].tag == pieces[x, y + 1].tag && pieces[x, y].tag == pieces[x, y + 2].tag)
            {
                matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x, y + 1], pieces[x, y + 2] });
            }
        }

        //Check row match rightwards
        if (x < boardWidth - 2)
        {
            if (pieces[x, y].tag == pieces[x + 1, y].tag && pieces[x, y].tag == pieces[x + 2, y].tag)
            {
                matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x + 1, y], pieces[x + 2, y] });
            }
        }

        // Check other four cases if required.
        if (allCases)
        {
            //Check column match downwards
            if (y > 1)
            {
                if (pieces[x, y].tag == pieces[x, y - 1].tag && pieces[x, y].tag == pieces[x, y - 2].tag)
                {
                    matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x, y - 1], pieces[x, y - 2] });
                }
            }

            //Check column match both sides
            if (y > 0 && y < boardHeight - 1)
            {
                if (pieces[x, y].tag == pieces[x, y - 1].tag && pieces[x, y].tag == pieces[x, y + 1].tag)
                {
                    matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x, y - 1], pieces[x, y + 1] });
                }
            }

            //Check row match leftwards
            if (x > 1)
            {
                if (pieces[x, y].tag == pieces[x - 1, y].tag && pieces[x, y].tag == pieces[x - 2, y].tag)
                {
                    matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x - 1, y], pieces[x - 2, y] });
                }
            }

            //Check row match both sides
            if (x > 0 && x < boardWidth - 1)
            {
                if (pieces[x, y].tag == pieces[x - 1, y].tag && pieces[x, y].tag == pieces[x + 1, y].tag)
                {
                    matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x - 1, y], pieces[x + 1, y] });
                }
            }
        }


        return matchedPieces;
    }

    // Simulate upwards and rightwards swaps start from (x,y), if it will result in a match return all pieces of the first potential match
    List<GameObject> CheckPotentialMatch(int x, int y)
    {
        List<GameObject> potentialMatchedPieces = new List<GameObject>();

        // Check if upwards swap is possible and necessary
        if (y < boardHeight-1)
        {
            if (pieces[x, y].tag != pieces[x, y + 1].tag)
            {
                // Simulate upwards swap
                GameObject[,] piecesAfterSwapUp = SimulateSwap(x, y, x, y + 1);

                // Check matches at all cases on the 1st swaped piece
                List<GameObject> piecesToAdd = CheckMatch(x, y, piecesAfterSwapUp, true);
                if (piecesToAdd.Count != 0)
                {
                    // Prevent duplication
                    foreach (GameObject piece in piecesToAdd)
                    {
                        if (!potentialMatchedPieces.Contains(piece))
                        {
                            potentialMatchedPieces.Add(piece);
                        }
                    }

                    return potentialMatchedPieces;
                }
                else
                {
                    // Check matches at all cases on the 2nd swaped piece
                    piecesToAdd = CheckMatch(x, y + 1, piecesAfterSwapUp, true);
                    if (piecesToAdd.Count != 0)
                    {
                        // Prevent duplication
                        foreach (GameObject piece in piecesToAdd)
                        {
                            if (!potentialMatchedPieces.Contains(piece))
                            {
                                potentialMatchedPieces.Add(piece);
                            }
                        }

                        return potentialMatchedPieces;
                    }
                }
            }
        }


        // Check if rightwards swap is possible and necessary
        if (x < boardWidth - 1)
        {
            if (pieces[x, y].tag != pieces[x + 1, y].tag)
            {
                // Simulate rightwards swap
                GameObject[,] piecesAfterSwapRight = SimulateSwap(x, y, x + 1, y);

                // Check matches at four directions on the 1st swaped piece
                List<GameObject> piecesToAdd = CheckMatch(x, y, piecesAfterSwapRight, true);
                if (piecesToAdd.Count != 0)
                {
                    // Prevent duplication
                    foreach (GameObject piece in piecesToAdd)
                    {
                        if (!potentialMatchedPieces.Contains(piece))
                        {
                            potentialMatchedPieces.Add(piece);
                        }
                    }

                    return potentialMatchedPieces;
                }
                else
                {
                    // Check matches at four directions on the 2nd swaped piece
                    piecesToAdd = CheckMatch(x + 1, y, piecesAfterSwapRight, true);
                    if (piecesToAdd.Count != 0)
                    {
                        // Prevent duplication
                        foreach (GameObject piece in piecesToAdd)
                        {
                            if (!potentialMatchedPieces.Contains(piece))
                            {
                                potentialMatchedPieces.Add(piece);
                            }
                        }

                        return potentialMatchedPieces;
                    }
                }
            }
        }

        return potentialMatchedPieces;
    }

    // Show hint after a delay, shuffle the board if no potential match found
    public IEnumerator ShowHintCoroutine()
    {
        int previousMoveCount = moveCount;

        // Wait a while before show hint
        yield return new WaitForSeconds(findHintDelay);

        // Continue when the board is stable and player makes no new match since this coroutine starts
        if (boardState == BoardState.stable && moveCount == previousMoveCount)
        {
            List<GameObject> firstPotentialMatch = CheckAllPotentialMatches();

            // Shuffle and shift if no potential match found, until a hint is found
            while (firstPotentialMatch == null)
            {
                ShuffleBoard();
                while (boardState == BoardState.shuffling)
                {
                    yield return new WaitForSeconds(.1f);
                }
                yield return new WaitForSeconds(shuffleDelay);

                StartShifting();
                while (boardState == BoardState.shifting)
                {
                    yield return new WaitForSeconds(.1f);
                }

                firstPotentialMatch = CheckAllPotentialMatches();
            }

            // Show hint after a delay 
            yield return new WaitForSeconds(showHintDelay);
            if (boardState == BoardState.stable && moveCount == previousMoveCount)
            {
                
                for (int i = 0; i < firstPotentialMatch.Count; i++)
                {
                    if (firstPotentialMatch[i] != null)
                    {
                        firstPotentialMatch[i].GetComponent<Animator>().SetBool("isHint", true);
                    }
                }

            }
        }
    }

    ////initialize score board to 0
    //public void InitializeBoards()
    //{
    //    blackSum = 0;
    //    blueSum = 0;
    //    greenSum = 0;
    //    redSum = 0;
    //    purpleSum = 0;
    //    BattlefieldController.battlefield.UpdateScores();

    //    distance = initialDistance;
    //    UpdateDistanceBoard(); 
    //}

    ////update score board
    //public void UpdateScoreBoard()
    //{
    //    blackScoreText.text = blackSum.ToString();
    //    blueScoreText.text = blueSum.ToString();
    //    greenScoreText.text = greenSum.ToString();
    //    redScoreText.text = redSum.ToString();
    //    purpleScoreText.text = purpleSum.ToString();
    //}    

    //public void UpdateDistanceBoard()
    //{
    //    distanceBoardText.text = distance.ToString();
    //}
    
}

