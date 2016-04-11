using System;
using System.IO.Ports;
using System.Threading;

using ZXing;
using ZXing.Client.Result;
using ZXing.Common;
using ZXing.Rendering;

using System.Drawing;

class SerialTest
{
	public static void Main()
	{	
		createQRCode("1234567890");
		
		SerialPortOperat();
	}
	
	public static void createQRCode(string contents)
	{
		 var writer = new BarcodeWriter
               {
                  Format = BarcodeFormat.QR_CODE,
                  Options = new EncodingOptions
                     {
                        Height = 100,
                        Width = 100
                     },
                  Renderer = (IBarcodeRenderer<Bitmap>)Activator.CreateInstance( typeof (BitmapRenderer))
               };
			   
        Bitmap img = writer.Write(contents);
		string filename = System.Environment.CurrentDirectory + "\\QR" + DateTime.Now.Ticks.ToString() + ".jpg";
		img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
	}
	
	public static void SerialPortOperat()
	{
		string [] strs = SerialPort.GetPortNames();
		
		foreach(string str in strs)
		{
			if(str == "COM1")
			{
				Thread thread = new Thread(new ThreadStart(ThreadWriteProcess));
			
				thread.Start();
			}
			else if(str == "COM2")
			{
				Thread thread = new Thread(new ThreadStart(ThreadReadProcess));
			
				thread.Start();
			}
			else
			{
				
			}
		}
	}
	
	public static void ThreadWriteProcess()
	{
		COMPort cp = new COMPort("COM1");
		cp.Open();
		for(int i=0;i<100;i++)
		{
			Thread.Sleep(100);
			cp.setBuffer(System.Text.Encoding.ASCII.GetBytes("hello world"));
			cp.Write(); 
		}
		cp.Close();
	}
	
	public static void ThreadReadProcess()
	{
		COMPort cp = new COMPort("COM2");
		cp.Open();
		while(true)
		{
			Thread.Sleep(100);
			if(cp.Read()>0)
				Console.WriteLine(System.Text.Encoding.ASCII.GetString(cp.getBuffer()));	
		}
	}
}


class COMPort
{
	protected SerialPort sp;
	
	protected byte [] buffer;
	
	public COMPort(string strPortName)
	{
		sp = new SerialPort();
		sp.PortName = strPortName;
		sp.BaudRate = 9600;
		sp.DataBits = 8;
		sp.StopBits = StopBits.One;
		sp.Parity=Parity.None;
	}
	
	public byte [] getBuffer()
	{
		return this.buffer;
	}
	
	public void setBuffer(byte [] buffer)
	{
		this.buffer = buffer;
	}
	
	public bool Open()
	{
		if(sp.IsOpen==false)
		{
			sp.Open();
		}
		return sp.IsOpen;
	}
	
	public bool Close()
	{
		if(sp.IsOpen==true)
		{
			sp.Close();
		}
		return !sp.IsOpen;
	}
	
	public int Write()
	{
		int bc = sp.BytesToWrite;
		bc = buffer.Length;
		if(sp.IsOpen== true)
		{
			sp.Write(buffer,0,bc);
		}
		return sp.BytesToWrite;
	}
	
	public int Read()
	{
		int bc = sp.BytesToRead;
		if(sp.IsOpen== true&&bc >0)
		{
			this.buffer = new byte[bc];
			sp.Read(buffer,0,bc);
		}
		return bc;
	}
}