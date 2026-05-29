using UnityEngine;

public class BoardController : MonoBehaviour
{
    int width = 8;
    int height = 8;
    int[,] gridArray;
    int maxColors = 4;

    public GameObject[] titlePrefabs;
    public float cellSize = 100f;
    public Transform boardParent;

    void Start()
    {
        gridArray = new int[height, width];

        for(int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = Random.Range(1, maxColors + 1);
                SpawnTile(gridArray[i, j], j, i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnTile(int color, int x, int y)
    {
        GameObject tile = Instantiate(titlePrefabs[color - 1], boardParent, false);
        tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
    }
}
