namespace VRPTW.AbstractObjects;

public interface ITransformations
{
    void TwoOpt(LinkedList<IElement> element);
    void RelocateIntra(LinkedList<IElement> element);
    void Exchange(LinkedList<IElement> element);
    void Reverse(LinkedList<IElement> element);
    
    void RelocateInter(LinkedList<IElement> element1, LinkedList<IElement> element2);
    void Cross(LinkedList<IElement> element1, LinkedList<IElement> element2);
    void CrossExchange(LinkedList<IElement> element1, LinkedList<IElement> element2);
    
}