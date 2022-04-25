using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estados de los soldados enemigos
/// </summary>
enum estadosSoldado { guardia, ataque, morir}

/// <summary>
/// Clase para el comportamiento del soldado enemigo
/// </summary>
public class Soldado : Enemigo {

    //Atributos del soldado
    public float velocidad;
    public int fuerza;

    //Para la deteccion del jugador
    public Transform objetivo;
    public float rangoVision;
    public float rangoFOV;
    private Vector3 JugadorDesdeIA;
    float distanciaAJugador = 0;
    float angulo = 0;
    
    //Para la maquina de estados
    int estado;

    //para el tiempo entre ataques
    float tiempoAtaque;

    // Para controlar la deteccion de obstaculos
    public float distanciaDeteccion;

    // Para lanzar rayos de deteccion de obstaculos
    private RaycastHit hitD;
    private RaycastHit hitI;

    //Vectores para la deteccion de obstaculos
    private Vector3 ladoDerecho;
    private Vector3 ladoIzquierdo;
    private Vector3 Objetivo;

    public float escala;

    //Game objects para los texto
    public TextMesh vida;

    public TextMesh ataque;

    //Para el ataque combo nuevo
    int escudoE;
    int ataqueComb = 0;

    // Use this for initialization
    void Start()
    {
        //Inicializamos sus valores
        estado = (int)estadosSoldado.guardia;
        objetivo = FindObjectOfType<Capitanes>().transform;

        tiempoAtaque = 0;
        salud = Random.Range(50, 100);
        escudo = Random.Range(1, 3);
        fuerza = Random.Range(8, 13);
        velocidad = Random.Range(1, 3);

        distanciaDeteccion = 4f;
        escala = 1f;

        rangoVision = 20;
        rangoFOV = 90;

        vida = gameObject.GetComponentsInChildren<TextMesh>()[0];
        ataque= gameObject.GetComponentsInChildren<TextMesh>()[1];

        ataque.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ValidarSalud();

        if (estado == (int)estadosCapitanes.morir)
        {
            Morir();
        }

        if (estado==(int)estadosSoldado.guardia)
        {
            Guardia();
        }

        if (estado == (int)estadosSoldado.ataque)
        {
            Atacar();   
        }
    }

    /// <summary>
    /// Metodo para destruir al enemigo cuando muere
    /// </summary>
    public void Morir()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Metodo para validar la salud del enemigo
    /// </summary>
    public void ValidarSalud()
    {
        //Colocamos el texto y color
        vida.text = salud.ToString();

        if (salud < 50)
            vida.color = Color.yellow;

        if (salud < 10)
            vida.color = Color.red;

        if (salud <= 0)
            estado = (int)estadosCapitanes.morir;
    }

    /// <summary>
    /// Metodo para atacar al jugador
    /// </summary>
    void Atacar()
    {
        //Obtenemos la distancia al jugador
        distanciaAJugador = Vector3.SqrMagnitude(transform.position - objetivo.position);
        
        //Si la distancia es menor a 12f realizamos el ataque
        if (distanciaAJugador < 12f)
        {
            //Si el tiempo de ataque es mayor a la fuerza
            if (tiempoAtaque >= fuerza)
            {
                ataqueComb = 0;

                //activamos el text de ataque
                ataque.gameObject.SetActive(true);
                ataque.characterSize = 0;

                //Obtenemos los datos
                escudoE = objetivo.gameObject.GetComponent<Capitanes>().escudo;
                ataqueComb = escudoE - fuerza;

                //Colocamos la salud de acuerdo al ataque recibido
                if (ataqueComb < 0)
                    objetivo.gameObject.GetComponent<Capitanes>().salud += ataqueComb;

                ataque.text = ataqueComb.ToString();

                tiempoAtaque = 0;
            }
            tiempoAtaque += 1 * Time.deltaTime;

            //Animacion para el texto
            if (ataque.characterSize >= 0 && ataqueComb<0)
            {
                if (tiempoAtaque > 2f)
                    ataque.characterSize -= Time.deltaTime * 0.5f;
                else
                    ataque.characterSize += Time.deltaTime * 0.5f;
            }
        }
        else
            estado = (int)estadosSoldado.guardia;
    }

    /// <summary>
    /// Metodo para la guardia del enemigo
    /// </summary>
    void Guardia()
    {
        ataque.gameObject.SetActive(false);

        bool visto = false;

        // Calculamos la distancia cuadrada
        distanciaAJugador = Vector3.SqrMagnitude(transform.position - objetivo.position);

        // Verificamos si esta en el rango de vision
        if (distanciaAJugador <= (rangoVision * rangoVision))
        {
            // Vector de la IA al personaje
            JugadorDesdeIA = objetivo.position - transform.position;

            // Calculamos el angulo
            angulo = Vector3.Angle(transform.forward, JugadorDesdeIA);

            // Verificamos si esta en el angulo de vision
            if (angulo <= rangoFOV)
            {
                visto = true;
            }
        }

        if (visto)
        {
            transform.LookAt(objetivo);

            // Nos movemos hacia el objetivo
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

            Esquivar();
        }

        //Si la distancia es menor a 2f cambiamos el estado
        if (distanciaAJugador <= 2f)
        {
            estado = (int)estadosSoldado.ataque;
        }
    }

    /// <summary>
    /// Metodo para esquivar obstaculos
    /// </summary>
    void Esquivar()
    {
        Objetivo = objetivo.transform.position;

        //Hacemos la deteccion de obstaculos
        ladoDerecho = transform.forward + (transform.right * 0.7f);
        ladoIzquierdo = transform.forward - (transform.right * 0.7f);

        if (Physics.Raycast(transform.position, ladoDerecho, out hitD, distanciaDeteccion))
        {
            if (hitD.transform.gameObject.CompareTag("Obstaculo"))
            {
                Objetivo -= transform.right * (distanciaDeteccion / hitD.distance);
            }
        }

        if (Physics.Raycast(transform.position, ladoIzquierdo, out hitI, distanciaDeteccion))
        {
            if (hitI.transform.gameObject.CompareTag("Obstaculo"))
            {
                Objetivo += transform.right * (distanciaDeteccion / hitI.distance);
            }
        }

        transform.LookAt(Objetivo);
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }
}
