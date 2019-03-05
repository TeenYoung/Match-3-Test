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
    }

    // Create a board without any initial match
    void SetupBoard()
    {
        // Create a 2D array to store pieces and determine its size.
        pieces = new GameObject[boardWidth, boardHeight];

        //Layout the board
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

    // Check if the prefab will result in an initial match should it be instantiated at (x,y).
    bool CheckInitialMatch(int x, int y, GameObject piecePrefab)
    {
        // Check columns first for efficiency
        if (y > 1)
        {
            // Every piece prefab has its unique tag
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
}
