using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool[] walls = new bool[4] { true, true, true, true};
    public (int, int) pos;
    public bool isCurrent;
    public SpriteRenderer isCurrentShow;
    public GameObject[] wallsObj = new GameObject[4];

    void Awake()
    {
        for (int i = 0; i < 4; i++) {
            wallsObj[i] = transform.GetChild(i).gameObject;
        }
    }

    public void SetTiles() {
        for (int i = 0; i < 4; i++) {
            if (walls[i] == false) {
                wallsObj[i].SetActive(false);
            }
        }
    }
}
