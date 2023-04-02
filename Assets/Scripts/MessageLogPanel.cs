using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class MessageLogPanel : MonoBehaviour
{
    public TMP_FontAsset font;
    public int font_size;

    private int MessageCount() {
        return transform.childCount;
    }

    private GameObject make_new_text(string valuetxt, Color color) {
        int y = (MessageCount()+1) * (font_size);
        GameObject res = new GameObject("message" + MessageCount().ToString());
        res.transform.SetParent(gameObject.transform);
        res.transform.localScale = new Vector3(1,1,1);

        LogMessage msg_data = res.AddComponent<LogMessage>();
        TextMeshProUGUI text_data = res.AddComponent<TextMeshProUGUI>();
        text_data.fontSize = font_size;

        RectTransform rect_transform = text_data.rectTransform;
        // rect_transform.anchorMin = new Vector2(0.0f, 0.0f);
        // rect_transform.anchorMax = new Vector2(0.0f, 0.0f);
        rect_transform.pivot = new Vector2(0.0f, 0.0f);
        rect_transform.anchoredPosition = new Vector2(0.0f, 0.0f);

        msg_data.text_data = text_data;
        text_data.color = color;
        text_data.text = valuetxt;
        text_data.font = font;

        return res;
    }

    public void Clear() {
         foreach (Transform child in transform) {
             GameObject.Destroy(child.gameObject);
         }
    }

    public void NewMessage(string txt, Color color) {
        make_new_text(txt, color);
    }

    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
