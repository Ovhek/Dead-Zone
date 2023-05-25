using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider soundSlider;
    public Slider musicSlider;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
        
        // Configurar los valores iniciales de los sliders con los volúmenes actuales
        soundSlider.value = audioManager.GetSoundVolume();
        musicSlider.value = audioManager.GetMusicVolume();
    }

    public void OnSoundVolumeChanged()
    {
        audioManager.SetSoundVolume(soundSlider.value);
    }

    public void OnMusicVolumeChanged()
    {
        audioManager.SetMusicVolume(musicSlider.value);
    }
}
