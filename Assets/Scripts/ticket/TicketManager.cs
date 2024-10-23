using System;
using System.Collections.Generic;

public static class TicketManager{
    private static readonly Dictionary<Type, Queue<Ticket>> ticketQueues = new Dictionary<Type, Queue<Ticket>>();
    private static readonly object queueLock = new object();

    public static void SubmitTicket<T>(T ticket) where T : Ticket{
        lock(queueLock){
            Type ticketType = typeof(T);

            if(!ticketQueues.ContainsKey(ticketType)){
                ticketQueues[ticketType] = new Queue<Ticket>();
            }

            ticketQueues[ticketType].Enqueue(ticket);
            Debug.Log($"TicketManager.SubmitTicket: Submitted ticket {ticket.TicketID}");
        }
    }

    // Process all tickets of all types
    public static void ProcessTickets<T>() where T : Ticket{
        lock(queueLock){
            Type ticketType = typeof(T);

            if(ticketQueues.ContainsKey(ticketType)){
                Queue<Ticket> queue = ticketQueues[ticketType];

                while(queue.Count > 0){
                    T currentTicket = (T)queue.Dequeue();
                    currentTicket.Execute();
                }
            }
        }
    }

    // Check for remaining type specific tickets
    public static bool HasTickets<T>() where T : Ticket{
        lock(queueLock){
            Type ticketType = typeof(T);
            return ticketQueues.ContainsKey(ticketType) && ticketQueues[ticketType].Count > 0;
        }
    }

    // Check for any type tickets remaining.
    public static bool HasAnyTickets(){
        lock(queueLock){
            foreach(var queue in ticketQueues.Values){
                if(queue.Count > 0){
                    return true;
                }
            }
            return false;
        }
    }
}