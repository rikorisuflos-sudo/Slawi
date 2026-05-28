using UnityEngine;

public class LvDirecter : MonoBehaviour
{
    int Lv = 1;
    int nextLvExp;
    int exp;

    void AddExp(int getExp) { 
        exp =+ getExp;
        if (exp > nextLvExp) { 
            exp =- nextLvExp;
            NextLv();
        }
    }

    void NextLv()
    {
        nextLvExp = (Mathf.FloorToInt(nextLvExp * 1.1f) + (Lv * 13)) / 2;
        Lv++;
    }
}
