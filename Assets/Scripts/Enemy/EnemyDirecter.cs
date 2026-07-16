using UnityEngine;
using UnityEngine.UI;


public class EnemyDirecter : MonoBehaviour, IDamageable, IAbsorble
{
    [SerializeField] Enemy enemyStatus;//エネミーのスクリプタブルオブジェクト
    public int enemyHp { get; set;}//エネミーの体力

    [SerializeField] Slider hpSlider;
    [SerializeField] Image fillImage;

    [Header("色の設定")]
    [SerializeField] Color highHpColor;
    [SerializeField] Color midHpColor;
    [SerializeField] Color lowHpColor;

    void Awake()
    {
        //スクリプタブルオブジェクトで初期化
        enemyHp = enemyStatus.Hp;
        Debug.Log(enemyHp);

        //Hpバーの初期化
        if (hpSlider != null)
        {
            hpSlider.maxValue = enemyStatus.Hp;
            hpSlider.value = enemyStatus.Hp;
        }
        else
        {
            hpSlider = GetComponent<Slider>();
            hpSlider.maxValue = enemyStatus.Hp;
            hpSlider.value = enemyStatus.Hp;
        }

        if (fillImage != null)
        {
            fillImage.color = highHpColor;
        }
        else
        {
            fillImage = GetComponent<Image>();
        }
    }

    public void Damage(int attackPower)
    {
        //ダメージ計算：アルテリオス計算式
        enemyHp -= Mathf.FloorToInt(attackPower - enemyStatus.DefensePower);
        hpSlider.value = enemyHp;
        ColorChange();
        Debug.Log("敵："+ enemyHp);

        //死亡判定
        if (enemyHp <= 0)
        {
            Debug.Log("倒された");
            Destroy(gameObject);
            //return true;
            //Death();
        }
        //return false;
    }

    public void Absorb()
    {
        int random = Random.Range(1, 100);

        int chances;//確率

        float hpRatio = (float)enemyHp / enemyStatus.Hp;
        Debug.Log("Hp割合" + hpRatio);

        switch (hpRatio)
        {
            case >=1.0f:
                chances = 1;
                Debug.Log(chances);
                break;
            case >=0.5f:
                chances = 5;
                Debug.Log(chances);
                break;
            case >=0.2f:
                chances = 35;
                Debug.Log(chances);
                break;
            case >=0.05f:
                chances = 60;
                Debug.Log(chances);
                break;
            default:
                chances = 1000;
                Debug.Log(chances);
                break;
        }

        Debug.Log("確率" + chances + ",乱数" + random);

        if (random <= chances)
        {
            Destroy(gameObject);
            Debug.Log("吸収された");
        }
    }

    //public void Death()
    //{
    //    ////オブジェクト削除
    //    //Debug.Log("倒された");
    //    //Destroy(gameObject);
    //}

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
}
