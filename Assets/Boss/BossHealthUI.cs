    using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthSlider;

    private BossGolem boss;

    public void Init(BossGolem bossController)
    {
        this.boss = bossController;
        healthSlider.maxValue = boss.maxHealth;
        healthSlider.value = boss.currentHealth;
        boss.OnHealthChanged += UpdateSlider;
    }

    void UpdateSlider(float current, float max)
    {
        healthSlider.value = current;
    }

    private void OnDisable()
    {
        if (boss != null)
        {
            boss.OnHealthChanged -= UpdateSlider;
        }
    }
}