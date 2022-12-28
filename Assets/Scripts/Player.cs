using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour
{
    public Camera sceneCamera;
    public Camera sideCam;
    public Camera topCam;
    public Camera frontCam;
    public int currentCam;

    public Transform CubeClones;
    public GameObject[] Cubes;
    public GameObject specialCube;
    public GameObject[] environmentCubes;

    public Vector3 currentSpawnLocation;
    public Vector3[] lastSpawns;
    public int cubeSpawns = 0;

    public float speed;
    private bool GameStarted;

    public AudioClip CoinGet;
    public AudioClip GameOver;

    public Text uiText;
    public Text buttonText;
    public Text cameraText;
    public Text exitText;

    private void Start()
    {
        SetupGame();
        //Change ButtonText to Start Game
        buttonText.text = ("Start Game");
        //Change Text to Title
        uiText.text = ("Iso Maze");
        //Change ExitText to Exit Game
        exitText.text = ("Exit Game");
    }
    void GenerateLevel()
    {
        System.Array.Clear(lastSpawns,0,lastSpawns.Length);
        System.Array.Clear(environmentCubes,0,environmentCubes.Length);
        //Destroy Previous Level
        environmentCubes = GameObject.FindGameObjectsWithTag("Environment");
        for(var i = 0; i < environmentCubes.Length; i++)
        {
            Destroy(environmentCubes[i]);
        }
        //Generate Level
        while (cubeSpawns != 100)
        {
            currentSpawnLocation = new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
            if (!lastSpawns.Contains(currentSpawnLocation) && currentSpawnLocation != new Vector3(4, -5, 4) && currentSpawnLocation != new Vector3(-5, 4, -5))
            {
                GameObject cubeClone = (GameObject)Instantiate(Cubes[Random.Range(0, Cubes.Length)], transform.position + currentSpawnLocation, Quaternion.identity);
                lastSpawns[cubeSpawns] = currentSpawnLocation;
                cubeClone.transform.parent = CubeClones;
                cubeSpawns = cubeSpawns + 1;

                if (cubeSpawns > lastSpawns.Length)
                {
                    cubeSpawns = 0;
                }
            }
        }
        Instantiate(specialCube, new Vector3(4, -5, 4), Quaternion.identity);
    }

    public void SetupGame()
    {
        //Change Camera Text
        cameraText.text = (" ");
        //Sets game started to false
        GameStarted = false;
        //Disable Player By Scaling it Down
        transform.localScale = new Vector3(0, 0, 0);
        //Disables all cameras
        sideCam.GetComponent<Camera>().enabled = false;
        topCam.GetComponent<Camera>().enabled = false;
        frontCam.GetComponent<Camera>().enabled = false;
        //Enables Scene Camera
        sceneCamera.GetComponent<Camera>().enabled = true;
        GenerateLevel();
    }

    public void StartGame()
    {
        Debug.Log("The Game has Started");
        //Hide Text
        uiText.text = (" ");
        //Hide ButtonText
        buttonText.text = (" ");
        //Hide ExitText
        exitText.text = (" ");
        GameStarted = true;
        //Enable Player By Scaling it Back Up
        transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        //Place Player
        transform.position = new Vector3(-5,4,-5);
        //Disable Scene Camera
        sceneCamera.GetComponent<Camera>().enabled = false;
        //Enable Top Camera
        topCam.GetComponent<Camera>().enabled = true;
        currentCam = 1;
    }

    private void Update()
    {
        if (GameStarted == true)
        {

            //Camera Switching
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                sideCam.GetComponent<Camera>().enabled = false;
                topCam.GetComponent<Camera>().enabled = true;
                frontCam.GetComponent<Camera>().enabled = false;
                currentCam = 1;
                Debug.Log("topCam activated");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                sideCam.GetComponent<Camera>().enabled = true;
                topCam.GetComponent<Camera>().enabled = false;
                frontCam.GetComponent<Camera>().enabled = false;
                currentCam = 2;
                Debug.Log("sideCam activated");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                sideCam.GetComponent<Camera>().enabled = false;
                topCam.GetComponent<Camera>().enabled = false;
                frontCam.GetComponent<Camera>().enabled = true;
                currentCam = 3;
                Debug.Log("frontCam activated");
            }

            //Movement
            if (currentCam == 1)
            {
                //Change Camera Text
                cameraText.text = ("Top View");
                if (Input.GetKey(KeyCode.W))
                    transform.Translate(0, 0, Time.deltaTime * speed);
                if (Input.GetKey(KeyCode.A))
                    transform.Translate(-Time.deltaTime * speed, 0, 0);
                if (Input.GetKey(KeyCode.S))
                    transform.Translate(0, 0, -Time.deltaTime * speed);
                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Time.deltaTime * speed, 0, 0);
            }
            if (currentCam == 2)
            {
                //Change Camera Text
                cameraText.text = ("Side View");
                if (Input.GetKey(KeyCode.W))
                    transform.Translate(0, Time.deltaTime * speed, 0);
                if (Input.GetKey(KeyCode.A))
                    transform.Translate(0, 0, Time.deltaTime * speed);
                if (Input.GetKey(KeyCode.S))
                    transform.Translate(0, -Time.deltaTime * speed, 0);
                if (Input.GetKey(KeyCode.D))
                    transform.Translate(0, 0, -Time.deltaTime * speed);
            }
            if (currentCam == 3)
            {
                //Change Camera Text
                cameraText.text = ("Front View");
                if (Input.GetKey(KeyCode.W))
                    transform.Translate(0, Time.deltaTime * speed, 0);
                if (Input.GetKey(KeyCode.A))
                    transform.Translate(-Time.deltaTime * speed, 0, 0);
                if (Input.GetKey(KeyCode.S))
                    transform.Translate(0, -Time.deltaTime * speed, 0);
                if (Input.GetKey(KeyCode.D))
                    transform.Translate(Time.deltaTime * speed, 0, 0);
            }
        }
    }

    void OnCollisionEnter (Collision col)
    {
        Debug.Log("You are touching something");
        if(col.gameObject.name == "Special Cube(Clone)")
        {
            Debug.Log("You are touching the coin");
            //PlayerWins
            Destroy(col.gameObject);
            AudioSource.PlayClipAtPoint(CoinGet, new Vector3(0,0,0));

            //Change Text to Different Values
            exitText.text = ("Exit Game");
            uiText.text = ("You Win");
            buttonText.text = (" ");

            SetupGame();
        }

        if(col.gameObject.tag == "Environment")
        {
            Debug.Log("You are touching an environment cube");
            //GameOver
            AudioSource.PlayClipAtPoint(GameOver, new Vector3(0, 0, 0));
            Destroy(GameObject.Find("Special Cube(Clone)"));

            //Change Text to Different Values
            exitText.text = ("Exit Game");
            uiText.text = ("Game Over");
            buttonText.text = ("Retry");

            SetupGame();
        }
    }

    public void exitGame()
    {
        Debug.Log("You Quit The Game");
        Application.Quit();
    }
}