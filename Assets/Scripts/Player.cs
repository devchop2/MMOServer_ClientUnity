using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int playerId;

    private void Start()
    {

    }

    #region ETC
    public void SetPos(Vector3 pos)
    {
        transform.position = pos;
    }
    #endregion
}
