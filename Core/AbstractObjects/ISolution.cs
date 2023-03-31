namespace VRPTW.AbstractObjects;

public interface ISolution
{
    
    double GetFitness();
    List<ISolution> GetNeighbours();
    
    void GenerateRandomSolution();

}