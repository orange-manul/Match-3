using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    // Сделали public, чтобы параметры железно брались из инспектора, если ты их там меняешь
    public int width = 8;
    public int height = 8;
    public int maxColors = 4;
    public float cellSize = 100f;

    public GameObject[] titlePrefabs;
    public Transform boardParent;

    private Tile[,] gridArray;
    private bool isProcessing = false;

    void Start()
    {
        gridArray = new Tile[height, width];

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int randomColor = GetValidStartingColor(j, i);
                SpawnTile(randomColor, j, i);
            }
        }
    }

    int GetValidStartingColor(int x, int y)
    {
        List<int> possibleColors = new List<int>();
        for (int c = 1; c <= maxColors; c++) possibleColors.Add(c);

        if (x >= 2)
        {
            Tile t1 = gridArray[y, x - 1];
            Tile t2 = gridArray[y, x - 2];
            if (t1 != null && t2 != null && t1.colorID == t2.colorID) possibleColors.Remove(t1.colorID);
        }

        if (y >= 2)
        {
            Tile t1 = gridArray[y - 1, x];
            Tile t2 = gridArray[y - 2, x];
            if (t1 != null && t2 != null && t1.colorID == t2.colorID) possibleColors.Remove(t1.colorID);
        }

        if (possibleColors.Count > 0) return possibleColors[Random.Range(0, possibleColors.Count)];
        return Random.Range(1, maxColors + 1);
    }

    // ИСПРАВЛЕНИЕ КОПИПАСТА: Вынесли общую сборку фишки в отдельный метод
    Tile CreateTileObject(int color, int x, int y)
    {
        GameObject tileObj = Instantiate(titlePrefabs[color - 1], boardParent, false);
        Tile tileScript = tileObj.GetComponent<Tile>();

        if (tileScript != null)
        {
            tileScript.x = x;
            tileScript.y = y;
            tileScript.colorID = color;
            gridArray[y, x] = tileScript;
        }
        return tileScript;
    }

    void SpawnTile(int color, int x, int y)
    {
        Tile tile = CreateTileObject(color, x, y);
        if (tile != null)
        {
            tile.GetComponent<RectTransform>().anchoredPosition = GetPositionForCell(x, y);
        }
    }

    Vector2 GetPositionForCell(int x, int y)
    {
        float startX = -((width - 1) * cellSize) / 2f;
        float startY = ((height - 1) * cellSize) / 2f;
        return new Vector2(startX + (x * cellSize), startY - (y * cellSize));
    }

    public void MoveTitle(Tile tile, Vector2 direction)
    {
        if (isProcessing) return;

        int targetX = tile.x + (int)direction.x;
        int targetY = tile.y - (int)direction.y;

        if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
        {
            Debug.LogWarning("Выход за пределы поля!");
            return;
        }

        Tile neighbor = gridArray[targetY, targetX];
        if (neighbor != null)
        {
            StartCoroutine(TurnCoroutine(tile, neighbor));
        }
    }

    IEnumerator TurnCoroutine(Tile tile, Tile neighbor)
    {
        isProcessing = true;

        int t1X = tile.x; int t1Y = tile.y;
        int t2X = neighbor.x; int t2Y = neighbor.y;

        gridArray[t1Y, t1X] = neighbor;
        gridArray[t2Y, t2X] = tile;

        tile.x = t2X; tile.y = t2Y;
        neighbor.x = t1X; neighbor.y = t1Y;

        Vector2 pos1 = GetPositionForCell(tile.x, tile.y);
        Vector2 pos2 = GetPositionForCell(neighbor.x, neighbor.y);

        StartCoroutine(MoveTileSmoothly(tile, pos1, 0.15f));
        StartCoroutine(MoveTileSmoothly(neighbor, pos2, 0.15f));

        yield return new WaitForSeconds(0.15f);

        List<Tile> matches = FindMatches();

        if (matches.Count > 0)
        {
            while (matches.Count > 0)
            {
                foreach (Tile match in matches)
                {
                    if (match != null)
                    {
                        gridArray[match.y, match.x] = null;
                        Destroy(match.gameObject);
                    }
                }

                yield return new WaitForSeconds(0.15f);

                yield return StartCoroutine(ApplyGravity());
                yield return StartCoroutine(RefillBoard());

                matches = FindMatches();
            }
        }
        else
        {
            gridArray[t1Y, t1X] = tile;
            gridArray[t2Y, t2X] = neighbor;

            tile.x = t1X; tile.y = t1Y;
            neighbor.x = t2X; neighbor.y = t2Y;

            StartCoroutine(MoveTileSmoothly(tile, GetPositionForCell(tile.x, tile.y), 0.15f));
            StartCoroutine(MoveTileSmoothly(neighbor, GetPositionForCell(neighbor.x, neighbor.y), 0.15f));

            yield return new WaitForSeconds(0.15f);
        }

        isProcessing = false;
    }

    IEnumerator MoveTileSmoothly(Tile tile, Vector2 targetPos, float duration)
    {
        if (tile == null) yield break;
        RectTransform rect = tile.GetComponent<RectTransform>();
        if (rect == null) yield break;

        Vector2 startPos = rect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (tile == null || rect == null) yield break;
            elapsed += Time.deltaTime;
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsed / duration);
            yield return null;
        }

        if (rect != null) rect.anchoredPosition = targetPos;
    }

    private List<Tile> FindMatches()
    {
        List<Tile> matches = new List<Tile>();

        // Horizontal
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width - 2; x++)
            {
                Tile t1 = gridArray[y, x];
                Tile t2 = gridArray[y, x + 1];
                Tile t3 = gridArray[y, x + 2];

                if (t1 != null && t2 != null && t3 != null)
                {
                    if (t1.colorID == t2.colorID && t2.colorID == t3.colorID)
                    {
                        if (!matches.Contains(t1)) matches.Add(t1);
                        if (!matches.Contains(t2)) matches.Add(t2);
                        if (!matches.Contains(t3)) matches.Add(t3);

                        int nextX = x + 3;
                        while (nextX < width && gridArray[y, nextX] != null && gridArray[y, nextX].colorID == t1.colorID)
                        {
                            Tile tNext = gridArray[y, nextX];
                            if (!matches.Contains(tNext)) matches.Add(tNext);
                            nextX++;
                        }
                    }
                }
            }
        }

        // Vertical
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height - 2; y++)
            {
                Tile t1 = gridArray[y, x];
                Tile t2 = gridArray[y + 1, x];
                Tile t3 = gridArray[y + 2, x];

                if (t1 != null && t2 != null && t3 != null)
                {
                    if (t1.colorID == t2.colorID && t2.colorID == t3.colorID)
                    {
                        if (!matches.Contains(t1)) matches.Add(t1);
                        if (!matches.Contains(t2)) matches.Add(t2);
                        if (!matches.Contains(t3)) matches.Add(t3);

                        int nextY = y + 3;
                        while (nextY < height && gridArray[nextY, x] != null && gridArray[nextY, x].colorID == t1.colorID)
                        {
                            Tile tNext = gridArray[nextY, x];
                            if (!matches.Contains(tNext)) matches.Add(tNext);
                            nextY++;
                        }
                    }
                }
            }
        }

        return matches;
    }

    IEnumerator ApplyGravity()
    {
        bool movedAny = false;

        for (int x = 0; x < width; x++)
        {
            for (int y = height - 1; y >= 0; y--)
            {
                if (gridArray[y, x] == null)
                {
                    for (int yCheck = y - 1; yCheck >= 0; yCheck--)
                    {
                        if (gridArray[yCheck, x] != null)
                        {
                            Tile tile = gridArray[yCheck, x];
                            gridArray[y, x] = tile;
                            gridArray[yCheck, x] = null;
                            tile.y = y;

                            Vector2 targetPos = GetPositionForCell(x, y);
                            StartCoroutine(MoveTileSmoothly(tile, targetPos, 0.15f));
                            movedAny = true;
                            break;
                        }
                    }
                }
            }
        }

        if (movedAny) yield return new WaitForSeconds(0.15f);
    }

    IEnumerator RefillBoard()
    {
        bool spawnedAny = false;

        for (int x = 0; x < width; x++)
        {
            // Считаем пустые ячейки в данном столбце, чтобы создать красивую «очередь» спавна сверху
            int missingTilesCount = 0;

            for (int y = height - 1; y >= 0; y--)
            {
                if (gridArray[y, x] == null)
                {
                    missingTilesCount++;
                    int randomColor = Random.Range(1, maxColors + 1);

                    // Передаем missingTilesCount как смещение по высоте (индекс -missingTilesCount)
                    SpawnTileAtTopAndDrop(randomColor, x, y, missingTilesCount);
                    spawnedAny = true;
                }
            }
        }

        if (spawnedAny) yield return new WaitForSeconds(0.2f);
    }

    // Изменили сигнатуру: теперь учитывается порядковый номер пустой ячейки
    void SpawnTileAtTopAndDrop(int color, int x, int y, int spawnOffset)
    {
        Tile tileScript = CreateTileObject(color, x, y);

        if (tileScript != null)
        {
            // Точка спавна теперь динамическая: чем больше пустых ячеек, тем выше создается префаб
            Vector2 spawnPos = GetPositionForCell(x, -spawnOffset);
            RectTransform rect = tileScript.GetComponent<RectTransform>();
            rect.anchoredPosition = spawnPos;

            Vector2 targetPos = GetPositionForCell(x, y);
            StartCoroutine(MoveTileSmoothly(tileScript, targetPos, 0.2f));
        }
    }
}