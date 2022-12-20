using Microsoft.Maui.Platform;
using RayTrace.Materials;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTrace
{
//	public class TextureSourceConverter : TypeConverter
//	{
//		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
//		{
//			if (sourceType == typeof(string))
//				return true;
//			return base.CanConvertFrom(context, sourceType);
//		}
//
//		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
//		{
//			if (destinationType == typeof(string))
//				return true;
//			return base.CanConvertTo(context, destinationType);
//		}
//
//		public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
//		{
//			if (value is string)
//			{
//				ColorConverter();
//
//                return new SolidColorTexture(ColorConverter.FromString((string)value));
//			}
//			return base.ConvertFrom(context, culture, value);
//		}
//
//		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
//		{
//			if (value is SolidColorTexture)
//			{
//				var solid = (SolidColorTexture)value;
//				return ColorConverter.ToString(solid.Color);
//			}
//			return base.ConvertTo(context, culture, value, destinationType);
//		}
//	}
}
