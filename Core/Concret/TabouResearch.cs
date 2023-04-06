using System.Collections;

namespace VRPTW.Concret;

public class TabouResearch
{
    public Routes solution;

    public ArrayList tabouList;
    public int MAX_TABOUS = 40;
    public double bestFitness { get; set; }
    public Routes bestSolution { get; set; }

    public TabouResearch(Routes randomSolution)
    {
        solution = randomSolution;
        tabouList = new ArrayList();
        bestFitness = solution.GetFitness();
        bestSolution = (Routes) solution.Clone();
    }
    
    public void ManageTabouList(string move)
    {
        if (tabouList.Contains(move))
        {
            tabouList.Remove(move);
        }
        else if (tabouList.Count >= MAX_TABOUS)
        {
            tabouList.RemoveAt(0);
        }
        tabouList.Add(move);
    }

    public void performSolution(int nbSteps)
    {
        using (var progress = new ProgressBar()) {
            for (var i = 0; i <= nbSteps; i++) {
                progress.Report((double) i / nbSteps);
                Exchange();
            }
        }
        Console.WriteLine("Exchange done : "+bestSolution.GetFitness());
        using (var progress = new ProgressBar()) {
            for (var i = 0; i <= nbSteps; i++) {
                progress.Report((double) i / nbSteps);
                RelocateInter();
            }
        }
        Console.WriteLine("RelocateInter done : "+bestSolution.GetFitness());
        DelEmptyVehicle(); 
        
        using (var progress = new ProgressBar()) {
            for (var i = 0; i <= nbSteps; i++) {
                progress.Report((double) i / nbSteps);
                TwoOpt();
            }
        }
        Console.WriteLine("TwoOpt done : "+bestSolution.GetFitness());

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
        
        for(var index = solution.Vehicles.Count-1; index >= 0; index--)
        {
            if (solution.Vehicles[index].Clients.Count == 2)
            {
                solution.Vehicles.RemoveAt(index);
            }
        }
        
        //re allocation des numéros des camions dans l'ordre
        for (var i = 0; i < bestSolution.Vehicles.Count; i++)
        {
            bestSolution.Vehicles[i].Id = i;
        }
        
        for (var i = 0; i < solution.Vehicles.Count; i++)
        {
            solution.Vehicles[i].Id = i;
        }
        
    }
    
