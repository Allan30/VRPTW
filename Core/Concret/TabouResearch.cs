using System.Collections;

namespace VRPTW.Concret;

public class TabouResearch
{
    public Routes solution;

    public Dictionary<String, ArrayList> tabouList; //KeyValuePair<String, ArrayList<Client>>
    public int MAX_TABOUS = 10;

    public TabouResearch(Routes randomSolution)
    {
        solution = randomSolution;
        tabouList = new Dictionary<string, ArrayList>();
    }

    public void performSolution(int nbSteps)
    {
        for (var i = 0; i < nbSteps; i++)
        {
            RelocateIntra();
        }
    }
    
    public void RelocateIntra() //n*(n-1)
    {
        var prevFitness = solution.GetFitness();
        if (!tabouList.ContainsKey("RelocateIntra")) tabouList["RelocateIntra"] = new ArrayList();
        var bestDelta = Double.MaxValue;//on cherche le delta le plus petit car nouvelle distance plus petite
        var bestVehicle = 0;
        LinkedListNode<Client> bestNodeToRelocate = null;
        LinkedListNode<Client> bestFirstNodeOfNewEdge = null;
        foreach (var vehicle in solution.Vehicles)
        {
            var nodeToRelocate = vehicle.Clients.First.Next;
            while(!nodeToRelocate.Equals(vehicle.Clients.Last))
            {
                if (tabouList["RelocateIntra"].Contains(nodeToRelocate.Value.Id))
                {
                    nodeToRelocate = nodeToRelocate.Next;
                    continue;
                }
                var firstNodeOfEdge = vehicle.Clients.First;
                while(!firstNodeOfEdge.Equals(vehicle.Clients.Last))
                {
                    if (firstNodeOfEdge.Equals(nodeToRelocate) || nodeToRelocate.Equals(firstNodeOfEdge.Next))
                    {
                        firstNodeOfEdge = firstNodeOfEdge.Next;
                        continue;
                    }

                    var currentDistance = nodeToRelocate.Value.GetDistance(nodeToRelocate.Previous.Value) +
                                       nodeToRelocate.Value.GetDistance(nodeToRelocate.Next.Value)
                                       + firstNodeOfEdge.Value.GetDistance(firstNodeOfEdge.Next.Value);
                    var newDistance = nodeToRelocate.Previous.Value.GetDistance(nodeToRelocate.Next.Value) +
                                      nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Value)
                                      + nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Next.Value);
                    var delta = newDistance - currentDistance;

                    if (delta > bestDelta)
                    {
                        firstNodeOfEdge = firstNodeOfEdge.Next;
                        continue;
                    }
                    bestDelta = delta;
                    bestVehicle = vehicle.Id;
                    bestNodeToRelocate = nodeToRelocate;
                    bestFirstNodeOfNewEdge = firstNodeOfEdge;
                    
                    firstNodeOfEdge = firstNodeOfEdge.Next;
                }
                nodeToRelocate = nodeToRelocate.Next;
            }
        }

        solution.Vehicles[bestVehicle].Clients.Remove(bestNodeToRelocate);
        solution.Vehicles[bestVehicle].Clients.AddAfter(bestFirstNodeOfNewEdge, bestNodeToRelocate);
        
        if (!(solution.GetFitness() > prevFitness)) return;
        if (tabouList["RelocateIntra"].Contains(bestNodeToRelocate.Value.Id))
        {
            tabouList["RelocateIntra"].Remove(bestNodeToRelocate.Value.Id);
        }
        if (tabouList["RelocateIntra"].Count >= MAX_TABOUS)
        {
            tabouList["RelocateIntra"].RemoveAt(0);
        }
        tabouList["RelocateIntra"].Add(bestNodeToRelocate.Value.Id);
        


    }
    
}