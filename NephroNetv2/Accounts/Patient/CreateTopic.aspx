<%@ Page Title="Create Topic" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="CreateTopic.aspx.cs" Inherits="NephroNet.Accounts.Patient.CreateTopic" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
        <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblTitle" runat="server" Text="Title"></asp:Label>
                &nbsp;
                <asp:TextBox ID="txtTitle" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Label ID="lblTitleError" runat="server" Text="Invalid input: Please type the title." Visible="false" ForeColor="red"></asp:Label>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblType" runat="server" Text="Type"></asp:Label>
                &nbsp;
                <asp:DropDownList ID="drpType" runat="server">
                    <asp:ListItem>Select type</asp:ListItem>
                    <asp:ListItem>Discussion</asp:ListItem>
                    <asp:ListItem>Dissemination</asp:ListItem>
                </asp:DropDownList>
                &nbsp;
                <asp:Label ID="lblTypeError" runat="server" Text="Invalid input: Please select a type." Visible="false" ForeColor="red"></asp:Label>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblTags" runat="server" Text="Tags"></asp:Label>
                &nbsp;
                <asp:TextBox ID="txtTags" runat="server"></asp:TextBox>
                &nbsp;
                <asp:Label ID="lblTagsError" runat="server" Text="Warning: If this field is blank, an admin may choose to deny the topic." Visible="true" ForeColor="yellow" BackColor="blue"></asp:Label>
                <br />
                <asp:Label ID="lblDescription" runat="server" Text="Description"></asp:Label>
                &nbsp;
                <style>
                    .content {
                        min-width: 100%;
                    }
                </style>
                <asp:TextBox ID="txtDescription" runat="server" Height="130px" Width="959px" TextMode="MultiLine" CssClass="content"></asp:TextBox>
                <br />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="lblDescriptionError" runat="server" Text="Invalid input: Please type a description." Visible="false" ForeColor="red"></asp:Label>
                <br />
                <br />


                <asp:FileUpload ID="FileUpload1" runat="server" Width="385px" AllowMultiple="true" onchange="onInputChange(event)" class="btn btn-primary" />
                <div id='fileNames'></div>
                <script type="text/javascript">
                    function onInputChange(e) {
                        var res = "";
                        for (var i = 0; i < $('#<%= FileUpload1.ClientID %>').get(0).files.length; i++) {
                            res += $('#<%= FileUpload1.ClientID %>').get(0).files[i].name + "<br />";
                        }
                        $('#fileNames').html(res);
                    }
                </script>
                &nbsp;
                <asp:Label ID="lblImageError" runat="server" Text="Image" Visible="false" ForeColor="red"></asp:Label>
                <%--Submit--%><br />
                <br />
                <asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnSubmit_Click" />
                &nbsp;
                
                <%--Cancel button--%>    
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" />
                <%--Error message--%>
                <br />
                <br />
                <asp:Label ID="lblError" runat="server" ForeColor="Red" Text="Label" Visible="False"></asp:Label>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
