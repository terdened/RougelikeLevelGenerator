using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DurabilityController : MonoBehaviour
{
    public float Durability = 100f;
    public List<Sprite> Sprites;

    private SpriteRenderer _spriteRenderer;
    private float _originalDurability;
    private float _spriteDurability;

    void OnEnable()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalDurability = Durability;

        if (Sprites == null || Sprites.Count == 0)
            return;

        _spriteDurability = _originalDurability / Sprites.Count;
        _spriteRenderer.sprite = Sprites[Sprites.Count - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (Durability <= 0f)
            Destroy(gameObject);

        HandleSprites();
    }

    private void HandleSprites()
    {
        if (Sprites == null || Sprites.Count == 0)
            return;


        if (Math.Abs(Durability - Sprites.Count * _spriteDurability) < 0.01f)
        {
            return;
        }

        int currentSprite = (int)(Durability / _spriteDurability);
        _spriteRenderer.sprite = Sprites[currentSprite];
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
