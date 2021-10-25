using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization;
using Accusoft.FormFixSdk;
using Appulate.Ocr.Accusoft;
using Appulate.Ocr.Accusoft.Identification;

namespace Appulate.Ocr.Forms {
	[DataContract(Name = "Form")]
	public class OcrForm {
		[DataMember(Name = "Fields")]
		private readonly List<OcrTemplateField> _fields = new ();
		[DataMember(Name = "ClearAreas")]
		private List<OcrClearArea> _clearAreas;
		[DataMember(Name = "ImageFileName")]
		private string _imageFileName;

		[DataMember(Name = "IdentificationHashCode")]
		public string IdentificationHashCode { get; set; }

		public List<OcrClearArea> ClearAreas => _clearAreas ??= new List<OcrClearArea>();

		[DataMember(Name = "PageId")]
		public Guid PageId { get; private set; }

		public OcrTemplateField[] Fields => _fields.ToArray();

		[DataMember(Name = "SyncId")]
		public Guid TemplateSyncId { get; private set; }

		public int PartialHashCode => ImageHashCode.GetHashCode();

		private string _imageHash;
		private object _imageHashLock;

		public OcrForm() {
			Initialize();
		}

		public OcrForm(OcrFormSet formSet) : this() {
			TemplateSyncId = formSet.TemplateSyncId;
			TemplatePath = Path.GetDirectoryName(formSet.FilePath);
		}

		public OcrForm(OcrFormSet formSet, string imageFolder, string imageFileName, Guid pageId) : this() {
			TemplateSyncId = formSet.TemplateSyncId;
			TemplatePath = Path.GetDirectoryName(formSet.FilePath);
			ImagesFolder = imageFolder;
			_imageFileName = imageFileName;
			PageId = pageId;
		}

		[OnDeserializing]
		private void SetValuesOnDeserializing(StreamingContext context) {
			Initialize();
		}

		private void Initialize() {
			// ReSharper disable once InconsistentlySynchronizedField
			_imageHashLock = new object();
		}

		public string ImageHashCode {
			get {
				lock (_imageHashLock) {
					if (_imageHash != null) {
						return _imageHash;
					}
					using (FileStream fileStream = File.OpenRead(ImageFilePath)) {
						_imageHash = CommonHashCodeFunctions.GetMd5Hash(fileStream);
						return _imageHash;
					}
				}
			}
		}

		private byte[] _content;
		private static readonly object Lock = new ();

		public List<OcrAcordType> ComparableAcords { get; set; }
		public int? MinOffsetY { get; set; }
		public int? MaxOffsetY { get; set; }

		public string TemplatePath { get; set; }

		public string ImagesFolder { get; set; }

		public string ImageFilePath => Path.Combine(ImagesFolder, _imageFileName);

		public FormImage ReadImage() {
			using (Bitmap bmp = ReadBitmap()) {
				return FormImage.FromBitmap(bmp, Workspace.FormFix);
			}
		}

		public Bitmap ReadBitmap() {
			MemoryStream stream = null;
			try {
				stream = new MemoryStream(Content);
			} catch {
				stream?.Dispose();
			}
			return new Bitmap(stream);
		}

		private byte[] Content {
			get {
				if (_content == null) {
					lock (Lock) {
						_content ??= File.ReadAllBytes(ImageFilePath);
					}
				}
				return _content;
			}
		}
	}
}