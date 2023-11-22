#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using System.Text.RegularExpressions;

using ACadSharp;
using ACadSharp.Entities;
using ACadSharp.Tables;

using SvgElements;


namespace ACadSvg {

    public static class TextUtils {

        private const double TextSizeFromHeight = 0.92;


        public static IList<TspanElement> ConvertMTextToHtml(double x, double y, string value, double textSize, TextStyle style) {
            if (string.IsNullOrEmpty(value)) {
                return null;
            }

            //  Somtimes text contains simple line breaks ...
            //  replace by markup line break.
            value = value.Replace("\n", @"\P");

            return parseMText(value, x, -y, textSize);
        }


        public static string HorizontalAlignmentToTextAnchor(TextAttachmentPointType horizontalAlignment) {
            switch (horizontalAlignment) {
            case TextAttachmentPointType.Center:
                return "middle";

            case TextAttachmentPointType.Left:
                break;

            case TextAttachmentPointType.Right:
                return "end";
            }

            return string.Empty;
        }


        public static string HorizontalAlignmentToTextAnchor(TextHorizontalAlignment horizontalAlignment) {
            switch (horizontalAlignment) {
            case TextHorizontalAlignment.Center:
                return "middle";

            case TextHorizontalAlignment.Left:
                break;

            case TextHorizontalAlignment.Right:
                return "end";
            }

            return string.Empty;
        }


        public static string HorizontalAlignmentToTextAnchor(DimensionTextHorizontalAlignment horizontalAlignment) {
            switch (horizontalAlignment) {
            case DimensionTextHorizontalAlignment.Left:
                break;

            case DimensionTextHorizontalAlignment.Right:
                return "end";

            case DimensionTextHorizontalAlignment.Centered:
                return "middle";
            }

            return string.Empty;
        }


        public static string HorizontalAlignmentToTextAnchor(ACadSharp.Entities.AttachmentPointType attachmentPoint) {
            switch (attachmentPoint) {
            case ACadSharp.Entities.AttachmentPointType.TopLeft:
                return string.Empty;

            case ACadSharp.Entities.AttachmentPointType.TopCenter:
                return "middle";

            case ACadSharp.Entities.AttachmentPointType.TopRight:
                return "end";

            case ACadSharp.Entities.AttachmentPointType.MiddleLeft:
                return string.Empty;

            case ACadSharp.Entities.AttachmentPointType.MiddleCenter:
                return "middle";

            case ACadSharp.Entities.AttachmentPointType.MiddleRight:
                return "end";

            case ACadSharp.Entities.AttachmentPointType.BottomLeft:
                return string.Empty;

            case ACadSharp.Entities.AttachmentPointType.BottomCenter:
                return "middle";

            case ACadSharp.Entities.AttachmentPointType.BottomRight:
                return "end";

            default:
                return string.Empty;
            }
        }


        public static double AlignmentToVerticalAdjustment(ACadSharp.Entities.AttachmentPointType attachmentPoint, double textHeight) {
            switch (attachmentPoint) {
            case ACadSharp.Entities.AttachmentPointType.TopLeft:
            case ACadSharp.Entities.AttachmentPointType.TopCenter:
            case ACadSharp.Entities.AttachmentPointType.TopRight:
                return textHeight * 0.8;

            case ACadSharp.Entities.AttachmentPointType.MiddleLeft:
            case ACadSharp.Entities.AttachmentPointType.MiddleCenter:
            case ACadSharp.Entities.AttachmentPointType.MiddleRight:
                return textHeight * 0.4;

            case ACadSharp.Entities.AttachmentPointType.BottomLeft:
            case ACadSharp.Entities.AttachmentPointType.BottomCenter:
            case ACadSharp.Entities.AttachmentPointType.BottomRight:
                return 0;

            default:
                return textHeight;
            }
        }


        public static void StyleToValues(TextStyle textStyle, double textHeight, out string fontFamily, out double fontSize, out bool bold, out bool italic) {
            var trueType = textStyle.TrueType;
            string font = fontFallback(textStyle.Name);
            fontFamily = font;
            fontSize = textHeight;
            bold = trueType.HasFlag(FontFlags.Bold);
            italic = trueType.HasFlag(FontFlags.Italic);
        }


        //  TODO Does textStyle.Hight overrule other values?
        public static double GetTextSize(bool overrideTextHeight, double styleTextHeight, TextStyle textStyle, double scaleFactor) {
            if (textStyle.Height != 0) {
                return textStyle.Height * scaleFactor * TextSizeFromHeight;
            }
            else {
                return styleTextHeight * scaleFactor * TextSizeFromHeight;
            }
        }


