<%@ Page Title="Terminate Account" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="TerminateAccount.aspx.cs" Inherits="NephroNet.Accounts.Admin.TerminateAccount" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Page body start:--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Show message information:--%>
                <asp:Label ID="lblMessageInformation" runat="server" Text="Are you sure you want to terminate the selected account?"></asp:Label>
                <br />
                <br />
                <asp:Button ID="btnTerminate" runat="server" Text="Terminate" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnTerminate_Click" />
                <%--Cancel, or Go Back button--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClientClick="javascript:window.close();" />
                <script type="text/javascript">
                    function CloseWindow() {
                        window.close();
                    }
                </script>

            </div>
        </div>
    </div>
    <%--Page body end.--%>
</asp:Content>
