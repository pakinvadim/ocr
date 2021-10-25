using System;
using System.IO;
using Accusoft.FormDirectorSdk;
using Accusoft.FormFixSdk;
using Accusoft.ImagXpressSdk;
using Accusoft.ScanFixXpressSdk;
using Accusoft.SmartZoneOCRSdk;
using Utilities;
using ErrorLevel = Accusoft.ScanFixXpressSdk.ErrorLevel;

namespace Appulate.Ocr.Accusoft {
	public static class Workspace {
		private static FormDirector _formDirector;
		private static FormFix _formFix;
		private static bool _debugLogEnabled;
		private static string _logDirectory;
		private const string SolutionName = "Appulate";
		private static readonly long[] SolutionKey = { 0xF74BD4E7, 0x7DC4BBBE, 0xE3F2BC2D, 0x2CEFB7D3 };

		public static void EnableDebug(string logDirectory, bool enable) {
			_logDirectory = Path.Combine(logDirectory, "Smartzone");
			_debugLogEnabled = enable;
		}

		public static FormDirector FormDirector {
			get {
				if (_formDirector == null) {
					_formDirector = new FormDirector();
					_formDirector.Licensing.SetSolutionName(SolutionName);
					_formDirector.Licensing.SetSolutionKey(SolutionKey[0], SolutionKey[1], SolutionKey[2], SolutionKey[3]);
					_formDirector.Licensing.SetOEMLicenseKey(License.FormDirectorOem);
				}
				return _formDirector;
			}
		}

		public static FormFix FormFix {
			get {
				if (_formFix == null) {
					_formFix = new FormFix();
					_formFix.Licensing.SetSolutionName(SolutionName);
					_formFix.Licensing.SetSolutionKey(SolutionKey[0], SolutionKey[1], SolutionKey[2], SolutionKey[3]);
					_formFix.Licensing.SetOEMLicenseKey(License.FormFixOem);
					if (_debugLogEnabled) {
						_formFix.Debug = true;
						_formFix.ErrorLevel = global::Accusoft.FormFixSdk.ErrorLevel.Detailed;
						_formFix.DebugLogFile = CreateLogPath("formfix");
					}
				}
				return _formFix;
			}
		}

		public static ImagXpress CreateImagXpress() {
			ImagXpress imagXpress = null;
			try {
				imagXpress = new ImagXpress();
				imagXpress.Licensing.SetSolutionName(SolutionName);
				imagXpress.Licensing.SetSolutionKey(SolutionKey[0], SolutionKey[1], SolutionKey[2], SolutionKey[3]);
				imagXpress.Licensing.SetOEMLicenseKey(License.ImgXpressOem);
				return imagXpress;
			} catch {
				imagXpress?.Dispose();
				throw;
			}
		}

		public static ScanFix CreateScanFix() {
			ScanFix scanFix = null;
			try {
				scanFix = new ScanFix();
				scanFix.Licensing.SetSolutionName(SolutionName);
				scanFix.Licensing.SetSolutionKey(SolutionKey[0], SolutionKey[1], SolutionKey[2], SolutionKey[3]);
				scanFix.Licensing.SetOEMLicenseKey(License.ScanFixOem);

				if (_debugLogEnabled) {
					scanFix.Debug = true;
					scanFix.ErrorLevel = ErrorLevel.Detailed;
					scanFix.DebugLogFile = CreateLogPath("scanfix");
				}
				return scanFix;
			} catch {
				scanFix?.Dispose();
				throw;
			}
		}

		public static SmartZoneOCR CreateSmartZoneOcr() {
			SmartZoneOCR smartZoneOcr = null;
			try {
				smartZoneOcr = new SmartZoneOCR();
				smartZoneOcr.Licensing.SetSolutionName(SolutionName);
				smartZoneOcr.Licensing.SetSolutionKey(SolutionKey[0], SolutionKey[1], SolutionKey[2], SolutionKey[3]);
				smartZoneOcr.Licensing.SetOEMLicenseKey(License.SmartZoneOem);
				smartZoneOcr.FastLoading = true;
				if (_debugLogEnabled) {
					smartZoneOcr.Debug = true;
					smartZoneOcr.ErrorLevel = ErrorLevelInfo.Detailed;
					smartZoneOcr.DebugLogFile = CreateLogPath("smartzoneocr");
				}
				return smartZoneOcr;
			} catch {
				smartZoneOcr?.Dispose();
				throw;
			}
		}

		private static string CreateLogPath(string suffix) {
			if (!Directory.Exists(_logDirectory)) {
				Directory.CreateDirectory(_logDirectory);
			}
			return Path.Combine(_logDirectory, DateTime.Now.ToString("yyyy-MM-dd-") + suffix + ".log");
		}
	}
}