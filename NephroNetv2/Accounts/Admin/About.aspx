<%@ Page Title="About" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="NephroNet.Accounts.Admin.About" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                <h3>Nephrology social network</h3>
                <p>This website serves as a social network for people who need to communicate with others having the same medical situations to find help.</p>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
