using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Dto
{
    public class CsvHeaderSetting
    {
        public float FontSize { get; set; } = 14F;
        public string FontFamily { get; set; }
        public string MarkupStart { get; set; } = "【";
        public string MarkupEnd { get; set; } = "】";
        public List<string> TargetItems { get; set; }
    }
}
