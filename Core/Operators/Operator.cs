namespace VRPTW.Concret;

public abstract class Operator
{
    
    protected double Delta;
    protected double PreviousFitness;
    protected (Vehicle src, Vehicle trg) Vehicles;
    protected (LinkedListNode<Client> src, LinkedListNode<Client> trg) Nodes;
    public Routes Solution;
    
    public void Execute(Routes solution)
    {
        Delta = double.MaxValue;
        PreviousFitness = solution.GetFitness();
        Vehicles = (null, null)!;
        Nodes = (null, null)!;
        Solution = solution;
        Browse();
        SetNewSolution();
    }
    protected abstract void Browse();
    protected abstract void SetNewSolution();
    
    protected abstract bool NodeSrcCondition(LinkedListNode<Client> nodeSrc, Vehicle vehicle);
    protected abstract bool NodeTrgCondition(LinkedListNode<Client> nodeTrg, Vehicle vehicle);
    protected abstract LinkedListNode<Client> GetNodeSrc(Vehicle vehicle);
    protected abstract LinkedListNode<Client> GetNodeTrg(Vehicle vehicle);
    protected abstract double CheckDelta(LinkedListNode<Client> nodeSrc, LinkedListNode<Client> nodeTrg, Vehicle vehicleSrc, Vehicle vehicleTrg);
}