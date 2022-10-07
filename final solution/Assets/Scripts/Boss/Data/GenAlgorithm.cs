using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;

public class GenAlgorithm : MonoBehaviour
{
    public float mutationRate = 0.055f;

    public int initialPopulation = 85;
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 3;
    public int numberToCrossover;

    public int layers = 1;
    public int neurals = 10;

    private List<int> genePool = new List<int>();

    private int naturallySelected;

    private NNetwork[] populationArray;

    public int currentGeneration;
    public int currentGenomeIter;

    private void Start()
    {
        currentGeneration = 0;
        currentGenomeIter = 0;
        naturallySelected = 0;

        CreatePopulation();
    }

    private void Update()
    {
        
    }

    private void CreatePopulation()
    {
        populationArray = new NNetwork[initialPopulation];

        FillPopulationWithRandomValues(populationArray, 0);
    }

    //float fitness as parameter
    public NNetwork ResetNetwork(float fitness)
    {
        if(currentGenomeIter < populationArray.Length-1)
        {
            populationArray[currentGenomeIter].fitness = fitness;
            currentGenomeIter++;
        }
        else
        {
            RePopulate();
        }
        return populationArray[currentGenomeIter];
    }

    private void RePopulate()
    {
        genePool.Clear();
        currentGeneration++;
        currentGenomeIter = 0;
        naturallySelected = 0;

        SortPopulation();

        NNetwork[] newPopulationArray = PickBestPopulation();

        Crossover(newPopulationArray);
        Mutate(newPopulationArray);

        FillPopulationWithRandomValues(newPopulationArray, naturallySelected);

        //Debug.Log("poulationArray: " + populationArray[0].);

        populationArray = newPopulationArray;
        

        //ResetToCurrentGenome
    }

    private void Crossover(NNetwork[] newPopulationArray)
    {
        for(int i = 0; i < numberToCrossover; i += 2)
        {
            int parentIndexA = i;
            int parentIndexB = i + 1;

            if(genePool.Count >= 1)
            {
                for(int l=0; l < 100; l++)
                //while(true)
                {
                    parentIndexA = genePool[Random.Range(0, genePool.Count)];
                    parentIndexB = genePool[Random.Range(0, genePool.Count)];

                    if(parentIndexA != parentIndexB)
                    {
                        break;
                    }
                }
            }

            NNetwork childNetA = new NNetwork();
            NNetwork childNetB = new NNetwork();

            childNetA.Initialise(layers, neurals);
            childNetB.Initialise(layers, neurals);

            childNetA.fitness = 0;
            childNetB.fitness = 0;

            //very simple crossover function that swaps the entire weight arrays
            for(int w = 0; w < childNetA.weights.Count; w++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    childNetA.weights[w] = populationArray[parentIndexA].weights[w];
                    childNetB.weights[w] = populationArray[parentIndexB].weights[w];
                }
                else
                {
                    childNetB.weights[w] = populationArray[parentIndexA].weights[w];
                    childNetA.weights[w] = populationArray[parentIndexB].weights[w];
                }
            }

            for (int w = 0; w < childNetA.biases.Count; w++)
            {
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    childNetA.biases[w] = populationArray[parentIndexA].biases[w];
                    childNetB.biases[w] = populationArray[parentIndexB].biases[w];
                }
                else
                {
                    childNetB.biases[w] = populationArray[parentIndexA].biases[w];
                    childNetA.biases[w] = populationArray[parentIndexB].biases[w];
                }
            }

            newPopulationArray[naturallySelected] = childNetA;
            naturallySelected++;

            newPopulationArray[naturallySelected] = childNetB;
            naturallySelected++;
        }
    }
    //mutating some of the children
    private void Mutate(NNetwork[] newPopulationArray)
    {
        for(int i = 0; i < naturallySelected; i++)
        {
            for(int c = 0; c < newPopulationArray[i].weights.Count; c++)
            {
                if(Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulationArray[i].weights[c] = MutateMatrix(newPopulationArray[i].weights[c]);
                }
            }
        }
    }

    
    private Matrix<float> MutateMatrix(Matrix<float> A)
    {
        //mutate some of the mutated children weights
        int randomPoints = Random.Range(1, (A.RowCount * A.ColumnCount) / 5);//5 experimental, some of the weights will change

        Matrix<float> C = A;

        for(int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, C.ColumnCount);
            int randomRow = Random.Range(0, C.RowCount);

            //changing a bit the mutated weight
            C[randomRow, randomColumn] = Mathf.Clamp(C[randomRow, randomColumn] + Random.Range(-1f, 1f), -1f, 1f);
        }

        return C;
    }
    

    //Pick best and some of the worst nns
    private NNetwork[] PickBestPopulation()
    {
        NNetwork[] newPopulationArray = new NNetwork[initialPopulation];

        for(int i=0; i < bestAgentSelection; i++)
        {
            //deep copy
            newPopulationArray[naturallySelected] = populationArray[i].DeepClone(layers, neurals);
            newPopulationArray[naturallySelected].fitness = 0;

            naturallySelected++;

            //the higher the fitness, the more times it will be added to the genepool
            int f = Mathf.RoundToInt(populationArray[i].fitness * 7);

            for(int c = 0; c<f; c++)
            {
                //adding index of newPopulationArray instead of the whole nn
                genePool.Add(i);
            }
        }

        for(int i=0; i < worstAgentSelection; i++)
        {
            int last = populationArray.Length - 1;
            last -= i;

            int f = Mathf.RoundToInt(populationArray[last].fitness * 7);

            for(int c=0; c<f; c++)
            {
                genePool.Add(last);
            }
        }

        return newPopulationArray;
    }

    private void SortPopulation()
    {
        for(int i=0; i< populationArray.Length; i++)
        {
            for(int j=i; j < populationArray.Length; j++)
            {
                if(populationArray[i].fitness < populationArray[j].fitness)
                {
                    NNetwork temp = populationArray[i];
                    populationArray[i] = populationArray[j];
                    populationArray[j] = temp;
                }
            }
        }
    }

    //after the first call of this function, I randomise only the nn that are not product of crossover nns
    private void FillPopulationWithRandomValues(NNetwork[] newPopulationArray, int startingIndex)
    {
        while(startingIndex < initialPopulation)
        {
            newPopulationArray[startingIndex] = new NNetwork();
            newPopulationArray[startingIndex].Initialise(layers, neurals);
            startingIndex++;
        }
    }

    public int GetGeneration()
    {
        return currentGeneration;
    }
}
