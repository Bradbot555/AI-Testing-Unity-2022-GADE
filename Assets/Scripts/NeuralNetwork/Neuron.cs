using System.Collections.Generic;
using System;

namespace NN{
    public class Neuron{
		public List<Synapse> InputSynapses { get; set; }
		public List<Synapse> OutputSynapses { get; set; }
		public double Value { get; set; }
		public double Bias { get; set; }
		public double BiasDelta { get; set; }
		public double Gradient { get; set; }
		public double Error { get; set; }

		public Neuron()
		{
			InputSynapses = new List<Synapse>();
			OutputSynapses = new List<Synapse>();
			Bias = 0;
            Value = 0;
			Error = 0;
		}

		public virtual double CalculateValue(){
            double weightedSum = 0;// check here as this may interfere with the value being +- may not be the best way to initialise the value
            // 1. Complete this method.
            //weighted sum
            foreach (Synapse synapse in InputSynapses)
            {
                weightedSum += synapse.Weight * synapse.InputNeuron.Value;
            }
            Value = ActivationFunctions.Sigmoid(weightedSum + Bias);
            //Console.WriteLine("Weighted sum: "+ weightedSum + " value: "+Value+ " input N :"+ InputSynapses.Count);
            //Utils.Hault();
     
			return Value;
		}

		// Calculate error for output neuron
		public double CalculateError(double target) {
            // 2. Complete this method.
            Error = target - Value;

			return Error;
		}

		// Calculate error for any hidden neuron
		public double CalculateError(){
            // 3. Complete this method
            double errorSum = 0;

            foreach (Synapse synapse in OutputSynapses)
            {
                errorSum += synapse.Weight * synapse.OutputNeuron.Error;
            }
            //Console.WriteLine("error sum: " + errorSum);
       
            Error = errorSum;

            return Error;
		}
		
		// Calculate gradient
		public double CalculateGradient() {
            // 4. Complete this method
            Gradient = ActivationFunctions.SigmoidDerivative(Value);
            //Console.WriteLine("gradient: "+ Gradient);
			return Gradient;
		}

		//Update all the weights of synapses that are inputs to this neuron
		public void UpdateWeights(double learnRate, double momentum) {
            // 5. Complete this method
            foreach (Synapse synapse in InputSynapses)
            {
                synapse.WeightDelta = learnRate * Error * Gradient * synapse.InputNeuron.Value;
                //Console.WriteLine("delta weight: " + synapse.WeightDelta);
                synapse.Weight += synapse.WeightDelta;
            }
        }

		//Update the bias or the neuron
		public void UpdateBias(double learnRate, double momentum){
            // 6. Complete this method
            BiasDelta = learnRate * Error * Gradient;
            //Console.WriteLine("delta bias: "+ BiasDelta);
            Bias += BiasDelta;
		}
    }
}