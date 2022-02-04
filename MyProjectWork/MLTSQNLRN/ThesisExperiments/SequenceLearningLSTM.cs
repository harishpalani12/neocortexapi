using CNTKUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Linq;
using CNTK;

namespace ThesisExperiments
{
    class SequenceClassficationLSTM
    {
        /// <summary>
        /// TRAINING FILE PATH
        /// </summary>
        static readonly string PassengerCountDataFile_MINI = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"\TrainingFiles\TaxiPassengerCountPrediction\TrainingFile_MINI.csv");
        static readonly string PassengerCountDataFile_FULL = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"\TrainingFiles\TaxiPassengerCountPrediction\TrainingFile_FULL.csv");

        static readonly string CancerSequenceDataFile = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"\TrainingFiles\CancerSequenceClassification\BreastCancer_trainingFile_MINI.csv");
        static readonly string CancerSequenceDataFile2 = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"\TrainingFiles\CancerSequenceClassification\LungCancer_trainingFile.csv");

        static readonly string[] CancerSequenceClasses = new string[] { "inactive - exp", "mod. active", "very active", "inactive - virtual" };
        static readonly float[][] CancerSequenceClassesOneHotEncoding = new float[][] { new float[] { 0, 0, 0, 1 }, new float[] { 0, 0, 1, 0 }, new float[] { 0, 1, 0, 0 }, new float[] { 1, 0, 0, 0 } };

        //**********************************************************************************

        /// <summary>
        ///  Passenger Count Prediction Experiment EntryPoint
        /// </summary>
        public void InitiatePassengerCountPredictionExperiment()
        {

            var trainingData = HelperMethods.ReadPassengerDataFromFile(PassengerCountDataFile_FULL);
            var processedData = HelperMethods.PassengerCountDataProcessing(trainingData[0]);

            RunLSTM_1(processedData.Values.ElementAt(0), processedData.Keys.ElementAt(0));
        }
        /// <summary>
        /// Cancer Sequence Classification Experiment EntryPoint
        /// </summary>
        public void InitiateCancerSequenceClassificationExperimentV1()
        {
            var trainingData = HelperMethods.ReadCancerSequencesDataFromFile(CancerSequenceDataFile);
            var processedData = HelperMethods.CancerSequenceDataProcessing(trainingData);

            RunLSTM_2(processedData.Keys.ElementAt(0), processedData.Values.ElementAt(0));
        }
        /// <summary>
        /// Cancer Sequence Classification Experiment EntryPoint
        /// </summary>
        public void InitiateCancerSequenceClassificationExperimentV2()
        {
            var trainingData = HelperMethods.ReadCancerSequencesDataFromFile_LSTM_v2(CancerSequenceDataFile);   
            RunLSTM_2(trainingData.Keys.ElementAt(0), trainingData.Values.ElementAt(0));
        }        
        /// <summary>
        /// RUN V1 takes Single Sequence as INPUT.
        /// </summary>
        /// <param name="data">Processed Data</param>
        /// <param name="label">Observation Label</param>
        public static void RunLSTM_1(float[][] data, float[] label)
        {
            Console.WriteLine($"Passenger Count Prediction Experiment: LSTM ");
            var type = CNTK.DataType.Float;
            var features = NetUtil.Var(new int[] { 1 }, type);
            var labels = NetUtil.Var(new int[] { 1 }, type);

            // process_Data_LSTM_for_np();
            // build a regression model
            var lstmUnits = 2048;

            var network = features
                .LSTM(50, 50)
                .Dense(1)
                .ToNetwork();

            Console.WriteLine("Model architecture:");

            Console.WriteLine("Model architecture:");

            Console.WriteLine(network.ToSummary());

            // set up the loss function and the classification error function

            var lossFunc = NetUtil.MeanSquaredError(network.Output, labels);
            var errorFunc = NetUtil.MeanAbsoluteError(network.Output, labels);

            // set up a trainer
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.001, 1),
                momentumSchedule: (0.9, 1),
                unitGain: false);

            // set up a trainer and an evaluator
            var trainer = network.GetTrainer(learner, lossFunc, errorFunc);

            // train the model
            Console.WriteLine("Epoch\tTrain\tTrain");
            Console.WriteLine("\tLoss\tError");
            Console.WriteLine("-----------------------");
            //int minibatchSize = 64;
            //int numMinibatchesToTrain = 1000;

            var maxEpochs = 1024; // 50;
            var batchSize = (int)(0.7 * label.Length); // 32;
            var loss = new double[maxEpochs];
            var trainingError = new double[maxEpochs];
            var testingError = new double[5];
            var batchCount = 0;
            //var fetchDataBatch = Data_conversion_LSTM();
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                // train one epoch on batches
                loss[epoch] = 0.0;
                trainingError[epoch] = 0.0;
                batchCount = 15;
                data.Index().Shuffle().Batch(batchSize, (indices, begin, end) =>
                {
                    // get the current batch
                    var featureBatch = features.GetBatch(data, indices, begin, end);
                    var labelBatch = labels.GetBatch(label, indices, begin, end);

                    // train the regression model on the batch
                    var result = trainer.TrainBatch(
                        new[] {
                            (features, featureBatch),
                            (labels,  labelBatch)
                    },
                    false
                );

                    loss[epoch] += result.Loss;
                    trainingError[epoch] += result.Evaluation;
                    batchCount++;
                });

                // show results
                loss[epoch] /= batchCount;
                trainingError[epoch] /= batchCount;

                Console.WriteLine($"{epoch}\t{loss[epoch]:F3}\t{trainingError[epoch]:F3}");
            }
            // show final results
            var finalError = trainingError[maxEpochs - 1];
            Console.WriteLine();
            Console.WriteLine($"Final MAE: {finalError:0.00}");
            string modelpath = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + "LSTM_model\\model");
            trainer.SaveCheckpoint(modelpath);

            //trainer.RestoreFromCheckpoint(@"C:\Users\Itachi_yv\source\repos\thesis_HTM_VS_LSTM\HTM VS LSTM\HTM VS LSTM\HTM VS LSTM\LSTM_1_");
            ////network.Restore(@"C:\Users\Itachi_yv\source\repos\thesis_HTM_VS_LSTM\HTM VS LSTM\HTM VS LSTM\HTM VS LSTM\LSTM_1_");
            ///
            var result_ = 0.0;
            for (var epoch = 0; epoch < 5; epoch++)
            {
                data.Index().Shuffle().Batch(batchSize, (indices, begin, end) =>
                {

                    // get the current batch
                    var featureBatch = features.GetBatch(data, indices, begin, end);
                    var labelBatch = labels.GetBatch(label, indices, begin, end);

                    var result = trainer.TestBatch(
                    new[] { (features, featureBatch),
                    (labels, labelBatch)
                    });
                    Console.WriteLine($"{epoch}\t Network Accuracy {result / 5}:F3");
                    result_ += result;
                });

            }

            DateTime now = DateTime.Now;
            string filename = "LSTM_LOGS" + now.ToString("g").Split(" ")[0] + now.Ticks.ToString() + ".txt";
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\TrainingLogs\\" + filename;
            using (StreamWriter swOutput = File.CreateText(path))
            {
                swOutput.WriteLine($"{filename}");
                swOutput.WriteLine("LOSS EPOCH-------------TRAIN ERROR");
                for (int i = 0; i < trainingError.Length; i++)
                {
                    swOutput.WriteLine($"{loss[i]}----------{trainingError[i]}");
                }

                swOutput.WriteLine("TESTING BATCH MEAN ABSOLUTE ERROR");
                swOutput.WriteLine($"Network Accuracy {result_ / 5}:F3");
            }

        }
        /// <summary>
        /// RUN V2 takes Multiple Sequence as INPUT.
        /// </summary>
        /// <param name="data">Processed Data</param>
        /// <param name="label">Observation Label</param>
        public void RunLSTM_2(float[][] data, float[][] label)
        {
            var type = CNTK.DataType.Float;
            var features = NetUtil.Var(new int[] { 1 }, type);
            var labels = NetUtil.Var(new int[] { 4 }, type);

            // process_Data_LSTM_for_np();
            // build a regression model
            var lstmUnits = 2048;

            var network = features
                .LSTM(50, 50)
                .Dense(1)
                .ToNetwork();

            Console.WriteLine("Model architecture:");

            Console.WriteLine("Model architecture:");

            Console.WriteLine(network.ToSummary());

            // set up the loss function and the classification error function

            var lossFunc = NetUtil.MeanSquaredError(network.Output, labels);
            var errorFunc = NetUtil.MeanAbsoluteError(network.Output, labels);

            // set up a trainer
            var learner = network.GetAdamLearner(
                learningRateSchedule: (0.001, 1),
                momentumSchedule: (0.9, 1),
                unitGain: false);

            // set up a trainer and an evaluator
            var trainer = network.GetTrainer(learner, lossFunc, errorFunc);

            // train the model
            Console.WriteLine("Epoch\tTrain\tTrain");
            Console.WriteLine("\tLoss\tError");
            Console.WriteLine("-----------------------");
            //int minibatchSize = 64;
            //int numMinibatchesToTrain = 1000;

            var maxEpochs = 1024; // 50;
            var batchSize = (int)(0.7 * label.Length); // 32;
            var loss = new double[maxEpochs];
            var trainingError = new double[maxEpochs];
            var testingError = new double[5];
            var batchCount = 0;
            //var fetchDataBatch = Data_conversion_LSTM();
            for (int epoch = 0; epoch < maxEpochs; epoch++)
            {
                // train one epoch on batches
                loss[epoch] = 0.0;
                trainingError[epoch] = 0.0;
                batchCount = 15;
                data.Index().Shuffle().Batch(batchSize, (indices, begin, end) =>
                {
                    // get the current batch
                    var featureBatch = features.GetBatch(data, indices, begin, end);
                    var labelBatch = labels.GetBatch(label, indices, begin, end);

                    // train the regression model on the batch
                    var result = trainer.TrainBatch(
                        new[] {
                            (features, featureBatch),
                            (labels,  labelBatch)
                    },
                    false
                );

                    loss[epoch] += result.Loss;
                    trainingError[epoch] += result.Evaluation;
                    batchCount++;
                });

                // show results
                loss[epoch] /= batchCount;
                trainingError[epoch] /= batchCount;

                Console.WriteLine($"{epoch}\t{loss[epoch]:F3}\t{trainingError[epoch]:F3}");
            }
            // show final results
            var finalError = trainingError[maxEpochs - 1];
            Console.WriteLine();
            Console.WriteLine($"Final MAE: {finalError:0.00}");
            string modelpath = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + "LSTM_model\\model");
            trainer.SaveCheckpoint(modelpath);

            //trainer.RestoreFromCheckpoint(@"C:\Users\Itachi_yv\source\repos\thesis_HTM_VS_LSTM\HTM VS LSTM\HTM VS LSTM\HTM VS LSTM\LSTM_1_");
            ////network.Restore(@"C:\Users\Itachi_yv\source\repos\thesis_HTM_VS_LSTM\HTM VS LSTM\HTM VS LSTM\HTM VS LSTM\LSTM_1_");
            ///
            var result_ = 0.0;
            for (var epoch = 0; epoch < 5; epoch++)
            {
                data.Index().Shuffle().Batch(batchSize, (indices, begin, end) =>
                {

                    // get the current batch
                    var featureBatch = features.GetBatch(data, indices, begin, end);
                    var labelBatch = labels.GetBatch(label, indices, begin, end);

                    var result = trainer.TestBatch(
                    new[] { (features, featureBatch),
                    (labels, labelBatch)
                    });
                    Console.WriteLine($"{epoch}\t Network Accuracy {result / 5}:F3");
                    result_ += result;
                });

            }

            DateTime now = DateTime.Now;
            string filename = "LSTM_LOGS" + now.ToString("g").Split(" ")[0] + now.Ticks.ToString() + ".txt";
            string path = System.AppDomain.CurrentDomain.BaseDirectory + "\\TrainingLogs\\" + filename;
            using (StreamWriter swOutput = File.CreateText(path))
            {
                swOutput.WriteLine($"{filename}");
                swOutput.WriteLine("LOSS EPOCH-------------TRAIN ERROR");
                for (int i = 0; i < trainingError.Length; i++)
                {
                    swOutput.WriteLine($"{loss[i]}----------{trainingError[i]}");
                }

                swOutput.WriteLine("TESTING BATCH MEAN ABSOLUTE ERROR");
                swOutput.WriteLine($"Network Accuracy {result_ / 5}:F3");
            }
        }
    }
}
