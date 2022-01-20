using MyMLConsoleAPP;

// Add input data
var sampleData = new MLModel1.ModelInput()
{
    Col0 = "This restaurant not good."
};

// Load model and predict output of sample data
var result = MLModel1.Predict(sampleData);

// If Prediction is 1, sentiment is "Positive"; otherwise, sentiment is "Negative"
string sentiment = result.Prediction == 1 ? "Positive" : "Negative";
Console.WriteLine($"Text: {sampleData.Col0}\nSentiment: {sentiment}");