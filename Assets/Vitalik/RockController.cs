using UnityEngine;

public class RockController : MonoBehaviour
{
    public GameObject rock;
    void Start()
    {
        if (rock != null)
        {
            rock.SetActive(false);
        }
    }

    public void EnableRock()
    {
        if (rock != null)
        {
            rock.SetActive(true);
        }
    }

    public void DisableRock()
    {
        if (rock != null)
        {
            rock.SetActive(false);
        }
    }
}
