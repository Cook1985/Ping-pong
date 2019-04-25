using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
	private Vector3 facetPos;
	private Vector3 pos;
	private Vector3 posTo;
	private bool move = false;
    public int ID = 0;

	void Awake () {
		facetPos = Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width - 24, 1f, 1f));
	}

    void OnPhotonSerializeView(PhotonStream strem, PhotonMessageInfo messageInfo)
    {
        Vector3 pos = transform.position;
        strem.Serialize(ref pos);
        if (strem.IsReading)
        {
            transform.position = pos;
        }
    }

    // Update is called once per frame
    void Update () {
        if (photonView.IsMine)
        {
            if (Input.GetMouseButtonDown(0))
                move = true;

            if (Input.GetMouseButtonUp(0))
                move = false;

            if (move && Manager.instance.isMoveRacket)
            {
                pos = gameObject.transform.localPosition;
                if (Mathf.Abs(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 1f, 1f)).x) < facetPos.x)
                {
                    posTo = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Screen.height / 20f, 1f));
                    gameObject.transform.localPosition = Vector3.Lerp(pos, new Vector3(posTo.x, pos.y, pos.z), 0.3f);
                }
                else
                {
                    if (Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, 1f, 1f)).x > 0)
                        gameObject.transform.localPosition = Vector3.Lerp(pos, new Vector3(facetPos.x, pos.y, pos.z), 0.3f);
                    else
                        gameObject.transform.localPosition = Vector3.Lerp(pos, new Vector3(-facetPos.x, pos.y, pos.z), 0.3f);
                }
            }
        }
	}

}
