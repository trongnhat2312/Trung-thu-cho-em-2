using System;
using System.Linq;
using UnityEngine;

namespace BarcodeScanner
{
	public class ScannerSettings
	{
		// Scanner Options
		public bool ScannerBackgroundThread { get; set; }
		public int ScannerDelayFrameMin { get; set; }
		public float ScannerDecodeInterval { get; set; }

		// Parser Options
		public bool ParserAutoRotate { get; set; }
		public bool ParserTryInverted { get; set; }
		public bool ParserTryHarder { get; set; }

		// Webcam Options
		public string WebcamDefaultDeviceName { get; set; }
		public int WebcamRequestedWidth { get; set; }
		public int WebcamRequestedHeight { get; set; }
		public FilterMode WebcamFilterMode { get; set; }

		public ScannerSettings(string name)
		{

			ScannerBackgroundThread = true;
			ScannerDelayFrameMin = 3;
			ScannerDecodeInterval = 0.1f;

			ParserAutoRotate = true;
			ParserTryInverted = true;
			ParserTryHarder = false;

			WebcamDefaultDeviceName = "";

			int index = -1;
			for(int i = 0; i < WebCamTexture.devices.Length; i++)
			{
				if (WebCamTexture.devices[i].name.Equals(name))
				{
					index = i;
					break;
				}
			}

			if(index == 0)
			{
				WebcamDefaultDeviceName = WebCamTexture.devices[1].name;
			} else
			{
				WebcamDefaultDeviceName = WebCamTexture.devices[0].name;

			}

			//WebcamDefaultDeviceName = (WebCamTexture.devices.Length > 0) ? WebCamTexture.devices.First().name : "";
			WebcamRequestedWidth = 512;
			WebcamRequestedHeight = 512;
			WebcamFilterMode = FilterMode.Trilinear;

			// Device dependent settings

			// Disable background thread for webgl : Thread not supported
#if UNITY_WEBGL
			ScannerDecodeInterval = 0.5f;
			ScannerBackgroundThread = false;
#endif

			// Enable only for desktop usage : heavy CPU consumption
#if UNITY_STANDALONE || UNITY_EDITOR
			ParserTryHarder = true;
#endif
		}

		public ScannerSettings()
		{
			ScannerBackgroundThread = true;
			ScannerDelayFrameMin = 3;
			ScannerDecodeInterval = 0.1f;

			ParserAutoRotate = true;
			ParserTryInverted = true;
			ParserTryHarder = false;

			WebcamDefaultDeviceName = "";


			//foreach (WebCamDevice wd in WebCamTexture.devices)
			//{
			//	Console.WriteLine("Camera: " + wd.name);
			//	if (!wd.isFrontFacing)
			//	{
			//		WebcamDefaultDeviceName = wd.name;
			//	}
			//}

			int index = -1;

			for(int i = 0; i < WebCamTexture.devices.Length; i++)
			{
				WebCamDevice wd = WebCamTexture.devices[i];
				if(!wd.isFrontFacing)
				{
					index = i;
					break;
				}	
			}

			if(index > -1)
			{
				WebcamDefaultDeviceName = WebCamTexture.devices[index].name;
			} else
			{
				if(WebCamTexture.devices.Length > 1) {
					WebcamDefaultDeviceName = WebCamTexture.devices[1].name;
				} else
				{
					WebcamDefaultDeviceName = WebCamTexture.devices[0].name;
				}
			}

			

			//WebcamDefaultDeviceName = (WebCamTexture.devices.Length > 0) ? WebCamTexture.devices.First().name : "";
			WebcamRequestedWidth = 512;
			WebcamRequestedHeight = 512;
			WebcamFilterMode = FilterMode.Trilinear;

			// Device dependent settings

			// Disable background thread for webgl : Thread not supported
			#if UNITY_WEBGL
			ScannerDecodeInterval = 0.5f;
			ScannerBackgroundThread = false;
			#endif

			// Enable only for desktop usage : heavy CPU consumption
			#if UNITY_STANDALONE || UNITY_EDITOR
			ParserTryHarder = true;
			#endif
		}
	}
}
