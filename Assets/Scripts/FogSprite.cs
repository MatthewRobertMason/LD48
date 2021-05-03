using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FogSprite: MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private LevelManager levelManager;
    public LevelManager LevelMan
    {
        get => levelManager;
        set => levelManager = value;
    }
    
    private Texture2D blackTexture;
    private int textureWidth;
    public int TextureWidth
    {
        get => textureWidth;
        set => textureWidth = value;
    }

    private int textureHeight;
    public int TextureHeight
    {
        get => textureHeight;
        set => textureHeight = value;
    }

    public void Initialize(LevelManager levelMan, int textureWidth, int textureHeight, Material material)
    {
        levelManager = FindObjectOfType<LevelManager>();
        spriteRenderer = FindObjectOfType<SpriteRenderer>();
        spriteRenderer.material = material;
        spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;

        this.textureWidth = levelManager.TextureWidth;
        this.textureHeight = levelManager.TextureHeight;
    }

    public void CreateFogSprite(int layer, Texture2D texture)
    {
        Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, textureWidth, textureHeight), new Vector2(0.0f, 0.0f), 32);
        sprite.name = "FogSprite_" + layer;
        
        spriteRenderer.sprite = sprite;
        spriteRenderer.sortingOrder = 12;

        spriteRenderer.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        
        this.transform.position = new Vector3(-0.5f, ((-levelManager.chunkHeight * (layer + 1)) + 0.5f), 0.0f);
    }


}
