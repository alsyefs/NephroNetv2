<%@ Page Title="Create Topic" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="CreateTopic.aspx.cs" Inherits="NephroNet.Accounts.Patient.CreateTopic" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
  <%--Body start--%>
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
                <asp:UpdatePanel ID="upContent" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div runat="server" id="divMain" >
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
                                <td><asp:TextBox ID="txtFindUser" runat="server" Visible ="false" Width ="100%" AutoPostBack="true" OnTextChanged="txtFindUser_TextChanged"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td><asp:Label ID="lblSelectUser" runat="server" Text="Select patient" Visible ="false" Width ="100%"></asp:Label></td>
                                <td><asp:ListBox ID="drpFindUser" runat="server" Visible ="false" Width="100%" AutoPostBack="true" OnSelectedIndexChanged="drpFindUser_SelectedIndexChanged"></asp:ListBox>
                                    <br />
                                <asp:Label ID="lblFindUserResult" runat="server" Text="" Visible ="false" Width ="100%"></asp:Label></td>
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
                                <td><asp:Label ID="lblDescriptionError" runat="server" Text="Invalid input: Please type a description." Visible="false" ForeColor="red" Width ="100%"></asp:Label></td>
                            </tr>
                            </table>
                            </div>
                        <div runat="server" id="divUserInformation" class="userInformationPopup">
                            <div runat="server" class="tableUserInformation">
                                <asp:Label ID="lblUserInformation" runat="server" Text="" Width ="100%"></asp:Label>
                                <asp:Button ID="btnOk" runat="server" Text="Ok" BackColor="Green" CssClass="userInformationButton" Font-Bold="True" Font-Size="Medium" Width ="140px" OnClick="btnOk_Click" />
                                <br />
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="btnSubmit" EventName="Click" />--%>
                        <asp:AsyncPostBackTrigger ControlID="drpType" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtFindUser" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="drpFindUser" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="btnOk" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <table>
                    <tr>
                                <td><asp:Label ID="lblFileUpload" runat="server" Text="Upload images" Width ="100%"></asp:Label></td>
                                <td><asp:FileUpload ID="FileUpload1" runat="server" Width ="100%" AllowMultiple="true" onchange="onInputChange(event)" class="btn btn-primary" /></td>
                                <td><div id='fileNames' style="width:100%"></div></td>
                            </tr>
                            <tr>
                                <asp:Label ID="lblImageError" runat="server" Text="Image" Visible="false" ForeColor="red" Width ="100%"></asp:Label>
                            </tr>
                            <tr>
                                <%--Submit--%>
                                <td><asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Width ="140px" OnClick="btnSubmit_Click" OnClientClick="pleaseWait();"/></td>
                                <td> </td>
                                <%--Cancel button--%>
                                <td><asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width ="140px" OnClick="btnCancel_Click" OnClientClick="pleaseWait();"/></td>
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
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
