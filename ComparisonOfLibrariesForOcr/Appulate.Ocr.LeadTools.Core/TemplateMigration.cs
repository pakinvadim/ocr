// using System;
// using System.IO;
// using System.Linq;
// using Appulate.Ocr.Forms;
// using Leadtools;
// using Leadtools.Barcode;
// using Leadtools.Codecs;
// using Leadtools.Forms.Auto;
// using Leadtools.Forms.Common;
// using Leadtools.Forms.Processing;
// using Leadtools.Forms.Recognition;
// using Leadtools.Forms.Recognition.Search;
// using Leadtools.Ocr;
//
// namespace Appulate.Uplink.OcrTemplatesDesignerLeadTools {
// 	public static class TemplateMigration {
// 		private const string TemplatesDirectory = @"D:\Projects\Appulate\Storage\Uplink\Templates";
// 		private const string ResourcesTemplateDirectory = @"D:\Projects\Appulate\Storage\Uplink\Resources\Templates";
// 		private static bool UseFullTextSearch = false;
//
// 		private static DiskMasterFormsRepository workingRepository;
// 		private static FormRecognitionEngine recognitionEngine;
//
// 		public static void Do(DiskMasterFormsRepository diskMasterFormsRepository,
// 		                      FormRecognitionEngine formRecognitionEngine,
// 		                      IOcrEngine ocrEngine,
// 		                      BarcodeEngine barcodeEngine) {
// 			workingRepository = diskMasterFormsRepository;
// 			recognitionEngine = formRecognitionEngine;
// 			var accusoftFormHeadSet = new FormHeadSet(TemplatesDirectory, ResourcesTemplateDirectory, "local");
//
// 			foreach (IGrouping<Guid, OcrForm> group in accusoftFormHeadSet.Forms.GroupBy(f => f.TemplateSyncId)) {
// 				Guid syncId = group.Key;
// 				OcrForm[] accusoftPages = group.ToArray();
//
// 				string directoryName = Path.GetFileNameWithoutExtension(accusoftPages.First().TemplatePath);
//
// 				IMasterFormsCategory root = workingRepository.RootCategory;
//
// 				IMasterFormsCategory templateFolder = root.ChildCategories.Single(c => c.Name == directoryName); //root.AddChildCategory(directoryName);
// 				IMasterFormsCategory ocrForm = templateFolder.ChildCategories.SingleOrDefault(c => c.Name == "OCR")
// 				                               ?? templateFolder.AddChildCategory("OCR");
//
// 				IMasterFormsCategory subForms = null;
// 				int pageIndex = 0;
// 				foreach (OcrForm accusoftPage in accusoftPages) {
// 					IMasterForm pageForm;
// 					FormRecognitionAttributes attributes = CreateMasterForm($"Page{pageIndex}");
// 					if (pageIndex == 0) {
// 						pageForm = ocrForm.AddMasterForm(attributes, null, (RasterImage)null);
// 					} else {
// 						subForms ??= ocrForm.AddChildCategory("Sub Pages");
// 						pageForm = subForms.AddMasterForm(attributes, null, (RasterImage)null);
// 					}
// 					pageIndex++;
//
// 					using (var codecs = new RasterCodecs()) {
// 						codecs.Options.Load.Resolution = 300;
// 						codecs.Options.RasterizeDocument.Load.Resolution = 300;
// 						RasterImage pageImage = codecs.Load(accusoftPage.ImageFilePath);
// 						pageImage.Page = 1;
//
// 						//FormRecognitionAttributes attributes = pageForm.ReadAttributes();
// 						recognitionEngine.OpenMasterForm(attributes);
// 						// //leaddoc will load svg from image
// 						// LEADDocument leadDoc = DocumentFactory.Create(CreateDocumentOptionsWithCache());
// 						// //necessary to set image
// 						// leadDoc.IsReadOnly = false;
// 						// leadDoc.Pages.Add(leadDoc.Pages.CreatePage(image.ImageSize.ToLeadSizeD(), image.XResolution));
// 						// leadDoc.Pages[0].SetImage(image);
// 						// //get svg with options
// 						// //don't load svg for now, there are issues in the leaddocument
// 						// //pageOptions.SvgDocument = leadDoc.Pages[0].GetSvg(null);
// 						var options = new PageRecognitionOptions { PageType = FormsPageType.Normal };
// 						recognitionEngine.InsertMasterFormPage(masterFormPageNumber: -1, attributes, pageImage.Clone(), options, callback: null);
// 						if (UseFullTextSearch) {
// 							recognitionEngine.FullTextSearchManager ??= new DiskFullTextSearchManager {
// 								IndexDirectory = Path.Combine(workingRepository.Path, "index")
// 							};
// 							recognitionEngine.UpsertMasterFormToFullTextSearch(attributes, "index", null, null, null, null);
// 						}
// 						recognitionEngine.CloseMasterForm(attributes);
// 						if (UseFullTextSearch) {
// 							recognitionEngine.FullTextSearchManager.Index();
// 						}
//
// 						RasterImage formImage = pageForm.ReadForm();
// 						if (formImage != null) {
// 							formImage.AddPages(pageImage.CloneAll(), 1, pageImage.PageCount);
// 						} else {
// 							formImage = pageImage.CloneAll();
// 						}
//
// 						FormPages formPages = pageForm.ReadFields();
// 						if (formPages != null) {
// 							for (int i = 0; i < pageImage.PageCount; i++) {
// 								formPages.Add(new FormPage(formPages.Count + 1, pageImage.XResolution, pageImage.YResolution));
// 							}
// 						} else {
// 							//No processing pages exist so we must create them
// 							var tempProcessingEngine = new FormProcessingEngine();
// 							tempProcessingEngine.OcrEngine = ocrEngine;
// 							tempProcessingEngine.BarcodeEngine = barcodeEngine;
//
// 							for (int i = 0; i < recognitionEngine.GetFormProperties(attributes).Pages; i++) {
// 								tempProcessingEngine.Pages.Add(new FormPage(i + 1, pageImage.XResolution, pageImage.YResolution));
// 							}
// 							formPages = tempProcessingEngine.Pages;
// 						}
//
// 						// FormPageTemplate vintaPage = AddImage(accusoftPage.ImageFilePath, templateManager);
//
// 						foreach (OcrTemplateField accusoftField in accusoftPage.Fields) {
// 							// var widthMargin = (int)(0.007 * newField.Bounds.Width);
// 							// var heightMargin = (int)(0.025 * newField.Bounds.Height);
//
// 							OcrFormField newField = null;
// 							if (accusoftField.Type == FieldTypes.Ocr) {
// 								newField = new TextFormField();
// 							} else if (accusoftField.Type == FieldTypes.Omr) {
// 								newField = new OmrFormField();
// 							} else {
// 								newField = new TextFormField();
// 							}
// 							newField.Name = accusoftField.Id.ToString();
// 							newField.Bounds = new LeadRect(left: accusoftField.Location.Left,
// 							                               top: accusoftField.Location.Top,
// 							                               width: accusoftField.Location.Width,
// 							                               height: accusoftField.Location.Height);
// 							formPages[0].Add(newField);
// 						}
//
// 						pageForm.WriteForm(formImage);
// 						pageForm.WriteAttributes(attributes);
// 						pageForm.WriteFields(formPages);
// 					}
// 				}
// 			}
// 		}
//
// 		private static FormRecognitionAttributes CreateMasterForm(string name) {
// 			var options = new FormRecognitionOptions();
// 			FormRecognitionAttributes attributes = recognitionEngine.CreateMasterForm(name, new Guid(), options);
// 			recognitionEngine.CloseMasterForm(attributes);
// 			return attributes;
// 		}
// 	}
// }