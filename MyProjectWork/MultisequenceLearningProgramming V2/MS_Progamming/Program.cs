using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ThesisExperiments
{
    class Program
    {

        /// <summary>
        /// Experiment Entry Point
        /// </summary>
        static void Main(string[] args)
        {



            /// <summary>
            /// Experiment-2  [A] HTM  [B] LSTM :::::----Cancer Sequence Classification i.e classify sequence in 4 Categories i.e Mod. Active , InActive, Very Active, Virtually Acitve
            /// </summary>

            SequenceLearningHTM experimentHTM = new SequenceLearningHTM();
            //SequenceLearningLSTM experimentLSTM = new SequenceLearningLSTM();

            Console.WriteLine("HELLO!!! Please Select Experiment To Begin:");

            //-----------------------------------HTM-----------------------------------
           // Console.WriteLine("1) Predict Passenger Demand {NYC taxi Dataset used for Training || ***HTM***");
            Console.WriteLine("2) Predict Anti Cancer_V1 Peptides Sequences class || ***HTM***");
            Console.WriteLine("3) Predict Anti Cancer_V2 Peptides Sequences class || ***HTM***");

            //-----------------------------------LSTM-----------------------------------
           // Console.WriteLine("4) Predict Passenger Demand {NYC taxi Dataset used for Training || ***LSTM***");
            Console.WriteLine("5) Predict Anti Cancer_V1 Peptides Sequences class || ***LSTM***");
            Console.WriteLine("6) Predict Anti Cancer_V2 Peptides Sequences class || ***LSTM***");

            Console.WriteLine("Please Enter Experimnt Number To Begin the Experiment");
            var selectedExperiment = Console.ReadLine();

            //**************************************************************************
            //                               HTM
            if (selectedExperiment == "2")
            {

                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v1 EXPERIMENT || ***HTM  ***-------------");
                experimentHTM.InitiateCancerSequenceClassificationExperiment();
            }
            else if (selectedExperiment == "3")
            {
                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v2 EXPERIMENT || ***HTM  ***-------------");
                experimentHTM.InitiateCancerSequenceClassificationExperimentV2();
            }
            //**************************************************************************
            //                               LSTM


            //**************************************************************************
            else
            {
                Console.WriteLine("Please Enter Correct Experiment Number");
            }

        }
    }
}
