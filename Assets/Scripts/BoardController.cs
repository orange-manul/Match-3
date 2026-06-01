using UnityEngine;

public class BoardController : MonoBehaviour
{
    int width = 8;
    int height = 8;

    // ИЗМЕНЕНИЕ: Теперь храним объекты Tile, а не просто цифры
    Tile[,] gridArray;
    int maxColors = 4;

    public GameObject[] titlePrefabs;
    public float cellSize = 100f;
    public Transform boardParent;

    void Start()
    {
        // Инициализируем массив под скрипты
        gridArray = new Tile[height, width];

        for (int i = 0; i < gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridArray.GetLength(1); j++)
            {
                int randomColor = Random.Range(1, maxColors + 1);
                SpawnTile(randomColor, j, i);
            }
        }
    }

    void SpawnTile(int color, int x, int y)
    {
        GameObject tileObj = Instantiate(titlePrefabs[color - 1], boardParent, false);

        // ВЫЧИСЛЯЕМ СДВИГ: сдвигаем начальную точку влево на пол-поля и вверх на пол-поля
        // Для поля 8х8 при cellSize = 100, это сдвиг влево на 350 пикселей и вверх на 350 пикселей
        float startX = -((width - 1) * cellSize) / 2f;
        float startY = ((height - 1) * cellSize) / 2f;

        // Итоговая позиция фишки с учетом смещения
        float posX = startX + (x * cellSize);
        float posY = startY - (y * cellSize);

        tileObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, posY);

        // Передаем данные в скрипт фишки
        Tile tileScript = tileObj.GetComponent<Tile>();
        if (tileScript != null)
        {
            tileScript.x = x;
            tileScript.y = y;
            tileScript.colorID = color;
            gridArray[y, x] = tileScript;
        }
    }

    public void MoveTitle(Tile tile, Vector2 direction)
    {
        int targetX = tile.x + (int)direction.x;
        // Свайп вверх по экрану (direction.y = 1) должен уменьшать индекс строки
        int targetY = tile.y - (int)direction.y;

        // Проверка границ поля
        if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height)
        {
            Debug.LogWarning("Выход за пределы поля!");
            return;
        }

        // Получаем соседа из нашей новой матрицы
        Tile neighbor = gridArray[targetY, targetX];

        if (neighbor != null)
        {
            // 1. Меняем фишки местами в логической матрице
            gridArray[tile.y, tile.x] = neighbor;
            gridArray[targetY, targetX] = tile;

            // 2. Меняем их внутренние координаты x и y местами
            int tempX = tile.x;
            int tempY = tile.y;

            tile.x = targetX;
            tile.y = targetY;

            neighbor.x = tempX;
            neighbor.y = tempY;

            float startX = -((width - 1) * cellSize) / 2f;
            float startY = ((height - 1) * cellSize) / 2f;

            // 3. Визуально двигаем ОБЕ фишки на их новые места
            tile.GetComponent<RectTransform>().anchoredPosition = new Vector2(tile.x * cellSize, -tile.y * cellSize);
            neighbor.GetComponent<RectTransform>().anchoredPosition = new Vector2(neighbor.x * cellSize, -neighbor.y * cellSize);

            Debug.Log("Фишки успешно поменялись местами!");
        }
    }
}