using UnityEngine;

public class DisableWhenOffScreen : MonoBehaviour
{
    private void OnBecameInvisible()
    {
        gameObject.SetActive(false);
    }

    private void OnBecameVisible()
    {
        gameObject.SetActive(true);
    }
}
