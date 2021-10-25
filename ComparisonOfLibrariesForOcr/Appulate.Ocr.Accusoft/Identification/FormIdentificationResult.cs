using System;
using System.Collections.Generic;
using System.Drawing;
using Accusoft.FormFixSdk;
using Accusoft.ImagXpressSdk;
using Appulate.Ocr.Forms;

namespace Appulate.Ocr.Accusoft.Identification {
	public class FormIdentificationResult : IDisposable {
		private RegistrationResult _registrationResult;
		private bool _disposed;
		private readonly OcrFormModel _model;
		private readonly FormImage _alignedImage;
		private static readonly object Lock = new ();
		private const int Margin = 10;
		private static int ClearMargin = 15;

		public int Confidence { get; }

		public FormImage AlignedImage => _alignedImage;

		public int PageNum { get; }

		public OcrFormModel Model => _model;

		public Guid SyncId => Model.FormDefinition.TemplateSyncId;

		public Guid PageId => Model.FormDefinition.PageId;

		public RegistrationResult RegistrationResult {
			get {
				if (_registrationResult == null) {
					lock (Lock) {
						if (_registrationResult == null) {
							using (var registrationProcessor = new RegistrationProcessor(Workspace.FormFix)) {
								using (FormImage registrationImage = DrawMarks(AlignedImage)) {
									using (FormImage formImage = Model.FormDefinition.ReadImage()) {
										using (FormImage templateImage = DrawMarks(formImage)) {
											_registrationResult = registrationProcessor.RegisterToImage(registrationImage, templateImage);
										}
									}
								}
							}
						}
					}
				}
				return _registrationResult;
			}
		}

		public FormIdentificationResult(OcrForm form, FormImage alignedImage, int pageNum, int confidence) {
			Confidence = confidence;
			if (alignedImage != null) {
				PageNum = pageNum;
				_alignedImage = new FormImage(Workspace.FormFix);
				_model = new OcrFormModel(form);
				alignedImage = ClearAreas(alignedImage);
				alignedImage.CopyTo(AlignedImage);
			}
		}

		private static FormImage ProcessImage(FormImage image, Action<Graphics, ImageX> action) {
			using (ImagXpress imagXpress = Workspace.CreateImagXpress()) {
				using (ImageX imageX = ImageX.FromHdib(imagXpress, image.ToHdib(false), true)) {
					using (Graphics graphics = imageX.GetGraphics()) {
						action(graphics, imageX);
						imageX.ReleaseGraphics(true);
						using (var processor = new Processor(imagXpress, imageX)) {
							processor.AutoBinarize2();
							return FormImage.FromHdib(processor.Image.ToHdib(true), ownDib: true, workspace: Workspace.FormFix);
						}
					}
				}
			}
		}

		private static FormImage DrawMarks(FormImage image) {
			return ProcessImage(image, (g, i) => {
				ClearMargins(g, i);
				DrawMarks(g, i);
			});
		}

		private FormImage ClearAreas(FormImage image) {
			List<OcrClearArea> clearAreas = Model.FormDefinition.ClearAreas;
			if (clearAreas == null || clearAreas.Count == 0) {
				return image;
			}
			return ProcessImage(image, (g, _) => { ClearAreas(g, clearAreas); });
		}

		private static void ClearAreas(Graphics graphics, List<OcrClearArea> clearAreas) {
			using (var brush = new SolidBrush(Color.White)) {
				foreach (OcrClearArea area in clearAreas) {
					graphics.FillRectangle(brush, area.Location);
				}
			}
		}

		private static void ClearMargins(Graphics graphics, ImageX imageX) {
			int width = imageX.Width;
			int height = imageX.Height;
			using (var brush = new SolidBrush(Color.White)) {
				graphics.FillRectangle(brush, new Rectangle(0, 0, width, ClearMargin));
				graphics.FillRectangle(brush, new Rectangle(0, 0, ClearMargin, height));
				graphics.FillRectangle(brush, new Rectangle(0, height - ClearMargin, width, ClearMargin));
				graphics.FillRectangle(brush, new Rectangle(width - ClearMargin, 0, ClearMargin, height));
			}
		}

		private static void DrawMarks(Graphics graphics, ImageX imageX) {
			int markLength = (int)imageX.HorizontalResolution * 10;
			int width = imageX.Width;
			int height = imageX.Height;
			SolidBrush brush = null;
			try {
				brush = new SolidBrush(Color.Black);
				using (var pen = new Pen(brush)) {
					pen.Width = 1;
					graphics.DrawLines(pen, new[] { new Point(Margin, markLength + Margin), new Point(Margin, Margin), new Point(markLength + Margin, Margin) });
					graphics.DrawLines(pen, new[] { new Point(width - 1 - Margin, markLength + Margin), new Point(width - 1 - Margin, Margin), new Point(width - markLength - 1 - Margin, Margin) });
					graphics.DrawLines(pen, new[] { new Point(Margin, height - markLength - 1 - Margin), new Point(Margin, height - Margin - 1), new Point(Margin + markLength, height - Margin - 1) });
					graphics.DrawLines(pen,
					                   new[] {
						                   new Point(width - markLength - 1 - Margin, height - Margin - 1), new Point(width - Margin - 1, height - Margin - 1),
						                   new Point(width - Margin - 1, height - markLength - Margin - 1)
					                   });
				}
			} finally {
				brush?.Dispose();
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!_disposed) {
				if (disposing) {
					_registrationResult?.Dispose();
					_model.Dispose();
					_alignedImage.Dispose();
				}
				_disposed = true;
			}
		}

		~FormIdentificationResult() {
			Dispose(false);
		}
	}
}