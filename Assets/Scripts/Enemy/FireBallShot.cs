using UnityEngine;

public class FireBallShot : MonoBehaviour
{
    [SerializeField] GameObject fireBall;
    [SerializeField] FireBall fireBallSc;
    [SerializeField] Transform enemyTr;
    

    void OnEnable()
    {
        GameObject newFireBoal = Instantiate(fireBall, enemyTr.position, Quaternion.identity);

        fireBallSc = newFireBoal.GetComponent<FireBall>();
        fireBallSc.EnemyTr = enemyTr;
    }
}
