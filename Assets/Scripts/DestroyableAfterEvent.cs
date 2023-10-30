using UnityEngine;

public class DestroyableAfterEvent : MonoBehaviour
{
    public void DestroyGameObject()
    {
        GameObject.Destroy(gameObject);
    }
}
