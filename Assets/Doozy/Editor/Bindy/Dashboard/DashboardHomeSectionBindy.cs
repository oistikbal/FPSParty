// Copyright (c) 2015 - 2023 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Linq;
using Doozy.Editor.Dashboard.WindowsLayouts;
using Doozy.Runtime.Bindy;
using Doozy.Runtime.Common.Utils;
using Doozy.Runtime.UIElements.Extensions;
// ReSharper disable StringLiteralTypo

namespace Doozy.Editor.Bindy.Dashboard
{
    public class DashboardHomeSectionBindy : DashboardHomeWindowLayout.HomeSection
    {
        public override int sectionOrder => 200;
        public override string sectionName => "Bindy Stats";
        
        public DashboardHomeSectionBindy()
        {
            this
                .AddChild(TitleLabel("Convertes"))
                .AddChild(ValueLabel($"{numberOfConverters}"))
                .AddSpaceBlock(3)
                .AddChild(TitleLabel("Transformers"))
                .AddChild(ValueLabel($"{numberOfTransformers}"))
                ;
        }
        
        private static int numberOfConverters =>
            ReflectionUtils.GetTypesThatImplementInterface<IValueConverter>().Count();

        private static int numberOfTransformers =>
            ReflectionUtils.GetDerivedTypes(typeof(ValueTransformer)).Count();
    }
}
