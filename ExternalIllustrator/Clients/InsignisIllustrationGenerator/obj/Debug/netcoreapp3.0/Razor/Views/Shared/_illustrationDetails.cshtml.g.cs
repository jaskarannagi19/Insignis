#pragma checksum "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "51d91f121898ce3759860b70b055d510e317d7c2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Shared__illustrationDetails), @"mvc.1.0.view", @"/Views/Shared/_illustrationDetails.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\_ViewImports.cshtml"
using InsignisIllustrationGenerator;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\_ViewImports.cshtml"
using InsignisIllustrationGenerator.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
using System.Globalization;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"51d91f121898ce3759860b70b055d510e317d7c2", @"/Views/Shared/_illustrationDetails.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"1bc31017ea0574b4bf5a5115fd6862ed4d7895b5", @"/Views/_ViewImports.cshtml")]
    public class Views_Shared__illustrationDetails : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<IllustrationDetailViewModel>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "Accepted", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("value", "Deleted", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 3 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
  
    CultureInfo gb = new CultureInfo("en-GB");

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n\r\n<div class=\"middle-section\">\r\n    <div class=\"container\">\r\n        <div class=\"blue-background\">\r\n            <div class=\"row\">\r\n                <div class=\"col-lg-4 col-sm-6\"><p>Name:<br><span>");
#nullable restore
#line 12 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                            Write(Model.PartnerName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <div class=\"col-lg-4 col-sm-6\"><p>Organisation:<br><span>");
#nullable restore
#line 13 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                    Write(Model.PartnerOrganisation);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <div class=\"col-lg-4 col-sm-12\"><p>Email:<br><span>");
#nullable restore
#line 14 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                              Write(Model.PartnerEmail);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <hr class=\"blue-line\">\r\n\r\n\r\n                <div class=\"col-lg-4 col-sm-6\"><p>Name / Reference:<br><span>");
#nullable restore
#line 18 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                        Write(Model.ClientName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n\r\n                <div class=\"col-lg-4 col-sm-6\">\r\n                    <p>\r\n                        Client Type:<br>");
#nullable restore
#line 22 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                         if (Model.ClientType == 0)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span>Individual</span>");
#nullable restore
#line 24 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                   }
                        else
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <span>Joint</span>\r\n");
#nullable restore
#line 28 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </p>\r\n                </div>\r\n\r\n                <div class=\"col-lg-4 col-sm-6\"><p>Illustration Status:<br><span id=\"txtStatus\">");
#nullable restore
#line 32 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                                          Write(Model.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <hr class=\"blue-line\">\r\n                <div class=\"col-lg-4 col-sm-6\"><p>Illustration Reference Id:<br><span>");
#nullable restore
#line 34 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                                 Write(Model.IllustrationUniqueReference);

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n            </div>\r\n        </div>\r\n        <h4 class=\"heading-1\">Summary</h4>\r\n        <div class=\"summary-gray-backgrund\">\r\n            <div class=\"row\">\r\n                <div class=\"col-lg-4 col-sm-4\"><p>Total deposited:<br><span>");
#nullable restore
#line 40 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                       Write(Model.TotalDeposit.Value.ToString("N", gb));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <div class=\"col-lg-4 col-sm-4\"><p>Annual gross interest earned:<br><span>");
#nullable restore
#line 41 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                                    Write(Model.AnnualGrossInterestEarned.ToString("N", gb));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <div class=\"col-lg-4 col-sm-4\"><p>Annual net interest earned:<br><span>");
#nullable restore
#line 42 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                                  Write(Model.AnnualNetInterestEarned.ToString("N", gb));

#line default
#line hidden
#nullable disable
            WriteLiteral("</span></p></div>\r\n                <hr class=\"gray-line\">\r\n                <div class=\"col-lg-4 col-sm-4\"><p>Gross average yield:<br><span>");
#nullable restore
#line 44 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                           Write(Model.GrossAverageYield.ToString("N", gb));

#line default
#line hidden
#nullable disable
            WriteLiteral(" %</span></p></div>\r\n                <div class=\"col-lg-4 col-sm-4\"><p>Net average yield:<br><span>");
#nullable restore
#line 45 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                                         Write(Model.NetAverageYield.ToString("N", gb));

#line default
#line hidden
#nullable disable
            WriteLiteral(" %</span></p></div>\r\n\r\n\r\n\r\n");
#nullable restore
#line 49 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                 if (ViewBag.User == "")
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <hr class=\"gray-line\">\r\n");
            WriteLiteral("                    <div class=\"col-lg-12 col-md-12 col-sm-12\">\r\n                        <p>\r\n                            Comment:<br>\r\n                            <span id=\"txtComment\">\r\n                                ");
