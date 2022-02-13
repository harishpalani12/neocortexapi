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

        public static List<Dictionary<string, string>> ReadSequencesData(string dataFilePath)
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
        public static List<Dictionary<string, int[]>> EncodeSequences(List<Dictionary<string, string>> trainingData)
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
    }
}