// Copyright (c) 2026 by Medtronic, plc.  All Rights Reserved

using System.Text;
using XamlToHtmlConverter.IntermediateRepresentation;

namespace XamlToHtmlConverter.Rendering
{
    /// <summary>
    /// Layout renderer responsible for handling Grid elements.
    /// Applies CSS Grid layout rules, including auto-generated template definitions
    /// when no explicit row or column definitions are provided.
    /// </summary>
    public class GridLayoutRenderer : ILayoutRenderer
    {
        public int Priority => 100;
        #region Public Methods

        /// <summary>
        /// Determines whether this renderer can process the given IR element.
        /// </summary>
        /// <param name="element">The IR element to evaluate.</param>
        /// <returns><c>true</c> if the element type is Grid; otherwise, <c>false</c>.</returns>
        public bool CanHandle(IntermediateRepresentationElement element)
            => element.Type == "Grid";

        /// <summary>
        /// Applies CSS grid layout styles to the provided style builder,
        /// including grid-template-rows and grid-template-columns derived from
        /// explicit definitions or inferred from child element placement.
        /// </summary>
        /// <param name="element">The Grid IR element to render layout for.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        public void ApplyLayout(IntermediateRepresentationElement element, StringBuilder styleBuilder)
        {
            styleBuilder.Append("display:grid;");

            ApplyRowTemplate(element, styleBuilder);
            ApplyColumnTemplate(element, styleBuilder);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Appends the grid-template-rows CSS property to the style builder.
        /// Uses explicit row definitions when available; otherwise infers row count
        /// from child element Grid.Row and Grid.RowSpan attached properties.
        /// </summary>
        /// <param name="element">The Grid IR element.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        private void ApplyRowTemplate(IntermediateRepresentationElement element, StringBuilder styleBuilder)
        {
            if (element.GridRowDefinitions.Count > 0)
            {
                var rows = element.GridRowDefinitions.Select(ConvertGridLength);
                styleBuilder.Append($"grid-template-rows:{string.Join(" ", rows)};");
                return;
            }

            var maxRow = 0;
            foreach (var child in element.Children)
            {
                var rowIndex = 0;
                var span = 1;

                if (child.AttachedProperties.TryGetValue("Grid.Row", out var rowValue) &&
                    int.TryParse(rowValue, out var parsedRow))
                {
                    rowIndex = parsedRow;
                }

                if (child.AttachedProperties.TryGetValue("Grid.RowSpan", out var spanValue) &&
                    int.TryParse(spanValue, out var parsedSpan))
                {
                    span = parsedSpan;
                }

                var lastRow = rowIndex + span - 1;
                if (lastRow > maxRow)
                    maxRow = lastRow;
            }

            if (element.Children.Count > 0)
            {
                var rows = new List<string>();
                for (int i = 0; i <= maxRow; i++)
                    rows.Add("auto");

                styleBuilder.Append($"grid-template-rows:{string.Join(" ", rows)};");
            }
        }

        /// <summary>
        /// Appends the grid-template-columns CSS property to the style builder.
        /// Uses explicit column definitions when available; otherwise infers column count
        /// from child element Grid.Column and Grid.ColumnSpan attached properties.
        /// </summary>
        /// <param name="element">The Grid IR element.</param>
        /// <param name="styleBuilder">The string builder to append CSS styles to.</param>
        private void ApplyColumnTemplate(IntermediateRepresentationElement element, StringBuilder styleBuilder)
        {
            if (element.GridColumnDefinitions.Count > 0)
            {
                var cols = element.GridColumnDefinitions.Select(ConvertGridLength);
                styleBuilder.Append($"grid-template-columns:{string.Join(" ", cols)};");
                return;
            }

            var maxCol = 0;
            foreach (var child in element.Children)
            {
                var colIndex = 0;
                var span = 1;

                if (child.AttachedProperties.TryGetValue("Grid.Column", out var colValue) &&
                    int.TryParse(colValue, out var parsedCol))
                {
                    colIndex = parsedCol;
                }

                if (child.AttachedProperties.TryGetValue("Grid.ColumnSpan", out var spanValue) &&
                    int.TryParse(spanValue, out var parsedSpan))
                {
                    span = parsedSpan;
                }

                var lastCol = colIndex + span - 1;
                if (lastCol > maxCol)
                    maxCol = lastCol;
            }

            if (element.Children.Count > 0)
            {
                var cols = new List<string>();
                for (int i = 0; i <= maxCol; i++)
                    cols.Add("auto");

                styleBuilder.Append($"grid-template-columns:{string.Join(" ", cols)};");
            }
        }

        /// <summary>
        /// Converts a XAML GridLength value into its CSS equivalent unit.
        /// Supports "Auto" (auto), star notation (fr), and fixed pixel values.
        /// </summary>
        /// <param name="value">The raw XAML GridLength string (e.g., "Auto", "2*", "100").</param>
        /// <returns>The equivalent CSS unit string (e.g., "auto", "2fr", "100px").</returns>

        private string ConvertGridLength(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return "auto";

            value = value.Trim();

            // WPF Auto
            if (value.Equals("Auto", StringComparison.OrdinalIgnoreCase))
                return "auto";

            // WPF star sizing
            if (value.EndsWith("*"))
            {
                var star = value.Replace("*", "");

                if (string.IsNullOrWhiteSpace(star))
                    star = "1";

                // use minmax for better CSS behaviour
                return $"minmax(0,{star}fr)";
            }

            // pixel values
            if (int.TryParse(value, out var px))
                return $"{px}px";

            return value;
        }

        #endregion
    }
}
