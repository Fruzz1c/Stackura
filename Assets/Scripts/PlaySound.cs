using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public AudioSource audioSource;

    // Вызывается при столкновении с другим объектом
    private void OnCollisionEnter(Collision collision)
    {
        // Проверяем, имеет ли объект тег "Block"
        if (collision.gameObject.CompareTag("Block"))
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
        }
    }
}
