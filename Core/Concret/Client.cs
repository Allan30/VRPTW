using VRPTW.AbstractObjects;

namespace VRPTW.Concret
{
    public class Client : IElement
    {
        public string Id;
        public Coordinate Coordinate;
        public int ReadyTime;
        public int DueTime;
        public int Demand;
        public int Service;
        
        public double TimeOnIt { get; set; }

        public double TimeAfterService => TimeOnIt + Service;

        public Client(string id, Coordinate coordinate, int readyTime, int dueTime, int demand = 0, int service = 0)
        {
            Id = id;
            Coordinate = coordinate;
            ReadyTime = readyTime;
            DueTime = dueTime;
            Demand = demand;
            Service = service;
            
        }
        
        public double TimeToReachAfter(Client otherClient)
        {
            var timeToReach = otherClient.TimeAfterService + GetDistance(otherClient);
            if (timeToReach < ReadyTime)
            {
                return ReadyTime;
            }
            if (timeToReach > DueTime)
            {
                return -1;
            }
            return timeToReach;
        }

        public double StayInTimeWithDelta(double delta)
        {
            if (TimeOnIt + delta > DueTime)
            {
                return double.NaN;
            }
            if (TimeOnIt + delta < ReadyTime)
            {
                return  ReadyTime - TimeOnIt;
            }

            return delta;
        }
        
        public void AddTime(double delta)
        {
            TimeOnIt += delta;
        }

        public double GetDistance(Client otherClient)
        {
            return Coordinate.GetDistance(otherClient.Coordinate);
        }
        
    }
}