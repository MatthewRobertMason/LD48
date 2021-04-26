using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureGauge : MonoBehaviour
{
    public Sprite[] sprites;
    private SpriteRenderer sprite_renderer;
    private float timeLeft = 0;
    private float countStart = 0;
    private float maxTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        sprite_renderer = GetComponent<SpriteRenderer>();
    }

    public void SetMaxTime(float max){
        maxTime = max;
    }

    public void SetTime(float left){
        timeLeft = left;
        countStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        float realTimeLeft = timeLeft - (Time.time - countStart);
        if(realTimeLeft <= 0){
            sprite_renderer.sprite = sprites[sprites.Length-1];
        } else {
            float ratio = realTimeLeft/maxTime;
            int index = (int)Mathf.Round(ratio*sprites.Length);
            index = Mathf.Clamp(index, 0, sprites.Length-1);
            sprite_renderer.sprite = sprites[sprites.Length - index - 1];
        }
    }
}
