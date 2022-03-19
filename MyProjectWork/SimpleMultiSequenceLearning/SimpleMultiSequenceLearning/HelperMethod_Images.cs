using System;
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
                // Initialize HTMModules 
                int inputBits = height * width;
                int numColumns = 1024;
                HtmConfig cfg = new HtmConfig(new int[] { inputBits }, new int[] { numColumns });
                var mem = new Connections(cfg);

                SpatialPoolerMT sp = new SpatialPoolerMT();
                sp.Init(mem);

                var trainingImageData2 = HelperMethod_Images.ReadImageDataSetsFromFolder(InputPath);

                foreach (var path in Directory.GetDirectories(InputPath))
                {
                    string label = Path.GetFileNameWithoutExtension(path);

                    foreach (var file in Directory.GetFiles(path))
                    {
                        string Outputfilename = Path.GetFileName(Path.Join(OutputPath, label, $"Binarized_{Path.GetFileName(file)}"));
                        ImageEncoder imageEncoder = new ImageEncoder(new BinarizerParams { InputImagePath = file, OutputImagePath = Path.Join(OutputPath, label), ImageWidth = height, ImageHeight = width });

                        imageEncoder.EncodeAndSaveAsImage(file, Outputfilename, "Png");
                        /*
                        CortexLayer<object, object> layer1 = new CortexLayer<object, object>("L1");
                        layer1.HtmModules.Add("encoder", imageEncoder);
                        layer1.HtmModules.Add("sp", sp);

                        //Test Compute method
                        var computeResult = layer1.Compute(file, true) as int[];
                        var activeCellList = GetActiveCells(computeResult);
                        Debug.WriteLine($"Active Cells computed from Image {label}: {activeCellList}");
                        */

                        MultiSequenceLearning experiment = new MultiSequenceLearning();

                        var trained_HTM_modelImage = experiment.RunImageLearning(height, width, trainingImageData2, true, imageEncoder);
                    }
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


        public void MultiSequenceLearning_Images(string InputPicPath,string OutputPicPath,int imageheight, int imagewidth )
        {
            MultiSequenceLearning experiment = new MultiSequenceLearning();

            var trainingImageData2 = HelperMethod_Images.ReadImageDataSetsFromFolder(InputPicPath);
            BinarizeImageTraining(InputPicPath, OutputPicPath, imageheight, imagewidth);
        }
    }
}
