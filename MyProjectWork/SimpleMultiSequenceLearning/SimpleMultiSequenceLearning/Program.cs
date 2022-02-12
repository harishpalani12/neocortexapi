using NeoCortexApi.Encoders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static SimpleMultiSequenceLearning.MultiSequenceLearning;

namespace SimpleMultiSequenceLearning
{
    class Program
    {
        /// <summary>
        /// This sample shows a typical experiment code for SP and TM.
        /// You must start this code in debugger to follow the trace.
        /// and TM.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
           // RunMultiSimpleSequenceLearningExperiment();
            RunMultiSequenceLearningExperiment();
        }

        private static void RunMultiSimpleSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            sequences.Add("S1", new List<double>(new double[] { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0 }));
            sequences.Add("S2", new List<double>(new double[] { 10.0, 11.0, 12.0, 13.0, 4.0, 15.0, 16.0 }));

            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

        }


        private static void RunMultiSequenceLearningExperiment()
        {
            Dictionary<string, List<double>> sequences = new Dictionary<string, List<double>>();

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 0.0, 2.0, 3.0, 4.0, 5.0, 6.0, 5.0, 4.0, 3.0, 7.0, 1.0, 9.0, 12.0, 11.0, 12.0, 13.0, 14.0, 11.0, 12.0, 14.0, 5.0, 7.0, 6.0, 9.0, 3.0, 4.0, 3.0, 4.0, 3.0, 4.0 }));
            //sequences.Add("S2", new List<double>(new double[] { 0.8, 2.0, 0.0, 3.0, 3.0, 4.0, 5.0, 6.0, 5.0, 7.0, 2.0, 7.0, 1.0, 9.0, 11.0, 11.0, 10.0, 13.0, 14.0, 11.0, 7.0, 6.0, 5.0, 7.0, 6.0, 5.0, 3.0, 2.0, 3.0, 4.0, 3.0, 4.0 }));

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 2.0, 3.0, 4.0, 2.0, 5.0, }));
            // sequences.Add("S2", new List<double>(new double[] { 8.0, 1.0, 2.0, 9.0, 10.0, 7.0, 11.00 }));

            //sequences.Add("S1", new List<double>(new double[] { 0.0, 1.0, 3.0, 5.0, 7.0 }));
            //sequences.Add("S2", new List<double>(new double[] { 2.0, 4.0, 6.0, 8.0, 10.00 }));

            // List of Prime Numbers from 0 to 100
            sequences.Add("S1", new List<double>(new double[] { 2, 3, 5, 7, 11, 13, 17, 19, 
                                                                23, 29, 31, 37, 41, 43, 47, 
                                                                53, 59, 61, 67, 71, 73, 79, 
                                                                83, 89, 97 }));

            // List of Composite Numbers from 1 to 100
            sequences.Add("S2", new List<double>(new double[] { 4, 6, 8, 9, 10, 12, 14, 15, 
                                                                16, 18, 20, 21, 22, 24, 25, 
                                                                26, 27, 28, 30, 32, 33, 34, 
                                                                35, 36, 38, 39, 40, 42, 44, 
                                                                45, 46, 48, 49, 50}));

            sequences.Add("S3", new List<double>(new double[] { 51, 52, 54, 55, 56, 57, 58, 
                                                                60, 62, 63, 64, 65, 66, 68, 
                                                                69, 70, 72, 74, 75, 76, 77,
                                                                78, 80, 81, 82, 84, 85, 86, 
                                                                87, 88, 90, 91, 92, 93, 94, 
                                                                95, 96, 98, 99,100}));



            //
            // Prototype for building the prediction engine.
            MultiSequenceLearning experiment = new MultiSequenceLearning();
            var predictor = experiment.Run(sequences);

            Console.WriteLine("Ready to Predict.....");

            var list1 = new double[] { 1.0, 2.0, 3.0 };
            var list2 = new double[] { 2.0, 3.0, 4.0 };
            var list3 = new double[] { 8.0, 1.0, 2.0 };
            var list4 = new double[] { 5.0, 1.0, 7.0 };

            // replace with alphabets 
            // own encoder for letter
            // change letter to scalar encoder
            // note : can use ascii value


            predictor.Reset();
            PredictNextElement(predictor, list4);

            predictor.Reset();
            PredictNextElement(predictor, list1);

            predictor.Reset();
            PredictNextElement(predictor, list2);

            predictor.Reset();
            PredictNextElement(predictor, list3);
        }

        private static void PredictNextElement(HtmPredictionEngine predictor, double[] list)
        {
            Console.WriteLine("------------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);

                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Console.WriteLine($"{pred.PredictedInput} - {pred.Similarity}");
                    }

                    var tokens = res.First().PredictedInput.Split('_'); 
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2[tokens.Length - 1]}");
                }
                else
                    Console.WriteLine("Nothing predicted :(");
            }

            Console.WriteLine("------------------------------");
        }
    }
}
