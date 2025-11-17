using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI gameOverScoreText;
    private int nextmark = 100;
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
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void UpdateScoreDisplay(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        if (score>nextmark) 
        {
            nextmark+=100;
            Animator textAnimator = scoreText.GetComponent<Animator>();
            textAnimator.SetBool("grow", !textAnimator.GetBool("grow"));
        }
    }

    public void ShowGameOver(int finalScore)
    {
        if (scoreText != null)
        {
            scoreText.text = "";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        
        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = "Final Score: " + finalScore;
        }
    }

    public void OnRestartButtonClick()
    {
        GameManager.Instance?.RestartGame();
    }
}