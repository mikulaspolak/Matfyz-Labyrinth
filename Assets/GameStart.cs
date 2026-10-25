using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Runtime.InteropServices.ComVisible(true)]
[System.Serializable]

public class GameStart : MonoBehaviour
{
    public GameObject stenaOriginal;
    public GameObject stenaContainer;
    public Transform rabbit;
    int zajacX, zajacY;
    float pocetSekund = 0;
    bool animujem = false;
    List<List<List<bool>>> stvorce; //pamata si kde si steny
    List<List<bool>> rekurzivneStvorce;
    public GameObject vitaznaHlaska;
    public GameObject casomiera;
    public Text casomieraText;

    // Start is called before the first frame update
    void Start()
    {
        System.Random rnd = new System.Random();
        int narocnost = 40; //%


        //        1
        //      0|_|2
        //        3


        //umiestnim zajaca na pociatocnu poziciu
        zajacX = rnd.Next(0, 23 / 2); //23/2 preto aby nebol blizko pri ciely
        zajacY = rnd.Next(0, 25);
        GameObject.Find("Rabbit").transform.position = new Vector3(59 + zajacX * 6, -1.4f, 54 + zajacY * 6);


        //teraz vytvorim labyrint
        bool labyrintJeDobry = false;
        while (!labyrintJeDobry)
        {
            //vytvorim 2 rozmerne pole stvorcov. Kazdy z nich si bude neskor pamatat ze z ktorej strany je stena.
            stvorce = new List<List<List<bool>>>();
            for (int x = 0; x < 24; x++)
            {
                stvorce.Add(new List<List<bool>>());
                for (int y = 0; y < 26; y++)
                {
                    stvorce[x].Add(new List<bool>());
                    for (int smer = 0; smer < 4; smer++)
                    {
                        stvorce[x][y].Add(false);
                    }
                }
            }


            //pridam steny na kraj
            for (int x = 0; x < 24; x++) stvorce[x][0][3] = true;
            for (int x = 0; x < 24; x++) stvorce[x][25][1] = true;
            for (int y = 0; y < 26; y++) stvorce[0][y][0] = true;
            for (int y = 0; y < 26; y++) stvorce[23][y][2] = true;


            for (int x = 0; x < 24; x++)
                for (int y = 0; y < 26; y++)
                {
                    if ((x != 23) && (rnd.Next(100) < narocnost))
                    {
                        stvorce[x][y][2] = true;
                        stvorce[x + 1][y][0] = true;
                    }

                    if ((y != 25) && (rnd.Next(100) < narocnost))
                    {
                        stvorce[x][y][1] = true;
                        stvorce[x][y + 1][3] = true;
                    }
                }

            rekurzivneStvorce = new List<List<bool>>();
            for (int x = 0; x < 24; x++)
            {
                rekurzivneStvorce.Add(new List<bool>());
                for (int y = 0; y < 26; y++)
                    rekurzivneStvorce[x].Add(false);
            }

            labyrintJeDobry = SkontrolujLabyrint(zajacX, zajacY);
        }


        //teraz vytvorim steny
        for (int x = 0; x < 24; x++)
        {
            for (int y = 0; y < 26; y++)
            {
                if (stvorce[x][y][2] && x != 23)
                {
                    GameObject StenaCloneV = Instantiate(stenaOriginal, new Vector3(x * 6 + 62, 0, y * 6 + 54), stenaOriginal.transform.rotation);
                    StenaCloneV.transform.parent = stenaContainer.transform;
                    StenaCloneV.name = "StenaV" + (x * y + 1);
                }

                if (stvorce[x][y][1] && y != 25)
                {
                    GameObject StenaCloneH = Instantiate(stenaOriginal, new Vector3(x * 6 + 59, 0, y * 6 + 57), stenaOriginal.transform.rotation);
                    StenaCloneH.transform.Rotate(new Vector3(0f, 90f, 0f));
                    StenaCloneH.transform.parent = stenaContainer.transform;
                    StenaCloneH.name = "StenaH" + (x * y + 1);
                }
            }
        }

        vitaznaHlaska.SetActive(false);
        casomiera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pocetSekund += Time.deltaTime;

        if (!animujem)
        {
            GameObject.Find("Camera zajac parent").transform.position = rabbit.position + new Vector3(0, 2, 0);

            GameObject.Find("arrowL").transform.position = rabbit.position + new Vector3(-3, 0.1f, 0);
            GameObject.Find("arrowU").transform.position = rabbit.position + new Vector3(0, 0.1f, +3);
            GameObject.Find("arrowR").transform.position = rabbit.position + new Vector3(+3, 0.1f, 0);
            GameObject.Find("arrowD").transform.position = rabbit.position + new Vector3(0, 0.1f, -3);
            GameObject.Find("Cylinder").transform.position = rabbit.position + new Vector3(0, 0.2f, 0);

            //GameObject.Find("Camera zajac parent").transform.rotation = rabbit.rotation;
            if (stvorce[zajacX][zajacY][0]) GameObject.Find("arrowL").transform.position = rabbit.position + new Vector3(-3, -5, 0);
            if (stvorce[zajacX][zajacY][1]) GameObject.Find("arrowU").transform.position = rabbit.position + new Vector3(0, -5, +3);
            if (stvorce[zajacX][zajacY][2]) GameObject.Find("arrowR").transform.position = rabbit.position + new Vector3(+3, -5, 0);
            if (stvorce[zajacX][zajacY][3]) GameObject.Find("arrowD").transform.position = rabbit.position + new Vector3(0, -5, -3);
        }
        else
        {
            //skryt sipky
            GameObject.Find("arrowL").transform.position = rabbit.position + new Vector3(-3, -5, 0);
            GameObject.Find("arrowU").transform.position = rabbit.position + new Vector3(0, -5, +3);
            GameObject.Find("arrowR").transform.position = rabbit.position + new Vector3(+3, -5, 0);
            GameObject.Find("arrowD").transform.position = rabbit.position + new Vector3(0, -5, -3);
            GameObject.Find("Cylinder").transform.position = rabbit.position + new Vector3(0, -5, 0);
        }
    }

