namespace VRPTW.AbstractObjects;

public interface ISolution
{
    
    double Fitness { get; }
    List<ISolution> GetNeighbours();
    
    void GenerateRandomSolution();

}