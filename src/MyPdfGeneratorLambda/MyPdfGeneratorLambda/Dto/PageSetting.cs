using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Dto
{
    public class PageSetting
    {
        public string Size { get; set; } = "A4";
        public string Orientation { get; set; } = "縦向き";
        public Margin Margin { get; set; } = new Margin() {
            Top = 0F, Left = 0F, Right = 0F, Bottom = 0F
        };
    }
}
