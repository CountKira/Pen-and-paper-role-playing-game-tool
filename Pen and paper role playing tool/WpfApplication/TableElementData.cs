using System;

namespace WpfApplication
{
    [Serializable]
    public class TableElementData
    {
        public string ImageUrl { get; set; }
        public double BaseSize { get; set; }
        public int SizeMultiplier { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public static TableElement ConvertToTableElement(TableElementData element)
        {
            return new TableElement()
            {
                ImageUrl = element.ImageUrl,
                BaseSize = element.BaseSize,
                SizeMultiplier = element.SizeMultiplier,
                X = element.X,
                Y = element.Y
            };
        }

        public static TableElementData ConvertFromTableElement(TableElement tableElement)
        {
            return new TableElementData
            {
                ImageUrl = tableElement.ImageUrl,
                BaseSize = tableElement.BaseSize,
                SizeMultiplier = tableElement.SizeMultiplier,
                X = tableElement.X,
                Y = tableElement.Y
            };
        }
    }
}