using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartBlock : MonoBehaviour
{
    [SerializeField] private Player PlayerPrefab;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public Player Initialize()
    {
        Player player = Instantiate(PlayerPrefab, transform.parent.parent);
        player.transform.position = transform.localPosition;
        return player;
    }
}
