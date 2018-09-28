<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Physician.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="NephroNet.Accounts.Physician.MyProfile" EnableEventValidation="false" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <%--<h2><%: Title %></h2>--%>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
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
                        <%--Table View start--%>
                        <div runat="server" id="View">
                            <div>
                                <table border="1" style="width: 100%;">
                                    <asp:Label ID="lblRow" runat="server" Text=" "></asp:Label>
                                </table>
                                <br />
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnCompleteProfile" runat="server" Text="Edit Complete Profile" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCompleteProfile_Click" /></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <%--Table View end--%>
                        <%--Table Edit Complete Profile start--%>
                        <div runat="server" id="EditCompleteProfile">
                            <div>
                                <h3>Edit Complete Profile Information</h3>
                                <%--Table Edit Complete Profile end--%>
                                <table style="width: 100%;">
                                    <tr>
                                        <td ><asp:Label ID="lblPrivate" runat="server" Text="Set Profile to Private" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td >
                                            <asp:CheckBox ID="chkIsPrivate" runat="server" Font-Size="Medium" Width="100%" OnCheckedChanged="chkIsPrivate_CheckedChanged" AutoPostBack="true"/>
                                        </td>
                                        <td ><asp:Label ID="lblPrivateMessage" runat="server" Text=" " Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblDialysis" runat="server" Text="Dialysis" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtDialysis" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblDialysisError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblHomeDialysis" runat="server" Text="Home Dialysis" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtHomeDialysis" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblHomeDialysisError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblTransplantation" runat="server" Text="Transplantation" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtTransplantation" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblTransplantationError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    
                                    <tr>
                                        <td ><asp:Label ID="lblHypertension" runat="server" Text="Hypertension" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtHypertension" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblHypertensionError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblGN" runat="server" Text="GN" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtGN" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblGNError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblPhysicianId" runat="server" Text="Physician ID" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtPhysicianId" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblPhysicianIdError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                </table>
                                <%--Table Edit Complete Profile end--%>
                                <br />
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td><asp:Button ID="btnSaveEditCompleteProfile" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveEditCompleteProfile_Click" /></td>
                                        <td><asp:Button ID="btnCancelEditCompleteProfile" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelEditCompleteProfile_Click" /></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblSaveCompleteProfileMessage" runat="server" Text="You have successfully updated your complete profile!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <div runat="server" id="UserAgreement">
                            <table style="width: 100%;">
                                <tr>
                                    <td><asp:TextBox ID="txtUserAgreement" runat="server" Font-Size="medium" Width="100%" Height="500px" TextMode="MultiLine" ReadOnly="True"></asp:TextBox></td>
                                </tr>
                            </table>
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:Label ID="lblBlank1" runat="server" Text=" " Font-Size="Medium" Width="100%"></asp:Label>
                                        <asp:CheckBox ID="chkAgree" runat="server" Font-Size="Medium" Checked="false" Width="100%" AutoPostBack="true" OnCheckedChanged="chkAgree_CheckedChanged"/>
                                        <asp:Label ID="lblBlank2" runat="server" Text=" " Font-Size="Medium" Width="100%"></asp:Label>
                                    </td>
                                    <td><asp:Label ID="lblAgree" runat="server" Text=" " Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                            </table>
                            <table style="width: 100%;">
                                <tr>
                                    <td><asp:Button ID="btnAgree" runat="server" Text="Agree and Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnAgree_Click"  /></td>
                                    <td><asp:Button ID="btnDisagree" runat="server" Text="Disagree" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnDisagree_Click"  /></td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td ><asp:Label ID="lblAgreeError" runat="server" Text=" " Font-Size="Medium" Width="100%" ForeColor="red"></asp:Label></td>
                                </tr>
                            </table>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveEditCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelEditCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="chkIsPrivate" EventName="CheckedChanged" />
                        <%--User Agreement controls:--%>
                        <asp:AsyncPostBackTrigger ControlID="btnAgree" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnDisagree" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="chkAgree" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
