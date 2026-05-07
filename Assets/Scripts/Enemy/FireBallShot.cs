using UnityEngine;

public class FireBallShot : MonoBehaviour
{
    [SerializeField] GameObject fireBall;
    [SerializeField] FireBall fireBallSc;
    [SerializeField] Transform enemyTr;
    

    private void OnEnable()
    {
        Instantiate(fireBall, enemyTr.position, Quaternion.identity);
    }
}
