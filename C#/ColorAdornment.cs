using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace IntraTextAdornmentSample
{
    internal sealed class ColorAdornment : StackPanel
    {
        #region Constructors
        internal ColorAdornment(ColorTag colorTag)
        {
            Update(colorTag);

            Orientation = Orientation.Horizontal;
        }
        #endregion

        #region Methods
        internal void Update(ColorTag colorTag)
        {
            UpdateText(colorTag);
        }

        void UpdateText(ColorTag variableInfo)
        {
            Children.Clear();

            if (variableInfo.VariableTypeName != null)
            {
                Children.Add(new TextBlock
                {
                    Foreground = Brushes.Blue,
                    FontStyle  = FontStyles.Italic,
                    FontFamily = new FontFamily("Consolas"),
                    Text       = variableInfo.VariableTypeName,
                    FontWeight = FontWeights.Bold
                });

               
            }

            if (variableInfo.VariableName != null)
            {
                Children.Add(new TextBlock
                {
                    FontStyle  = FontStyles.Italic,
                    FontFamily = new FontFamily("Consolas"),
                    Text       = " " + variableInfo.VariableName + " = ",
                    FontWeight = FontWeights.Bold
                });
            }

            Children.Add(new TextBlock
            {
                FontStyle  = FontStyles.Italic,
                FontFamily = new FontFamily("Consolas"),
                Text       = variableInfo.AssignedValue + ";",
                FontWeight = FontWeights.Bold
            });
        }
        #endregion
    }
}