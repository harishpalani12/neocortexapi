using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;


using NeoCortexApi;
using NeoCortexApi.Encoders;
using NeoCortexApi.Utility;
using NeoCortexApi.Entities;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Network;

namespace SimpleMultiSequenceLearning
{
    public class MyHelperMethod
    {
        static readonly string[] SequenceClasses = new string[] { "inactive - exp", "mod. active", "very active", "inactive - virtual" };
        static readonly float[][] SequenceClassesOneHotEncoding = new float[][] { new float[] { 0, 0, 0, 1 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 1, 0, 0 }, new float[] { 1, 0, 0, 0 } };

        public static List<Dictionary<string, string>> ReadSequencesDataFromCSV(string dataFilePath)
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
        ///     Encoding Alphabetic Sequences
        /// </summary>
        /// <param name="trainingData"></param>
        /// <returns></returns>
        /// 
        public static List<Dictionary<string, int[]>> TrainEncodeSequencesFromCSV(List<Dictionary<string, string>> trainingData)
        {
            List<Dictionary<string, int[]>> ListOfEncodedTrainingSDR = new List<Dictionary<string, int[]>>();

            ScalarEncoder encoder_Alphabets = FetchAlphabetEncoder();

            foreach (var sequence in trainingData)
            {
                int keyForUniqueIndex = 0;
                var tempDictionary = new Dictionary<string, int[]>();

                foreach (var element in sequence)
                {
                    keyForUniqueIndex++;
                    var elementLabel = element.Key + "," + element.Value;
                    var elementKey = element.Key;
                    int[] sdr = new int[0];
                    sdr = sdr.Concat(encoder_Alphabets.Encode(char.ToUpper(element.Key.ElementAt(0)) - 64)).ToArray();
                   
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

        public static List<int[]> PredictInputSequence(string userInput, Boolean EncodeSingleAlphabet)
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


        /*public static List<Dictionary<string, string>> ReadImageDataSets(string dataFilePath)
        {
            List<Dictionary<string, string>> SequencesCollection = new List<Dictionary<string, string>>();

            var directories = Directory.GetDirectories(dataFilePath).ToList();

            (Dictionary<string, int[]> binaries,Dictionary<string, List<string>> inputsPath ) = imageBinarization(directories, width, height);


            int keyForUniqueIndexes = 0;

            if (File.Exists(dataFilePath))
            {
                foreach (KeyValuePair<string, List<string>> entry in inputsPath)
                {
                    var filePathList = entry.Value;
                    var numberOfImages = filePathList.Count;
                }
                    

                using (StreamReader sr = new StreamReader(dataFilePath))
                {
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        string[] values = line.Split(",");

                        Dictionary<string, string> Sequence = new Dictionary<string, string>();

                        string label = values[1];
                        string sequenceString = values[0];

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
            return null;*/











/*
            //////////////////////////////////////////////////////////////////////////
            string filename = Path.GetFileName(InputApple_1);

            string inputImage = Path.Combine("TestFiles", filename);

            //Image image1 = Image.FromFile(inputImage);

            ImageEncoder imageEncoder = new ImageEncoder(new BinarizerParams { ImageWidth = 20, ImageHeight = 40 });

            int[] encodedValue = imageEncoder.Encode(InputApple_1);

            imageEncoder.EncodeAndSaveAsImage(InputApple_1, $"encodedImage_{filename}");

            imageEncoder.EncodeAndSave(InputApple_1, $"encodedImage_{Path.GetFileNameWithoutExtension(filename)}.txt");
            *//*  
              imageEncoder.EncodeAndSaveAsImage(InputApple_1, OutputApple_1, "Bmp");

              imageEncoder.EncodeAndSave(InputApple_1, OutputApple_1);
            *//*

            Console.WriteLine("EncodeAndSaveAsImage.....");*/

        //}

        public string EnsureFolderExist(string foldername)
        {
            if (!Directory.Exists(foldername))
            {
                Directory.CreateDirectory(foldername);
            }

            while (!Directory.Exists(foldername))
            {
                Thread.Sleep(250);
            }

            return foldername;
        }
    }
}