#nullable restore
#line 57 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                           Write(Model.Comment);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n                            </span>\r\n                        </p>\r\n                    </div>\r\n");
#nullable restore
#line 61 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                </div>
        </div>
    </div>
    <div class=""container"">
        <div class=""col-lg-10 offset-lg-1 col-sm-12"">
            <h4 class=""heading-1"">Deposits</h4>
            <div class=""deposits-table"">
                <table class=""table table-striped table-bordered"" id=""tblIllustration"">
                    <thead class=""thead-dark"">
                        <tr class=""d-flex"">
                            <th class=""col-4"">Institution</th>
                            <th class=""col-3"">Term</th>
                            <th class=""col-2 right"">Rate</th>
                            <th class=""col-3 right"">Deposit Size</th>
                        </tr>
                    </thead>
                    <tbody>
");
#nullable restore
#line 79 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                         for (int i = 0; i < @Model.ProposedPortfolio.ProposedInvestments.Count(); i++)
                        {

#line default
#line hidden
#nullable disable
            WriteLiteral("                            <tr class=\"d-flex\">\r\n                                <a href=\"#\">\r\n                                    <td class=\"col-4\">");
#nullable restore
#line 83 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                 Write(Model.ProposedPortfolio.ProposedInvestments[i].InstitutionName);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                    <td class=\"col-3\">");
#nullable restore
#line 84 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                 Write(Model.ProposedPortfolio.ProposedInvestments[i].InvestmentTerm.TermText);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                    <td class=\"col-2 right\">");
#nullable restore
#line 85 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                       Write(Model.ProposedPortfolio.ProposedInvestments[i].Rate);

#line default
#line hidden
#nullable disable
            WriteLiteral(" %</td>\r\n                                    <td class=\"col-3 right\">");
#nullable restore
#line 86 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                       Write(Model.ProposedPortfolio.ProposedInvestments[i].DepositSize.ToString("N",gb));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\r\n                                </a>\r\n                            </tr>\r\n");
#nullable restore
#line 89 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                        }

#line default
#line hidden
#nullable disable
            WriteLiteral("                    </tbody>\r\n                </table>\r\n");
            WriteLiteral("            </div>\r\n            <div class=\"clear1\"></div>\r\n");
            WriteLiteral("\r\n\r\n        <div class=\"center\">\r\n");
#nullable restore
#line 103 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
             if (ViewBag.User == "")
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <a class=\"btn btn-primary\" href=\"/Home/PreviousIllustration\">Back</a>\r\n");
#nullable restore
#line 106 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                 if (Model.Status.ToLower() != "deleted")
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <button type=\"button\" class=\"btn btn-primary\" data-toggle=\"modal\" data-target=\"#exampleModal\">\r\n                        Update Status\r\n                    </button>\r\n");
#nullable restore
#line 111 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                <a");
            BeginWriteAttribute("href", " href=\"", 5355, "\"", 5374, 1);
#nullable restore
#line 112 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
WriteAttributeValue("", 5362, ViewBag.URL, 5362, 12, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" target=\"_blank\" class=\"btn btn-primary\">Download Illustration</a>\r\n                <a");
            BeginWriteAttribute("href", " href=\"", 5461, "\"", 5570, 1);
#nullable restore
#line 113 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
WriteAttributeValue("", 5468, Url.Action("UpdateIllustration","Home", new {uniqueReferenceId= @Model.IllustrationUniqueReference }), 5468, 102, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(" class=\"btn btn-primary\">Update Illustration</a>\r\n");
#nullable restore
#line 115 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                 if (Model.Status.ToLower() == "accepted")
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                    <a href=\"https://www.insigniscash.com/partner/LoggedOut/Landing.aspx\" target=\"_blank\" class=\"btn btn-primary\">Create Client Pack</a>\r\n");
#nullable restore
#line 118 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                }
                else
                {

#line default
#line hidden
#nullable disable
            WriteLiteral(@"                    <button id=""btnClientPack"" type=""submit"" class=""btn btn-secondary disabled"" onclick=""window.open('https://www.insigniscash.com/partner/LoggedOut/Landing.aspx','_blank')"" value=""w3docs"" title=""This is available for Accepted Illustration"" disabled >Create Client Pack</button>    
");
#nullable restore
#line 122 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                    }

#line default
#line hidden
#nullable disable
#nullable restore
#line 122 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                     

            }
            else
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <a class=\"btn btn-primary\" href=\"/SuperUser/IllustrationList\">Back</a>\r\n                <a class=\"btn btn-primary\" target=\"_blank\"");
            BeginWriteAttribute("href", " href=\"", 6435, "\"", 6454, 1);
