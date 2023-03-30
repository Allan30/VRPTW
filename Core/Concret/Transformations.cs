﻿using System.Formats.Asn1;
using VRPTW.AbstractObjects;

namespace VRPTW.Concret;

public class Transformations : ITransformations
{
    public List<LinkedList<IElement>> TwoOpt(LinkedList<IElement> graph)
    {
        //TODO implement
        var result = new List<LinkedList<IElement>>();
        (LinkedListNode<IElement> n1, LinkedListNode<IElement> n2) edgeI = (graph.First, graph.First.Next);
        
        while (edgeI.n2 != null)
        {
            (LinkedListNode<IElement> n1, LinkedListNode<IElement> n2) edgeJ = (graph.First.Next, graph.First.Next.Next);
            while (edgeJ.n2 != null)
            {
                if (edgeI.n1 == edgeI.n2 || edgeI.n1 == edgeJ.n1 || edgeI.n2 == edgeJ.n1 || edgeI.n2 == edgeJ.n2) continue;
                var newGraph = new LinkedList<IElement>(graph);
                var nodeI = newGraph.Find(edgeI.n1.Value);
                var nodeJ = newGraph.Find(edgeJ.n1.Value);
            }
        }

        return result;
    }

    public IEnumerable<LinkedList<Client>> RelocateIntra(LinkedList<Client> graph) //n*(n-1)
    {
        var result = new List<LinkedList<Client>>();
        
        var nodeToRelocate = graph.First.Next;
        while(!nodeToRelocate.Equals(graph.Last))
        {
            var firstNodeOfEdge = graph.First;
            while(!firstNodeOfEdge.Equals(graph.Last))
            {
                if (firstNodeOfEdge.Equals(nodeToRelocate) || nodeToRelocate.Equals(firstNodeOfEdge.Next))
                {
                    firstNodeOfEdge = firstNodeOfEdge.Next;
                    continue;
                }
                var newGraph = new LinkedList<Client>(graph);
                var newNodeToRelocate = newGraph.Find(nodeToRelocate.Value);
                var newFirstNodeOfEdge = newGraph.Find(firstNodeOfEdge.Value);
                newGraph.Remove(newNodeToRelocate);
                newGraph.AddAfter(newFirstNodeOfEdge, newNodeToRelocate);
                result.Add(newGraph);
                firstNodeOfEdge = firstNodeOfEdge.Next;
            }
            nodeToRelocate = nodeToRelocate.Next;
        }

        return result;
    }

    public List<LinkedList<IElement>> Reverse(LinkedList<IElement> graph)
    {
        var result = new List<LinkedList<IElement>>();
        var newGraph = new LinkedList<IElement>();
        foreach (var node in new LinkedList<IElement>(graph))
        {
            newGraph.AddFirst(node);
        }
        result.Add(newGraph);
        return result;

    }

    public List<LinkedList<IElement>> ExchangeIntra(LinkedList<IElement> graph)
    {
        throw new NotImplementedException();
    }

    public (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) CrossExchange(LinkedList<IElement> graph1, LinkedList<IElement> graph2)
    {
        throw new NotImplementedException();
    }

    public (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) ExchangeInter(LinkedList<IElement> graph1, LinkedList<IElement> graph2)
    {
        throw new NotImplementedException();
    }

    public (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) RelocateInter(LinkedList<IElement> graph1, LinkedList<IElement> graph2)
    {
        throw new NotImplementedException();
    }
}