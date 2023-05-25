using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    public enum Tipos
    {
        Pistola,
        Rifle
    };

    public string nombreArma;
    public float ArmaDMG;
    public float gunHeat;
    public float tiempoEntreDisparos;
    public Tipos tipoArma;

    // TODO: Añadir sonido disparo 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
