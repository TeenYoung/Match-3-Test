using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum State
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
    public State boardState = State.initializing;
    public AudioClip clearPieces;         
    
    public Text blackScoreText, blueScoreText, greenScoreText, purpleScoreText, redScoreText;   //count of different color pieces

  //battle creature info
    //creatures
    public GameObject playerPrefab, monsterPrefab;
    public Player player;
    public Monster monster;

    //distance
    public Text distanceBoardText;
    public float initialDistance;
    public static float distance; //distance between monster and player    
   
    
    int blackSum, blueSum, greenSum, purpleSum, redSum ;     // To calculate how many pieces was cleared in certain color
    int purpleMax = 6;     // The num of purple pieces to trigger special 

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
        InitializeCreature(100f,200f,5f);
        SetupBoard();
        StartShifting();
        InitializeBoards();
    }

    // Clear the board then start again
    public void Restart()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(player.gameObject);
        Destroy(monster.gameObject);
        InitializeCreature(100f, 200f, 5f);
        SetupBoard();
        StartShifting();
        InitializeBoards();
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
        boardState = State.shuffling;

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

        boardState = State.idle;
    }

    // Stop accepting user inputs and sort the board
    public void StartShifting()
    {
        boardState = State.shifting;

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
                    //count sum of matched pieces in colors
                    switch (piece.tag)
                    {
                        case "Color_Black":
                            blackSum ++;
                            break;
                        case "Color_Blue":
                            blueSum ++;
                            break;
                        case "Color_Green":
                            greenSum ++;
                            break;
                        case "Color_Purple": //add purpleSum when pieces less than purple max
                            if (purpleSum <= purpleMax) { purpleSum++; }
                            else { purpleSum = purpleMax; }                            
                            break;
                        case "Color_Red":
                            redSum ++;
                            break;                            
                    }

                    UpdateScoreBoard();

                    int x = piece.GetComponent<PieceController>().myPosition.x;
                    int y = piece.GetComponent<PieceController>().myPosition.y;
                    pieces[x, y] = null;
                    Destroy(piece);
                }
            }
            UpdateScoreBoard();

            ///show sum at certain color board ******************************unfinished, edited here when unity ready******************************
            Debug.Log(string.Format( "black =  {0} , blue =  {1}， green = {2}, purple = {3}, red = {4}" , blackSum ,blueSum ,greenSum , purpleSum , redSum));            

            //Debug.Log( "black = " + blackSum + "; blue = " + blueSum + "; green = " + greenSum + "; purple = " + purpleSum + "; red = " + redSum);

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

            //when no match, player act in certain order
            if (matchedPieces.Count == 0)
            {
                //if special pieces ( purple ) matched , player active special
                if (purpleSum == purpleMax)
                {
                    //Debug.Log(string.Format("Player special. Purple is {0}", purpleSum));
                    player.UseItem(purpleSum);                   
                }

                //if move pieces ( blue & black) matched , player move
                if (blueSum != 0 || blackSum != 0)
                {
                    player.Move(blackSum);
                    UpdateDistanceBoard();
                    // if bulesum >0, player add dodge buff 2 turns
                    if (blueSum != 0)
                    {
                        if (distance < Mathf.Max(monster.PowerAttackRange, monster.NormalAttackRange))
                        player.AddBuff("dodge", blueSum);
                        else player.Move(blueSum);
                    }
                }                

                //if attack pieces(green & red) matched , player attack
                if (greenSum != 0 || redSum != 0)
                {
                    player.Attack(greenSum, redSum);   
                }


             //player turn ends
                //reset martched pieces sum when no match
                blackSum = 0;
                blueSum = 0;
                greenSum = 0;
                redSum = 0;
                if (purpleSum == purpleMax) purpleSum = 0;  //reset purple when special triggered
                player.BuffDecreaseOne();


             //monster turn begins, monster act after player's action
                //monster attack if player in attack range
                if ( distance < Mathf.Max(monster.PowerAttackRange,monster.NormalAttackRange))
                {                    
                    //monster attack miss if player has dodge buff
                    //if(player.buffList.Exists(x => x.GetComponent<Buff>())
                    //{
                    //    Debug.Log("Monster attack miss.");
                    //}
                    if (distance > monster.PowerAttackRange)
                    {
                        monster.NormalAttack(player.IsDodge);
                    }
                    else monster.PowerAttack(player.IsDodge);
                }
                // monster move if player out of attack range
                else
                {
                    Debug.Log("Monster move, player out of attack range.");
                    monster.Move(-5f);
                    UpdateDistanceBoard();
                }  
             //monster turn ends   
                monster.BuffDecreaseOne();
            }
        };
        //Debug.Log("Player's current HP is" + monster.CurrentHP);

        //player lose when HP not more than 0 or move count come to end but monster still alive
        if ((moveCount >= moveLimit && monster.CurrentHP > 0) || player.CurrentHP <= 0)
        {
            levelEndText.text = "FAILED";
            levelEndPanel.SetActive(true);
        }
        //player win when monster HP not more than 0
        else if (monster.CurrentHP <= 0)
        {
            levelEndText.text = "COMPLETED";
            levelEndPanel.SetActive(true);
        }
        else
        {
            //    Start to accept user inputs again after a small delay
            yield return new WaitForSeconds(acceptInputDelay);
            boardState = State.stable;

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
        if (boardState == State.stable && moveCount == previousMoveCount)
        {
            List<GameObject> firstPotentialMatch = CheckAllPotentialMatches();

            // Shuffle and shift if no potential match found, until a hint is found
            while (firstPotentialMatch == null)
            {
                ShuffleBoard();
                while (boardState == State.shuffling)
                {
                    yield return new WaitForSeconds(.1f);
                }
                yield return new WaitForSeconds(shuffleDelay);

                StartShifting();
                while (boardState == State.shifting)
                {
                    yield return new WaitForSeconds(.1f);
                }

                firstPotentialMatch = CheckAllPotentialMatches();
            }

            // Show hint after a delay 
            yield return new WaitForSeconds(showHintDelay);
            if (boardState == State.stable && moveCount == previousMoveCount)
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

    //initialize score board to 0
    public void InitializeBoards()
    {
        blackSum = 0;
        blueSum = 0;
        greenSum = 0;
        redSum = 0;
        purpleSum = 0;
        UpdateScoreBoard();

        distance = initialDistance;
        UpdateDistanceBoard();
        
    }

    //update score board
    public void UpdateScoreBoard()
    {
        blackScoreText.text = blackSum.ToString();
        blueScoreText.text = blueSum.ToString();
        greenScoreText.text = greenSum.ToString();
        redScoreText.text = redSum.ToString();
        purpleScoreText.text = purpleSum.ToString();
    }    

    public void UpdateDistanceBoard()
    {
        distanceBoardText.text = distance.ToString();
    }

    /// <summary>
    /// Initialize monster and player
    /// </summary>
    /// <param name="playerMaxHP">player's initial HP</param>
    /// <param name="monsterMaxHp">monster's initial HP</param> 
    /// <param name="initialDistance">initial distance</param>
    public void InitializeCreature(float playerMaxHP, float monsterMaxHp, float initialDistance)
    {
        //initialize creature and distance
        player = Instantiate(playerPrefab, new Vector3(0.5f, 8.8f, 90f), Quaternion.identity).GetComponent<Player>();
        player.InitializeHPSlider(100f);
        monster = Instantiate(monsterPrefab, new Vector3(4.5f, 8.8f, 90f), Quaternion.identity).GetComponent<Monster>();
        monster.InitializeHPSlider(200f);
        monster.InitializeAttackInfo(10f,1f,5f,1f);
        this.initialDistance = initialDistance;
    }
    
}

