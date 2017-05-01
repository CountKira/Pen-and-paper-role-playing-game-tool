using System;
using System.Collections.Generic;
using WpfApplication.ViewModel;

namespace WpfApplication
{
    [Serializable]
    public class TableElementData
    {
        public string ImageUrl { get; set; }
        public string ImageName { get; set; }
        public double BaseSize { get; set; }
        public int SizeMultiplier { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public List<Item> CharacterSheet { get; set; }

        public static TableElement ConvertToTableElement(TableElementData elementData)
        {
            return new TableElement()
            {
                ImageName = elementData.ImageName,
                ImageUrl = elementData.ImageUrl,
                BaseSize = elementData.BaseSize,
                SizeMultiplier = elementData.SizeMultiplier,
                X = elementData.X,
                Y = elementData.Y,
                CharacterSheet = elementData.CharacterSheet,
            };
        }

        public static TableElementData ConvertFromTableElement(TableElement tableElement)
        {
            return new TableElementData
            {
                ImageName = tableElement.ImageName,
                ImageUrl = tableElement.ImageUrl,
                BaseSize = tableElement.BaseSize,
                SizeMultiplier = tableElement.SizeMultiplier,
                X = tableElement.X,
                Y = tableElement.Y,
                CharacterSheet = tableElement.CharacterSheet,
            };
        }
    }
}