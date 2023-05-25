using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour {

    // Efecto visual al golpear un objetivo
    [SerializeField] private Transform vfxHitGreen;
    // Efecto visual alternativo al golpear un objetivo
    [SerializeField] private Transform vfxHitRed;

    // Referencia al componente Rigidbody del proyectil
    private Rigidbody bulletRigidbody;
    // Velocidad del proyectil
    public float speed = 50f;
    // Tiempo máximo antes de que el proyectil se autodestruya
    public float targetTime = 10f;
    private void Awake() {
        // Obtiene el componente Rigidbody al iniciar el objeto
        bulletRigidbody = GetComponent<Rigidbody>();
    }

    private void Start() {
        // Establece la velocidad inicial del proyectil en la dirección hacia adelante
        bulletRigidbody.velocity = transform.forward * speed;
    }

    private void Update()
    {
        // Actualiza el tiempo objetivo
        targetTime -= Time.deltaTime;
        // Destruye el objeto si el tiempo objetivo ha llegado a cero
        if (targetTime <= 0f) Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {

        //So el collider tiene el script Bullet Target
        if (other.GetComponent<BulletTarget>() != null) {
            // Crea el efecto visual posición de impacto
            Instantiate(vfxHitGreen, transform.position, Quaternion.identity);
            // Informa al objetivo de que ha sido dañado
            other.GetComponent<EnemyManager>().getHit();
        }
        // Destruye el proyectil si colisiona con otro objeto que no sea un activador
        if (!other.isTrigger) Destroy(bulletRigidbody.gameObject);
    }

}