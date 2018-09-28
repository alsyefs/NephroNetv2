<%@ Page Title="Alerts" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="NephroNet.Accounts.Admin.Notifications" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

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
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNewUsers" runat="server" Text="Label"></asp:Label></td>
                        <td>
                            <asp:Button ID="btnNewUsers" runat="server" Text="Review Users" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewUsers_Click" OnClientClick="pleaseWait();"/></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblNewTopics" runat="server" Text="Label"></asp:Label></td>
                        <td>
                            <asp:Button ID="btnNewTopics" runat="server" Text="Review Topics" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewTopics_Click" OnClientClick="pleaseWait();"/></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblNewMessages" runat="server" Text="Label"></asp:Label></td>
                        <td><asp:Button ID="btnNewMessages" runat="server" Text="Review Messages" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewMessages_Click" OnClientClick="pleaseWait();"/></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblNewComplains" runat="server" Text="Label"></asp:Label></td>
                        <td><asp:Button ID="btnNewComplains" runat="server" Text="Review new complains" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewComplains_Click" OnClientClick="pleaseWait();"/></td>
                    </tr>
                </table>
                <br />
                <%--General error message:--%>
                <asp:Label ID="lblError" runat="server" Text="Label" Visible="false" Font-Bold="true" ForeColor="Red" Font-Size="Medium"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
