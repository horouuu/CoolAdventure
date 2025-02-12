using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static int maxHp = 3;
    public static int playerCurrentHp;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public static void SetCurrentHp(int hp)
    {
        playerCurrentHp = hp;
    }

    public static void Heal(int hp)
    {
        if (playerCurrentHp + hp <= maxHp)
        {
            playerCurrentHp += hp;
        }
    }

    void Update()
    {
        foreach (Image i in hearts)
        {
            i.sprite = emptyHeart;
        }

        for (int i = 0; i < playerCurrentHp; i++)
        {
            hearts[i].sprite = fullHeart;
        }
    }
}
