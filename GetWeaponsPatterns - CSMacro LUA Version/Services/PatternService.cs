using GetPattern.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GetPattern.Services
{
    public class PatternService
    {
        public List<FunctionCall> GetFunctionCallsWithParameters(string code, string functionName)
        {
            var functionCalls = new List<FunctionCall>();
            int index = 0;

            while ((index = code.IndexOf(functionName, index)) != -1)
            {
                int startIndex = code.IndexOf("(", index) + 1;
                int endIndex = code.IndexOf(")", startIndex);
                string parametersString = code.Substring(startIndex, endIndex - startIndex);
                string[] parameters = parametersString.Split(',');

                if (parameters.Length == 2)
                {
                    var parameterValues = new Dictionary<string, string>
                {
                    { "x", parameters[0].Trim() },
                    { "y", parameters[1].Trim() }
                };
                    functionCalls.Add(new FunctionCall(functionName, parameterValues));
                }
                else
                {
                    throw new FormatException("Invalid parameter format.");
                }

                index += functionName.Length;
            }

            return functionCalls;
        }

        public double ParseNumericValue(string value)
        {
            // Assuming the value is in the format "number*identifier", like "2*CSMacro"
            string[] parts = value.Split('*');
            if (parts.Length == 2 && double.TryParse(parts[0], out double numericValue))
            {
                return numericValue;
            }
            else
            {
                throw new FormatException($"Invalid numeric value format: {value}");
            }
        }
    }
}
