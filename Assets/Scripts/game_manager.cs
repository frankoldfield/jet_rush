using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Score Settings")]
    [SerializeField] private int pointsPerCollectible = 10;
    [SerializeField] private float pointsPerSecond = 1f;
    
    [Header("References")]
    [SerializeField] private ShipController shipController;
    
    private int currentScore = 0;
    private float timePlayed = 0f;
    private bool gameOver = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!gameOver)
        {
            // Aumentar puntuación por tiempo
            timePlayed += Time.deltaTime;
            int timeScore = Mathf.FloorToInt(timePlayed * pointsPerSecond);
            UpdateScore(timeScore);
        }
    }

    private void UpdateScore(int baseTimeScore)
    {
        // La puntuación total es la puntuación base por tiempo más los coleccionables
        // Para evitar contar el tiempo dos veces, usamos un enfoque diferente
        currentScore = baseTimeScore;
        UIManager.Instance.UpdateScoreDisplay(currentScore);
    }

    public void CollectItem()
    {
        if (gameOver) return;
        
        // Añadir puntos por coleccionable (equivalente a segundos de juego)
        timePlayed += pointsPerCollectible / pointsPerSecond;
        
        // Actualizar inmediatamente
        int totalScore = Mathf.FloorToInt(timePlayed * pointsPerSecond);
        currentScore = totalScore;
        UIManager.Instance.UpdateScoreDisplay(currentScore);
    }

    public void GameOver()
    {
        if (gameOver) return;
        
        gameOver = true;
        shipController.SetGameOver(true);
        UIManager.Instance.ShowGameOver(currentScore);
    }

    public bool IsGameOver()
    {
        return gameOver;
    }


    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );
    }
}