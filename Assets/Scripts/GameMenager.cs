using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using System.Text;
using System;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts;

public class GameMenager : MonoBehaviour
{
    public static GameMenager Instance { set; get; }
    public GameObject mainMenu;
    public GameObject TextW;
    public GameObject TextB;
    public Client client;
    public CheckersBoard boards;
    public GameObject clientPrefab;

    void Start()
    {
        Instance = this;
        this.client = Instantiate(clientPrefab).GetComponent<Client>();
        TextB.SetActive(false);
        TextW.SetActive(false);
        DontDestroyOnLoad(gameObject);
    }
    public void ConnectButton()
    {
        mainMenu.SetActive(false);
        string hostAddress = "192.168.1.46";
        try
        {
            var isok = this.client.ConnectToServer(hostAddress, 1234);
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void TryMove(char[] aData)
    {

        var boards = FindObjectOfType<CheckersBoard>();
        boards.TryMove(int.Parse(aData[1].ToString()), int.Parse(aData[2].ToString()), int.Parse(aData[3].ToString()), int.Parse(aData[4].ToString()), true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
}
