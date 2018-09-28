<%@ Page Title="Terminate Topic" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="TerminateTopic.aspx.cs" Inherits="NephroNet.Accounts.Admin.TerminateTopic" %>

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
                <asp:Label ID="lblMessageInformation" runat="server" Text="Are you sure you want to terminate the selected topic?"></asp:Label>
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
