namespace Appulate.Ocr.Forms {
	public static class FormConst {
		// Field Type values
		public const string DropoutOp = "FormFix/DropOut";
		public const string IdentificationOp = "FormFix/Identification";
		public const string OmrOp = "FormFix/OMR";
		public const string EnhancementOp = "ScanFix/Enhancement";
		public const string RecognitionOp = "SmartZone/Recognition"; // SZ2 (ICR or OCR)
		public const string IcrOp = "SmartZoneICR/Recognition"; // SZ3 ICR
		public const string OcrOp = "SmartZoneOCR/Recognition"; // SZ3 OCR

		// FRD OtherDataItem Type values
		public const string Type = "Pegasus/Type";
		public const string Type2 = "Accusoft/Type";

		// OtherDataItem Content values
		public const string OmrFieldType = "OMR";
		public const string ClipFieldType = "Clip";
		public const string IcrFieldType = "ICR";
		public const string OcrFieldType = "OCR";

		// FRS OtherDataItem Type values
		public const string Version = "FormAssist/Version";
		public const string OriginalImage = "FormAssist/Original Image";

		public const string SyncId = "_Uplink/SyncId";
		public const string FormPath = "_Uplink/FormPath";
		public const string PageId = "_Uplink/PageId";

		public const string InternalIdentificationOp = "FormFix-Internal/Identification";
	}
}