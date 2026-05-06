using UnityEngine;

public class FireBallShot : MonoBehaviour
{
    [SerializeField] GameObject fireBall;
    

    private void OnEnable()
    {
        Instantiate(fireBall);
    }
}
