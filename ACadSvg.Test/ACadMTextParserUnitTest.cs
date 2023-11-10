#region copyright LGPL nanoLogika
//  Copyright 2023, nanoLogika GmbH.
//  All rights reserved. 
//  This source code is licensed under the "LGPL v3 or any later version" license. 
//  See LICENSE file in the project root for full license information.
#endregion

using ACadSvg;
using ACadSharp.Tables;
using SvgElements;


namespace ACadSvg.Test {

    public class ACadMTextParserUnitTest {

        [Fact]
        public void TestParagraphs() {

            string mText = @"Andra moi ennepe mousa" +
                @"\Ppolytropon hos mala polla" +
                @"\Pplanchtei epei Troies hieron ptolietron epersen.";

            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);

            Assert.Equal(3, tspans.Count);
            Assert.Equal("Andra moi ennepe mousa", tspans[0].Value);
            Assert.Equal("polytropon hos mala polla", tspans[1].Value);
            Assert.Equal("planchtei epei Troies hieron ptolietron epersen.", tspans[2].Value);
        }


        [Fact]
        public void TestFont() {

            string mText = @"{\fArial|b1|i0|c0|p34;1 %}";
            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);

            Assert.Equal(1, tspans.Count);
            Assert.Equal("<tspan x=\"100\" y=\"-100\" font-family=\"Arial\" font-weight=\"bold\">1 %</tspan>", tspans[0].ToString());
        }


        [Fact]
        public void TestSubSpanFont() {

            string mText = @"Slope is {\fArial|b1|i0|c0|p34;1 %}.";
            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);

            Assert.Equal(3, tspans.Count);
            Assert.Equal("<tspan x=\"100\" y=\"-100\">Slope is </tspan>", tspans[0].ToString());
            Assert.Equal("<tspan font-family=\"Arial\" font-weight=\"bold\">1 %</tspan>", tspans[1].ToString());
            Assert.Equal("<tspan>.</tspan>", tspans[2].ToString());
        }


        [Fact]
        public void TestSubSpanUnderline() {

            string mText = @"This is \Lunderline\l text.";
            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);

            Assert.Equal(3, tspans.Count);
            Assert.Equal("<tspan x=\"100\" y=\"-100\">This is </tspan>", tspans[0].ToString());
            Assert.Equal("<tspan text-decoration=\"underline\">underline</tspan>", tspans[1].ToString());
            Assert.Equal("<tspan> text.</tspan>", tspans[2].ToString());
        }


        [Fact]
        public void TestSubSpanCrazy() {

            string mText = @"Normal {\fArial|b0|i1|c0|p34;Italic \fArial|b0|i0|c0|p34;\KStrikeout \L\kUnderline \fArial|b1|i0|c0|p34;\lBold \fArial|b0|i0|c0|p34;\OOverstrike\fArial|b1|i0|c0|p34;\o \fArial|b1|i1|c0|p34;\L\KMixed^J}next line";
            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);
        }


        [Fact]
        public void TestSubSpanOverstrike() {

            string mText = @"This is \Oline-through\o text.";
            var textStyle = new TextStyle("TestTextStyle");

            IList<TspanElement> tspans = TextUtils.ConvertMTextToHtml(100, 100, mText, 30, textStyle);

            Assert.Equal(3, tspans.Count);
            Assert.Equal("<tspan x=\"100\" y=\"-100\">This is </tspan>", tspans[0].ToString());
            Assert.Equal("<tspan text-decoration=\"line-through\">line-through</tspan>", tspans[1].ToString());
            Assert.Equal("<tspan> text.</tspan>", tspans[2].ToString());
        }
    }
}