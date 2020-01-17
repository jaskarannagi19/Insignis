using Insignis.Asset.Management.Reports.Helper;
using Octavo.Gate.Nabu.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Insignis.Asset.Management.PowerPoint.Generator
{
    public class RenderAbstraction : BaseType
    {
        public string InternalFacingOutputFolder = "";
        public string PublicFacingOutputFolder = "";

        public RenderAbstraction(string pInternalFacingOutputFolder, string pPublicFacingOutputFolder)
        {
            InternalFacingOutputFolder = pInternalFacingOutputFolder;
            PublicFacingOutputFolder = pPublicFacingOutputFolder;
        }

        public ExtendedReportContent MergeDataWithPowerPointTemplate(string pUniqueReference, List<KeyValuePair<string, string>> pTextReplacements, string pFullyQualifiedPathToSourceTemplate, string pRequiredOutputNameWithoutExtension, bool pAlsoGeneratePDF)
        {
            ExtendedReportContent extendedReportContent = new ExtendedReportContent();
            try
            {
                if (PublicFacingOutputFolder != null && PublicFacingOutputFolder.Trim().Length > 0)
                {
                    if (InternalFacingOutputFolder != null && InternalFacingOutputFolder.Trim().Length > 0)
                    {
                        if (Directory.Exists(InternalFacingOutputFolder))
                        {
                            if (pFullyQualifiedPathToSourceTemplate != null && pFullyQualifiedPathToSourceTemplate.Trim().Length > 0 && File.Exists(pFullyQualifiedPathToSourceTemplate))
                            {
                                if (pUniqueReference != null && pUniqueReference.Trim().Length > 0)
                                {
                                    if (pTextReplacements != null && pTextReplacements.Count > 0)
                                    {
                                        string internalFacingOutputFolder = InternalFacingOutputFolder;
                                        string publicFacingOutputFolder = PublicFacingOutputFolder;

                                        if (internalFacingOutputFolder.EndsWith("\\") == false)
                                            internalFacingOutputFolder += "\\";

                                        if (publicFacingOutputFolder.EndsWith("/") == false)
                                            publicFacingOutputFolder += "/";

                                        if (Directory.Exists(internalFacingOutputFolder + pUniqueReference) == false)
                                        {
                                            Directory.CreateDirectory(internalFacingOutputFolder + pUniqueReference);

                                            Octavo.Gate.Nabu.Office.Interface.OpenOfficePowerPointWrapper powerPointWrapper = new Octavo.Gate.Nabu.Office.Interface.OpenOfficePowerPointWrapper();
                                            File.Copy(pFullyQualifiedPathToSourceTemplate, internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx");
                                            powerPointWrapper.Open(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx", false);

                                            powerPointWrapper.SearchAndReplaceTextAllSlides(pTextReplacements);

                                            // Save resulting PPTX document.
                                            powerPointWrapper.Save();
                                            //powerPointWrapper.SaveAs(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx");
                                            extendedReportContent.OtherFiles.Add(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx");
                                            powerPointWrapper.Close();
                                            powerPointWrapper = null;

                                            if (pAlsoGeneratePDF)
                                            {
                                                if (System.IO.File.Exists(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx"))
                                                {
                                                    System.IO.FileInfo fi = new System.IO.FileInfo(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx");

                                                    Spire.Presentation.Presentation presentation = new Spire.Presentation.Presentation();
                                                    presentation.LoadFromFile(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pptx");
                                                    presentation.SaveToFile(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pdf", Spire.Presentation.FileFormat.PDF);

                                                    extendedReportContent.OtherFiles.Add(internalFacingOutputFolder + pUniqueReference + "\\" + pRequiredOutputNameWithoutExtension + ".pdf");

                                                    publicFacingOutputFolder += pUniqueReference;
                                                    publicFacingOutputFolder += "/";
                                                    publicFacingOutputFolder += pRequiredOutputNameWithoutExtension;
                                                    publicFacingOutputFolder += ".pdf";
                                                    extendedReportContent.URI = publicFacingOutputFolder;

                                                    extendedReportContent.OtherFiles.Add(publicFacingOutputFolder);
                                                    extendedReportContent.OtherFiles.Add(publicFacingOutputFolder.Replace(".pdf", ".pptx"));
                                                }
                                            }
                                            else
                                            {
                                                publicFacingOutputFolder += pUniqueReference;
                                                publicFacingOutputFolder += "/";
                                                publicFacingOutputFolder += pRequiredOutputNameWithoutExtension;
                                                publicFacingOutputFolder += ".pptx";
                                                extendedReportContent.URI = publicFacingOutputFolder;

                                                extendedReportContent.OtherFiles.Add(publicFacingOutputFolder);
                                            }
                                        }
                                        else
                                        {
                                            extendedReportContent.ErrorsDetected = true;
                                            extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "That unique reference has already been used"));
                                        }
                                    }
                                    else
                                    {
                                        extendedReportContent.ErrorsDetected = true;
                                        extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No Text Replacements have been specified"));
                                    }
                                }
                                else
                                {
                                    extendedReportContent.ErrorsDetected = true;
                                    extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Unique reference is not specified"));
                                }
                            }
                            else
                            {
                                extendedReportContent.ErrorsDetected = true;
                                extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Template not specified or not found"));
                            }
                        }
                        else
                        {
                            extendedReportContent.ErrorsDetected = true;
                            extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "Internal facing output folder does not exist"));
                        }
                    }
                    else
                    {
                        extendedReportContent.ErrorsDetected = true;
                        extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No internal facing output folder specified"));
                    }
                }
                else
                {
                    extendedReportContent.ErrorsDetected = true;
                    extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, "No public facing output folder specified"));
                }
            }
            catch(Exception exc)
            {
                extendedReportContent.StackTrace = exc.StackTrace;
                extendedReportContent.ErrorsDetected = true;
                extendedReportContent.ErrorDetails.Add(new Octavo.Gate.Nabu.Entities.Error.ErrorDetail(-1, exc.Message));
            }
            return extendedReportContent;
        }
    }
}
