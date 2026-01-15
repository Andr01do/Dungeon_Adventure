using UnityEngine;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"У тригер зайшов об'єкт: {other.name}, Тег: {other.tag}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Це гравець! Завершуємо рівень.");
            GameManager.Instance.FinishLevelLogic();
        }
        else
        {
            Debug.Log("Це НЕ гравець (невірний тег).");
        }
    }
}