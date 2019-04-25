using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball :  MonoBehaviourPun
{

	public float speed = 5f;
    private SpriteRenderer spriteRenderer;
	private Rigidbody2D rigidbodyObject;
    private Vector2 _networkPosition;
    private Quaternion _networkRotation;
    private PhotonView photonView;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(rigidbodyObject.position);
            stream.SendNext(rigidbodyObject.rotation);
            stream.SendNext(rigidbodyObject.velocity);
        }
        else
        {
            _networkPosition = (Vector2)stream.ReceiveNext();
            _networkRotation = (Quaternion)stream.ReceiveNext();
            rigidbodyObject.velocity = (Vector2)stream.ReceiveNext();

            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
            _networkPosition += (rigidbodyObject.velocity * lag);
        }
    }

    public void FixedUpdate()
    {
        if (!photonView.IsMine)
        {
            rigidbodyObject.position = Vector3.MoveTowards(rigidbodyObject.position, _networkPosition, Time.fixedDeltaTime);
        }
    }
    void Awake()
	{
		rigidbodyObject = gameObject.GetComponent<Rigidbody2D> ();
        photonView = gameObject.GetComponent<PhotonView>();
	}

    [PunRPC]
    void addSettingsBall(float _rColor, float _gColor, float _bColor)
    {
        spriteRenderer.color = new Color(_rColor, _gColor, _bColor);
    }


    private void Start()
    {
        rigidbodyObject = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        float rColor = Random.Range(0f, 1f);
        float gColor = Random.Range(0f, 1f);
        float bColor = Random.Range(0f, 1f);
        float scale = Random.Range(0.2f, 0.45f);

        spriteRenderer.color = new Color(rColor, gColor, bColor);

        gameObject.transform.localScale = new Vector3(scale, scale, 1f);
        photonView.RPC("addSettingsBall", RpcTarget.All, rColor, gColor, bColor);
        Invoke("StartMove", 3f);
    }

    void OnCollisionEnter2D(Collision2D col)
	{
        if (col.gameObject.GetComponent<PlayerController>() != null)
        {
            if (col.gameObject.GetComponent<PlayerController>().ID == 1)
            {
                float x = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);
                Vector2 dir = new Vector2(x, 1f).normalized;
                rigidbodyObject.velocity = dir * speed;
            }
            if (col.gameObject.GetComponent<PlayerController>().ID == 0)
            {
                float x = hitFactor(transform.position, col.transform.position, col.collider.bounds.size.x);
                Vector2 dir = new Vector2(x, -1f).normalized;
                rigidbodyObject.velocity = dir * speed;
            }
        }
        if (col.gameObject.name == "WallDown" || col.gameObject.name == "WallUp") {
            Destroy(gameObject);
            PhotonNetwork.Destroy(gameObject);
            Manager.instance.addBall();
		}
	}

	private float hitFactor(Vector2 ballPos, Vector2 racketPos, float racketWidth)
	{
		return (ballPos.x - racketPos.x) / racketWidth;
	}

	// Use this for initialization

	void Update()
	{

	}
		
	public void StartMove()
	{
        Vector2 vec = new Vector2(Random.Range(-5f, 5f),Random.Range(-5f, 5f)).normalized;
        speed = Random.Range(3f, 6f);
		rigidbodyObject.velocity = vec * speed;
	}

}
