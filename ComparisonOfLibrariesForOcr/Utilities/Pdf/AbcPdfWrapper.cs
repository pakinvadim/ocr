using System;
using System.IO;
using WebSupergoo.ABCpdf11;
using WebSupergoo.ABCpdf11.Internal;

namespace Utilities.Pdf {
	public enum ReadDocumentType {
		Default = ReadModuleType.Default,
		Pdf = ReadModuleType.Pdf
	}

	public static class AbcPdfWrapper {
		static AbcPdfWrapper() {
			XSettings.InstallLicense(License.AbcPdfLicense);
		}

		public static void Touch() {
			// dummy method to call static ctor
		}

		public static Doc CreateDocument() {
			Doc doc = null;
			try {
				doc = new Doc();
				if (doc.HtmlOptions != null) {
					doc.HtmlOptions.Engine = EngineType.Chrome;
				}
				return doc;
			} catch {
				doc?.Dispose();
				throw;
			}
		}

		public static PdfDocument Read(byte[] content, ReadDocumentType moduleType = ReadDocumentType.Default) {
			if (content.Length <= 1) {
				throw new ArgumentException();
			}
			return ReadContent(content, moduleType);
		}

		public static PdfDocument Read(Stream stream, ReadDocumentType moduleType = ReadDocumentType.Default) {
			return ReadContent(stream, moduleType);
		}

		public static PdfDocument Read(string path, ReadDocumentType moduleType = ReadDocumentType.Default) {
			return ReadContent(path, moduleType);
		}

		private static PdfDocument ReadContent(dynamic content, ReadDocumentType moduleType) {
			Doc doc = null;
			try {
				doc = new Doc();
				using (var options = new XReadOptions()) {
					options.ReadModule = (ReadModuleType)moduleType;
					try {
						doc.Read(content, options);
					} catch (PDFException) {
						options.Repair = true;
						doc.Read(content, options);
					}
				}
				return new PdfDocument(doc);
			} catch (PDFException) {
				doc?.Dispose();
				throw;
			} catch {
				doc?.Dispose();
				throw;
			}
		}

		public static byte[] AppendPdf(this byte[] pdfFirst, string pdfSecondFilePath) {
			return CombinePdfs(doc => doc.Read(pdfFirst), doc => doc.Read(pdfSecondFilePath));
		}

		public static byte[] AppendPdf(this byte[] pdfFirst, byte[] pdfSecond) {
			return CombinePdfs(doc => doc.Read(pdfFirst), doc => doc.Read(pdfSecond));
		}

		public static bool IsAbcPdfException(Exception exception) {
			return exception is PDFException;
		}

		private static byte[] CombinePdfs(Action<Doc> readFirst, Action<Doc> readSecond) {
			using (Doc pdf = CreateDocument()) {
				using (Doc appendix = CreateDocument()) {
					readFirst(pdf);
					readSecond(appendix);
					pdf.Append(appendix);
					return pdf.GetData();
				}
			}
		}
	}
}