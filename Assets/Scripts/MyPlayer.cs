using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : Player
{
    NetworkManager netMgr;
    private void Start()
    {
        StartCoroutine(ISend());
        netMgr = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
    }

    IEnumerator ISend()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.25f);
            C_Move move = new C_Move();
            move.posX = UnityEngine.Random.Range(-10, 10);
            move.posY = UnityEngine.Random.Range(-7, 7);
            move.posZ = 0;

            if (netMgr != null) netMgr.Send(move.Serialize());

            //C_Chat chat = new C_Chat();
            //chat.chat = "i'm Unity";

            //var segment = chat.Serialize();
            //Debug.Log("send!");
            //session.Send(segment);
        }

    }


}
