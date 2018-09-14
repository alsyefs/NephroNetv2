<%@ Page Title="Alerts" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="NephroNet.Accounts.Patient.Notifications" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container">
        <br />
    <h2><%: Title %></h2>
        <div class="panel panel-default">
        <div class="panel-body">

             <table>
                  <%--New requests to join topics:--%>
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
