using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TicketManager;
public class FocusController : MonoBehaviour{
    //Act as the player.
    public Vector3 focusPosition;
    public Vector3Int focusChunkPosition;
    public Vector3Int oldFocusChunkPosition;
    public Grid grid;

    void Start(){

        focusChunkPosition = BiomeUtility.GetVariableChunkPosition(focusPosition);
        oldFocusChunkPosition = focusChunkPosition;
    }

    void Update(){
        focusPosition = this.transform.position;
        focusChunkPosition = BiomeUtility.GetVariableChunkPosition(focusPosition);
        if(focusChunkPosition != oldFocusChunkPosition){
            //DispatchTickets(focusChunkPosition);
            WorldManager.AssessAndSubmitTickets(focusChunkPosition);
            oldFocusChunkPosition = focusChunkPosition;
        }

    }

    void DispatchTickets(Vector3Int chunkPosition){
        Debug.Log($"FocusController.DispatchTickets: Dispatched for chunk ({chunkPosition.x}, {chunkPosition.y}).");
        SubmitTicket(new ChunkTicket(chunkPosition, ChunkStatus.FullLoad, 1));
        SubmitTicket(new ChunkTicket(oldFocusChunkPosition, ChunkStatus.Unload, 1));
    }   
}