        public static double GetTextSize(double textHeight) {
            return textHeight * TextSizeFromHeight;
        }


        private static IList<TspanElement> parseMText(string text, double x, double y, double textSize) {

            string pattern = @"(?<leadingText>[^\\{}]*)" +
                @"(?<code>" +
                @"\\A[0-2];" +
                @"|\\[f,F](?<font>[^{}]+)\|b(?<bold>(0|1))\|i(?<italic>(0|1))\|c(?<codePage>\d)\|p(?<pitch>\d+);" +
                @"|\\H\d+;" +
                @"|\\W\d+;" +
                @"|\\[c,C][1-7];" +
                @"|\\Q\d;" +
                @"|\\P" +
                @"|^J" +
                @"|\\O|\\o" +
                @"|\\L|\\l" +
                @"|\\K|\\k" +
                @"|{" +
                @"|}" +
                @")?" +
                "(?<remainder>.*)";

            double lineHeight = textSize * 1.2;
            List<TspanElement> tspans = new List<TspanElement>();
            TspanElement currentTspan = null;
            Regex p = new Regex(pattern);
            while (text.Length > 0) {
                Match m = p.Match(text);
                if (!m.Success) {
                    //  The remainder is simply a piece of text.
                    //  Add a <tspan> element containing the text, no attributes.
                    TspanElement tspan = new TspanElement();
                    tspan.Value = text;
                    tspans.Add(tspan);
                    break;
                }

                string leadingText = m.Groups["leadingText"].Value;
                if (!string.IsNullOrEmpty(leadingText)) {
                    if (currentTspan == null) {
                        TspanElement tspan = new TspanElement();
                        tspan.Value = leadingText;
                        tspans.Add(tspan);
                    }
                    else if (currentTspan.Tspans.Count > 0){
                        TspanElement tspan = new TspanElement();
                        tspan.Value = leadingText;
                        currentTspan.AddChild(tspan);
                    }
                    else if (!string.IsNullOrEmpty(currentTspan.Value)) {

                    }
                    else {
                        currentTspan.Value = leadingText;
                    }
                }

                string code = m.Groups["code"].Value;
                if (string.IsNullOrEmpty(code)) {
                    //  No codes encountered, if there is text it was handeled as leading text, see above.
                    break;
                }

                if (code.Length == 1) {
                    switch (code) {
                    case "{":
                        //  start sub-tspan element
                        //  --> create <tspan> add it to current (if not null)
                        //      set sub-tspan as current
                        TspanElement tspan = new TspanElement();
                        if (currentTspan != null) {
                            currentTspan.AddChild(tspan);
                        }
                        else {
                            tspans.Add(tspan);
                        }
                        currentTspan = tspan;
                        break;
                    case "}":
                        //  close sub-tspan token and return to parent token
                        //  --> If current token was null, this code is ignored.
                        currentTspan = currentTspan.ParentElement;
                        break;
                    }
                }
                else {
                    string code2 = code.Substring(0, 2);
                    switch (code2) {
                    case @"\O":
                        //  start sub-tspan token
                        //  - ensure current tspan has no text, ceate sub-tspan with current text
                        //  - create new sub-tspan with Overstrike
                        currentTspan = ensureCurrentTspanForInlineCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Overstrike = true;
                        break;

                    case @"\o":
                        //  close sub-tspan token and return to parent token
                        //  --> return to parent tspan
                        if (currentTspan == null || !currentTspan.Overstrike) {
                            //  Ignore this code
                            break;
                        }
                        currentTspan = currentTspan.ParentElement;
                        break;

                    case @"\L":     //  Start underline
                        // create sub-tspan token
                        //  - ensure current tspan has no text, ceate sub-tspan with current text
                        //  - create new sub-tspan with Underline
                        currentTspan = ensureCurrentTspanForInlineCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Underline = true;
                        break;

                    case @"\l":
                        //  close sub-tspan token and return to parent token
                        //  --> return to parent tspan
                        if (currentTspan == null || !currentTspan.Underline) {
                            //  Ignore this code
                            break;
                        }
                        currentTspan = currentTspan.ParentElement;
                        break;

                    case @"\K":
                        //  start sub-tspan token
                        //  - ensure current tspan has no text, ceate sub-tspan with current text
                        //  - create new sub-tspan with Overstrike
                        currentTspan = ensureCurrentTspanForInlineCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Strikethrough = true;
                        break;

                    case @"\k":
                        //  close sub-tspan token and return to parent token
                        //  --> return to parent tspan
                        if (currentTspan == null || !currentTspan.Overstrike) {
                            //  Ignore this code
                            break;
                        }
                        currentTspan = currentTspan.ParentElement;
                        break;

                    case @"\A":     //  Alignment = 0, 1, 2; other values 3...9 default to 0
                        currentTspan = ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        //  set current-token attribute
                        currentTspan.Alignment = parseAlignmentValue(code);
                        break;

                    case @"\f":
                        //  \\f(?< font >\|b(?<bold>(0|1))\|i(?<italic>(0|1))\|c(?<codePage>\d)\|p(?<pitch>\d+);"
                        currentTspan = ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        currentTspan.FontFamily = fontFallback(m.Groups["font"].Value);
                        currentTspan.Bold = m.Groups["bold"].Value != "0";
                        currentTspan.Italic = m.Groups["italic"].Value != "0";
                        currentTspan.CodePage = m.Groups["codePage"].Value;
                        int pitch = int.Parse(m.Groups["pitch"].Value);
                        break;

                    case @"\H":     //  \H#.#;
                        //  set current-token attribute Height
                        currentTspan = ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Height = parseDouble(code);
                        break;

                    case @"\W":     //  \W#.#;
                        //  set current-token attribute Width
                        currentTspan = ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Width = parseDouble(code);
                        break;

                    case @"\C":
                        //  set current-token attribute Color
                        //  \C[1-7];
                        //  Must set the Fill
                        currentTspan =  ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        currentTspan.Fill = "aqua"; 
                        break;
                    case @"\c":
                        //  set current-token attribute Color
                        //  \C[1-7];
                        break;

                    case @"\Q":     //  \Q#.#   oblique/slant angle
                        //  set current-token attribute SlantAngle
                        currentTspan = ensureTspanForNextCommand(tspans, currentTspan, lineHeight);
                        currentTspan.SlantAngle = parseDouble(code);
                        break;

                    case "^J":      //  ^J  new Line
                    case @"\P":     //  \P  new paragraph
                        //  Close current token, create new one for next line
                        TspanElement precedingTspan = tspans.Count == 0 ? null : tspans[tspans.Count-1];
                        if (precedingTspan == null) {
                            currentTspan = new TspanElement(x, lineHeight);
                        }
                        else {
                            currentTspan = precedingTspan.Clone();
                            currentTspan.X = x;
                            currentTspan.Dy = lineHeight;
                        }
                        tspans.Add(currentTspan);
                        break;
                    }
                }

                text = m.Groups["remainder"].Value;
            }

            //  Ensure that first tspan has absolute location
            tspans[0].X = x;
            tspans[0].Y = y;
            tspans[0].Dy = null;
            return tspans;
        }


