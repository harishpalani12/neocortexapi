using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSequenceLearning
{
    class SequenceLearningSTM
    {
        static readonly int numColumns = 2048;

        static readonly string LungCancerSequenceDataFile = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"Final Project\Sequence\Anticancer_Peptides\ACPs_Lung_cancer.csv");
        static readonly string BreastCancerSequenceDataFile = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + @"Final Project\Sequence\Anticancer_Peptides\ACPs_Breast_cancer.csv");

        public void InitiateCancerSequenceClassification()
        {

            int inputBits = 31;
            int maxCycles = 30;

        }


    }
}
