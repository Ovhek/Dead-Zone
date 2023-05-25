using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;  // Instancia estática del AudioManager para acceder desde otras clases

    private List<AudioSource> soundSources = new List<AudioSource>();  // Lista de fuentes de sonido para los efectos de sonido
    private AudioSource musicSource;  // Fuente de sonido para la música

    private float soundVolume = 1f;  // Volumen de los efectos de sonido
    private float musicVolume = 1f;  // Volumen de la música

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;  // Establecer esta instancia como la instancia activa si no existe una instancia previa
            DontDestroyOnLoad(gameObject);  // Mantener este objeto AudioManager al cargar nuevas escenas
        }
        else
        {
            Destroy(gameObject);
        }

        // Obtener todas las referencias a los componentes AudioSource en los sonidos
        AudioSource[] allSoundSources = FindObjectsOfType<AudioSource>();
        soundSources.AddRange(allSoundSources);

        // Obtener la referencia al componente AudioSource de la música
        musicSource = GameObject.Find("2D_Background").GetComponent<AudioSource>();
    }
    public float GetSoundVolume()
    {
        return soundVolume;
    }

    public void SetSoundVolume(float volume)
    {
        AudioSource[] allSoundSources = FindObjectsOfType<AudioSource>();
        soundSources.Clear();
        soundSources.AddRange(allSoundSources);

        soundVolume = volume;  // Establecer el volumen de los efectos de sonido
        foreach (AudioSource soundSource in soundSources)
        {
            soundSource.volume = soundVolume;  // Actualizar el volumen de cada fuente de sonido de efectos de sonido
        }
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public void SetMusicVolume(float volume)
    {
        // Obtener la referencia al componente AudioSource de la música
        musicSource = GameObject.Find("2D_Background").GetComponent<AudioSource>();
        musicVolume = volume;  // Establecer el volumen de la música
        musicSource.volume = musicVolume;  // Actualizar el volumen de la fuente de sonido de la música
    }
}