    public void RelocateIntra() //n*(n-1)
    {
        var prevFitness = solution.GetFitness();
        var bestDelta = Double.MaxValue;//on cherche le delta le plus petit car nouvelle distance plus petite
        var bestVehicle = 0;
        LinkedListNode<Client> bestNodeToRelocate = null;
        LinkedListNode<Client> bestFirstNodeOfNewEdge = null;
        foreach (var vehicle in solution.Vehicles)
        {
            var nodeToRelocate = vehicle.Clients.First.Next;
            while(!nodeToRelocate.Equals(vehicle.Clients.Last))
            {
                if (tabouList.Contains("RelocateIntra_"+nodeToRelocate.Value.Id))
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
        ManageTabouList("RelocateIntra_"+bestNodeToRelocate.Value.Id);
    }

    public void RelocateInter()
    {
        var prevFitness = solution.GetFitness();
        var bestDelta = double.MaxValue;//on cherche le delta le plus petit car nouvelle distance plus petite
        (int src, int trg) bestVehicles = (0, 0);
        LinkedListNode<Client> bestNodeToRelocate = null;
        LinkedListNode<Client> bestFirstNodeOfNewEdge = null;
        foreach (var vehicleSrc in solution.Vehicles)
        {
            var nodeToRelocate = vehicleSrc.Clients.First.Next;
            while (!nodeToRelocate.Equals(vehicleSrc.Clients.Last))
            {
                if (tabouList.Contains("RelocateInter_"+nodeToRelocate.Value.Id))
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

                        if (delta > bestDelta || !vehicleTrg.StayCorrect(newDistanceTrg, nodeToRelocate.Value.Demand))
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
        
        solution.Vehicles[bestVehicles.src].RemoveClient(bestNodeToRelocate);
        solution.Vehicles[bestVehicles.trg].AddClientAfter(bestFirstNodeOfNewEdge, bestNodeToRelocate);
        //Console.WriteLine(bestDelta);
        
        //Console.WriteLine(bestNodeToRelocate.Value.Id);
        if (prevFitness + bestDelta < bestFitness)
        {
            bestFitness = prevFitness + bestDelta;
            bestSolution = (Routes) solution.Clone();
        }
        if (bestDelta < -1) return; //-1 car sinon boucle sur de petites valeurs
        //Console.WriteLine("Solution dégradée !!");
        ManageTabouList("RelocateInter_"+bestNodeToRelocate.Value.Id);
    }


    public void TwoOpt()
    {
        
        //Console.WriteLine("=====================================");
        var prevFitness = solution.GetFitness();
        //Console.WriteLine(prevFitness);
        var bestDelta = double.MaxValue;
        var bestVehicle = 0;
        LinkedListNode<Client> bestSrcNode = null;
        LinkedListNode<Client> bestTrgNode = null;
        foreach (var vehicle in solution.Vehicles)
        {
            var SrcNode = vehicle.Clients.First;
            while (SrcNode.Next != null)
            {
                var TrgNode = vehicle.Clients.First;
                while (TrgNode.Next != null)
                {
                    
                    if (tabouList.Contains("TwoOpt_"+SrcNode.Value.Id+TrgNode.Value.Id) || tabouList.Contains("TwoOpt_"+TrgNode.Value.Id+SrcNode.Value.Id))
                    {
                        TrgNode = TrgNode.Next;
                        continue;
                    }

                    if (SrcNode.Value.Id != TrgNode.Value.Id &&
                        SrcNode.Value.Id != TrgNode.Next.Value.Id &&
                        SrcNode.Next.Value.Id != TrgNode.Value.Id &&
                        SrcNode.Next.Value.Id != TrgNode.Next.Value.Id)
                    {
                        var currentDistance = SrcNode.Value.GetDistance(SrcNode.Next.Value) + TrgNode.Value.GetDistance(TrgNode.Next.Value);
                        var newDistance = SrcNode.Value.GetDistance(TrgNode.Value) + SrcNode.Next.Value.GetDistance(TrgNode.Next.Value);
                        var delta = newDistance - currentDistance;
                    
                        if (delta < bestDelta && vehicle.StayCorrect(delta, 0))
                        {
                            bestDelta = delta;
                            bestVehicle = vehicle.Id;
                            bestSrcNode = SrcNode;
                            bestTrgNode = TrgNode;
                        }
                    }
                    
                    
                    
                    TrgNode = TrgNode.Next;
                }
                
                SrcNode = SrcNode.Next;
            }
            
        }
        //Console.WriteLine("SrcNode : " + bestSrcNode.Value.Id);
        //Console.WriteLine("TrgNode : " + bestTrgNode.Value.Id);
        var SrcNodeNext = bestSrcNode.Next;
        var TrgNodeNext = bestTrgNode.Next;
        //Console.WriteLine("SrcNodeNext : " + SrcNodeNext.Value.Id);
        //Console.WriteLine("TrgNodeNext : " + TrgNodeNext.Value.Id);
        
        //Console.WriteLine(solution.Vehicles[bestVehicle].ToStringClient());
        var currentNode = bestSrcNode.Next;
        var lastNode = bestTrgNode.Next;
        while(currentNode != bestTrgNode)
        {
            var nextNode = currentNode.Next;
            solution.Vehicles[bestVehicle].Clients.Remove(currentNode);
            solution.Vehicles[bestVehicle].Clients.AddBefore(lastNode, currentNode);
            lastNode = currentNode;
            currentNode = nextNode;
        }

        /*
        Console.WriteLine(solution.Vehicles[bestVehicle].ToStringClient());

        Console.WriteLine(prevFitness + bestDelta);
        Console.WriteLine(solution.GetFitness());
        Console.WriteLine(bestDelta);
        */

        if (prevFitness + bestDelta < bestFitness)
        {
            bestFitness = prevFitness + bestDelta;
            bestSolution = (Routes) solution.Clone();
        }
        
        if (bestDelta < -1) return; //-1 car sinon boucle sur de petites valeurs
        //Console.WriteLine("Solution dégradée !!");
        if(tabouList.Contains("TwoOpt_"+bestSrcNode.Value.Id+bestTrgNode.Value.Id))
            ManageTabouList("TwoOpt_"+bestSrcNode.Value.Id+bestTrgNode.Value.Id);
        else
            ManageTabouList("TwoOpt_"+bestTrgNode.Value.Id+bestSrcNode.Value.Id);
    }

    public void Exchange()
    {
        var prevFitness = solution.GetFitness();
        var bestDelta = double.MaxValue;
        (int vehicleOfNode1, int vehicleOfNode2) bestVehicles = (0, 0);
        LinkedListNode<Client> bestNodeToExchange1 = null;
        LinkedListNode<Client> bestNodeToExchange2 = null;
        
        foreach (var vehicle1 in solution.Vehicles)
        {
            var nodeToExchange1 = vehicle1.Clients.First.Next;
            while (nodeToExchange1 != vehicle1.Clients.Last)
            {
                foreach (var vehicle2 in solution.Vehicles)
                {
                    if (vehicle1.Id == vehicle2.Id) continue; ///!\ important pour le rapport, c'est un exchange intra si on fait ça
                    var nodeToExchange2 = vehicle2.Clients.First.Next;
                    while (nodeToExchange2 != vehicle2.Clients.Last)
                    {
                        if (nodeToExchange1.Value.Id == nodeToExchange2.Value.Id ||tabouList.Contains("Exchange_"+nodeToExchange1.Value.Id+nodeToExchange2.Value.Id) || tabouList.Contains("Exchange_"+nodeToExchange2.Value.Id+nodeToExchange1.Value.Id))
                        {
                            nodeToExchange2 = nodeToExchange2.Next;
                            continue;
                        }
                        var currentDistanceVehicle1 = nodeToExchange1.Value.GetDistance(nodeToExchange1.Next.Value) + nodeToExchange1.Value.GetDistance(nodeToExchange1.Previous.Value);
                        var newDistanceVehicle1 = nodeToExchange1.Value.GetDistance(nodeToExchange2.Next.Value) + nodeToExchange1.Value.GetDistance(nodeToExchange2.Previous.Value);
                        var currentDistanceVehicle2 = nodeToExchange2.Value.GetDistance(nodeToExchange2.Next.Value) + nodeToExchange2.Value.GetDistance(nodeToExchange2.Previous.Value);
                        var newDistanceVehicle2 = nodeToExchange2.Value.GetDistance(nodeToExchange1.Next.Value) + nodeToExchange2.Value.GetDistance(nodeToExchange1.Previous.Value);
                        var deltaVehicle1 = newDistanceVehicle1 - currentDistanceVehicle1;
                        var deltaVehicle2 = newDistanceVehicle2 - currentDistanceVehicle2;
                        var delta = deltaVehicle1 + deltaVehicle2;
                        if (delta < bestDelta && vehicle1.StayCorrect(deltaVehicle1, nodeToExchange2.Value.Demand) && vehicle2.StayCorrect(deltaVehicle2, nodeToExchange1.Value.Demand))
                        {
                            bestDelta = delta;
                            bestVehicles = (vehicle1.Id, vehicle2.Id);
                            bestNodeToExchange1 = nodeToExchange1;
                            bestNodeToExchange2 = nodeToExchange2;
                        }
                        nodeToExchange2 = nodeToExchange2.Next;
                    }
                }
                nodeToExchange1 = nodeToExchange1.Next;
            }
        }
        //Console.WriteLine(solution.Vehicles[bestVehicles.vehicleOfNode1].ToString());
        //Console.WriteLine(solution.Vehicles[bestVehicles.vehicleOfNode2].ToString());
        var nodeToExchange2Next = bestNodeToExchange2.Next.Value;
        solution.Vehicles[bestVehicles.vehicleOfNode2].Clients.Remove(bestNodeToExchange2);
        solution.Vehicles[bestVehicles.vehicleOfNode1].Clients.AddBefore(bestNodeToExchange1, bestNodeToExchange2);
        solution.Vehicles[bestVehicles.vehicleOfNode1].Clients.Remove(bestNodeToExchange1);
        solution.Vehicles[bestVehicles.vehicleOfNode2].Clients.AddBefore(solution.Vehicles[bestVehicles.vehicleOfNode2].Clients.FindLast(nodeToExchange2Next), bestNodeToExchange1);
        
        /*
         Console.WriteLine("=======================================");
        Console.WriteLine(prevFitness + bestDelta);
        Console.WriteLine(solution.GetFitness());
        Console.WriteLine("=======================================");
        Console.WriteLine(solution.Vehicles[bestVehicles.vehicleOfNode1].ToString());
        Console.WriteLine(solution.Vehicles[bestVehicles.vehicleOfNode2].ToString());
        */
        if (prevFitness + bestDelta < bestFitness)
        {
            bestFitness = prevFitness + bestDelta;
            bestSolution = (Routes) solution.Clone();
        }
        //Console.WriteLine(bestDelta);
        
        if (bestDelta < -1) return; //-1 car sinon boucle sur de petites valeurs
        if(tabouList.Contains("Exchange_"+bestNodeToExchange1.Value.Id+bestNodeToExchange2.Value.Id))
            ManageTabouList("Exchange_"+bestNodeToExchange1.Value.Id+bestNodeToExchange2.Value.Id);
        else
            ManageTabouList("Exchange_"+bestNodeToExchange2.Value.Id+bestNodeToExchange1.Value.Id);
        
    }
    
}