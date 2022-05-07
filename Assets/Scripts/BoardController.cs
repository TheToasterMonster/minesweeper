using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardController : MonoBehaviour
{
    public GameObject squarePrefab;
    public GameObject bombPrefab;
    public GameObject gameWonPrefab;
    public GameObject gameLostPrefab;

    public int boardLength;
    public float bombRatio;

    private int bombCount;
    private int safeCount;
    private float timeTaken;

    private float squareSize;
    private float borderSize;
    private float cameraSize;

    private int[,] board;
    private GameObject[,] squareObjects;
    private bool[,] visited;

    private bool firstClick;
    private bool gameOver;

    // Start is called before the first frame update
    void Start()
    {
        cameraSize = Camera.main.orthographicSize;

        float squareCount = boardLength + (float)(boardLength + 1) / 20;
        squareSize = cameraSize * 2 / squareCount;
        borderSize = squareSize / 20;

        squarePrefab.transform.localScale = new Vector3(squareSize, squareSize, 1);

        bombCount =  Mathf.RoundToInt(boardLength * boardLength * bombRatio);
        safeCount = boardLength * boardLength - bombCount;

        board = new int[boardLength, boardLength];
        squareObjects = new GameObject[boardLength, boardLength];
        visited = new bool[boardLength, boardLength];

        CreateBoard();

        int row = 0, col = 0;
        for (float i = -cameraSize + borderSize; i < cameraSize; i += squareSize + borderSize)
        {
            for (float j = -cameraSize + borderSize; j < cameraSize; j += squareSize + borderSize)
            {
                squareObjects[row, col] = Instantiate(squarePrefab, new Vector3(i + squareSize / 2, j + squareSize / 2, 0), Quaternion.identity);
                col++;
            }
            row++;
            col = 0;
        }

        Square[] squares = FindObjectsOfType<Square>();
        foreach (Square square in squares)
        {
            square.onClick += ProcessClick;
        }
        Square.flagCount = bombCount;

        gameOver = false;
        Square.gameOver = gameOver;

        firstClick = true;
    }

    void Update()
    {
        if (!firstClick && !gameOver)
        {
            timeTaken += Time.deltaTime;
        }

        transform.GetChild(0).transform.GetChild(0).GetComponent<TextMeshPro>().text = "Flags: " + Square.flagCount;
        transform.GetChild(0).transform.GetChild(1).GetComponent<TextMeshPro>().text = "Time: " + timeTaken.ToString("0.0") + "s";
    }

    void CreateBoard()
    {
        int bombs = 0;
        while (bombs < bombCount)
        {
            int x = Random.Range(0, boardLength);
            int y = Random.Range(0, boardLength);
            if (board[x, y] < 0)
            {
                continue;
            }

            board[x, y] = -10;

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int xNew = x + dx, yNew = y + dy;
                    if (IsValidCoord(xNew, yNew))
                    {
                        board[xNew, yNew]++;
                    }
                }
            }

            bombs++;
        }
    }

    void ClearBoard()
    {
        for (int i = 0; i < boardLength; i++)
        {
            for (int j = 0; j < boardLength; j++)
            {
                board[i, j] = 0;
            }
        }
    }

    bool IsValidCoord(int i, int j)
    {
        return i >= 0 && i < boardLength && j >= 0 && j < boardLength;
    }

    void ProcessClick(int i, int j)
    {
        if (gameOver)
        {
            return;
        }

        if (firstClick)
        {
            while (board[i, j] != 0)
            {
                ClearBoard();
                CreateBoard();
            }
            firstClick = false;
        }

        if (board[i, j] < 0)
        {
            RevealBomb(i, j);
            gameOver = true;
            Square.gameOver = true;
            GameLost();
        }
        else
        {
            RevealNeighbors(i, j);

            if (safeCount <= 0)
            {
                gameOver = true;
                Square.gameOver = true;
                GameWon();
            }
        }
    }

    void GameLost()
    {
        Instantiate(gameLostPrefab, new Vector3(0, 0, -9), Quaternion.identity);
        StartCoroutine(RevealAllBombs());
    }

    void GameWon()
    {
        GameObject won = Instantiate(gameWonPrefab, new Vector3(0, 0, -9), Quaternion.identity);
        won.transform.GetChild(1).GetComponent<TextMeshPro>().text = "Time: " + timeTaken.ToString("0.0") + "s";
    }

    IEnumerator RevealAllBombs()
    {
        yield return new WaitForSecondsRealtime(.5f);
        for (int j = boardLength - 1; j >= 0; j--)
        {
            for (int i = 0; i < boardLength; i++)
            {
                if (board[i, j] < 0 && !squareObjects[i, j].GetComponent<Square>().getIsFlagged())
                {
                    RevealBomb(i, j);
                    yield return new WaitForSecondsRealtime(.25f);
                }
            }
        }
    }

    void Reveal(int i, int j)
    {
        squareObjects[i, j].GetComponent<SpriteRenderer>().color = new Color(150f / 255, 150f / 255, 150f / 255);
        if (board[i, j] > 0)
        {
            squareObjects[i, j].transform.GetChild(0).GetComponent<TextMeshPro>().text = board[i, j].ToString();
        }

        safeCount--;
    }

    void RevealBomb(int i, int j)
    {
        GameObject bomb = Instantiate(bombPrefab, squareObjects[i, j].transform.position + new Vector3(0, 0, -5), Quaternion.identity);
        bomb.transform.localScale = squareObjects[i, j].transform.localScale * .6f;
        squareObjects[i, j].GetComponent<SpriteRenderer>().color = new Color(1, 0, 0);
    }

    void RevealNeighbors(int x, int y)
    {
        visited[x, y] = true;
        Reveal(x, y);

        if (board[x, y] == 0)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                    {
                        continue;
                    }

                    int xNew = x + dx, yNew = y + dy;
                    if (!IsValidCoord(xNew, yNew) || visited[xNew, yNew])
                    {
                        continue;
                    }

                    RevealNeighbors(xNew, yNew);
                }
            }
        }
    }
}
