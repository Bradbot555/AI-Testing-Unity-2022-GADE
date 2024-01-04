using System.Collections.Generic;
using System.Linq;
using System;

namespace NN{
    public class NeuralNetwork{
        List<Neuron> inputLayer = new List<Neuron>();
        List<List<Neuron>> hiddenLayers = new List<List<Neuron>>();
        List<Neuron> outputLayer = new List<Neuron>();

        double learnRate;
        double momentum;

        public NeuralNetwork(int inputs, int[] hidden, int output, double learnRate, double momentum){
            this.learnRate = learnRate;
            this.momentum = momentum;

            CreateLayers(inputs, hidden, output);
            ConnectAllLayers();
        }


		public double[] Compute(double[] inputs) {
			ForwardPropagate(inputs);

            double[] output = new double[outputLayer.Count];
            int count = 0;
            foreach(Neuron neuron in outputLayer){
                output[count] = neuron.Value;
                count++;
            }

			return output;
		}

        void ForwardPropagate(double[] inputs){
            // 6. Complete this method.
            for (int inputNeuronreff = 0; inputNeuronreff < inputLayer.Count; inputNeuronreff++)
            {
                inputLayer[inputNeuronreff].Value = inputs[inputNeuronreff];
            }
            Console.WriteLine("hidden layers: "+ hiddenLayers.Count);
            for (int layer = 0; layer < hiddenLayers.Count; layer ++)//Loops through each hidden Layer
            {
                for (int cell = 0; cell < hiddenLayers[layer].Count; cell ++) //Loops through each neuron in the layer
                {
                    hiddenLayers[layer][cell].CalculateValue();
                }
            }
            
            foreach (Neuron outputNeuron in outputLayer) //Final output
            {
                outputNeuron.CalculateValue();
            }
        }

        public void BackPropagate(params double[] targets)
		{
            for (int output = outputLayer.Count-1; output >= 0; output--)//output neurons still have an activation function so they also need to be adapted 
            {
                outputLayer[output].CalculateError(targets[output]);
                outputLayer[output].CalculateGradient();
                outputLayer[output].UpdateWeights(learnRate, momentum);
                outputLayer[output].UpdateBias(learnRate, momentum);
            }
            for (int layer = hiddenLayers.Count-1; layer >= 0; layer--)//Loops through each of the hidden layers
            {
                for (int cell = 0; cell < hiddenLayers[layer].Count; cell++) //Loops through each neuron in the layer and updates and calculates everything for the neuron
                {
                    //Console.WriteLine(layer+":"+cell);
                    hiddenLayers[layer][cell].CalculateError();
                    hiddenLayers[layer][cell].CalculateGradient();
                    hiddenLayers[layer][cell].UpdateWeights(learnRate, momentum);
                    hiddenLayers[layer][cell].UpdateBias(learnRate, momentum);
                }
            }
            // 7. Complete this method.
        }
        
        double CalculateError(params double[] targets) {
			int i = 0;
			return outputLayer.Sum(a => Math.Abs(a.CalculateError(targets[i++])));
		}

        void CreateLayers(int inputs, int[] hidden, int output){
            for(int i = 0; i < inputs; i++){
                inputLayer.Add(new Neuron());
            }

            foreach (int hiddenLayerSize in hidden){
                List<Neuron> hiddenLayer = new List<Neuron>();
                for(int i = 0; i < hiddenLayerSize; i++){
                    hiddenLayer.Add(new Neuron());
                }
                hiddenLayers.Add(hiddenLayer);
            }

            for(int i = 0; i < output; i++){
                outputLayer.Add(new Neuron());
            }
        }

        void ConnectAllLayers(){
            if(hiddenLayers.Count == 0){
                return;
            }

            List<Neuron> firstHiddenLayer = hiddenLayers[0];
            List<Neuron> lastHiddenLayer = hiddenLayers[hiddenLayers.Count -1];

            ConnectLayers(inputLayer, firstHiddenLayer);

            for(int i = 0; i < hiddenLayers.Count-1; i++){
                ConnectLayers(hiddenLayers[i], hiddenLayers[i+1]);
            } 

            ConnectLayers(lastHiddenLayer, outputLayer);
        }

        void ConnectLayers(List<Neuron> fromLayer, List<Neuron> toLayer){
            foreach(Neuron input in fromLayer){
                foreach(Neuron output in toLayer){
                    Synapse synapse = new Synapse(input, output);
                    input.OutputSynapses.Add(synapse);
                    output.InputSynapses.Add(synapse);
                }
            }  
        }
    }
}