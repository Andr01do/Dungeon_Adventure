using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    [Header("Керування Дверима")]
    public GameObject entryGate; 
    public GameObject exitGate; 

    [Header("Посилання")]
    private BossHealthUI bossUI; 
    private BossGolem currentBoss; 

    private bool isTriggered = false;

    private void Start()
    {

        bossUI = FindObjectOfType<BossHealthUI>(true);

        if (entryGate != null) entryGate.SetActive(false);
        if (exitGate != null) exitGate.SetActive(true);
    }
    public void Setup(GameObject bossObj)
    {
        if (bossObj != null)
            currentBoss = bossObj.GetComponent<BossGolem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isTriggered) return;

        if (other.CompareTag("Player"))
        {
            isTriggered = true;
            Debug.Log("BossRoom: Бій почався!");
            if (currentBoss == null)
            {
                currentBoss = GetComponentInParent<Room>()?.GetComponentInChildren<BossGolem>();
                if (currentBoss == null)
                    currentBoss = FindObjectOfType<BossGolem>();
            }
            if (currentBoss != null)
            {
                if (entryGate != null) entryGate.SetActive(true);
                if (bossUI != null)
                {
                    bossUI.gameObject.SetActive(true);
                    bossUI.Init(currentBoss);
                }
                currentBoss.OnDeathEvent += HandleBossDeath;
            }
            else
            {
                Debug.LogError("BossRoom: Боса не знайдено! Двері не закриються.");
            }
        }
    }
    private void HandleBossDeath()
    {
        Debug.Log("BossRoom: Перемога! Відкриваю двері.");
        if (exitGate != null) exitGate.SetActive(false);
        if (entryGate != null) entryGate.SetActive(false);
        if (bossUI != null)
        {
            StartCoroutine(HideUIRoutine());
        }
    }

    private System.Collections.IEnumerator HideUIRoutine()
    {
        yield return new WaitForSeconds(3f);
        if (bossUI != null) bossUI.gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (currentBoss != null)
        {
            currentBoss.OnDeathEvent -= HandleBossDeath;
        }
    }
}