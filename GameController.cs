using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que define el comportamiento para controlar el juego
/// </summary>
public class GameController : MonoBehaviour {

	//Referencia al movimiento del jugador
	public Capitanes movGraf;
	public Transform jugador;
	public GameObject[] fortalezas;

	//Referencia al movimiento de la camara
	public Camera cam;
	public float x;
	public float y;
	public float z;

	//Para controlar la camara
	public bool libre;

	//UI
	public GameObject UI;
	public Text final;

	// Use this for initialization
	void Start()
	{
		libre = false;
		y = cam.transform.position.y;
		x = cam.transform.position.x;
		z = cam.transform.position.z;

		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 60;
	}

	// Update is called once per frame
	void Update()
	{
		ControlCamara();
		ControlJugador();
		ControlJuego();
	}

	/// <summary>
	/// Metodo para controlar al jugador
	/// </summary>
	void ControlJugador()
    {
		//Cuando presionamos clic en el mouse
		if (Input.GetMouseButtonDown(0))
		{
			//Lanzamos un rayo
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hit, 100.0f))
			{
				//Si el objeto con el que choca es un checkpoint movemos al jugador
				if (hit.transform != null && hit.transform.gameObject.tag == "CheckPoint")
				{
					movGraf.estado = (int)estadosCapitanes.mover;
					movGraf.NuevoPunto(int.Parse(hit.transform.gameObject.name));
				}
			}
		}
	}

	/// <summary>
	/// Metodo para controlar la camara
	/// </summary>
	void ControlCamara()
    {
		//Si se presiona la tecla 1 se cambia el tipo de camara 
		if (Input.GetKeyDown("1"))
			libre = !libre;

		//Si el valor de y esta entre 50 y 10 se va restando el valor del scroll
		if (y < 50 && y > 10)
		{
			y -= Input.mouseScrollDelta.y;
		}
		else if (y >= 50)
			y = 49;
		else
			y = 11;
		//Valores de seguridad

		//Si la camara es libre se puede mover con WASD
		if (libre)
		{
			x = cam.transform.position.x;
			z = cam.transform.position.z;

			if (Input.GetKey(KeyCode.W))
				z++;

			if (Input.GetKey(KeyCode.S))
				z--;

			if (Input.GetKey(KeyCode.D))
				x++;

			if (Input.GetKey(KeyCode.A))
				x--;

			cam.transform.position = new Vector3(x, y, z);
		}
		else
		{
			//Seguimos al jugador
			cam.transform.position = new Vector3(jugador.position.x, y, jugador.position.z);
		}
	}

	/// <summary>
	/// Metodo para el control del flujo del juego
	/// </summary>
	void ControlJuego()
    {
		if (Input.GetKey(KeyCode.Escape))
			Application.Quit();

		bool terminado=true;
		bool ganado=true;

		foreach(GameObject fortaleza in fortalezas)
        {
			if (fortaleza.activeInHierarchy)
            {
				ganado = false;
				terminado = false;
			}
				
        }

		if (movGraf.salud <= 0)
        {
			terminado = true;
			ganado = false;
		}

		if (terminado)
        {
			libre = true;
			UI.SetActive(true);
			final.text = "Game Over";
			final.color = Color.red;

			if(ganado)
            {
				final.text = "You Win";
				final.color = Color.green;
            }

			Time.timeScale = 0;
		}
			
    }
}
