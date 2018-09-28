<%@ Page Title="Account" Language="C#" MasterPageFile="~/Physician.Master" AutoEventWireup="true" CodeBehind="Account.aspx.cs" Inherits="NephroNet.Accounts.Physician.Account" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <script>
                    function pleaseWait() {
                        $(".modal").show();
                        return true;
                    }
                </script>
                <div id="modal" class="modal" style="background-color:rgba(64,64,64,0.5);width:100%;height:100%;z-index:1000;display:none"></div>
                <div id="wait" class="modal" style="width:200px;height:20px;margin:100px auto 0 auto;display:none;background-color:#fff;z-index:1001;text-align:center;">PLEASE WAIT...</div>
                <%--Content start--%>
                <%--Change password:--%>
                <%--<asp:Label ID="lblChangePassword" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangePassword_Click"  OnClientClick="pleaseWait();"/>
                <br />
                <%--Change security questions:--%>
                <%--<asp:Label ID="lblChangeSecurityQuestions" runat="server" Text="Label"></asp:Label>--%>
                &nbsp;
            <asp:Button ID="btnChangeSecurityQuestions" runat="server" Text="Change Security Questions" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeSecurityQuestions_Click" OnClientClick="pleaseWait();" />
                <br />
                <%--&nbsp;
            <asp:Button ID="btnSetViewShortProfilePermissions" runat="server" Text="Set Short Profile View Permissions" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnSetViewShortProfilePermissions_Click"  />
                <br />
                &nbsp;
            <asp:Button ID="btnChangeShortProfileInfo" runat="server" Text="Change Short Profile Information" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeShortProfileInfo_Click"  />
                <br />
                &nbsp;
            <asp:Button ID="btnChangeCompleteProfileInfo" runat="server" Text="Change Complete Profile Information" Width="295px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnChangeCompleteProfileInfo_Click"  />
                <br />--%>
                <%--General error message:--%>
                <asp:Label ID="lblError" runat="server" Text="Label" Visible="false" Font-Bold="true" ForeColor="Red" Font-Size="Medium"></asp:Label>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
