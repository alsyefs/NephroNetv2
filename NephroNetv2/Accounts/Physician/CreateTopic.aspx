﻿<%@ Page Title="Create Topic" Language="C#" MasterPageFile="~/Physician.Master" AutoEventWireup="true" CodeBehind="CreateTopic.aspx.cs" Inherits="NephroNet.Accounts.Physician.CreateTopic" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <asp:UpdatePanel ID="upContent" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td><asp:Label ID="lblTitle" runat="server" Text="Title" Width ="100%"></asp:Label></td>
                                <td><asp:TextBox ID="txtTitle" runat="server" Width ="100%"></asp:TextBox></td>
                                <td><asp:Label ID="lblTitleError" runat="server" Text="Invalid input: Please type the title." Visible="false" ForeColor="red" Width ="100%"></asp:Label></td>

                            </tr>
                            <tr>
                                <td><asp:Label ID="lblType" runat="server" Text="Type" Width ="100%"></asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="drpType" runat="server" OnSelectedIndexChanged="drpType_SelectedIndexChanged" AutoPostBack="true" Width ="100%">
                                        <asp:ListItem>Select type</asp:ListItem>
                                        <asp:ListItem>Discussion</asp:ListItem>
                                        <asp:ListItem>Dissemination</asp:ListItem>
                                        <asp:ListItem>Consultation</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td><asp:Label ID="lblTypeError" runat="server" Text="Invalid input: Please select a type." Visible="false" ForeColor="red" Width ="100%"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblFindUser" runat="server" Text="Find patient" Visible ="false" Width ="100%"></asp:Label></td>
                                <td><asp:TextBox ID="txtFindUser" runat="server" Visible ="false" Width ="100%" AutoPostBack="true" OnTextChanged="txtFindUser_TextChanged"></asp:TextBox>
                                    <br />
                                <asp:ListBox ID="drpFindUser" runat="server" Visible ="false" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="drpFindUser_SelectedIndexChanged"></asp:ListBox></td>
                                <td><asp:Label ID="lblFindUserResult" runat="server" Text=" user" Visible ="false" Width ="100%"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblTags" runat="server" Text="Tags" Width ="100%"></asp:Label></td>
                                <td><asp:TextBox ID="txtTags" runat="server" Width ="100%"></asp:TextBox>
                                    <br />
                                    <asp:Label ID="lblTagsError" runat="server" Text="Warning: If the tags field is blank, an admin may choose to deny the topic." Width ="100%" Visible="true" ForeColor="yellow" BackColor="blue"></asp:Label></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblDescription" runat="server" Text="Description" Width ="100%"></asp:Label></td>
                                <td><asp:TextBox ID="txtDescription" runat="server" Height="100px" Width ="100%" TextMode="MultiLine" CssClass="content"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td> </td>
                                <td><asp:Label ID="lblDescriptionError" runat="server" Text="Invalid input: Please type a description." Visible="false" ForeColor="red" Width ="100%"></asp:Label></td>
                            </tr>
                            <tr>
                                <td> </td>
                                <td><asp:FileUpload ID="FileUpload1" runat="server" Width ="100%" AllowMultiple="true" onchange="onInputChange(event)" class="btn btn-primary" /></td>
                                <td><div id='fileNames' style="width:100%"></div></td>
                            </tr>
                            <tr>
                                <asp:Label ID="lblImageError" runat="server" Text="Image" Visible="false" ForeColor="red" Width ="100%"></asp:Label>
                            </tr>
                            <tr>
                                <%--Submit--%>
                                <td><asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Width ="100%" OnClick="btnSubmit_Click" /></td>
                                <td> </td>
                                <%--Cancel button--%>
                                <td><asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width ="140px" OnClick="btnCancel_Click" /></td>
                            </tr>
                        </table>
                        <%--Error message--%>
                        <br />
                        <br />
                        <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
                        <style>.content {min-width: 100%;}</style>
                        <script type="text/javascript">
                            function onInputChange(e) {
                                var res = "";
                                for (var i = 0; i < $('#<%= FileUpload1.ClientID %>').get(0).files.length; i++) {
                                    res += $('#<%= FileUpload1.ClientID %>').get(0).files[i].name + "<br />";
                                }
                                $('#fileNames').html(res);
                            }
                        </script>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="drpType" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtFindUser" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="drpFindUser" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>