using System.Collections;

namespace VRPTW.Concret;

public class TabouResearch
{
    public Routes solution;

    public Dictionary<string, ArrayList> tabouList; //KeyValuePair<String, ArrayList<Client>>
    public int MAX_TABOUS = 10;
    public double bestFitness { get; set; }
    public Routes bestSolution { get; set; }

    public TabouResearch(Routes randomSolution)
    {
        solution = randomSolution;
        tabouList = new Dictionary<string, ArrayList>();
        bestFitness = solution.GetFitness();
        bestSolution = (Routes) solution.Clone();
    }

    public void performSolution(int nbSteps)
    {
        
        using (var progress = new ProgressBar()) {
            for (int i = 0; i <= nbSteps; i++) {
                progress.Report((double) i / nbSteps);
                RelocateInter();
            }
        }

        DelEmptyVehicle();
    }

    public void DelEmptyVehicle()
    {
        for(var index = bestSolution.Vehicles.Count-1; index >= 0; index--)
        {
            if (bestSolution.Vehicles[index].Clients.Count == 2)
            {
                bestSolution.Vehicles.RemoveAt(index);
            }
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

    public void RelocateInter()
    {
        var prevFitness = solution.GetFitness();
        if (!tabouList.ContainsKey("RelocateInter")) tabouList["RelocateInter"] = new ArrayList();
        var bestDelta = double.MaxValue;//on cherche le delta le plus petit car nouvelle distance plus petite
        (int src, int trg) bestVehicles = (0, 0);
        LinkedListNode<Client> bestNodeToRelocate = null;
        LinkedListNode<Client> bestFirstNodeOfNewEdge = null;
        foreach (var vehicleSrc in solution.Vehicles)
        {
            var nodeToRelocate = vehicleSrc.Clients.First.Next;
            while (!nodeToRelocate.Equals(vehicleSrc.Clients.Last))
            {
                if (tabouList["RelocateInter"].Contains(nodeToRelocate.Value.Id))
                {
                    nodeToRelocate = nodeToRelocate.Next;
                    continue;
                }
                
                foreach (var vehicleTrg in solution.Vehicles)
                {
                    //if (vehicleSrc.Id == vehicleTrg.Id) continue; //permet de pas faire de relocate intra
                    
                    var firstNodeOfEdge = vehicleTrg.Clients.First;
                    while (!firstNodeOfEdge.Equals(vehicleTrg.Clients.Last))
                    {
                        //à décommenter pour faire du relocate intra
                        if (firstNodeOfEdge.Equals(nodeToRelocate) || nodeToRelocate.Equals(firstNodeOfEdge.Next))
                        {
                            firstNodeOfEdge = firstNodeOfEdge.Next;
                            continue;
                        }
                        
                        var currentDistance = nodeToRelocate.Value.GetDistance(nodeToRelocate.Previous.Value) +
                                              nodeToRelocate.Value.GetDistance(nodeToRelocate.Next.Value)
                                              + firstNodeOfEdge.Value.GetDistance(firstNodeOfEdge.Next.Value);
                        
                        var newDistanceTrg = nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Value)
                                             + nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Next.Value);
                        var newDistance = nodeToRelocate.Previous.Value.GetDistance(nodeToRelocate.Next.Value) +
                                          nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Value)
                                          + nodeToRelocate.Value.GetDistance(firstNodeOfEdge.Next.Value);
                        var delta = newDistance - currentDistance;

                        if (delta > bestDelta || newDistanceTrg + vehicleTrg.GetTravelledDistance() > 230)
                        {
                            firstNodeOfEdge = firstNodeOfEdge.Next;
                            continue;
                        }
                        bestDelta = delta;
                        bestVehicles = (vehicleSrc.Id, vehicleTrg.Id);
                        bestNodeToRelocate = nodeToRelocate;
                        bestFirstNodeOfNewEdge = firstNodeOfEdge;
                        
                        firstNodeOfEdge = firstNodeOfEdge.Next;
                    }

                }
                nodeToRelocate = nodeToRelocate.Next;
            }
        }
        
        solution.Vehicles[bestVehicles.src].Clients.Remove(bestNodeToRelocate);
        solution.Vehicles[bestVehicles.trg].Clients.AddAfter(bestFirstNodeOfNewEdge, bestNodeToRelocate);
        //Console.WriteLine(bestDelta);
        
        //Console.WriteLine(bestNodeToRelocate.Value.Id);
        if (prevFitness + bestDelta < bestFitness)
        {
            bestFitness = prevFitness + bestDelta;
            bestSolution = (Routes) solution.Clone();
        }
        if (bestDelta < -1) return; //-1 car sinon boucle sur de petites valeurs
        //Console.WriteLine("Solution dégradée !!");
        if (tabouList["RelocateInter"].Contains(bestNodeToRelocate.Value.Id))
        {
            tabouList["RelocateInter"].Remove(bestNodeToRelocate.Value.Id);
        }
        if (tabouList["RelocateInter"].Count >= MAX_TABOUS)
        {
            tabouList["RelocateInter"].RemoveAt(0);
        }
        tabouList["RelocateInter"].Add(bestNodeToRelocate.Value.Id);
    }
    
}