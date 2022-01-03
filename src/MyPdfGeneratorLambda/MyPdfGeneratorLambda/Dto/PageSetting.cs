using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Dto
{
    public class PageSetting
    {
        public string Size { get; set; }
        public string Orientation { get; set; }
        public Direction Margin { get; set; }
    }
}
