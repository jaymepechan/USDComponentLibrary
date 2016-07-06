/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.USD.ComponentLibrary.Utilities
{
    public class ImageCache
    {
        static Dictionary<string, System.Windows.Media.Imaging.BitmapImage> imageCache = new Dictionary<string, System.Windows.Media.Imaging.BitmapImage>();
        public static System.Windows.Media.Imaging.BitmapImage Retrieve(string imageName)
        {
            lock (imageCache)
            {
                if (imageCache.ContainsKey(imageName))
                    return imageCache[imageName].Clone();
                else return null;
            }
        }

        public static void Store(string imageName, System.Windows.Media.Imaging.BitmapImage image)
        {
            lock (imageCache)
            {
                if (imageCache.ContainsKey(imageName))
                    imageCache[imageName] = image;
                else
                    imageCache.Add(imageName, image);
            }
        }
    }
}
