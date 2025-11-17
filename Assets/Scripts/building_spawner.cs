using UnityEngine;
using System.Collections.Generic;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Grid Settings")]
    [SerializeField] private int initialRows = 40;
    [SerializeField] private int buildingsPerRow = 120;
    [SerializeField] private float buildingBaseWidth = 3f;
    [SerializeField] private float buildingBaseDepth = 3f;
    private float rowSpacing;

    [Header("Building Heights")]
    [SerializeField] private float lowBuildingProbability = 0.3f;
    [SerializeField] private float minLowHeight = 0.5f;
    [SerializeField] private float maxLowHeight = 3f;
    [SerializeField] private float minHighHeight = 6f;
    [SerializeField] private float maxHighHeight = 12f;

    [Header("Collectible Settings")]
    [SerializeField] private float collectibleSpawnProbability = 0.05f;
    [SerializeField] private float collectibleHeightOffset = 1f;

    [Header("References")]
    [SerializeField] private Transform shipTransform;

    private Queue<List<GameObject>> buildingRows = new Queue<List<GameObject>>();
    private float nextRowZ;
    private float lastCheckZ;
    private int[] possibleAngles = { 0, 90, 180, 270 };


    void Start()
    {
        // Empezar a generar edificios adelante de la nave
        rowSpacing = buildingBaseWidth * 2;
        nextRowZ = shipTransform.position.z - 18f + rowSpacing;
        lastCheckZ = shipTransform.position.z;
        
        // Crear filas iniciales
        for (int i = 0; i < initialRows/4; i++)
        {
            CreateRow(true);
        }
        for (int i = initialRows / 4; i < initialRows; i++)
        {
            CreateRow(false);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver()) return;

        CheckForNewRow();
        CheckForRowDeletion();
    }

    private void CreateRow(bool isInitial) //Uso este booleano para que las primeras filas no tengan obstáculos y así el jugador tenga tiempo para reaccionar
    {
        List<GameObject> row = new List<GameObject>();
        
        float startX = shipTransform.position.x - (buildingsPerRow * buildingBaseWidth) + buildingBaseWidth;

        for (int i = 0; i < buildingsPerRow; i++)
        {
            float xPos = startX + i * buildingBaseWidth*2f;
            Vector3 position = new Vector3(xPos, 0, nextRowZ);

            // Decidir si es edificio bajo o alto
            float randomValue = Random.value;
            bool isLowBuilding = randomValue < lowBuildingProbability;
            if (isInitial) 
            {
                isLowBuilding = true;
            }
            
            int randomIndex = Random.Range(0, possibleAngles.Length);
            float yRotation = possibleAngles[randomIndex];

            // Rotación
            Quaternion rotation = Quaternion.Euler(0f, yRotation, 0f);

            float height;
            if (isLowBuilding)
            {
                height = Random.Range(minLowHeight, maxLowHeight);
            }
            else
            {
                height = Random.Range(minHighHeight, maxHighHeight);
            }

            // Crear edificio
            GameObject building = Instantiate(buildingPrefab, position, rotation, transform);

            // Escalar edificio
            building.transform.localScale = new Vector3(
                buildingBaseWidth,
                height,
                buildingBaseDepth
            );

            row.Add(building);

            // Intentar crear coleccionable si es edificio bajo
            if (isLowBuilding && Random.value < collectibleSpawnProbability)
            {
                Vector3 collectiblePos = new Vector3(
                    xPos,
                    height + collectibleHeightOffset,
                    nextRowZ
                );

                GameObject collectible = Instantiate(collectiblePrefab, collectiblePos, rotation, transform);
                row.Add(collectible);
            }
        }

        buildingRows.Enqueue(row);
        nextRowZ += rowSpacing;
    }

    private void CheckForNewRow()
    {
        if (shipTransform.position.z >= lastCheckZ + rowSpacing)
        {
            CreateRow(false);
            lastCheckZ += rowSpacing;
        }
    }

    private void CheckForRowDeletion()
    {
        if (buildingRows.Count > 0) //De esta forma evitamos acceder a una lista vacía
        {
            List<GameObject> firstRow = buildingRows.Peek();

            if (firstRow.Count > 0 && firstRow[0] != null)
            {
                float firstRowZ = firstRow[0].transform.position.z;

                // Si la fila está suficientemente detrás del avión, eliminarla
                if (shipTransform.position.z - firstRowZ > rowSpacing * 2)
                {
                    List<GameObject> rowToDelete = buildingRows.Dequeue();

                    foreach (GameObject obj in rowToDelete)
                    {
                        if (obj != null)
                        {
                            Destroy(obj);
                        }
                    }
                }
            }
        }
    }
}