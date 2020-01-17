<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RunTimeError.aspx.cs" Inherits="Insignis.Asset.Management.External.Illustrator.RunTimeError" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentHead" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentHeaderTop" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentHeader" runat="server">
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="ContentPreBody" runat="server">
<div id="page-header" style="background-image:url(../Images/HeaderBackground1920x250-1.jpg);">
    <div id="page-header-overlay"></div>
    <div class="row">
        <div class="span1">
        </div>
        <div class="span9">
            <h3>An Error Has Occurred</h3>
        </div>
        <div class="span2">
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="ContentBody" runat="server">
<div class="row">
    <div class="span12">
        <div class="headline-2">
            <h4>An Error has Occured</h4>
        </div>
        <p></p>
        <p>An unexpected error has occured on our website. The support team have been notified and will investigate further.</p>
        <p></p>
    </div>
</div>
<div class="row">
    <div class="span12" id="panelDetails" runat="server">
    </div>
</div>
</asp:Content>
<asp:Content ID="Content6" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
<asp:Content ID="Content7" ContentPlaceHolderID="ContentFooterBottom" runat="server">
</asp:Content>
<asp:Content ID="Content8" ContentPlaceHolderID="ContentBelowFooter" runat="server">
</asp:Content>