    public void MoveRabbit(int smer)
    {
        if (!stvorce[zajacX][zajacY][smer] && !animujem) //ak tym smerom nie je stena
        {
            GameObject.Find("Rabbit").transform.eulerAngles = new Vector3(0, (smer - 1) * 90, 0);
            StartCoroutine(ChviluUtekaj(smer));
        }
    }

    IEnumerator ChviluUtekaj(int smer)
    {
        GameObject.Find("Rabbit").GetComponent<Animator>().SetInteger("AnimIndex", 1);
        GameObject.Find("Rabbit").GetComponent<Animator>().SetTrigger("Next");
        animujem = true;

        yield return new WaitForSeconds(2.5f); // tu cakam

        GameObject.Find("Rabbit").GetComponent<Animator>().SetInteger("AnimIndex", 0);
        GameObject.Find("Rabbit").GetComponent<Animator>().SetTrigger("Next");
        StartCoroutine(ChviluStoj(smer));
    }

    IEnumerator ChviluStoj(int smer)
    {
        yield return new WaitForSeconds(1f); // tu cakam
        animujem = false;

        if (smer == 0) zajacX -= 1;
        if (smer == 1) zajacY += 1;
        if (smer == 2) zajacX += 1;
        if (smer == 3) zajacY -= 1;
        GameObject.Find("Rabbit").transform.position = new Vector3(59 + zajacX * 6, -1.4f, 54 + zajacY * 6);

        //skontrolujem, ci je uz v ciely
        if (zajacX == 23 && zajacY == 12)
        {
            StartCoroutine(ZobrazVitaznuHlasku());
        }
    }

    IEnumerator ZobrazVitaznuHlasku()
    {
        //zobrazim vitaznu hlasku
        casomiera.SetActive(true);
        int pocetSekund = (int)System.Math.Round(this.pocetSekund);
        casomieraText.text = "Your time:\n" + pocetSekund + " seconds";
        vitaznaHlaska.SetActive(true);

        yield return new WaitForSeconds(5); // tu cakam

        //resetujem hru
        SceneManager.LoadScene(0);
    }

    private bool SkontrolujLabyrint(int x, int y)
    {
        if (x == 23 && y == 12) return true;
        if (rekurzivneStvorce[x][y]) return false;

        rekurzivneStvorce[x][y] = true;

        bool answer = false;

        if (!stvorce[x][y][0]) answer = answer || SkontrolujLabyrint(x - 1, y);
        if (!stvorce[x][y][1]) answer = answer || SkontrolujLabyrint(x, y + 1);
        if (!stvorce[x][y][2]) answer = answer || SkontrolujLabyrint(x + 1, y);
        if (!stvorce[x][y][3]) answer = answer || SkontrolujLabyrint(x, y - 1);

        return answer;
    }
}
