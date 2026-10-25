using UnityEngine;
using UnityEngine.UI;

public class Raycast : MonoBehaviour
{
    public Camera cameraZajac;
    public Camera cameraMapa;

    bool pozeramSaNaNieco = false;
    readonly float totalTime = 1;
    float lookingTimer;
    public Image imgGaze;
    public GameStart gameStartScript;
    public GameObject mapaCamera;
    public GameObject zajacCamera;
    public GameObject teren;
    public GameObject OpenMapText;
    public Canvas canvas;

    void Start()
    {
        mapaCamera.SetActive(false);
    }

    void Update()
    {
        //inicializujem kameru a vystrelim luc
        Camera cam = cameraZajac;
        if (zajacCamera.activeSelf == false) cam = cameraMapa;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;

        //ak sa na nieco pozeram, tak zobrazim nacitavaci kruzok
        imgGaze.fillAmount = 0;
        if (pozeramSaNaNieco)
        {
            lookingTimer += Time.deltaTime;
            imgGaze.fillAmount = lookingTimer / totalTime;
        }

        //nastavim sipky na normalnu velkost a skryjem napis
        GameObject.Find("arrowL").transform.localScale = new Vector3(0.2f, 0.2f, 1);
        GameObject.Find("arrowU").transform.localScale = new Vector3(0.2f, 0.2f, 1);
        GameObject.Find("arrowR").transform.localScale = new Vector3(0.2f, 0.2f, 1);
        GameObject.Find("arrowD").transform.localScale = new Vector3(0.2f, 0.2f, 1);
        OpenMapText.SetActive(false);


        //ak som nieco trafil lucom
        if (Physics.Raycast(ray, out hit))
        {
            //print("I'm looking at " + hit.transform.name);
            pozeramSaNaNieco = true;


            //ak sa pozeram na sipku
            if (hit.transform.name[0] == 'a') hit.transform.localScale = new Vector3(0.3f, 0.3f, 1);
            if (hit.transform.name == "Cylinder") OpenMapText.SetActive(true);


            //ak vyprsal cas
            if (lookingTimer >= totalTime)
            {
                //otvor mapu
                if (hit.transform.name == "Cylinder")
                {
                    mapaCamera.SetActive(true);
                    zajacCamera.SetActive(false);
                    teren.SetActive(false);
                    pozeramSaNaNieco = false;
                    lookingTimer = 0;
                    canvas.worldCamera = cameraMapa;
                    canvas.planeDistance = 50;
                }

                //posun zajaca na novu poziciu
                gameStartScript = GameObject.FindObjectOfType(typeof(GameStart)) as GameStart;
                if (hit.transform.name == "arrowL") gameStartScript.MoveRabbit(0); //0
                if (hit.transform.name == "arrowU") gameStartScript.MoveRabbit(1); //1
                if (hit.transform.name == "arrowR") gameStartScript.MoveRabbit(2); //2
                if (hit.transform.name == "arrowD") gameStartScript.MoveRabbit(3); //3

                //zatvor mapu
                if (hit.transform.name == "button")
                {
                    mapaCamera.SetActive(false);
                    zajacCamera.SetActive(true);
                    teren.SetActive(true);
                    pozeramSaNaNieco = false;
                    lookingTimer = 0;
                    canvas.worldCamera = cameraZajac;
                    canvas.planeDistance = 1;
                }
            }
        }
        else
        {
            //print("I'm looking at nothing!");
            pozeramSaNaNieco = false;
            lookingTimer = 0;
        }
    }
}