        private static TspanElement ensureCurrentTspanForInlineCommand(IList<TspanElement> tspans, TspanElement currentTspan, double height) {
            if (currentTspan != null) {
                if (!string.IsNullOrEmpty(currentTspan.Value)) {
                    string currentText = currentTspan.Value;
                    currentTspan.Value = null;
                    currentTspan.AddChild(new TspanElement() { Value = currentText });
                }
                TspanElement tspan = new TspanElement();
                currentTspan.AddChild(tspan);
                currentTspan = tspan;
            }
            else {
                currentTspan = new TspanElement();
                tspans.Add(currentTspan);
            }

            return currentTspan;
        }


        private static string fontFallback(string value) {
            if (value == "REHAU" || value == "Standard") {
                value = "Arial";
            }
            return value;
        }


        private static TspanElement ensureTspanForNextCommand(IList<TspanElement> tspans, TspanElement? currentTspan, double height) {
            if (currentTspan == null || !string.IsNullOrEmpty(currentTspan.Value)) {
                //  Start new token
                currentTspan = new TspanElement();
                tspans.Add(currentTspan);
            }

            return currentTspan;
        }


        private static double parseDouble(string code) {
            if (code.EndsWith(";")) {
                code = code.Substring(0, code.Length - 1);
            }
            code = code.Substring(2);
            double codeValue;
            if (!double.TryParse(code, out codeValue)) {
                return 0.0;
            }
            return codeValue;
        }


        private static string parseAlignmentValue(string code) {
            var alignment = code.Substring(2, 1);
            switch (alignment) {
            case "1":
                return "center";
            case "2":
                return "right";
            default:
                return "left";
            }
        }
    }
}
