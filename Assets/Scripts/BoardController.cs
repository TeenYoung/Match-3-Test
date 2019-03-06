using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public int boardWidth;
    public int boardHeight;
    public GameObject[] piecePrefabs;
    public GameObject[,] pieces;


    // Start is called before the first frame update
    void Start()
    {
        SetupBoard();

        // For test and debug uses only
        List<GameObject> allMatchedPieces = CheckAllMatches();
        Debug.Log("Macthed pieces count:" + allMatchedPieces.Count);

        List<GameObject> hintPieces = CheckPotentialMatch(2, 2);
        Debug.Log("Hint:" + hintPieces.Count);
    }

    // Create a board (without any initial match)
    void SetupBoard()
    {
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

                // Show the position of the piece after its name in hierarchy.
                piece.name += coordinate.ToString();

                // Store the piece into the 2D array
                pieces[i, j] = piece;
            }
        }
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

        return allMatchedPieces;
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

    // Check if the piece at (x,y) is in a match, return all matched pieces of this match. Only check upwards and rightwards if fourDirection is false.
    List<GameObject> CheckMatch(int x, int y, GameObject[,] pieces, bool fourDirections = false)
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

        // Check other two directions if required.
        if (fourDirections)
        {
            //Check column match downwards
            if (y > 1)
            {
                if (pieces[x, y].tag == pieces[x, y - 1].tag && pieces[x, y].tag == pieces[x, y - 2].tag)
                {
                    matchedPieces.AddRange(new GameObject[] { pieces[x, y], pieces[x, y - 1], pieces[x, y - 2] });
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
                GameObject[,] piecesAfterSwapUp = (GameObject[,])pieces.Clone();
                piecesAfterSwapUp[x, y] = pieces[x, y + 1];
                piecesAfterSwapUp[x, y + 1] = pieces[x, y];

                // Check matches at four directions on the 1st swaped piece
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
                    // Check matches at four directions on the 2nd swaped piece
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
                GameObject[,] piecesAfterSwapRight = (GameObject[,])pieces.Clone();
                piecesAfterSwapRight[x, y] = pieces[x + 1, y];
                piecesAfterSwapRight[x + 1, y] = pieces[x, y];

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
}
