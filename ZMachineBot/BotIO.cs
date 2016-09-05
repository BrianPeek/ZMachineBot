using System.IO;
using System.Text;
using ZMachineLib;

namespace ZMachineBot
{
	public class BotIO : IZMachineIO
	{
		private readonly StringBuilder _sb = new StringBuilder();

		public string Text => _sb.ToString().Replace("\r\n", "\r\n\r\n");

		public void Print(string s)
		{
			_sb.Append(s);
		}

		public string Read(int max)
		{
			return null;
		}

		public char ReadChar()
		{
			return '\0';
		}

		public void SetCursor(ushort line, ushort column, ushort window)
		{
		}

		public void SetWindow(ushort window)
		{
		}

		public void EraseWindow(ushort window)
		{
		}

		public void BufferMode(bool buffer)
		{
		}

		public void SplitWindow(ushort lines)
		{
		}

		public void ShowStatus()
		{
		}

		public void SetTextStyle(TextStyle textStyle)
		{
		}

		public bool Save(Stream stream)
		{
			return false;
		}

		public Stream Restore()
		{
			return null;
		}

		public void SetColor(ZColor foreground, ZColor background)
		{
		}

		public void SoundEffect(ushort number)
		{
		}

		public void Quit()
		{
		}
	}
}