using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///Estados de los aliados
enum estadosAliados { perseguir, morir, atacar, seguir}

/// <summary>
/// Clase para el comportamiento de los personajes aliados
/// </summary>
public class Personaje : MonoBehaviour
{
    //Para el comportamiento de flock
    public Vector3 posicion;
    public Vector3 direccion;
    public Vector3 combinado;

    //Referencia al jugador
    public Transform Jugador;

    private float maxDireccion = 5;
    private float maxCombinado = 25;

    private float factorCohesion = 60;
    private float factorSeparacion = 90;
    private float factorAlineacion = 90;

    private float rangoCohesion = 20;
    private float rangoSeparacion = 6;
    private float rangoAlineacion = 10;

    private Personaje[] agentes;

    //Atributos
    private float velocidad = 5f;
    float tiempoAtaque;
    public int fuerza;

    //Para el ataque combo nuevo
    int escudoE;
    int ataqueComb = 0;

    // Para la maquina de estados
    public int estado;

    //Para perseguir al enemigo
    public Transform enemigo;
    private Vector3 Objetivo;
    
    //Game object de textos
    public TextMesh ataque;

    void Start()
    {
        //Inicializamos los valores
        tiempoAtaque = 0;
        estado = (int)estadosAliados.seguir;
        fuerza = Random.Range(2,8);

        // Obtenemos los agentes que hay
        agentes = FindObjectsOfType<Personaje>();

        // Colocamos la posicion inicial
        posicion = transform.position;

        // Colocamos una direccion aleatoria
        direccion = new Vector3(Random.Range(-5, 5), 0, Random.Range(-5, 5));

        ataque = gameObject.GetComponentsInChildren<TextMesh>()[0];

        ataque.gameObject.SetActive(false);
    }

    void Update()
    {
        if (estado == (int)estadosAliados.seguir)
        {
            ataque.gameObject.SetActive(false);

            //Obtenemos la distancia al jugador
            Vector3 distJug = Jugador.position - transform.position;

            //Combinamos los factores de flock
            combinado = factorCohesion * Cohesion() + factorSeparacion * Separacion() + factorAlineacion * Alineacion() + distJug;
            combinado = Vector3.ClampMagnitude(combinado, maxCombinado);

            // Calculamos la direccion
            direccion = direccion + combinado * Time.deltaTime;
            direccion = Vector3.ClampMagnitude(direccion, maxDireccion);

            // Actualizamos la posicion
            posicion = posicion + direccion * Time.deltaTime;

            transform.position = posicion;

            if (direccion.magnitude > 0)
                transform.LookAt(posicion + direccion);
        }
        if (estado == (int)estadosAliados.perseguir)
        {
            Perseguir();
        }
        if (estado == (int)estadosAliados.atacar)
        {
            Atacar();
        }
    }


    private Vector3 Cohesion()
    {
        Vector3 posPromCohesion = new Vector3();
        Vector3 vectorCohesion = new Vector3();

        // Obtenemos a los vecinos
        List<Personaje> vecinos = ObtieneVecinos(rangoCohesion);

        // Si no hay vecinos no necesitamos procesar
        if (vecinos.Count == 0)
            return posPromCohesion;

        // Obtenemos la posicion promedio de los vecinos
        foreach (Personaje vecino in vecinos)
        {
            posPromCohesion += vecino.posicion;
        }

        posPromCohesion /= vecinos.Count;

        // Obtenemos el vector de nuestra posicion hacia la posicion promedio
        vectorCohesion = posPromCohesion - this.posicion;

        vectorCohesion.Normalize();

        return vectorCohesion;
    }

    private Vector3 Separacion()
    {
        Vector3 vectorSeparacion = new Vector3();

        // Obtenemos los vecinos
        List<Personaje> vecinos = ObtieneVecinos(rangoSeparacion);

        // Si no hay vecinos no necesitamos procesar
        if (vecinos.Count == 0)
            return vectorSeparacion;

        // Calculamos el vector total de las fuerzas de separacion
        foreach (Personaje vecino in vecinos)
        {
            Vector3 vecinoAgente = this.posicion - vecino.posicion;

            // La fuerza es inversamente proporcional a la distancia
            vectorSeparacion += vecinoAgente.normalized / vecinoAgente.magnitude;
        }

        vectorSeparacion.Normalize();

        return vectorSeparacion;
    }

