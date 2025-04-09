using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public Transform boardParent;
    public GameObject tilePref;
    public int startY, startX = 0;
    public int endY, endX;
    public int height, width = 8;
    public float waitTime = 0f;
    Tile[,] board;

    private void CreateBoard() {
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                if (board[i, j] == null) {
                    GameObject tileObj = Instantiate(tilePref, boardParent);
                    tileObj.name = "tile" + i + "" + j;
                    tileObj.transform.position = new Vector2((j - board.GetLength(1) / 2) *  tileObj.transform.localScale.x * 0.8f, (-i + board.GetLength(0) / 2) * tileObj.transform.localScale.y * 0.8f);
                    Tile tile = tileObj.GetComponent<Tile>();   
                    board[i, j] = tile;                 
                }
                Tile tilePiece = board[i, j];
                tilePiece.pos = (i, j);
                tilePiece.isCurrent = false;
                tilePiece.isCurrentShow.enabled = false;
                tilePiece.walls = new bool[4] {true, true, true, true};

                tilePiece.SetTiles();
            }
        }
    }

    private void SetEveryTile() {
        for (int i = 0; i < board.GetLength(0); i++) {
            for (int j = 0; j < board.GetLength(1); j++) {
                board[i, j].SetTiles();
            }
        }
    }

    private List<Tile> GetConnectingRooms(Tile[,] board, (int, int) currentPos) {
        List<Tile> connectingTiles = new List<Tile>();

        (int, int)[] directions = new (int, int)[4] {(1, 0), (0, 1), (-1, 0), (0, -1)};

        foreach ((int, int) direction in directions) {
            int newY = currentPos.Item1 + direction.Item1;
            int newX = currentPos.Item2 + direction.Item2;

            if (newY >= 0 && newY < board.GetLength(0) && newX >= 0 && newX < board.GetLength(1)) {
                Tile tile = board[newY, newX];
                if (tile.walls[0] && tile.walls[1] && tile.walls[2] && tile.walls[3]) {
                    connectingTiles.Add(tile);
                }
            }
        }

        return connectingTiles;
    }

    private IEnumerator DepthFirstSearch(Stack<(int, int)> stack, int depth) {
        int y = stack.Peek().Item1;
        int x = stack.Peek().Item2;

        if (stack.Count == 0 || depth >= height * width - 1) {
            board[y, x].isCurrentShow.enabled = false;
            board[y, x].isCurrent = false;
            yield break;
        }

        yield return new WaitForSeconds(waitTime);


        Tile curTile = board[y, x];

        List<Tile> connectingTiles = GetConnectingRooms(board, (y, x));

        if (connectingTiles.Count == 0) {
            board[y, x].isCurrentShow.enabled = false;
            board[y, x].isCurrent = false;
            stack.Pop();

            int i = stack.Peek().Item1;
            int j = stack.Peek().Item2;

            board[i, j].isCurrentShow.enabled = true;
            board[i, j].isCurrent = true;
            yield return StartCoroutine(DepthFirstSearch(stack, depth));
            yield break;
        }

        Tile nextTile = connectingTiles[Random.Range(0, connectingTiles.Count)];

        int y2 = nextTile.pos.Item1;
        int x2 = nextTile.pos.Item2;

        stack.Push((y2, x2));

        if (y2 - 1 == y) {
            curTile.walls[1] = false;
            nextTile.walls[0] = false; 
        } else if (y2 + 1 == y) {
            curTile.walls[0] = false;
            nextTile.walls[1] = false; 
        } else if (x2 - 1 == x) {
            curTile.walls[2] = false;
            nextTile.walls[3] = false; 
        } else if (x2 + 1 == x) {
            curTile.walls[3] = false;
            nextTile.walls[2] = false; 
        }

        board[y, x].isCurrentShow.enabled = false;
        board[y, x].isCurrent = false;
        
        board[y2, x2].isCurrentShow.enabled = true;
        board[y2, x2].isCurrent = true;

        board[y, x].SetTiles();
        board[y2, x2].SetTiles();

        yield return StartCoroutine(DepthFirstSearch(stack, depth + 1));
    }



    public void SetUpBoard() {
        board = new Tile[height, width];

        CreateBoard();

        Stack<(int, int)> stack = new Stack<(int, int)>();
        stack.Push((startY, startX));

        board[startY, startX].isCurrentShow.enabled = true;
        board[startY, startX].isCurrent = true;

        StartCoroutine(DepthFirstSearch(stack, 0));


        // SetEveryTile();
    }

    void Start()
    {
        SetUpBoard();
    }

}
