using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{

    public GameObject player;
    public GameObject chunkManager;

    public int highRadius = 8;

    int xChunkPlayer;
    int zChunkPlayer;

    public int xChunkSize = 16;
    public int zChunkSize = 16;

    // Update is called once per frame
    void Update()
    {
        getPlayerChunk();
    }


    void getPlayerChunk ()
    {
        float x = this.player .transform.position.x;
        float z = this.player .transform.position.z;

        this.xChunkPlayer = (int) Mathf.Floor((float)x / this.xChunkSize);
        this.zChunkPlayer = (int) Mathf.Floor((float)z / this.zChunkSize);
    }
}
