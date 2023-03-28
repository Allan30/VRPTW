namespace VRPTW.AbstractObjects;

public interface ISolution
{
    
    float GetFitness();
    List<ISolution> GetNeighbours();
    
    void GenerateRandomSolution();

}