namespace VRPTW.AbstractObjects;

public interface ITransformations
{
    List<LinkedList<IElement>> TwoOpt(LinkedList<IElement> graph);
    List<LinkedList<IElement>> RelocateIntra(LinkedList<IElement> graph);
    List<LinkedList<IElement>> Reverse(LinkedList<IElement> graph);
    List<LinkedList<IElement>> ExchangeIntra(LinkedList<IElement> graph);

    (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) CrossExchange(LinkedList<IElement> graph1, LinkedList<IElement> graph2);
    (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) ExchangeInter(LinkedList<IElement> graph1, LinkedList<IElement> graph2);
    (List<LinkedList<IElement>> graph1, List<LinkedList<IElement>> graph2) RelocateInter(LinkedList<IElement> graph1, LinkedList<IElement> graph2);
    
}