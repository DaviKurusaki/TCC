using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;  // Referência ao Player
    public Vector3 offset = new Vector3(0, 10, -10); // Distância da câmera
    public float smoothSpeed = 0.125f; // Suavidade da movimentação

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 desiredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.LookAt(player.position); // Faz a câmera olhar para o jogador
    }
}
