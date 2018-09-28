<%@ Page Title="Review Message" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="ReviewMessage.aspx.cs" Inherits="NephroNet.Accounts.Admin.ReviewMessage" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
     <%--Page body start:--%>
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

            <%--Show message information:--%>
            <asp:Label ID="lblMessageInformation" runat="server" Text="Label"></asp:Label>

            <%--Approve--%>
            <br /> <br />
            <asp:Button ID="btnApprove" runat="server" Text="Approve" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnApprove_Click" OnClientClick="pleaseWait();"/>            
            <%--Deny:--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDeny" runat="server" Text="Deny" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnDeny_Click" OnClientClick="pleaseWait();"/>
            <%--Go back:--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Go Back" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" OnClientClick="pleaseWait();"/>
            <br /><br />
            &nbsp;
            <asp:Label ID="lblMessage" runat="server" ForeColor="green" Text="Label" Visible="False"></asp:Label>



             </div>
            </div>
        </div>
    <%--Page body end.--%>
</asp:Content>
