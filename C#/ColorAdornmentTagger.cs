//***************************************************************************
//
//    Copyright (c) Microsoft Corporation. All rights reserved.
//    This code is licensed under the Visual Studio SDK license terms.
//    THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
//    ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
//    IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
//    PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//***************************************************************************

// This controls whether the adornments are positioned next to the hex values or instead of them.
#define HIDING_TEXT

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

namespace IntraTextAdornmentSample
{
    /// <summary>
    /// Provides color swatch adornments in place of color constants.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a sample usage of the <see cref="IntraTextAdornmentTagTransformer"/> utility class.
    /// </para>
    /// </remarks>
    internal sealed class ColorAdornmentTagger


        : IntraTextAdornmentTagTransformer<ColorTag, ColorAdornment>


    {
        internal static ITagger<IntraTextAdornmentTag> GetTagger(IWpfTextView view, Lazy<ITagAggregator<ColorTag>> colorTagger)
        {
            return view.Properties.GetOrCreateSingletonProperty<ColorAdornmentTagger>(
                () => new ColorAdornmentTagger(view, colorTagger.Value));
        }


        private ColorAdornmentTagger(IWpfTextView view, ITagAggregator<ColorTag> colorTagger)
            : base(view, colorTagger)
        {
        }

        public override void Dispose()
        {
            base.view.Properties.RemoveProperty(typeof(ColorAdornmentTagger));
        }


        protected override ColorAdornment CreateAdornment(ColorTag dataTag, SnapshotSpan span)
        {
            return new ColorAdornment(dataTag);
        }

        protected override bool UpdateAdornment(ColorAdornment adornment, ColorTag dataTag)
        {
            adornment.Update(dataTag);
            return true;
        }
    }
}
