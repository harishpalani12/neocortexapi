using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiSequenceLearning
{
    class Program
    {
    static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            /// <summary>
            /// Experiment-2  [A] HTM  [B] LSTM :::::----Cancer Sequence Classification i.e classify sequence in 4 Categories i.e Mod. Active , InActive, Very Active, Virtually Acitve
            /// </summary>
            /// 
            SequenceLearningSTM experimentHTM = new SequenceLearningSTM();

            Console.WriteLine("HELLO!!! Please Select Experiment To Begin:");

            Console.WriteLine("1) Predict Anti Cancer_V1 Peptides Sequences class || ***HTM***");
            Console.WriteLine("2) Predict Anti Cancer_V2 Peptides Sequences class || ***HTM***");

            Console.WriteLine("Please Enter Experimnt Number To Begin the Experiment");
            var selectedExperiment = Console.ReadLine();

            if (selectedExperiment == "1")
            {
                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v1 EXPERIMENT || ***HTM  ***-------------");
                experimentHTM.InitiateCancerSequenceClassification();
            }
            else if (selectedExperiment == "2")
            {
                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v2 EXPERIMENT || ***HTM  ***-------------");
                experimentHTM.InitiateCancerSequenceClassificationExperimentV2();
            }
            else
            {
                Console.WriteLine("Please Enter Correct Experiment Number");
            }

        }
    }

}

