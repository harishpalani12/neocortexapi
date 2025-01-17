﻿using NeoCortexApi.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Diagnostics;
using NeoCortexApi;
using NeoCortexApi.Encoders;
//using NeoCortex;
using NeoCortexApi.Utility;
using System.Text.RegularExpressions;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Network;

namespace ThesisExperiments
{
    public class HelperMethods
    {
        static readonly string[]  CancerSequenceClasses = new string[] { "inactive - exp", "mod. active", "very active", "inactive - virtual" };
        static readonly float[][] CancerSequenceClassesOneHotEncoding = new float[][] { new float[] { 0, 0, 0, 1 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 1, 0, 0 }, new float[] { 1, 0, 0, 0 } };

        // READING TRAINING DATA
        /// <summary>
        ///     Fetch Taxi Data from file
        /// </summary>
        /// <param name="dataFilePath">Training Data File Path</param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadPassengerDataFromFile(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {

                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    Dictionary<string, string> Sequence = new Dictionary<string, string>();
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        string[] values = line.Split(",");


                        keyForUniqueIndexes++;

                        var observationPassengerCount = values[1];
                        var observationDateTime = values[0];

                        if (Sequence.ContainsKey(values[1]))
                        {
                            var newKey = values[1] + "," + keyForUniqueIndexes;
                            Sequence.Add(newKey, observationDateTime);
                        }
                        else
                        {
                            Sequence.Add(observationPassengerCount, observationDateTime);
                        }
                    }
                    SequencesCollection.Add(Sequence);
                }
                return SequencesCollection;
            }
            return null;
        }
        /// <summary>
        ///     Fetch Cancer Peptides data from file
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadCancerSequencesDataFromFile(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {

                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        string[] values = line.Split(",");



                        Dictionary<string, string> Sequence = new Dictionary<string, string>();

                        string label = values[1];
                        string sequenceString = values[0];

                        int count = 0;

                        foreach (var alphabet in sequenceString)
                        {
                            keyForUniqueIndexes++;
                            if (Sequence.ContainsKey(alphabet.ToString()))
                            {
                                var newKey = alphabet.ToString() + "," + keyForUniqueIndexes;
                                Sequence.Add(newKey, label);
                            }
                            else
                            {
                                Sequence.Add(alphabet.ToString(), label);
                            }
                        }

                        SequencesCollection.Add(Sequence);
                    }

                }

                return SequencesCollection;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static Dictionary<float[][], float[][]> ReadCancerSequencesDataFromFile_LSTM_v2(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {

                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        string[] values = line.Split(",");

                        Dictionary<string, string> Sequence = new Dictionary<string, string>();
                        int count = 0;

                        string label = values[1];
                        string sequenceString = values[0];

                        if (sequenceString.Length < 33)
                        {
                            int remainingLength = 33 - sequenceString.Length;
                            for (int i = 0; i < remainingLength; i++)
                            {
                                sequenceString = sequenceString + "Z";
                            }
                        }
                        Sequence.Add(sequenceString, label);
                        SequencesCollection.Add(Sequence);
                    }
                }

                float[][] processedDataSet = new float[SequencesCollection.Count][];
                float[][] processedLabel = new float[SequencesCollection.Count][];

                int index = 0;
                foreach (var sequence in SequencesCollection)
                {

                    var sequenceDict = sequence;
                    var sequenceString = sequenceDict.Keys.ElementAt(0);
                    var sequenceLabel = sequenceDict.Values.ElementAt(0).Split("_")[0];

                    float[] sequenceProcessed = new float[0];

                    foreach (var element in sequenceString)
                    {
                        var numericval = char.ToUpper(element) - 64;
                        sequenceProcessed = sequenceProcessed.Concat(new float[] { numericval }).ToArray();
                    }

                    processedDataSet[index] = sequenceProcessed;
                    var label_encoded = new float[0];
                    if (sequenceLabel == CancerSequenceClasses[0])
                    {
                        label_encoded = CancerSequenceClassesOneHotEncoding[0];
                    }
                    else if (sequenceLabel == CancerSequenceClasses[1])
                    {
                        label_encoded = CancerSequenceClassesOneHotEncoding[1];
                    }
                    else if (sequenceLabel == CancerSequenceClasses[2])
                    {
                        label_encoded = CancerSequenceClassesOneHotEncoding[2];
                    }
                    else if (sequenceLabel == CancerSequenceClasses[3])
                    {
                        label_encoded = CancerSequenceClassesOneHotEncoding[3];
                    }
                    processedLabel[index] = label_encoded;
                    index++;
                }
                var returnDict = new Dictionary<float[][], float[][]>();
                returnDict.Add(processedDataSet, processedLabel);

                return returnDict;
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static List<Dictionary<string, int[]>> ReadAndEncodeCancerSequencesDataFromFileV2(string dataFilePath)
        {
            List<Dictionary<string, int[]>> SequencesCollection = new List<Dictionary<string, int[]>>();
            ScalarEncoder encoder_Alphabets = FetchAlphabetEncoder();
            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {

                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        string[] values = line.Split(",");


                        string label = values[1];
                        string sequenceString = values[0];
                        Dictionary<string, int[]> tempDictionary = new Dictionary<string, int[]>();

                        if (sequenceString.Length < 33)
                        {
                            int remainingLength = 33 - sequenceString.Length;
                            for (int i = 0; i < remainingLength; i++)
                            {
                                sequenceString = sequenceString + "Z";
                            }
                        }

                        int[] sdr = new int[0];
                        foreach (var alphabet in sequenceString)
                        {
                            sdr = sdr.Concat(encoder_Alphabets.Encode(char.ToUpper(alphabet) - 64)).ToArray();
                        }
                        tempDictionary.Add(label, sdr);
                        SequencesCollection.Add(tempDictionary);
                    }

                }

                return SequencesCollection;
            }
            return null;
        }
    
        //*********************************************************************************************
        // ENCODING TRAINING DATA
        /// <summary>
        ///     Encode Taxi Passenger Data
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        public static List<Dictionary<string, int[]>> EncodePassengerData(List<Dictionary<string, string>> trainingData)
        {
            List<Dictionary<string, int[]>> ListOfEncodedTrainingSDR = new List<Dictionary<string, int[]>>();

            ScalarEncoder DayEncoder = FetchDayEncoder();
            ScalarEncoder MonthEncoder = FetchMonthEncoder();
            ScalarEncoder YearEncoder = FetchYearEncoder();
            ScalarEncoder DayOfWeekEncoder = FetchWeekDayEncoder();

            foreach (var sequence in trainingData)
            {

                var tempDictionary = new Dictionary<string, int[]>();

                foreach (var keyValuePair in sequence)
                {

                    var observationLabel = keyValuePair.Key;
                    var element = keyValuePair.Value;

                    DateTime observationDateTime = DateTime.Parse(element);
                    int day = observationDateTime.Day;
                    int month = observationDateTime.Month;
                    int year = observationDateTime.Year;
                    int dayOfWeek = (int)observationDateTime.DayOfWeek;

                    int[] sdr = new int[0];

                    sdr = sdr.Concat(DayEncoder.Encode(day)).ToArray();
                    sdr = sdr.Concat(MonthEncoder.Encode(month)).ToArray();
                    sdr = sdr.Concat(YearEncoder.Encode(year)).ToArray();
                    sdr = sdr.Concat(DayOfWeekEncoder.Encode(dayOfWeek)).ToArray();

                    //    UNCOMMENT THESE LINES TO DRAW SDR BITMAP
                    //    int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(sdr, 100, 100);
                    //    var twoDimArray = ArrayUtils.Transpose(twoDimenArray);
                    //    NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{observationDateTime.Day.ToString()}.png", null);
                    tempDictionary.Add(observationLabel, sdr);
                }
                ListOfEncodedTrainingSDR.Add(tempDictionary);
            }
            return ListOfEncodedTrainingSDR;
        }

        /// <summary>
        ///     Encoding Amino Alphabetic Sequences
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        public static List<Dictionary<string, int[]>> EncodeCancerSequences(List<Dictionary<string, string>> trainingData)
        {

            List<Dictionary<string, int[]>> ListOfEncodedTrainingSDR = new List<Dictionary<string, int[]>>();

            ScalarEncoder encoder_Alphabets = FetchAlphabetEncoder();

            foreach (var sequence in trainingData)
            {
                int keyForUniqueIndex = 0;
                int index = 0;
                var tempDictionary = new Dictionary<string, int[]>();

                foreach (var element in sequence)
                {
                    keyForUniqueIndex++;
                    var elementLabel = element.Key + "," + element.Value;
                    var elementKey = element.Key;
                    int[] sdr = new int[0];
                    sdr = sdr.Concat(encoder_Alphabets.Encode(char.ToUpper(element.Key.ElementAt(0)) - 64)).ToArray();

                    //UNCOMMENT THESE LINES TO DRAW SDR BITMAP
                    //int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(ElementWiseSdrList[ElementWiseSdrList.Count - 1], 100, 100);
                    //    var twoDimArray = ArrayUtils.Transpose(twoDimenArray
                    // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{observationLabel.ToString()}.png", null);

                    if (tempDictionary.ContainsKey(elementLabel))
                    {
                        var newKey = elementLabel + "," + keyForUniqueIndex;
                        tempDictionary.Add(newKey, sdr);
                    }
                    else
                    {
                        tempDictionary.Add(elementLabel, sdr);
                    }
                }
                ListOfEncodedTrainingSDR.Add(tempDictionary);
            }
            return ListOfEncodedTrainingSDR;
        }


        public static Dictionary<string, int[]> EncodeCancerSequencesApproach2(List<Dictionary<string, string>> trainingData)
        {


            var tempDictionary = new Dictionary<string, int[]>();
            ScalarEncoder encoder_Alphabets = FetchAlphabetEncoder();

            foreach (var sequence in trainingData)
            {
                int keyForUniqueIndex = 0;
                int index = 0;

                int[] sdr = new int[0];
                var elementLabel = "";
                foreach (var element in sequence)
                {
                    keyForUniqueIndex++;
                    elementLabel = element.Value.Split("_")[0];
                    var elementKey = element.Key;

                    sdr = sdr.Concat(encoder_Alphabets.Encode(char.ToUpper(element.Key.ElementAt(0)) - 64)).ToArray();
                    //UNCOMMENT THESE LINES TO DRAW SDR BITMAP
                    //int[,] twoDimenArray = ArrayUtils.Make2DArray<int>(ElementWiseSdrList[ElementWiseSdrList.Count - 1], 100, 100);
                    //    var twoDimArray = ArrayUtils.Transpose(twoDimenArray
                    // NeoCortexUtils.DrawBitmap(twoDimArray, 1024, 1024, $"{observationLabel.ToString()}.png", null);


                }

                tempDictionary.Add(elementLabel, sdr);
            }
            return tempDictionary;
        }

        public static Dictionary<float[][], float[][]> CancerSequenceDataProcessing(List<Dictionary<string, string>> trainingData)
        {

            var ListOfProcessedSequenceDictionary = new List<Dictionary<float[][], float[][]>>();


            foreach (var sequence in trainingData)
            {

                var ProcessedSequenceDictionary = new Dictionary<float[][], float[][]>();
                var processedSequence = new float[sequence.Count][];
                var processedLabel = new float[sequence.Count][];
                int elementIndex = 0;

                foreach (var element in sequence)
                {

                    var observationElement = char.ToUpper(element.Value.ElementAt(0)) - 64;
                    var observationLabel = element.Key.Split("_")[0];
                    var observationClassOneHotEncoding = new float[0];

                    if (observationLabel.Equals(CancerSequenceClasses[0]))
                    {
                        observationClassOneHotEncoding = CancerSequenceClassesOneHotEncoding[0];
                    }
                    else if (observationLabel.Equals(CancerSequenceClasses[1]))
                    {
                        observationClassOneHotEncoding = CancerSequenceClassesOneHotEncoding[1];
                    }
                    else if (observationLabel.Equals(CancerSequenceClasses[2]))
                    {
                        observationClassOneHotEncoding = CancerSequenceClassesOneHotEncoding[2];

                    }
                    else if (observationLabel.Equals(CancerSequenceClasses[3]))
                    {
                        observationClassOneHotEncoding = CancerSequenceClassesOneHotEncoding[3];
                    }

                    processedSequence[elementIndex] = new float[] { observationElement };
                    processedLabel[elementIndex] = observationClassOneHotEncoding;

                    ProcessedSequenceDictionary.Add(processedLabel, processedSequence);
                    ListOfProcessedSequenceDictionary.Add(ProcessedSequenceDictionary);

                    elementIndex++;
                }
                return ProcessedSequenceDictionary;
            }
            return null;
        }

        public static Dictionary<float[], float[][]> PassengerCountDataProcessing(Dictionary<string, string> trainingData)
        {
            var processedData = new float[trainingData.Count][];
            var obesrvationLabel = new float[trainingData.Count];
            var returnDictionary = new Dictionary<float[], float[][]>();

            int index = 0;
            foreach (var element in trainingData)
            {
                var observationDateTime = DateTime.Parse(element.Value);
                var observationLabel = element.Key;

                var dayOfWeek = (int)observationDateTime.DayOfWeek;
                var day = observationDateTime.Day;
                var month = observationDateTime.Month;
                var year = observationDateTime.Year;

                var processedDatarow = new float[] { day, month, year, dayOfWeek };
                processedData[index] = processedDatarow;
                obesrvationLabel[index] = float.Parse(observationLabel) / 73739; // DATA NORMALIZATION
                index++;
            }

            returnDictionary.Add(obesrvationLabel, processedData);
            return returnDictionary;
        }

        //*********************************************************************************************
        // HTM HELPER METHODS
        
        /// <summary>
        ///     Fetch HTM Config
        /// </summary>
        /// <param name="inputBits"></param>
        /// <param name="numColumns"></param>
        /// <returns></returns>
        public static HtmConfig FetchConfig(int inputBits, int numColumns)
        {
            HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns })
            {
                Random = new ThreadSafeRandom(42),

                CellsPerColumn = 32,
                GlobalInhibition = true,
                LocalAreaDensity = -1,
                NumActiveColumnsPerInhArea = 0.02 * numColumns,
                PotentialRadius = 65,
                InhibitionRadius = 15,

                MaxBoost = 10.0,
                DutyCyclePeriod = 25,
                MinPctOverlapDutyCycles = 0.75,
                MaxSynapsesPerSegment = 128,

                ActivationThreshold = 15,
                ConnectedPermanence = 0.5,

                // Learning is slower than forgetting in this case.
                PermanenceDecrement = 0.25,
                PermanenceIncrement = 0.15,

                // Used by punishing of segments.
                PredictedSegmentDecrement = 0.1


            };
            return cfg;
        }
        public static string GetKey(List<string> prevInputs)
        {
            string key = String.Empty;

            for (int i = 0; i < prevInputs.Count; i++)
            {
                if (i > 0)
                    key += "-";

                key += (prevInputs[i]);
            }

            return key;
        }
        //**********************************************************************************************

        //*****************************ENCODE USERINPUT FOR TES*****************************************
        
        /// <summary>
        ///         ENCODE USER INPUT 
        /// </summary>
        public static int[] EncodeSingleInput_testingExperiment_1(string userInput)
        {

            DateTime date = DateTime.Parse(userInput);
            var day = date.Day;
            var month = date.Month;
            var year = date.Year;
            var weekDay = (int)date.DayOfWeek;

            ScalarEncoder DayEncoder = FetchDayEncoder();
            ScalarEncoder MonthEncoder2 = FetchMonthEncoder();
            ScalarEncoder YearEncoder3 = FetchYearEncoder();
            ScalarEncoder WeekDayEncoder = FetchWeekDayEncoder();

            int[] sdr = new int[0];
            sdr = sdr.Concat(DayEncoder.Encode(day)).ToArray();
            sdr = sdr.Concat(MonthEncoder2.Encode(month)).ToArray();
            sdr = sdr.Concat(YearEncoder3.Encode(year)).ToArray();
            sdr = sdr.Concat(WeekDayEncoder.Encode(weekDay)).ToArray();
            return sdr;
        }
        public static List<int[]> EncodeSingleInput_testingExperiment_2(string userInput, Boolean EncodeSingleAlphabet)
        {

            var alphabetEncoder = FetchAlphabetEncoder();

            var Encoded_Alphabet_SDRs = new List<int[]>();
            if (!EncodeSingleAlphabet)
            {
                if (userInput.Length < 33)
                {
                    int remainingLength = 33 - userInput.Length;
                    for (int i = 0; i < remainingLength; i++)
                    {
                        userInput = userInput + "Z";
                    }
                }

                foreach (var alphabet in userInput)
                {
                    Encoded_Alphabet_SDRs.Add(alphabetEncoder.Encode(char.ToUpper(alphabet) - 64));
                }
            }
            else
            {
                Encoded_Alphabet_SDRs.Add(alphabetEncoder.Encode(char.ToUpper(userInput.ElementAt(0)) - 64));
            }

            return Encoded_Alphabet_SDRs;
        }
        public static int[] EncodeSingleInput_testingExperiment_3(string userInput) 
        {
            var alphabetEncoder = FetchAlphabetEncoder();
            var encodedSDR = new int[0];

            foreach (var alphabet in userInput) {
                var encodedElement = alphabetEncoder.Encode(alphabet);
                encodedSDR = encodedSDR.Concat(encodedElement).ToArray();
            }
            return encodedSDR;
        }
        //***********************************************************************************************

        //**********************************FETCHING ENCODERS SECTION****************************************
        
        /// <summary>
        ///         FETCH ENCODERS 
        /// </summary>
        /// <returns> SCALAR ENCODERS</returns>
        public static ScalarEncoder FetchWeekDayEncoder()
        {
            ScalarEncoder MonthEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 3},
                { "N", 11},
                { "MinVal", (double)0}, // Min value = (0).
                { "MaxVal", (double)7}, // Max value = (7).
                { "Periodic", true}, // Since Monday would repeat again.
                { "Name", "WeekDay"},
                { "ClipInput", true},
            });
            return MonthEncoder;
        }
        public static ScalarEncoder FetchDayEncoder()
        {
            ScalarEncoder DayEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 3},
                { "N", 35},
                { "MinVal", (double)1}, // Min value = (0).
                { "MaxVal", (double)32}, // Max value = (7).
                { "Periodic", true},
                { "Name", "Date"},
                { "ClipInput", true},
           });

            return DayEncoder;
        }
        public static ScalarEncoder FetchMonthEncoder()
        {
            ScalarEncoder MonthEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 3},
                { "N", 15},
                { "MinVal", (double)1}, // Min value = (0).
                { "MaxVal", (double)12}, // Max value = (7).
                { "Periodic", true}, // Since Monday would repeat again.
                { "Name", "Month"},
                { "ClipInput", true},
            });
            return MonthEncoder;
        }
        public static ScalarEncoder FetchYearEncoder()
        {
            ScalarEncoder YearEncoder = new ScalarEncoder(new Dictionary<string, object>()
            {
                { "W", 3},
                { "N", 11},
                { "MinVal", (double)2018}, // Min value = (2018).
                { "MaxVal", (double)2022}, // Max value = (2021).
                { "Periodic", false}, // Since Monday would repeat again.
                { "Name", "Year"},
                { "ClipInput", true},
            });
            return YearEncoder;
        }
        public static ScalarEncoder FetchAlphabetEncoder()
        {
            ScalarEncoder AlphabetEncoder = new ScalarEncoder(new Dictionary<string, object>()
                {
                    { "W", 5},
                    { "N", 31},
                    { "Radius", -1.0},
                    { "MinVal", (double)1},
                    { "Periodic", true},
                    { "Name", "scalar"},
                    { "ClipInput", false},
                    { "MaxVal", (double)27}
                });
            return AlphabetEncoder;
        }
        //************************************************************************************************

        //*************************TESTING METHODS*******************************************
        
        /// <summary>
        /// FOR PASSENGER COUNT PREDICTION EXPERIMENT
        /// </summary>
        /// <param name="trainingData">TRAINING DATA</param>
        /// <param name="trained_CortexLayer">TRAINED HTM CORTEX LAYER</param>
        /// <param name="trained_Classifier">TRAINED HTM CLASSIFIER</param>
        public static void BeginAutomatedTestingExperiment_1(List<Dictionary<string, string>> trainingData, CortexLayer<object, object> layer, HtmClassifier<string, ComputeCycle> cls)
        {
            //** We will use 40% of trainingData for tesing.
            Random rnd = new Random();

            var testingResults = new List<string>();

            var numberOfDataPoints = (int)(0.7 * trainingData[0].Count);
            int correctPrediction = 0;

            //FORMULAE -> Summation[Predictied-true]/numberOfDataPoints
            double MeanAbsoluteError = 0.0;


            for (int i = 0; i < numberOfDataPoints; i++)
            {
                var index = rnd.Next(1, trainingData[0].Count - 1); // WE ARE USING RANGE FROM 1 because FIRST ELEMENT OF SEQUENCE CANNOT BE PREDICTED *******
                var sequenceElement = trainingData[0].ElementAt(index);

                var testInput = sequenceElement.Value;
                var testLabel = trainingData[0].ElementAt(index + 1).Key;

                var sdr = EncodeSingleInput_testingExperiment_1(testInput);
                var lyrOutput = layer.Compute(sdr, false) as ComputeCycle;
                var predictedValues = cls.GetPredictedInputValues(lyrOutput.PredictiveCells.ToArray(), 3);
                Boolean IsPredictionCorrect = false;
                foreach (var prediction in predictedValues)
                {
                    testingResults.Add($"Actual Value : {testLabel} \t Predicted Value:{prediction.PredictedInput}");
                    Console.WriteLine($"Actual Value : {testLabel} \t Predicted Value:{prediction.PredictedInput}");
                    if (prediction.PredictedInput == testLabel)
                    {
                        IsPredictionCorrect = true;
                        break;
                    }
                    else
                    {
                        MeanAbsoluteError += (double.Parse(prediction.PredictedInput) - double.Parse(testLabel));
                    }
                }


                if (IsPredictionCorrect)
                {
                    correctPrediction++;
                }

            }
            if (MeanAbsoluteError < 0) { MeanAbsoluteError = 0 - MeanAbsoluteError; }
            MeanAbsoluteError = MeanAbsoluteError / trainingData[0].Count;
            Console.WriteLine($"Mean Absolute Error : {MeanAbsoluteError}");

            double accuracy = ((double)correctPrediction / numberOfDataPoints) * 100;
            Console.WriteLine($"AUTOMATED TESTING COMPLELETED");
            Console.WriteLine($"ACCURACY OBTAINED: {accuracy}");
            testingResults.Add($"ACCURACY OBTAINED: {accuracy}");

            DateTime now = DateTime.Now;
            long n = long.Parse(now.ToString("yyyyMMddHHmmss"));
            string filename = "CancerSequenceAutomatedTesting" + now.ToString("g").Split(" ")[0] + n + ".txt";
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\TestingLogs\\" + filename;
            using (StreamWriter swOutput = File.CreateText(path))
            {
                swOutput.WriteLine($"{filename}");
                foreach (var result in testingResults)
                {
                    swOutput.WriteLine(result);
                }
            }

        }

        /// <summary>
        ///  CANCER SEQUENCE CLASSIFICATION EXPERIMENT V1
        /// </summary>
        /// <param name="trainingData">TRAINING DATA</param>
        /// <param name="trained_CortexLayer">TRAINED HTM CORTEX LAYER</param>
        /// <param name="trained_Classifier">TRAINED HTM CLASSIFIER</param>
        public static void BeginAutomatedTestingExperiment_2(List<Dictionary<string, string>> trainingData, CortexLayer<object, object> trained_CortexLayer, HtmClassifier<string, ComputeCycle> trained_Classifier)
        {
            Random rnd = new Random();

            var testingResults = new List<string>();

            var numberOfDataPoints = (int)(0.7 * trainingData.Count);
            int correctPrediction = 0;
            for (int i = 0; i < numberOfDataPoints; i++)
            {
                var index = rnd.Next(0, trainingData.Count - 1);
                var sequence = trainingData[index];
                Dictionary<string, List<string>> predictedInput = new Dictionary<string, List<string>>();

                List<string> possibleClasses = new List<string>();

                for (int j = 0; j < sequence.Count; j++)
                {

                    var element = sequence.ElementAt(j);
                    var elementLabel = element.Value;

                    // WHILE CREATING TRAINING UNIQUE KEY INDEXING PROCESS
                    // INVOLVES INCLUDING "originalKey +,{Number} " , therefore To fetch 
                    // original key we are spliting the element key.
                    // example A,7  F,23 and so on.
                    var elementString = element.Key.Split(",")[0];

                    var elementSDR = EncodeSingleInput_testingExperiment_2(elementString, true);

                    var lyr_Output = trained_CortexLayer.Compute(elementSDR[0], false) as ComputeCycle;
                    var classifierPrediction = trained_Classifier.GetPredictedInputValues(lyr_Output.PredictiveCells.ToArray(), 5);

                    // CHECK IF INPUT HAS PRODUCED ANY OUTPUTS
                    if (classifierPrediction.Count > 0)
                    {
                        foreach (var prediction in classifierPrediction)
                        {
                            if (j < sequence.Count - 1)
                            {
                                var nextElement = sequence.ElementAt(j + 1);
                                var nextElementString = nextElement.Key.Split(",")[0];
                                if (prediction.PredictedInput.Split(",")[0] == nextElementString)
                                {
                                    possibleClasses.Add(nextElement.Value);
                                }
                            }
                        }

                    }
                }

                var Classcounts = possibleClasses.GroupBy(x => x)
                    .Select(g => new { possibleClass = g.Key, Count = g.Count() })
                    .ToList();
                var possibleClass = "";
                if (Classcounts.Count > 0)
                    possibleClass = Classcounts.Max().possibleClass;

                var elmentClass = sequence.ElementAt(0).Value;
                testingResults.Add($"ELEMENT CLASS :{elmentClass} \t PREDICTED CLASS:{possibleClass}");
                if (possibleClass.Split("_")[0] == elmentClass.Split("_")[0])
                {
                    correctPrediction++;
                }
            }
            
            //************************************PREDICTION ACCURACY************************************************************
            double accuracy = ((double)correctPrediction / numberOfDataPoints) * 100;

            Console.WriteLine($"ACCURACY : {accuracy} \t NumberOfCorrect_Prediction : {correctPrediction} \t TotalDataPoints:{numberOfDataPoints}");
            testingResults.Add($"ACCURACY : {accuracy} \t NumberOfCorrect_Prediction : {correctPrediction} \t TotalDataPoints:{numberOfDataPoints}");

        }
    }
}
