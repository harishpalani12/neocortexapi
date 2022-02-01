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
        static void Main(string[] args){


            /// <summary>
            /// Experiment-1  [A] HTM  [B] LSTM :::::----Taxi Passenger Count Prediction 1 timestamp in advance i.e 1 day in advance
            /// </summary>

            /// <summary>
            /// Experiment-2  [A] HTM  [B] LSTM :::::----Cancer Sequence Classification i.e classify sequence in 4 Categories i.e Mod. Active , InActive, Very Active, Virtually Acitve
            /// </summary>

            SequenceLearningHTM experimentHTM = new SequenceLearningHTM();
            SequenceClassficationLSTM experimentLSTM = new SequenceClassficationLSTM();

            Console.WriteLine("HELLO!!! Please Select Experiment To Begin:");
            
            //-----------------------------------HTM-----------------------------------
            Console.WriteLine("1) Predict Passenger Demand {NYC taxi Dataset used for Training || ***HTM***");
            Console.WriteLine("2) Predict Anti Cancer_V1 Peptides Sequences class || ***HTM***");
            Console.WriteLine("3) Predict Anti Cancer_V2 Peptides Sequences class || ***HTM***");

            //-----------------------------------LSTM-----------------------------------
            Console.WriteLine("4) Predict Passenger Demand {NYC taxi Dataset used for Training || ***LSTM***");
            Console.WriteLine("5) Predict Anti Cancer_V1 Peptides Sequences class || ***LSTM***");
            Console.WriteLine("6) Predict Anti Cancer_V2 Peptides Sequences class || ***LSTM***");

            Console.WriteLine("Please Enter Experimnt Number To Begin the Experiment");
            var selectedExperiment = Console.ReadLine();
            
            //**************************************************************************
            //                               HTM
            if (selectedExperiment == "1") 
            {

                Console.WriteLine("-------------INITIATING PREDICT PASSENGER COUNT PREDICTION EXPERIMENT || ***HTM***-------------");
                experimentHTM.InitiatePassengerCountPredictionExperiment();  

            }
            else if (selectedExperiment == "2")
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

            else if (selectedExperiment == "4")
            {
                Console.WriteLine("-------------INITIATING PREDICT PASSENGER COUNT PREDICTION EXPERIMENT || ***LSTM***-------------");
                experimentLSTM.InitiatePassengerCountPredictionExperiment();
            }
            else if (selectedExperiment == "5")
            {
                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v1 EXPERIMENT || ***LSTM***-------------");
                experimentLSTM.InitiateCancerSequenceClassificationExperimentV1();
            }
            else if (selectedExperiment == "6")
            {
                Console.WriteLine("-------------INITIATING CANCER SEQUENCE CLASSIFICATION_v2 EXPERIMENT || ***LSTM***-------------");
                experimentLSTM.InitiateCancerSequenceClassificationExperimentV2();
            }

            //**************************************************************************
            else
            {
                Console.WriteLine("Please Enter Correct Experiment Number");
            }
            
        }
    }
}
