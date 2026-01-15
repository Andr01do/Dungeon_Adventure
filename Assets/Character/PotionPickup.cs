using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("Налаштування")]
    public int amount = 1;
    public string pickupSound = "Potion_Pickup";

    [Header("Візуальні ефекти")]
    public float rotateSpeed = 10f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    private Vector3 startPos;
    private IAudioService _audioService;

    void Start()
    {
        startPos = transform.position;

        try { _audioService = ServiceLocator.Get<IAudioService>(); } catch { }

        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true; 
    }

    void Update()
    {

        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();

            if (player != null)
            {
                player.AddPotion(amount);
                if (_audioService != null) _audioService.PlaySFX(pickupSound, transform.position);
                Debug.Log("Зілля підібрано!");
                Destroy(gameObject);
            }
        }
    }
}