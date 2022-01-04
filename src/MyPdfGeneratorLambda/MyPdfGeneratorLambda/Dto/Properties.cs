using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Dto
{
    public class Properties
    {
        public List<string> PageSizes { get; set; }
        public List<string> Orientations { get; set; }
        public List<string> FontFamilies { get; set; }

        public static Properties Init()
        {
            return new Properties()
            {
                PageSizes = new List<string>()
                {
                    nameof(PageSize.A0), nameof(PageSize.A1), nameof(PageSize.A2), nameof(PageSize.A3),
                    nameof(PageSize.A4), nameof(PageSize.A5), nameof(PageSize.A6), nameof(PageSize.A7),
                    nameof(PageSize.A8), nameof(PageSize.A9), nameof(PageSize.A10),
                    nameof(PageSize.B0), nameof(PageSize.B1), nameof(PageSize.B2), nameof(PageSize.B3),
                    nameof(PageSize.B4), nameof(PageSize.B5), nameof(PageSize.B6), nameof(PageSize.B7),
                    nameof(PageSize.B8), nameof(PageSize.B9), nameof(PageSize.B10)
                },
                Orientations = new List<string>()
                {
                    "縦向き", "横向き"
                },
                FontFamilies = Properties.GetFontFamilies()
            };
        }

        private static List<string> GetFontFamilies()
        {
            List<string> ret = new List<string>();
            foreach (string fontname in FontFactory.RegisteredFonts)
            {
                ret.Add(fontname);
            }
            ret.Sort();
            return ret;
        }
    }
}