#nullable restore
#line 128 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
WriteAttributeValue("", 6442, ViewBag.URL, 6442, 12, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(">Download Illustration</a>\r\n");
#nullable restore
#line 129 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        </div>
        </div>
    </div>
</div>


<!-- Modal popup -->

<div class=""modal"" id=""exampleModal"" tabindex=""-1"" role=""dialog"" aria-labelledby=""exampleModalLabel"" aria-hidden=""true"">
    <div class=""modal-dialog modal-dialog-centered"" role=""document"">
        <div class=""modal-content modal-content-gray-bg"">
            <div class=""modal-header modal-header-bg"">
                <h5 class=""modal-title-white"" id=""exampleModalLabel"">Update Illustration Status</h5>
                <button type=""button"" class=""close"" data-dismiss=""modal"" aria-label=""Close"">
                    <span aria-hidden=""true"">&times;</span>
                </button>
            </div>
            <div class=""modal-body"">
                <div class=""popup-update-status"">
                    <div class=""row"">
                        <span id=""validation"" class=""text-danger""></span>
                    </div>
                    <div class=""form-group-popup row"">

                        <label for=""inputEma");
            WriteLiteral("il3\" class=\"col-sm-4 col-form-label\">Illustration Status</label>\r\n                        <div class=\"col-sm-8 col-form-label\"><b>");
#nullable restore
#line 155 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                                           Write(Model.Status);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</b></div>
                    </div>
                    <div class=""form-group-popup row"">
                        <label class=""col-sm-4 col-form-label"">Update Status</label>
                        <div class=""col-sm-8"">
                            <select id=""status"" name=""status"" class=""form-control"">
");
#nullable restore
#line 161 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                 if (Model.Status.ToLower() == "created")
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "51d91f121898ce3759860b70b055d510e317d7c222301", async() => {
                WriteLiteral("Accepted");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_0.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_0);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "51d91f121898ce3759860b70b055d510e317d7c223499", async() => {
                WriteLiteral("Deleted");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 165 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                }
                                else if (Model.Status.ToLower() == "accepted")
                                {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                    ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("option", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "51d91f121898ce3759860b70b055d510e317d7c225070", async() => {
                WriteLiteral("Deleted");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.OptionTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_OptionTagHelper.Value = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n");
#nullable restore
#line 169 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
                                }

#line default
#line hidden
#nullable disable
            WriteLiteral("                            </select>\r\n                        </div>\r\n                    </div>\r\n                    <div class=\"form-group-popup row\">\r\n");
            WriteLiteral("                        <div class=\"col-sm-8\">\r\n");
            WriteLiteral("                            <input id=\"uniqueReferenceId\" type=\"hidden\"");
            BeginWriteAttribute("value", " value=\"", 9015, "\"", 9057, 1);
#nullable restore
#line 177 "D:\insignis\ExternalIllustrator\Clients\InsignisIllustrationGenerator\Views\Shared\_illustrationDetails.cshtml"
WriteAttributeValue("", 9023, Model.IllustrationUniqueReference, 9023, 34, false);

#line default
#line hidden
#nullable disable
            EndWriteAttribute();
            WriteLiteral(@" name=""uniqueReferenceId"" />
                        </div>
                    </div>
                    <div class=""form-group-popup row"">
                        <label class=""col-sm-4 col-form-label"">Comment</label>
                        <div class=""col-sm-8"">
                            <textarea id=""comment"" class=""form-control""");
            BeginWriteAttribute("id", " id=\"", 9403, "\"", 9408, 0);
            EndWriteAttribute();
            WriteLiteral(@" rows=""4"" name=""comment""></textarea>
                        </div>
                    </div>
                    <div class=""form-group-popup row center"">
                        <div class=""col-sm-12"">
                            <input id=""btnUpdateStatus"" type=""button"" class=""btn btn-primary"" value=""Save"">
                        </div>
                    </div>

                </div>
            </div>

        </div>
    </div>
</div>





<script>
    $(document).ready(function () {


        $('#exampleModal').on('shown.bs.modal', function () {
            $(""#validation"").text("""");
            $(""#comment"").val("""");
        })

        $(document).on(""click"", ""#btnUpdateStatus"", function () {
            
            var comment = $(""#comment"").val();
            var referredBy = $(""#referredBy"").val();
            var status = $(""#status"").val();
            var uniqueId = $(""#uniqueReferenceId"").val();
            $.ajax({
                type: ""POST"",
      ");
            WriteLiteral(@"          url: ""../../Home/UpdateStatus"",
                data: {
                    comment: comment,
                    referredBy: referredBy,
                    status: status,
                    uniqueReferenceId: uniqueId
                },
                success: function (success) {
                    debugger;

                    if (success.success == true) {
                        $(""#exampleModal"").modal('hide');
                        $(""#txtComment"").text(comment);
                        $(""#txtStatus"").text(status);
                        if (status == ""Accepted"") {
                            $(""#btnClientPack"").removeClass(""disabled"");
                            $(""#btnClientPack"").attr(""disabled"", false);
                            $(""#btnClientPack"").tooltip(""disable"");;

                        }
                    } else {
                        $(""#validation"").text("""");
                        for (var i = 0; i < success.data.length; i++) {
        ");
            WriteLiteral("                    $(\"#validation\").append(success.data[i]);\r\n                        }\r\n\r\n                    }\r\n\r\n                },\r\n\r\n            });\r\n        });\r\n    });\r\n\r\n</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<IllustrationDetailViewModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
