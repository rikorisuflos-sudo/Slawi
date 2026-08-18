using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDirecter : MonoBehaviour, IDamageable
{
    [SerializeField] Player playerStatus;
    [SerializeField] Slider hpSlider;
    [SerializeField] Image fillImage;
    [SerializeField] Slider expSlider;

    [Header("色の設定")]
    [SerializeField] Color highHpColor;
    [SerializeField] Color midHpColor;
    [SerializeField] Color lowHpColor;

    int baseHp;
    int baseAttackPower;
    int baseDefensePower;

    int hp;
    int attackPower;
    int defensePower;

    float growthHp;
    float growthAttackPower;
    float growthDefensePower;
    public int AttackPower => attackPower;

    [SerializeField] int level = 1;
    [SerializeField] TextMeshProUGUI levelText;

    int currentExp;
    int needExp = 100; // レベル1→2に必要な経験値
    void Awake()
    {
        //ステータスの初期化
        hp = playerStatus.HP;
        attackPower = playerStatus.AttackPower;
        defensePower = playerStatus.DefensePower;

        //Hpバーの初期化
        if (hpSlider != null)
        {
            hpSlider.maxValue = playerStatus.HP;
            hpSlider.value = playerStatus.HP;
        }
        else
        {
            hpSlider = GetComponent<Slider>();
            hpSlider.maxValue = playerStatus.HP;
            hpSlider.value = playerStatus.HP;
        }

        //Expバーの初期化
        if (hpSlider != null)
        {
            expSlider.maxValue = needExp;
            expSlider.value = 0;
        }
        else
        {
            expSlider = GetComponent<Slider>();
            expSlider.maxValue = needExp;
            expSlider.value = 0;
        }

        if (fillImage != null)
        {
            fillImage.color = highHpColor;
        }
        else
        {
            fillImage = GetComponent<Image>();
        }
        Debug.Log(hp);
    }


    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式
        hp -= Mathf.FloorToInt(attackPower - defensePower);
        hpSlider.value = hp;
        ColorChange();
        Debug.Log("プレイヤー:" + hp);

        //死亡判定
        if (hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        //Debug.Log("倒された");
        SceneManager.LoadScene("TitleScene");
    }

    void ColorChange()
    {
        float ratio = hpSlider.value / hpSlider.maxValue;

        if (ratio > 0.5f)
        {
            fillImage.color = highHpColor;
        }
        else if (ratio > 0.2f)
        {
            fillImage.color = midHpColor;
        }
        else
        {
            fillImage.color = lowHpColor;
        }
    }

    public void AddExp(int exp)
    {
        currentExp += exp;
        expSlider.value = currentExp;

        Debug.Log(currentExp);

        while (currentExp >= needExp)
        {
            currentExp -= needExp;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        levelText.text = Convert.ToString(level);

        expSlider.maxValue = needExp;
        expSlider.value = currentExp;

        hp = Mathf.FloorToInt(baseHp * (1 + growthHp * (level - 1)));
        attackPower = Mathf.FloorToInt(baseAttackPower * (1 + growthAttackPower * (level - 1)));
        defensePower = Mathf.FloorToInt(baseDefensePower * (1 + growthDefensePower * (level - 1)));

        hpSlider.maxValue = hp;
        hpSlider.value = hp;
        ColorChange();

        needExp = 50 * level * level + 100;

        Debug.Log($"レベル{level}になりました！");
    }
}
