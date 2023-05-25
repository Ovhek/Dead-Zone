using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

/// <summary>
/// Enum que hace referencia a la dificultad del juego.
/// </summary>
public enum Dificultad
{
    Facil,
    Normal,
    Dificil
}

/// <summary>
/// Clase Singleton encargada de manejar la dificultad del juego
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    //Instancia de la clase
    public static DifficultyManager instance;

    //Enum que representa la dificultad actual
    public Dificultad dificultadActual = Dificultad.Facil;

    //Variables que se modifican según la dificultad:
    public int vidaZombies;
    public int obtencionPuntos;
    public int danoZombies;
    public int enemigosPorRonda;

    /// <summary>
    /// Clase que instancia el Singletom. Si existe lo destruye. Si no lo instancia.
    /// </summary>
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Clase encargada de modificar la dificultad del nivel. Cambiado así el valor de las variables.
    /// </summary>
    /// <param name="dificultad"></param>
    public void SetDifficulty(Dificultad dificultad)
    {
        dificultadActual = dificultad;

        // Adjust variables based on difficulty
        switch (dificultadActual)
        {
            case Dificultad.Facil:
                    vidaZombies = 80;
                    obtencionPuntos = 300;
                    danoZombies = 20;
                    enemigosPorRonda = 5;
                break;
            case Dificultad.Normal:
                    vidaZombies = 100;
                    obtencionPuntos = 225;
                    danoZombies = 30;
                    enemigosPorRonda = 7;
                break;
            case Dificultad.Dificil:
                    vidaZombies = 150;
                    obtencionPuntos = 150;
                    danoZombies = 40;
                    enemigosPorRonda = 10;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Método que devuelve la representación en cadena de una dificultad.
    /// </summary>
    /// <param name="dificultad">La dificultad seleccionada</param>
    /// <returns>La representación en cadena de la dificultad</returns>
    public string toString(Dificultad dificultad)
    {
        return dificultad switch
        {
            Dificultad.Facil => "Fácil",
            Dificultad.Normal => "Normal",
            Dificultad.Dificil => "Díficil",
            _ => "No válido"
        };
    }
}
