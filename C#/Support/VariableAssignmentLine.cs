using System;
using System.Linq;

namespace IntraTextAdornmentSample
{
    public class VariableAssignmentLine
    {
        public string VariableName { get; set; }
        public string VariableTypeName { get; set; }
        public string AssignedValue { get; set; }

        public static VariableAssignmentLine Parse(string line)
        {

            if (line == null)
            {
                return null;
            }


            var arr = line.Split('=');

            if (arr.Length != 2)
            {
                return null;
            }

            var namePart = arr[0]?.Trim().Split(' ').Where(x=>!string.IsNullOrWhiteSpace(x)).ToArray();


            VariableAssignmentLine variableAssignmentLine = null;

            if (namePart?.Length ==1 && namePart[0].Contains('.') == false)
            {
                variableAssignmentLine = new VariableAssignmentLine()
                {
                    VariableName     = namePart[0],
                };

            }
            else if (namePart?.Length == 2 )
            {
                variableAssignmentLine = new VariableAssignmentLine()
                {
                    VariableName     = namePart[1],
                    VariableTypeName = namePart[0]
                };

            }
            else
            {
                return null;
            }
            

            var valuePart = arr[1]?.Trim();
            if (string.IsNullOrWhiteSpace(valuePart))
            {
                return null;
            }
            if (valuePart.EndsWith(";")==false)
            {
                return null;
            }

            variableAssignmentLine.AssignedValue =  valuePart.Substring(0, valuePart.Length - 1);

            return variableAssignmentLine;

        }
    }
}