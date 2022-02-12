using System.Collections.Generic;
using System.Drawing;
using TileFitter.Models;
using Windows.Storage;

namespace TileFitterPro.Models
{
    public class TileFitterNavigationArguments
    {
        public Container Container { get; set; }

        public List<Rectangle> Tiles { get; set; }

        public StorageFile InputFile { get; set; }
    }
}
