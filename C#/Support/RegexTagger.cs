using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

namespace IntraTextAdornmentSample
{
    internal class RegexTagger<T> : ITagger<T> where T : class, ITag
    {
       

        #region Constructors
        public RegexTagger(ITextBuffer buffer)
        {
            // buffer.Changed += (sender, args) => HandleBufferChanged(args);
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Handle buffer changes. The default implementation expands changes to full lines and sends out
        ///     a <see cref="TagsChanged" /> event for these lines.
        /// </summary>
        /// <param name="args">The buffer change arguments.</param>
        protected virtual void HandleBufferChanged(TextContentChangedEventArgs args)
        {
            if (args.Changes.Count == 0)
            {
                return;
            }

            var temp = TagsChanged;
            if (temp == null)
            {
                return;
            }

           

            // Combine all changes into a single span so that
            // the ITagger<>.TagsChanged event can be raised just once for a compound edit
            // with many parts.

            var snapshot = args.After;

            var start = args.Changes[0].NewPosition;
            var end   = args.Changes[args.Changes.Count - 1].NewEnd;

            var totalAffectedSpan = new SnapshotSpan(
                                                     snapshot.GetLineFromPosition(start).Start,
                                                     snapshot.GetLineFromPosition(end).End);

            temp(this, new SnapshotSpanEventArgs(totalAffectedSpan));
        }

        //IEnumerable<ITextSnapshotLine> GetIntersectingLines(NormalizedSnapshotSpanCollection spans)
        //{
        //    if (spans.Count == 0)
        //    {
        //        yield break;
        //    }

        //    var lastVisitedLineNumber = -1;
        //    var snapshot = spans[0].Snapshot;
        //    foreach (var span in spans)
        //    {
        //        var firstLine = snapshot.GetLineNumberFromPosition(span.Start);
        //        var lastLine = snapshot.GetLineNumberFromPosition(span.End);

        //        for (var i = Math.Max(lastVisitedLineNumber, firstLine); i <= lastLine; i++)
        //        {
        //            yield return snapshot.GetLineFromLineNumber(i);
        //        }

        //        lastVisitedLineNumber = lastLine;
        //    }




        //}


        List<ITextSnapshotLine> GetIntersectingLines(NormalizedSnapshotSpanCollection spans)
        {
            if (spans.Count == 0)
            {
                return null;
            }

            var lines = new List<ITextSnapshotLine>();

            var lastVisitedLineNumber = -1;
            var snapshot              = spans[0].Snapshot;
            foreach (var span in spans)
            {
                var firstLine = snapshot.GetLineNumberFromPosition(span.Start);
                var lastLine  = snapshot.GetLineNumberFromPosition(span.End);

                for (var i = Math.Max(lastVisitedLineNumber, firstLine); i <= lastLine; i++)
                {
                    lines.Add(snapshot.GetLineFromLineNumber(i));
                }

                lastVisitedLineNumber = lastLine;
            }

            return lines;

        }

        #endregion

        #region ITagger implementation
        public virtual IEnumerable<ITagSpan<T>> GetTags(NormalizedSnapshotSpanCollection spans)
        {
            var returnList = new List<TagSpan<T>>();

            // Here we grab whole lines so that matches that only partially fall inside the spans argument are detected.
            // Note that the spans argument can contain spans that are sub-spans of lines or intersect multiple lines.
            // var textSnapshotLines = GetIntersectingLines(spans);

            if (spans.Count<=0)
            {
                return returnList;
            }

            var textSnapshotLines = spans[0].Snapshot.Lines.ToList();

            var lineCount = textSnapshotLines?.Count;

            Func<int, string> GetTextAtLine = (int i) => textSnapshotLines[i].GetText();

            for (var i = 0; i < lineCount; i++)
            {


                

                var line = textSnapshotLines[i];

                if (i + 4 >= textSnapshotLines.Count)
                {
                    continue;
                }

                var nextLine = GetTextAtLine(i + 1)?.Replace(" ", "");
                var isResponseCheck = nextLine?.StartsWith("if(!") == true &&
                                      nextLine.EndsWith(".Success)");

                if (isResponseCheck)
                {
                    //var response = bo.call();                    
                    //if (!response.Success)
                    //{
                    //    return returnObject.Add(response);       -> var x = bo.call();
                    //}
                    // var x = response.Value;

                    var leftBracketOfset = 2;
                    if (GetTextAtLine(i + leftBracketOfset)?.Trim() != "{")
                    {
                        continue;
                    }

                    var righttBracketOfset = 4;

                    if (GetTextAtLine(i + leftBracketOfset + 1)?.Trim().StartsWith("returnObject.Results.Add") == true &&
                        GetTextAtLine(i + leftBracketOfset + 2)?.Trim() == "return returnObject;" &&
                        GetTextAtLine(i + leftBracketOfset + 3)?.Trim() == "}")
                    {
                        righttBracketOfset =leftBracketOfset + 3;
                    }
                    else if (GetTextAtLine(i + leftBracketOfset + 1)?.Trim().StartsWith("return returnObject") == true &&
                             GetTextAtLine(i + leftBracketOfset + 2)?.Trim() == "}")
                    {
                        righttBracketOfset =  leftBracketOfset + 2;
                    }
                    else
                    {
                        continue;
                    }

                    var firstChar = GetTextAtLine(i + 1).First(c => c != ' ');

                    var firstCharIndex = GetTextAtLine(i + 1).IndexOf(firstChar);

                    var startPoint = new SnapshotPoint(line.Snapshot, line.Start + firstCharIndex);

                    var currentLine = GetTextAtLine(i);

                    var currentLineAsAssignmentLine = VariableAssignmentLine.Parse(currentLine);
                    if (currentLineAsAssignmentLine == null)
                    {
                        continue;
                    }

                    T tag;

                    // walk empty lines
                    var k = righttBracketOfset+1;
                    while (true)
                    {
                        if (i + k >= lineCount)
                        {
                            break;
                        }

                        if (!string.IsNullOrEmpty(GetTextAtLine(i + k)))
                        {
                            break;
                        }

                        k++;
                    }

                    var responseValueAssingmentToAnotherVariable = VariableAssignmentLine.Parse(GetTextAtLine(i + k));

                    if (responseValueAssingmentToAnotherVariable != null &&
                        currentLineAsAssignmentLine.VariableName + ".Value" == responseValueAssingmentToAnotherVariable.AssignedValue)
                    {
                        tag = new ColorTag(Colors.Red)
                        {
                            VariableTypeName = responseValueAssingmentToAnotherVariable.VariableTypeName,
                            VariableName     = responseValueAssingmentToAnotherVariable.VariableName,
                            AssignedValue    = currentLineAsAssignmentLine.AssignedValue
                        } as T;

                        if (tag != null)
                        {
                            var span = new SnapshotSpan(startPoint, textSnapshotLines[i + k].End);

                            returnList.Add(new TagSpan<T>(span, tag));
                            i = i + k;
                            continue;
                        }
                    }

                    tag = new ColorTag(Colors.Red)
                    {
                        AssignedValue = currentLineAsAssignmentLine.AssignedValue
                    } as T;

                    if (tag != null)
                    {
                        var span = new SnapshotSpan(startPoint, textSnapshotLines[i + righttBracketOfset].End);
                        returnList.Add(new TagSpan<T>(span, tag));
                        i = i + righttBracketOfset;
                    }
                }
            }

            return returnList;
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
        #endregion
    }
}