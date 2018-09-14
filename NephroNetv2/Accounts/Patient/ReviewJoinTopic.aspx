<%@ Page Title="Review Join Request" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="ReviewJoinTopic.aspx.cs" Inherits="NephroNet.Accounts.Patient.ReviewJoinTopic" %>


<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start:--%>
                <%--<br />--%>
                <asp:Label ID="lblTopicInformation" runat="server" Text="Label"></asp:Label>
                <asp:Label ID="lblRequesterInfo" runat="server" Text="Label"></asp:Label>
                <br />
                <%--Table start--%>
                        <div runat="server" id="View">
                            <div >
                                <table border="1" style="width: 100%;">
                                    <asp:Label ID="lblRow" runat="server" Text=" "></asp:Label>
                                </table>
                            </div>
                        </div>
                        <%--Table end--%>
                <%--Approve--%>
                <br />
                <br />
                <asp:Button ID="btnApprove" runat="server" Text="Accept" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnApprove_Click" />
                <%--Deny:--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnDeny" runat="server" Text="Deny" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnDeny_Click" />
                <%--Go back:--%>
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="btnCancel" runat="server" Text="Go Back" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" />
                <br />
                <br />
                &nbsp;
            <asp:Label ID="lblMessage" runat="server" ForeColor="green" Text="Label" Visible="False"></asp:Label>
                <%--Content end.--%>
            </div>
        </div>
    </div>
</asp:Content>
