namespace GetPattern.Models
{
    public class FunctionCall
    {
        public string FunctionName { get; }
        public Dictionary<string, string> Parameters { get; }

        public FunctionCall(string functionName, Dictionary<string, string> parameters)
        {
            FunctionName = functionName;
            Parameters = parameters;
        }
    }
}
