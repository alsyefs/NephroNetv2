<%@ Page Title="Change Password" Language="C#" MasterPageFile="~/Physician.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="NephroNet.Accounts.Physician.ChangePassword" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
    <h2><%: Title %></h2>
        <div class="panel panel-default">
        <div class="panel-body">
            <%--Type the new password:--%>
            &nbsp;&nbsp;&nbsp;
            <asp:Label ID="lblP1" runat="server" Text="Type new password" Font-Size="Medium"></asp:Label>
            &nbsp;
            <asp:TextBox ID="txtP1" runat="server" type="password" Font-Size="Medium"></asp:TextBox>
            &nbsp;
            <asp:Label ID="lblP1Error" runat="server" Text="P1 error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label>
            <%--Repeat the new password:--%>
            <br /><br />
            <asp:Label ID="lblP2" runat="server" Text="Repeat new password" Font-Size="Medium"></asp:Label>
            &nbsp;
            <asp:TextBox ID="txtP2" runat="server" type="password" Font-Size="Medium"></asp:TextBox>
            &nbsp;
            <asp:Label ID="lblP2Error" runat="server" Text="P1 error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label>
            <br /><br />
            <%--Password rules:--%>
            <pre style="font-family: Consolas; font-size:large; text-align:left; color: #dadada; background: #e9e894;">
<span style="color:Black;"> The&nbsp;password&nbsp;must&nbsp;match&nbsp;the&nbsp;following password&nbsp;requirements:</span>
<span style="color:Black;"> 1.&nbsp;At&nbsp;least&nbsp;eight&nbsp;characters&nbsp;long.</span>
<span style="color:Black;"> 2.&nbsp;Contains&nbsp;at&nbsp;least&nbsp;one&nbsp;upper-case&nbsp;letter.</span>
<span style="color:Black"> 3.&nbsp;Contains&nbsp;at&nbsp;least&nbsp;one&nbsp;lower-case&nbsp;letter.</span>
<span style="color:Black"> 4.&nbsp;Contains&nbsp;at&nbsp;least&nbsp;one&nbsp;digit&nbsp;(0-9).</span>
<span style="color:Black;"> 5.&nbsp;Contains&nbsp;one&nbsp;of&nbsp;the&nbsp;following&nbsp;special&nbsp;characters&nbsp;;&nbsp;,&nbsp;.&nbsp;!&nbsp;@&nbsp;#&nbsp;$&nbsp;%&nbsp;^&nbsp;&amp;&nbsp;*&nbsp;(&nbsp;)</span></pre>
            <%--Submit--%>
                <%--<br /><br />--%>
               <asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnSubmit_Click"/>
                &nbsp;
                
                <%--Cancel button--%>    
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click"  />
            <br />
            <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
            <asp:Label ID="lblSuccess" runat="server" ForeColor="green" Text="Label" Visible="False"></asp:Label>

            </div>
            </div>
        </div>
</asp:Content>
