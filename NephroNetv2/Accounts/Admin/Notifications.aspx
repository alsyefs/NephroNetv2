<%@ Page Title="Alerts" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="NephroNet.Accounts.Admin.Notifications" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lblNewUsers" runat="server" Text="Label"></asp:Label></td>
                        <td>
                            <asp:Button ID="btnNewUsers" runat="server" Text="Review Users" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewUsers_Click" /></td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lblNewTopics" runat="server" Text="Label"></asp:Label></td>
                        <td>
                            <asp:Button ID="btnNewTopics" runat="server" Text="Review Topics" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewTopics_Click" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblNewMessages" runat="server" Text="Label"></asp:Label></td>
                        <td><asp:Button ID="btnNewMessages" runat="server" Text="Review Messages" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewMessages_Click" /></td>
                    </tr>
                    <tr>
                        <td><asp:Label ID="lblNewJoinTopicRequests" runat="server" Text="Label"></asp:Label></td>
                        <td><asp:Button ID="btnNewJoinTopicRequests" runat="server" Text="Review Join Requests" Width="190px" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" OnClick="btnNewJoinTopicRequests_Click" /></td>
                    </tr>
                </table>
                <br />
                <%--General error message:--%>
                <asp:Label ID="lblError" runat="server" Text="Label" Visible="false" Font-Bold="true" ForeColor="Red" Font-Size="Medium"></asp:Label>
            </div>
        </div>
    </div>
</asp:Content>
