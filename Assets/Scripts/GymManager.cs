using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GymManager : MonoBehaviour
{
    private Seats seats = new Seats();
    private Vector2 stagecenter = new Vector2(-18, 0);
    // Start is called before the first frame update
    void Start()
    {
        seats.InitSeats(transform);
        seats.SortWithDis(stagecenter);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Seats GetSeats()
    {
        return seats;
    }
}

public class Seats
{
    private List<Seat> seats = new List<Seat>();
    private List<Seat> seatsUsed = new List<Seat>();

    public void InitSeats(Transform parent)
    {
        foreach (Transform t in parent.GetComponentInChildren<Transform>())
        {
            Seat seat = new Seat();
            seat.pos.x = t.position.x;
            seat.pos.y = t.position.z;
            seats.Add(seat);
        }
    }

    public void SortWithDis(Vector2 focus)
    {
        foreach(Seat s in seats)
        {
            int dis = (int)Mathf.Abs(focus.x - s.pos.x) + (int)Mathf.Abs(focus.y - s.pos.y);
            s.dis = dis;
        }
        seats.Sort((a, b) => a.dis - b.dis);
        foreach (Seat s in seats)
        {
            s.prior = seats.IndexOf(s);
        }
    }

    public Seat UseSeat(int i)
    {
        Seat s = seats[i];
        s.SetIsSeated(true);
        seatsUsed.Add(s);
        seats.Remove(s);
        return s;
    }

    public void ReleaseSeat(Seat s)
    {
        s.SetIsSeated(false);
        seatsUsed.Remove(s);
        for (int i = 0; i < seats.Count; i++)
        {
            if(s.prior < seats[i].prior)
            {
                seats.Insert(i, s);
                break;
            }
        }
    }

    public Vector2 GetSeatPos(int i)
    {
        return seats[i].pos;
    }

    public int IdleSeatsCount()
    {
        return seats.Count;
    }

    public void DebugOutput()
    {
        foreach (Seat s in seats)
            Debug.Log(s.pos);
    }
}

public class Seat : IComparable<Seat>
{
    public Vector2 pos = Vector2.zero;
    public int prior = 0;
    public int dis = 0;
    private bool isSeated = false;

    public bool GetIsSeated()
    {
        return isSeated;
    }

    public void SetIsSeated(bool flag)
    {
        isSeated = flag;
    }

    public int CompareTo(Seat s)
    {
        if (dis > s.dis)
            return 1;
        else
            return -1;
    }
}
