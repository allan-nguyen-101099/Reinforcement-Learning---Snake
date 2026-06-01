using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Snake snake;
    private SnakeAgent agent;
    private GameObject gameOverCanvas;
    private Text scoreText;

    void Awake()
    {
        Instance = this;
        BuildGameOverUI();
    }

    public void RegisterSnake(Snake s) => snake = s;
    public void RegisterAgent(SnakeAgent a) => agent = a;

    void BuildGameOverUI()
    {
        gameOverCanvas = new GameObject("GameOverCanvas");
        Canvas canvas = gameOverCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        gameOverCanvas.AddComponent<CanvasScaler>();

        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(gameOverCanvas.transform, false);
        Image bg = panel.AddComponent<Image>();
        bg.color = new Color(0f, 0f, 0f, 0.6f);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.sizeDelta = Vector2.zero;

        Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        GameObject gameOverGO = new GameObject("GameOverText");
        gameOverGO.transform.SetParent(panel.transform, false);
        Text gameOverLabel = gameOverGO.AddComponent<Text>();
        gameOverLabel.text = "GAME OVER";
        gameOverLabel.font = font;
        gameOverLabel.fontSize = 80;
        gameOverLabel.fontStyle = FontStyle.Bold;
        gameOverLabel.alignment = TextAnchor.MiddleCenter;
        gameOverLabel.color = Color.red;
        RectTransform gameOverRT = gameOverGO.GetComponent<RectTransform>();
        gameOverRT.anchorMin = new Vector2(0f, 0.6f);
        gameOverRT.anchorMax = new Vector2(1f, 0.85f);
        gameOverRT.sizeDelta = Vector2.zero;

        GameObject scoreGO = new GameObject("ScoreText");
        scoreGO.transform.SetParent(panel.transform, false);
        scoreText = scoreGO.AddComponent<Text>();
        scoreText.font = font;
        scoreText.fontSize = 50;
        scoreText.alignment = TextAnchor.MiddleCenter;
        scoreText.color = Color.white;
        RectTransform scoreRT = scoreGO.GetComponent<RectTransform>();
        scoreRT.anchorMin = new Vector2(0f, 0.42f);
        scoreRT.anchorMax = new Vector2(1f, 0.6f);
        scoreRT.sizeDelta = Vector2.zero;

        GameObject btnGO = new GameObject("RestartButton");
        btnGO.transform.SetParent(panel.transform, false);
        Image btnImage = btnGO.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.6f, 0.2f);
        Button btn = btnGO.AddComponent<Button>();
        btn.onClick.AddListener(Restart);
        RectTransform btnRT = btnGO.GetComponent<RectTransform>();
        btnRT.anchorMin = new Vector2(0.35f, 0.2f);
        btnRT.anchorMax = new Vector2(0.65f, 0.36f);
        btnRT.sizeDelta = Vector2.zero;

        GameObject btnLabelGO = new GameObject("Label");
        btnLabelGO.transform.SetParent(btnGO.transform, false);
        Text btnLabel = btnLabelGO.AddComponent<Text>();
        btnLabel.text = "Restart";
        btnLabel.font = font;
        btnLabel.fontSize = 36;
        btnLabel.fontStyle = FontStyle.Bold;
        btnLabel.alignment = TextAnchor.MiddleCenter;
        btnLabel.color = Color.white;
        RectTransform btnLabelRT = btnLabelGO.GetComponent<RectTransform>();
        btnLabelRT.anchorMin = Vector2.zero;
        btnLabelRT.anchorMax = Vector2.one;
        btnLabelRT.sizeDelta = Vector2.zero;

        gameOverCanvas.SetActive(false);
    }

    public void GameOver()
    {
        // In AI training mode, end the episode instead of showing UI
        if (agent != null)
        {
            agent.OnDied();
            return;
        }

        int score = snake != null ? snake.GetLength() : 0;
        scoreText.text = "Score: " + score;
        gameOverCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
