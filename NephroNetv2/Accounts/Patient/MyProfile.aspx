<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="NephroNet.Accounts.Patient.MyProfile" EnableEventValidation="false" %>

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
                                        <td ><asp:Label ID="lblHighBloodPressure" runat="server" Text="High Blood Pressure" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:textbox ID="txtHighBloodPressure" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:textbox></td>
                                        <td ><asp:Label ID="lblHighBloodPressureError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblDiabetes" runat="server" Text="Diabetes" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtDiabetes" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblDiabetesError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblKidneyTransplant" runat="server" Text="Kidney Transplant" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtKidneyTransplant" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblKidneyTransplantError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblDialysis" runat="server" Text="Dialysis" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtDialysis" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblDialysisError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblKidneyStone" runat="server" Text="Kidney Stone" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtKidneyStone" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblKidneyStoneError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblKidneyInfection" runat="server" Text="Kidney Infection" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtKidneyInfection" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblKidneyInfectionError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblHeartFailure" runat="server" Text="Heart Failure" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtHeartFailure" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblHeartFailureError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblCancer" runat="server" Text="Cancer" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtCancer" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblCancerError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblComments" runat="server" Text="Comments" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtComments" TextMode="MultiLine" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblCommentsError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td ><asp:Label ID="lblPatientId" runat="server" Text="Patient ID" Font-Size="Medium" Width="100%"></asp:Label></td>
                                        <td ><asp:TextBox ID="txtPatientId" runat="server" Font-Size="Medium" Width="100%" Enabled="false"></asp:TextBox></td>
                                        <td ><asp:Label ID="lblPatientIdError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td> </td>
                                        <td><asp:Button ID="btnSetNewPassword" runat="server" Text="Set New Password" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Width="100%" OnClick="btnSetNewPassword_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td> </td>
                                        <td><asp:Button ID="btnUpdatePassword" runat="server" Text="Update Profile Password" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Width="100%" OnClick="btnUpdatePassword_Click" /></td>
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
                        <div runat="server" id="SetNewPassword">
                            <h4>Please enter your new password</h4>
                            <p style="color:red; font:bold;" >
                                Remember to store your password in a safe location. This password is very secure and no one has access to it, not even the system administrators. In case this password is lost, your information will be encrypted and locked, and there will be no way to access it again!
                            </p>
                            <table style="width: 100%;">
                                <tr>
                                    <td><asp:Label ID="lblPassword1" runat="server" Text="Type your new password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    <td><asp:TextBox ID="txtPassword1" type="password" runat="server" Font-Size="medium" Width="100%" ></asp:TextBox></td>
                                    <td ><asp:Label ID="lblPassword1Error" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblPassword2" runat="server" Text="Repeat your new password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    <td><asp:TextBox ID="txtPassword2" type="password" runat="server" Font-Size="medium" Width="100%"></asp:TextBox></td>
                                    <td ><asp:Label ID="lblPassword2Error" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                            </table>
                            <table style="width: 100%;">
                                    <tr>
                                        <td><asp:Button ID="btnSaveNewPassword" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveNewPassword_Click"  /></td>
                                        <td><asp:Button ID="btnCancelNewPassword" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelNewPassword_Click"  /></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblSaveNewPasswordMessage" runat="server" Text="You have successfully updated your complete profile password!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                        </div>
                        <div runat="server" id="UpdatePassword">
                            <p style="color:red; font:bold;" >
                                Remember to store your password in a safe location. This password is very secure and no one has access to it, not even the system administrators. In case this password is lost, your information will be encrypted and locked, and there will be no way to access it again!
                            </p>
                            <table style="width: 100%;">
                                <tr>
                                    <td><asp:Label ID="lblOldPassword" runat="server" Text="Type your old password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    <td><asp:TextBox ID="txtOldPassword" type="password" runat="server" Font-Size="medium" Width="100%" ></asp:TextBox></td>
                                    <td ><asp:Label ID="lblOldPasswordError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblUpdatePassword1" runat="server" Text="Type your new password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    <td><asp:TextBox ID="txtUpdatePassword1" type="password" runat="server" Font-Size="medium" Width="100%" ></asp:TextBox></td>
                                    <td ><asp:Label ID="lblUpdatePassword1Error" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td><asp:Label ID="lblUpdatePassword2" runat="server" Text="Repeat your new password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                    <td><asp:TextBox ID="txtUpdatePassword2" type="password" runat="server" Font-Size="medium" Width="100%" ></asp:TextBox></td>
                                    <td ><asp:Label ID="lblUpdatePassword2Error" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                                </tr>
                            </table>
                            <table style="width: 100%;">
                                    <tr>
                                        <td><asp:Button ID="btnSaveUpdatePassword" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveUpdatePassword_Click"   /></td>
                                        <td><asp:Button ID="btnCancelUpdatePassword" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelUpdatePassword_Click"   /></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblUpdatePasswordMessage" runat="server" Text="You have successfully updated your complete profile password!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                        </div>
                        <div runat="server" id="TypePassword">
                            <h3>Your account is set to private</h3>
                            <p>Please enter your password</p>
                            <table style="width: 100%;">
                            <tr>
                                <td><asp:Label ID="lblPassword" runat="server" Text="Type your profile's password" Font-Size="Medium" Width="100%"></asp:Label></td>
                                <td><asp:TextBox ID="txtPassword" type="password" runat="server" Font-Size="medium" Width="100%" ></asp:TextBox></td>
                                <td ><asp:Label ID="lblPasswordError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium" Width="100%"></asp:Label></td>
                            </tr>
                            </table>
                            <table style="width: 100%;">
                                    <tr>
                                        <td><asp:Button ID="btnSubmitPassword" runat="server" Text="Submit" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSubmitPassword_Click" /></td>
                                    </tr>
                                </table>
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
                        <asp:AsyncPostBackTrigger ControlID="btnSetNewPassword" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnUpdatePassword" EventName="Click" />
                        <%--User Agreement controls:--%>
                        <asp:AsyncPostBackTrigger ControlID="btnAgree" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnDisagree" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="chkAgree" EventName="CheckedChanged" />
                        <%--Set/Update password--%>
                        <asp:AsyncPostBackTrigger ControlID="btnSaveNewPassword" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelNewPassword" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveUpdatePassword" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelUpdatePassword" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSubmitPassword" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
