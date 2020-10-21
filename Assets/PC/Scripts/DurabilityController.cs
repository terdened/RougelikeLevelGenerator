using UnityEngine;

public class DurabilityController : MonoBehaviour
{
    public float Durability = 100f;

    // Update is called once per frame
    void Update()
    {
        if (Durability <= 0f)
            Destroy(gameObject);
    }

    public void GatherDamage(float damageValue)
    {
        Durability -= damageValue;
    }

    public void Repair(float repairValue)
    {
        Durability += repairValue;
    }
}
