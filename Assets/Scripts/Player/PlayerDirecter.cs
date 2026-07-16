using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerDirecter : MonoBehaviour, IDamageable, IStatusble
{
    [SerializeField] Player playerStatus;
    [SerializeField] Slider hpSlider;
    [SerializeField] Image fillImage;

    [Header("色の設定")]
    [SerializeField] Color highHpColor;
    [SerializeField] Color midHpColor;
    [SerializeField] Color lowHpColor;

    int playerHp;
    int attackPower;
    int defensePower;
    public int AttackPower => attackPower;

    void Awake()
    {
        //ステータスの初期化
        playerHp = playerStatus.HP;
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

        if (fillImage != null)
        {
            fillImage.color = highHpColor;
        }
        else
        {
            fillImage = GetComponent<Image>();
        }
        Debug.Log(playerHp);
    }

   
    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式
        playerHp -= Mathf.FloorToInt(attackPower - defensePower);
        hpSlider.value = playerHp;
        ColorChange();
        Debug.Log("プレイヤー:" + playerHp);

        //死亡判定
        if (playerHp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        //Debug.Log("倒された");
        SceneManager.LoadScene("TitleScene");
    }

    public void StatusReset()
    {
        attackPower = playerStatus.AttackPower;
        defensePower = playerStatus.DefensePower;
    }

    public void Buff(int plus)
    {
        attackPower = (attackPower + plus);
    }

    public void Debuff(int minus)
    {
       
    }

    void ColorChange() { 
        float ratio = hpSlider.value / hpSlider.maxValue;

        if(ratio > 0.5f)
        {
            fillImage.color = highHpColor;
        }else if(ratio > 0.2f)
        {
            fillImage.color = midHpColor;
        }
        else
        {
            fillImage.color = lowHpColor;
        }
    }
}
