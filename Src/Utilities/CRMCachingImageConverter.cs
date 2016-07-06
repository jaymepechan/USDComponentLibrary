/********************************************************
*                                                       *
*   Copyright (C) Microsoft. All rights reserved.   *
*                                                       *
********************************************************/
using Microsoft.Crm.UnifiedServiceDesk.CommonUtility.DataLoader;
using Microsoft.Uii.AifServices;
using Microsoft.Xrm.Tooling.WebResourceUtility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Microsoft.USD.ComponentLibrary.Utilities
{
    class CRMCachingImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                System.Windows.Media.Imaging.BitmapImage btm = ImageCache.Retrieve((string)value);
                if (btm != null)
                {
                    return btm;
                }

                System.Windows.Controls.Image img3;
                img3 = new System.Windows.Controls.Image();
                IUsdConfiguraitonManager _cfgMgr = AifServiceContainer.Instance.GetService<IUsdConfiguraitonManager>();
                if (_cfgMgr != null && _cfgMgr.IsUsdConfigDataReady)
                {
                    ImageResources _crmWebResource = new ImageResources(_cfgMgr.CrmManagementSvc);
                    System.Windows.Media.Imaging.BitmapImage bm;
                    bm = _crmWebResource.GetImageFromCRMWebResource((string)value);
                    if (bm != null)
                    {
                        ImageCache.Store((string)value, bm);
                        return bm.Clone();
                    }
                    else
                        return null;
                }
                else
                {
                    return null;
                }
            }
            catch
            { return null; }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