    private Vector3 Alineacion()
    {
        Vector3 vectorAlineacion = new Vector3();

        // Obtenemos los vecinos
        List<Personaje> vecinos = ObtieneVecinos(rangoAlineacion);

        // Si no hay vecinos, no procesamos
        if (vecinos.Count == 0)
            return vectorAlineacion;

        // Encontramos el vector resultante de la suma de todos
        foreach (Personaje vecino in vecinos)
        {
            vectorAlineacion += vecino.direccion;
        }

        vectorAlineacion.Normalize();

        return vectorAlineacion;
    }

    // Obtenemos a los vecinos que estan en nuestro rango de vision
    private List<Personaje> ObtieneVecinos(float rango)
    {
        List<Personaje> vecinos = new List<Personaje>();
        float d = 0;

        foreach (Personaje a in agentes)
        {
            // Obtenemos la distancia
            d = Vector3.Magnitude(transform.position - a.transform.position);

            // Verificamos que no sea uno mismo
            if (d == 0)
                continue;

            // Verificamos que estamos dentro del rango de vision
            if (d <= rango)
            {

                vecinos.Add(a);

            }
        }

        //Debug.Log(vecinos.Count);

        return vecinos;
    }

    /// <summary>
    /// Metodo para cuando el enemigo entra en el collider
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        //Si estamos siguiendo al jugador
        if (estado == (int)estadosAliados.seguir)
        {
            //Si es un enemigo lo referenciamos y cambiamos de estado
            if (other.gameObject.CompareTag("Enemigo"))
            {
                enemigo = other.gameObject.transform;
                estado = (int)estadosAliados.perseguir;
            }
        }

    }

    /// <summary>
    /// Metodo para perseguir al enemigo
    /// </summary>
    void Perseguir()
    {
        //Si el enemigo no es null
        if (enemigo != null)
        {
            //Indicamos que el objetivo es el enemigo y lo seguimos
            Objetivo = enemigo.transform.position;
            transform.LookAt(Objetivo);
            transform.Translate(Vector3.forward * velocidad * Time.deltaTime);

            //obtenemos la distancia al enemigo
            Vector3 distancia = enemigo.position - transform.position;

            //Si es mayor a 22 lo eliminamos y cambiamos de estado
            if (distancia.magnitude > 22)
            {
                enemigo = null;
                posicion = transform.position;
                estado = (int)estadosAliados.seguir;
            }
            else if (distancia.magnitude < 1.5f)
                estado = (int)estadosAliados.atacar; //Si es menor a 1.5 cambiamos a ataque
        }
        else
        {
            posicion = transform.position;
            estado = (int)estadosAliados.seguir;
           
        }
            

    }


    /// <summary>
    /// Metodo para atacar al enemigo
    /// </summary>
    void Atacar()
    {
        //Si el enemigo no es null
        if (enemigo != null)
        {
            //Obtenemos la distancia al enemigo
            float distanciaAEnemigo = Vector3.SqrMagnitude(transform.position - enemigo.position);

            //Si esta cerca atacamos
            if (distanciaAEnemigo < 12f)
            {
                //Si el tiempo de ataque es mayor a la fuerza
                if (tiempoAtaque >= fuerza)
                {
                    ataqueComb = 0;

                    //activamos el text de ataque
                    ataque.gameObject.SetActive(true);
                    ataque.characterSize = 0;

                    //Realizamos el calculo del ataque
                    escudoE = enemigo.gameObject.GetComponent<Enemigo>().escudo;
                    ataqueComb = escudoE - fuerza;

                    if (ataqueComb < 0)
                        enemigo.gameObject.GetComponent<Enemigo>().salud += ataqueComb;

                    ataque.text = ataqueComb.ToString();

                    tiempoAtaque = 0;
                }

                //Aumentamos el tiempo de ataque
                tiempoAtaque += 1 * Time.deltaTime;

                //Animacion para el texto
                if (ataque.characterSize >= 0 && ataqueComb < 0)
                {
                    if (tiempoAtaque > 2f)
                        ataque.characterSize -= Time.deltaTime * 0.5f;
                    else
                        ataque.characterSize += Time.deltaTime * 0.5f;
                }
            }
            else
            {
                posicion = transform.position;
                estado = (int)estadosAliados.seguir;
            }

            if (!enemigo.gameObject.activeInHierarchy)
                enemigo = null;
        }
        else
        {
            posicion = transform.position;
            estado = (int)estadosAliados.seguir;
        }
    }
}
