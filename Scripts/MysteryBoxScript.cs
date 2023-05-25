using UnityEngine;
using System.Collections;
using StarterAssets;
using System;
using UnityEngine.UIElements;
using TMPro;
using UnityEditor;

public class MysteryBoxScript : MonoBehaviour
{
    Animator controller;
    Animation animate;
    public bool openBox = false, boxIsOpen, boxIsClosing;

    public GameObject[] guns;
    public TextMeshProUGUI precio;
    public int selectedWeapon = 0;
    public float timer;
    public float weaponTimer;
    public float timerToWait = 4f;
    public int counter, counterCompare;
    public Transform gunPosition;
    public StarterAssetsInputs starterAssetsInputs;
    public bool weaponIsSelected;
    public float minDistanceToBox = 2f;
    public GameObject puntosJugador;
    private TextMeshProUGUI puntos;
    public int puntosParaAbrir = 500;
    public GameObject player;

    public GameObject abrirCaja;

    public GameObject seleccionaArma;

    /// <summary>
    /// Función que se ejecuta al incializar. Asigna los puntos del jugador. El controlador y la animación de subir las armas.
    /// </summary>
    void Start()
    {
        puntos = puntosJugador.GetComponent<TextMeshProUGUI>();
        controller = GetComponentsInChildren<Animator>()[0];
        animate = GetComponentsInChildren<Animation>()[0];
    }


    /// <summary>
    /// Función que se ejecuta una vez por frame
    /// </summary>
    void FixedUpdate()
    {

        // Si se está pulsando el boton de abrir la caja pero la caja está abierta --> cambia el valor de pulsado a falso.
        if (starterAssetsInputs.openbox && boxIsOpen) starterAssetsInputs.openbox = false;

        //Si la distancia del jugado a la caja es la misma que minDistanceBox
        if (Vector3.Distance(transform.position, player.transform.position) <= minDistanceToBox)
        {
            //Si la caja no está abierta activa el canvas
            if (!boxIsOpen) abrirCaja.SetActive(true);

            //Si los puntos del jugador son mayor a los puntos necesarios para abrir la caja
            if (int.Parse(puntos.text) >= puntosParaAbrir)
            {
                precio.color = Color.green;

                //Si se pulsa el botón de abrir la caja y la caja no está abierta
                if (starterAssetsInputs.openbox && !boxIsOpen)
                {
                    //Reduce a la puntación el coste de la caja
                    puntos.text = (int.Parse(puntos.text) - puntosParaAbrir).ToString();
                    //Cambia el valor del pulsamiento para abrir la caja a falso.
                    starterAssetsInputs.openbox = false;
                    //Activa el boleano de abrir la caja.
                    openBox = true;
                    //Desactiva el canvas de abrir la caja
                    abrirCaja.SetActive(false);
                    //Invoca la función a los 5 segundos
                    Invoke("activarSeleccionaArma", 4f);
                }
            }
            else
            {
                //cambia el color del precio del panel abrir caja a rojo.
                precio.color = Color.red;
            }

        }
        else
        {
            //desactiva el panel de abrir caja
            abrirCaja.SetActive(false);
            //cambia el valor del botón de abrir la caja a falso
            starterAssetsInputs.openbox = false;
        }

        //si se abre la caja
        if (openBox)
        {
            openBox = false;
            OpenMysteryBox();
        }

        //Si se están ejecutando las animaciones de abrir la caja o subir las armas
        if ((controller.GetCurrentAnimatorStateInfo(0).IsName("LidOpen")) || animate.IsPlaying("liftAnim"))
        {

            //contador
            timer += Time.deltaTime;
            boxIsOpen = true;

            //si el contador es menor a 4 segundos y el counter es diferente al counter compare
            if (timer < 4f && counter < counterCompare)
            {
                counter++;
            }
            //si el counter y el countercompare son iguales selecciona un arma
            else if (counter == counterCompare)
            {
                counter = 0;
                RandomizeWeapon();
                counterCompare++;
            }
            guns[selectedWeapon].transform.position = gunPosition.transform.position;
        }
        //Si la caja está abierta la cierra y reinicia las variables.
        else if (boxIsOpen)
        {
            CloseLid();
            counter = 0;
            counterCompare = 0;
            timer = 0;
            weaponIsSelected = false;

        }
    }

    /// <summary>
    /// Función que se encarga de animar la apertura de la caja y el movimiento del arma.
    /// </summary>
    public void OpenMysteryBox()
    {
        OpenLid();
        RunGunMovement();
    }

    /// <summary>
    /// Ejecuta la animación de abrir la caja.
    /// </summary>
    void OpenLid()
    {
        controller.Play("LidOpen");
    }

    /// <summary>
    /// Ejecuta la animación de cerrar la caja y activa el canvas de abrir la caja. 
    /// Desactiva el canvas de seleccionar arma.
    /// </summary>
    void CloseLid()
    {
        controller.Play("LidClose");
        boxIsClosing = true;
        seleccionaArma.SetActive(false);
        Invoke("ActiveCanvas", 1f);
    }

    /// <summary>
    /// Ejecuta la animación de subir las armas hacía arriba.
    /// </summary>
    void RunGunMovement()
    {
        animate.Play();
    }

    /// <summary>
    /// Selecciona un arma, deshabilita el gameobject de las demás armas y activa el de la arma seleccionada.
    /// </summary>
    void RandomizeWeapon()
    {
        int rand = UnityEngine.Random.Range(0, guns.Length);
        selectedWeapon = rand;

        for (int i = 0; i < guns.Length; i++)
        {
            if (i != selectedWeapon)
                guns[i].SetActive(false);
        }
        guns[selectedWeapon].SetActive(true);
        guns[selectedWeapon].transform.position = gunPosition.transform.position;
        Invoke("MarkWeaponAsSelected", timerToWait);

    }

    /// <summary>
    /// Marca el arma como seleccionada.
    /// </summary>
    public void MarkWeaponAsSelected()
    {
        weaponIsSelected = true;
    }

    /// <summary>
    /// Activa el panel de abrir la caja.
    /// </summary>
    public void ActiveCanvas()
    {
        abrirCaja.SetActive(true);
        boxIsOpen = false;
        boxIsClosing = false;

    }

    /// <summary>
    /// Activa el panel de seleccionar el arma.
    /// </summary>
    public void activarSeleccionaArma()
    {
        seleccionaArma.SetActive(true);
    }
}
