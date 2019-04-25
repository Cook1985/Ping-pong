using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using System;
using Photon.Realtime;

public class Manager : MonoBehaviourPunCallbacks
{
	public Camera cameraLocal;	
	public bool isStart = false;
	public bool isMoveBall = false;
	public bool isMoveRacket = false;
	private double worldScreenHeight;
	private double worldScreenWidth;
	public static Manager instance;
    private GameObject Background;
    private GameObject WallRight;
    private GameObject WallLeft;
    private GameObject WallUp;
    private GameObject WallDown;
    private GameObject playerFirst;
    private GameObject playerSecond;
    private GameObject ball;
    public PhotonView photonView;
    public float widthMin;
    public float heightMin;
    public float widthMax;
    public float heightMax;
    public GameObject WindowWaiting;
    private Vector3 pos;
    private Vector3 posBall;


    void Start () {
        WindowWaiting.SetActive(true);
        photonView = GetComponent<PhotonView>();
	}

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            heightMin = (float)Screen.height;
            widthMin = (float)Screen.width;
            worldScreenHeight = Camera.main.orthographicSize * 2.0;
            worldScreenWidth = worldScreenHeight / heightMin * widthMin;
            addPlayerFirst();
        }
        else
        {
            worldScreenHeight = Camera.main.orthographicSize * 2.0;
            worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
            photonView.RPC("addGameElements", RpcTarget.MasterClient, (float)Screen.width, (float)Screen.height, (float)worldScreenHeight, (float)worldScreenWidth);
            WindowWaiting.SetActive(false);
            addPlayerSecond();
        }
    }

    private void addPlayerSecond()
    {
        playerSecond = PhotonNetwork.Instantiate("Racket", new Vector3(0f, 0f, -1f), Quaternion.identity);
        playerSecond.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, (float)Screen.height - ((float)Screen.height / 20f), 1f));
    }

    private void addPlayerFirst()
    {
        playerFirst =  PhotonNetwork.Instantiate("Racket", new Vector3(0f, 0f, -1f), Quaternion.identity);
        playerFirst.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, (float)Screen.height / 20f, 1f));
        playerFirst.gameObject.GetComponent<PlayerController>().ID = 1;
    }

    private void addElements()
    {
        Background = PhotonNetwork.Instantiate("Background", Vector3.zero, Quaternion.identity);
        WallRight = PhotonNetwork.Instantiate("Wall", pos, Quaternion.identity);
        WallLeft = PhotonNetwork.Instantiate("Wall", pos, Quaternion.identity);
        WallUp = PhotonNetwork.Instantiate("Wall", pos, Quaternion.identity);
        WallUp.name = "WallUp";
        WallDown = PhotonNetwork.Instantiate("Wall", pos, Quaternion.identity);
        WallDown.name = "WallDown";
        ball = PhotonNetwork.Instantiate("Ball", pos, Quaternion.identity);

        WallRight.transform.localScale = new Vector3(0.5f, 1f * (float)worldScreenHeight, 1f);
        WallLeft.transform.localScale = new Vector3(0.5f, 1f * (float)worldScreenHeight, 1f);
        WallUp.transform.localScale = new Vector3(1f * (float)worldScreenWidth, 0.5f, 1f);
        WallDown.transform.localScale = new Vector3(1f * (float)worldScreenWidth, 0.5f, 1f);
        Background.transform.localScale = new Vector3(1f * (float)worldScreenWidth, 1f * (float)worldScreenHeight, 0f);

        
    }

    public void addBall()
    {
        ball = PhotonNetwork.Instantiate("Ball", pos, Quaternion.identity);
        ball.transform.localPosition = cameraLocal.ScreenToWorldPoint(posBall);
    }

    [PunRPC]
    void addGameElementsNoMaster() 
    {
        WindowWaiting.SetActive(false);
        addElements();

        WallRight.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(Screen.width, (float)Screen.height / 2f, 1f));
        WallLeft.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(0f, (float)Screen.height / 2f, 1f));
        WallUp.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, (float)Screen.height, 1f));
        WallDown.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3((float)Screen.width / 2f, 0, 1f));
        posBall = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 1f);
        ball.transform.localPosition = cameraLocal.ScreenToWorldPoint(posBall);
    }


    [PunRPC]
    void addGameElements(float secondWidth, float secondHeight, float worldScreenHeightSecond, float worldScreenWidthSecond)
    {
        WindowWaiting.SetActive(false);
        if ((float)worldScreenHeight >= worldScreenHeightSecond)
            worldScreenHeight = worldScreenHeightSecond;
        if ((float)worldScreenWidth >= worldScreenWidthSecond)
            worldScreenWidth = worldScreenWidthSecond;
        
        if (heightMin >= secondHeight && widthMin >= secondWidth)
        {
            addElements();
            //Debug.Log("heightMin = " + heightMin + " heightMax = " + heightMax + " widthMin = " + widthMin + " widthMax = " + widthMax);

            //Debug.Log("heightMin >= secondHeight && widthMin >= secondWidth");
            heightMax = heightMin;
            heightMin = secondHeight;
            widthMax = widthMin;
            widthMin = secondWidth;

            WallRight.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(Screen.width, heightMin / 2f + Math.Abs((heightMax - heightMin) / 2f), 1f));
            WallLeft.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(0f, heightMin / 2f + Math.Abs((heightMax - heightMin) / 2f), 1f));
            WallUp.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(widthMax / 2f, heightMin + Math.Abs(heightMax - heightMin), 1f));
            WallDown.transform.localPosition = cameraLocal.ScreenToWorldPoint(new Vector3(widthMax / 2f, 0, 1f));
            posBall = new Vector3(widthMax / 2f, heightMin / 2f + Math.Abs((heightMax - heightMin) / 2f), 1f);
            ball.transform.localPosition = cameraLocal.ScreenToWorldPoint(posBall);
        }
        else if (heightMin < secondHeight && widthMin < secondWidth)
        {
            photonView.RPC("addGameElementsNoMaster", RpcTarget.Others);
            heightMax = secondHeight;
            widthMax = secondWidth;
            //Debug.Log("heightMin < secondHeight && widthMin < secondWidth");
            //float x = widthMin;
        }

        Debug.Log("worldScreenWidth = " + worldScreenWidth.ToString() + ", worldScreenWidthSecond = " + worldScreenWidthSecond);
        Debug.Log(secondWidth.ToString());
    }

    void Awake()
	{
		if (instance == null) 
			instance = this;
        pos = new Vector3(0f, 0f, 1f); 

    }


	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0) && !isStart)
			isMoveBall = true;

		if (Input.GetMouseButtonDown (0))
			isMoveRacket = true;

		if (Input.GetMouseButtonUp (0) && !isStart && isMoveRacket) {
			isStart = true;
			isMoveBall = false;
		}

	}

    public override void OnLeftRoom()
    {
        photonView.RPC("RestartGame", RpcTarget.All);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        photonView.RPC("RestartGame", RpcTarget.All);
    }

    private void OnApplicationQuit()
    {
        
    }

    [PunRPC]
    void RestartGame()
	{
        Debug.Log("RestertGame");
        PhotonNetwork.LoadLevel(0);
        SceneManager.LoadScene ("Game");
	}

}
