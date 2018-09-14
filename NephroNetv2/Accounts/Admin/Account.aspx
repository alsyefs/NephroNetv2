<%@ Page Title="Account" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="NephroNet.Accounts.Admin.Account" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                <%--Change password:--%>
                <%--<asp:Label ID="lblChangePassword" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangePassword_Click"  />
                <br />
                <%--Change security questions:--%>
                <%--<asp:Label ID="lblChangeSecurityQuestions" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangeSecurityQuestions" runat="server" Text="Change Security Questions" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeSecurityQuestions_Click"  />
                <br />
                <%--Change short profile permissions:--%>
                <%--<asp:Label ID="lblSetViewShortProfilePermissions" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnSetViewShortProfilePermissions" runat="server" Text="Set Short Profile View Permissions" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnSetViewShortProfilePermissions_Click"  />
                <br />
                <%--Change short profile information:--%>
                <%--<asp:Label ID="lblChangeShortProfileInfo" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangeShortProfileInfo" runat="server" Text="Change Short Profile Information" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeShortProfileInfo_Click"  />
                <br />
                <%--Change complete profile information:--%>
                <%--<asp:Label ID="lblChangeCompleteProfileInfo" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangeCompleteProfileInfo" runat="server" Text="Change Complete Profile Information" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeCompleteProfileInfo_Click"  />
                <br />
                <%--General error message:--%>
                <asp:Label ID="lblError" runat="server" Text="Label" Visible="false" Font-Bold="true" ForeColor="Red" Font-Size="Medium"></asp:Label>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
