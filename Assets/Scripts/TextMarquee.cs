using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMarquee : MonoBehaviour
{
    public RectTransform MaskContainer;
    public Text text;

    public float speed;
    public float percentDistance = 0.0f;
    public float distanceRequired;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        audioManager.SongName = this;
        audioManager.SetSongName(audioManager.SourceAudio.clip);
        SetUpText(text.text);
    }

    public void SetUpText(string value)
    {
        text.text = value;

        float x = MaskContainer.rect.width;
        float y = text.rectTransform.anchoredPosition.y;

        text.rectTransform.anchoredPosition = new Vector2(x, y);
    }

    // Update is called once per frame
    void Update()
    {
        float moveDist = speed * Time.deltaTime;
        
        float x = text.rectTransform.anchoredPosition.x - moveDist;
        float y = text.rectTransform.anchoredPosition.y;
        
        if (text.rectTransform.anchoredPosition.x <= -text.preferredWidth)
        {
            x = MaskContainer.rect.width;
        }

        text.rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
