
namespace Insignis.Asset.Management.Reports.Helper
{
    public class Theme
    {
        private static string[] colourPalette = { "#004a61", "#3b889e", "#b12a30", "#ce836f", "#b4891f", "#dcc597", "#009989", "#b1d6d2", "#d6589e", "#eab9d5", "#b8ac00", "#d8ce78", "#6950a1", "#9e8ec4", "#f26322", "#f9a475", "#ffd800", "#ffe689", "#9bb585", "#c4d1b4", "#a93493", "#c78abc", "#976031", "#cdb095", "#819dc4", "#c7d1e3", "#00a3b0", "#afdbe1", "#d68157", "#e6b294" };

        public static string GetColourFromPalette(int index)
        {
            return colourPalette[index];
        }
    }
}
