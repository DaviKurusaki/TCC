using UnityEngine;

public class LightningEffect : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float duration = 0.2f;

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogError("LineRenderer não encontrado.");
            return;
        }

        if (startPoint != null && endPoint != null)
        {
            lr.positionCount = 2;
            lr.SetPosition(0, startPoint.position);
            lr.SetPosition(1, endPoint.position);
        }

        Destroy(gameObject, duration); // autodestruição após o efeito
    }
}
