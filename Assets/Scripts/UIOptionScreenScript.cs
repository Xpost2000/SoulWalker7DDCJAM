using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class UIOptionScreenScript : MonoBehaviour
{
    // Start is called before the first frame update
    public Button quit_button;
    public Toggle animated_toggle;
    public Slider volume_slider;
    void Start() {
        quit_button.onClick.AddListener(OnQuit);
        animated_toggle.onValueChanged.AddListener(OnAnimatedValueChange);
        volume_slider.onValueChanged.AddListener(OnVolumeValueChange);

        animated_toggle.isOn = GameManagerScript.instance().interpolate_animations;
    }

    void OnQuit() {
        GameManagerScript.instance().State = GameManagerScript.instance().LastState;
    }

    void OnAnimatedValueChange(bool value) {
        GameManagerScript.instance().interpolate_animations = value;
    }

    void OnVolumeValueChange(float value) {
        AudioListener.volume = value;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
