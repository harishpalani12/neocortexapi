﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

using NeoCortexApi;
using NeoCortexApi.Classifiers;
using NeoCortexApi.Encoders;
using NeoCortexApi.Entities;
using NeoCortexApi.Network;
using NeoCortexApi.Utility;

using HtmImageEncoder;

using Daenet.ImageBinarizerLib.Entities;
using Daenet.ImageBinarizerLib;

using Newtonsoft.Json;

using SkiaSharp;

using static SimpleMultiSequenceLearning.MultiSequenceLearning;



namespace SimpleMultiSequenceLearning
{
    public class HelperMethod_Images
    {
        /// <summary>
        ///     Fetch Data Sequence from the File 
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> ReadImageDataSetsFromFolder(string dataFilePath)
        {
            Dictionary<string, List<string>> SequencesCollection = new Dictionary<string, List<string>>();

            if (Directory.Exists(dataFilePath))
            {
                foreach (var path in Directory.GetDirectories(dataFilePath))
                {
                    string label = Path.GetFileNameWithoutExtension(path);
                    List<string> list = new List<string>();
                    foreach (var file in Directory.GetFiles(path))
                    {
                        list.Add(file);
                    }
                    SequencesCollection.Add(label, list);
                }
            }
            return SequencesCollection;
        }

        public void BinarizeImageTraining(string InputPath, string OutputPath, int height, int width)
        {
            if (Directory.Exists(InputPath))
            {
                var trainingImageData2 = HelperMethod_Images.ReadImageDataSetsFromFolder(InputPath);

                Multiseq_ImageLearning multiseq_Image = new Multiseq_ImageLearning();
                var trained_HTM_modelImage = multiseq_Image.RunImage(trainingImageData2, height, width);

                foreach (var path in Directory.GetDirectories(InputPath))
                {
                    string label = Path.GetFileNameWithoutExtension(path);

                    foreach (var file in Directory.GetFiles(path))
                    {
                        string Outputfilename = Path.GetFileName(Path.Join(OutputPath, label, $"Binarized_{Path.GetFileName(file)}"));
                        ImageEncoder imageEncoder = new ImageEncoder(new BinarizerParams { InputImagePath = file, OutputImagePath = Path.Join(OutputPath, label), ImageWidth = height, ImageHeight = width });

                        imageEncoder.EncodeAndSaveAsImage(file, Outputfilename, "Png");

                        MultiSequenceLearning experiment = new MultiSequenceLearning();
                    }
                }

                Console.WriteLine("Input an Image Here.....(Drag and Drop image here) \n");

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                string TestingImage = (Console.ReadLine().Trim('"'));
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                Console.WriteLine("\n\n");

                if (TestingImage != null)
                {
                    trained_HTM_modelImage.Reset();
                    var res = trained_HTM_modelImage.Predict(TestingImage);

                    if (res.Count > 0)
                    {
                        foreach (var pred in res)
                        {
                            Debug.WriteLine($"PredictedInput = {pred.PredictedInput} <---> Similarity = {pred.Similarity}\n");
                        }
                        var tokens = res.First().PredictedInput.Split('_');
                        var tokens2 = res.First().PredictedInput.Split('-');
                        //Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2[tokens.Length - 3]}\n");
                        Console.WriteLine($"Predicted Sequence: {tokens[0]}\n");
                    }
                    else
                    {
                        Console.WriteLine("Invalid Match..... \n");
                    }
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("Invalid Input \n");
                }


            }
            else
            {
                Console.WriteLine("Please check the Directory Path");
            }
        }

        /// <summary>
        /// Convert int array to string for better representation
        /// </summary>
        /// <param name="computeResult"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        private string GetActiveCells(int[] computeResult)
        {
            string result = String.Join(",", computeResult);
            return result;
        }

        /// <summary>
        /// After Number Sequence is Learnt, PredictNextElement will carry out prediction of the elements from the
        /// Sequence which is input from the user 
        /// </summary>
        /// <param name="list"></param>
        public static void PredictImage(HtmPredictionEngine predictor, double[] list)
        {
            Console.WriteLine("-------------------Start of PredictNextElement Function------------------------");

            foreach (var item in list)
            {
                var res = predictor.Predict(item);
                if (res.Count > 0)
                {
                    foreach (var pred in res)
                    {
                        Debug.WriteLine($"PredictedInput = {pred.PredictedInput} <---> Similarity = {pred.Similarity}\n");
                    }
                    var tokens = res.First().PredictedInput.Split('_');
                    var tokens2 = res.First().PredictedInput.Split('-');
                    Console.WriteLine($"Predicted Sequence: {tokens[0]}, predicted next element {tokens2[tokens.Length - 1]}\n");
                }
                else
                    Console.WriteLine("Invalid Match..... \n");
            }
            Console.WriteLine("------------------------End of PredictNextElement ------------------------");
        }

        /// <summary>
        /// MultiSequenceLearning_Images will Read Images from Solution Folder and Binarize the images
        /// followed by training of Images
        /// </summary>
        /// <param name="InputPicPath"></param>
        /// <param name="OutputPicPath"></param>
        /// <param name="imageheight"></param>
        /// <param name="imagewidth"></param>
        public void MultiSequenceLearning_Images(string InputPicPath, string OutputPicPath, int imageheight, int imagewidth)
        {
            MultiSequenceLearning experiment = new MultiSequenceLearning();

            var trainingImageData2 = HelperMethod_Images.ReadImageDataSetsFromFolder(InputPicPath);
            BinarizeImageTraining(InputPicPath, OutputPicPath, imageheight, imagewidth);
        }
    }
}