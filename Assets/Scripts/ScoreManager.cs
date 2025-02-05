using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static int score;
    public static int badgeScore = 0;
    public TextMeshProUGUI scoreGUI;
    public GameObject badgeContainer;
    private Image badgeOutlineImage;
    private Image badgeFillImage;
    private Image badgeWingsImage;
    private Image[] badgeParts;
    public GameObject gameOverUI;
    private TextMeshProUGUI gameOverUIScore;

    void Awake()
    {
        badgeWingsImage = badgeContainer.transform.GetChild(0).gameObject.GetComponent<Image>();
        badgeFillImage = badgeContainer.transform.GetChild(1).gameObject.GetComponent<Image>();
        badgeOutlineImage = badgeContainer.transform.GetChild(2).gameObject.GetComponent<Image>();

        badgeWingsImage.enabled = false;
        badgeFillImage.enabled = false;
        badgeOutlineImage.enabled = false;
        score = 0;

        badgeParts = new Image[3] { badgeOutlineImage, badgeFillImage, badgeWingsImage };

        if (badgeScore > 0)
        {
            for (int i = 0; i < badgeScore; i++)
            {
                badgeParts[i].enabled = true;
            }

        }

        gameOverUIScore = gameOverUI.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreGUI.text = "Score: " + score;
        gameOverUIScore.text = "Score:\n" + score;
        if (score >= 15)
        {
            if (badgeScore < 3)
            {
                badgeScore += 1;
            }

            score = 0;

            if (badgeScore > 0)
            {
                badgeParts[badgeScore - 1].enabled = true;
            }
        }
    }
}
