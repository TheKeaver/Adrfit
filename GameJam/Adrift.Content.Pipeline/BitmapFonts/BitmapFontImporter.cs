/**
MonoGame.Extended License:

The MIT License (MIT)

Copyright (c) 2015 Dylan Wilson

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
**/

using System.IO;
using System.Xml.Serialization;
using Adrift.Content.Common.BitmapFonts;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace Adrift.Content.Pipeline.BitmapFonts
{
    [ContentImporter(".fnt", DefaultProcessor = "BitmapFontProcessor",
         DisplayName = "[Adrift] BMFont Importer - MonoGame.Extended")]
    public class BitmapFontImporter : ContentImporter<BitmapFontFile>
    {
        public override BitmapFontFile Import(string filename, ContentImporterContext context)
        {
            context.Logger.LogMessage("Importing XML file: {0}", filename);

            using (var streamReader = new StreamReader(filename))
            {
                var deserializer = new XmlSerializer(typeof(BitmapFontFile));
                return (BitmapFontFile)deserializer.Deserialize(streamReader);
            }
        }
    }
}