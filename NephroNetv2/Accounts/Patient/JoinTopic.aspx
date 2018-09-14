<%@ Page Title="Request To Join" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="JoinTopic.aspx.cs" Inherits="NephroNet.Accounts.Patient.JoinTopic" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <%--<h2><%: Title %></h2>--%>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                <%--Show topic basic information:--%>
                <asp:Label ID="lblTopic" runat="server" Text=" "></asp:Label>
                <br />
                <%--Request to join--%>
                <asp:Button ID="btnRequest" runat="server" Text="Join Topic" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnRequest_Click" />
                &nbsp;
                
                <%--Cancel button--%>    
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" />
                <%--Error message--%>
                <br />
                <br />
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
                <%--Content end--%>
            </div>
        </div>
    </div>
</asp:Content>
