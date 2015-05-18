using System;
using System.Reflection;

namespace MonoBrickFirmware.Display
{
	public class CharStreamer : BitStreamer
	{
		public uint width;
		public uint height;
		public CharStreamer(uint height, UInt32[] data, int offset) : base(data, offset)
		{
			this.height = height;
			this.width = GetBits(8);
		}
	}
	
	public class Font
	{
		public uint maxWidth;
		public uint maxHeight;
		uint firstChar;
		uint charWordSize;
		UInt32[] data;
		
		static public Font FromResource(System.Reflection.Assembly asm, string resourceName)
		{
			System.IO.Stream s = asm.GetManifestResourceStream(resourceName);
			byte[] bytedata = new byte[s.Length];
			s.Read(bytedata, 0, (int)s.Length);
			UInt32[] data = new UInt32[s.Length/4];
			for (int i = 0; i != s.Length/4; ++i)
				data[i] = BitConverter.ToUInt32(bytedata, i*4);
			return new Font(data);
		}
		
		public Font (UInt32[] data)
		{
			if (data[0] != 0x544E4F46)
				throw new ArgumentException("Invalid value in font data");
			maxWidth = data[1];
			maxHeight = data[2];
			charWordSize = data[3];
			firstChar = 4;			
			this.data = data;
		}
		
		public CharStreamer getChar(char c)
		{
			int index = (int)c - 32;
			if (index < 0 || (index > 128-32))
				index = 0;
			CharStreamer result = new CharStreamer(maxHeight, data, (int)(firstChar+index*charWordSize));			
			return result;
		}		
		
		public Point TextSize(string text)
		{
			int width = 0;
			foreach(char c in text)
			{
				CharStreamer cs = getChar(c);				
				width += (int)cs.width;				
			}
			return new Point(width, (int)maxHeight);
		}
		
		static Font font_info56 = Font.FromResource(Assembly.GetExecutingAssembly(), "font.info56_12");
		static Font font_profont_7 = Font.FromResource(Assembly.GetExecutingAssembly(), "font.profont_7");
		
		static public Font SmallFont { get { return font_profont_7; } }
		static public Font MediumFont { get { return font_info56; } }
	}
}

