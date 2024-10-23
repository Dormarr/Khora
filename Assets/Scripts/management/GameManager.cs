using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public class GameManager : MonoBehaviour{

    private CancellationTokenSource ctSource;

    void Start(){

        // WorldManager.activeChunks.Clear();

        ctSource = new CancellationTokenSource();

        Task.Run(() => ProcessTicketLoop(ctSource.Token), ctSource.Token);
    }

    void Update(){
         TicketManager.ProcessTickets<ChunkTicket>();
    }

    private void OnDestroy(){
        ctSource.Cancel();
    }

    private void ProcessTicketLoop(CancellationToken ct){
        while(!ct.IsCancellationRequested){
            Thread.Sleep(50);
        }
    }
}