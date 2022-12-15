using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

using MathNet.Numerics.LinearAlgebra;

public class NNetwork : MonoBehaviour
{
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 13);
    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();
    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 7);

    public List<Matrix<float>> weights = new List<Matrix<float>>();
    public List<float> biases = new List<float>();

    public float fitness;

    public void Initialise(int hiddenLayerCount, int hiddenNeuralCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();


        for(int i=0; i<hiddenLayerCount; i++)
        {
            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuralCount);
            hiddenLayers.Add(f);
            biases.Add(Random.Range(-1f, 1f));

            if (i == 0)
            {
                Matrix<float> inputToHidden = Matrix<float>.Build.Dense(13, hiddenNeuralCount);
                weights.Add(inputToHidden);
            }
            else
            {
                Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuralCount, hiddenNeuralCount);
                weights.Add(hiddenToHidden);
            }
        }

        Matrix<float> hiddenToOutput = Matrix<float>.Build.Dense(hiddenNeuralCount, 7);
        weights.Add(hiddenToOutput);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();
    }
    

    public void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for(int i=0; i<hiddenLayerCount + 1; i++)
        {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenNeuronCount);
            hiddenLayers.Add(newHiddenLayer);
        }
    }


    public void RandomiseWeights()
    {
        for(int i=0; i < weights.Count; i++)
        {
            for(int x = 0; x < weights[i].RowCount; x++)
            {
                for(int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public NNetwork DeepClone(int hiddenLayerCount, int hiddenNeuralCount)
    {
        NNetwork net = new NNetwork();

        List<Matrix<float>> newWeights = new List<Matrix<float>>();

        for (int i = 0; i < hiddenLayerCount; i++)
        {
            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuralCount);
            net.hiddenLayers.Add(f);
            net.biases.Add(Random.Range(-1f, 1f));

            if (i == 0)
            {
                Matrix<float> inputToHidden = Matrix<float>.Build.Dense(13, hiddenNeuralCount);
                net.weights.Add(inputToHidden);
            }
            else
            {
                Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuralCount, hiddenNeuralCount);
                net.weights.Add(hiddenToHidden);
            }
        }

        Matrix<float> hiddenToOutput = Matrix<float>.Build.Dense(hiddenNeuralCount, 7);
        net.weights.Add(hiddenToOutput);
        net.biases.Add(Random.Range(-1f, 1f));

        for (int i = 0; i < this.weights.Count; i++)
        {
            for (int x = 0; x < this.weights[i].RowCount; x++)
            {
                for (int y = 0; y < this.weights[i].ColumnCount; y++)
                {
                    net.weights[i][x, y] = this.weights[i][x, y];
                }
            }
        }

        return net;
    }

    public BossAction RunNetwork(PlayerState PS)
    {
        inputLayer[0, 0] = PS.lightAttack.x;
        inputLayer[0, 1] = PS.lightAttack.y;
        inputLayer[0, 2] = PS.heavyAttack.x;
        inputLayer[0, 3] = PS.heavyAttack.y;
        inputLayer[0, 4] = PS.rangeAttack.x;
        inputLayer[0, 5] = PS.rangeAttack.y;
        inputLayer[0, 6] = PS.jump.x;
        inputLayer[0, 7] = PS.jump.y;
        inputLayer[0, 8] = PS.dodge.x;
        inputLayer[0, 9] = PS.dodge.y;
        inputLayer[0, 10] = PS.parry.x;
        inputLayer[0, 11] = PS.parry.y;
        inputLayer[0, 12] = PS.distance;

        //maybe just sigmoid
        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count-1] * weights[weights.Count-1])+biases[biases.Count-1]).PointwiseTanh();

        BossAction action;

        //I need clear numbers for these
        action.block = (float)System.Math.Tanh(outputLayer[0, 0]);
        action.charge = (float)System.Math.Tanh(outputLayer[0, 1]);
        action.fireAttack = (float)System.Math.Tanh(outputLayer[0, 2]);
        action.meleeAttack = (float)System.Math.Tanh(outputLayer[0, 3]);
        action.fireball = (float)System.Math.Tanh(outputLayer[0, 4]);
        action.firepillar = (float)System.Math.Tanh(outputLayer[0, 5]);
        action.hook = (float)System.Math.Tanh(outputLayer[0, 6]);

        return action;
    }

    private float Sigmoid(float s)
    {
        return (1 / (1 + Mathf.Exp(-s)));
    }
}