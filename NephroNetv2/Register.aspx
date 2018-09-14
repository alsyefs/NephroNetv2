<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="NephroNet.Register" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="default" runat="server">
</asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container">
        <br />
        <h2>Please fill the registration form</h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <asp:UpdatePanel ID="upPatientId" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <div class="registerTable">
                        <table >
                            <tr>
                                <td>
                                    <asp:Label ID="lblFirstname" runat="server" Text="First name" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtFirstname" runat="server" Font-Size="Medium" ></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblFirstnameError" runat="server" Text="Firstname error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblLastname" runat="server" Text="Last name" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtLastname" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblLastnameError" runat="server" Text="Lastname error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblEmail" runat="server" Text="Email" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtEmail" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblEmailError" runat="server" Text="Email error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCountry" runat="server" Text="Country" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="drpCountries" runat="server" Font-Size="Medium" OnSelectedIndexChanged="drpCountries_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList></td>
                                <td>
                                    <asp:Label ID="lblCountryError" runat="server" Text="Country error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblCity" runat="server" Text="City" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtCity" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblCityError" runat="server" Text="City error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblState" runat="server" Text="State" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="drpStates" runat="server" Font-Size="Medium">
                                        <asp:ListItem>Select a state</asp:ListItem>
                                        <asp:ListItem>Alabama</asp:ListItem>
                                        <asp:ListItem>Alaska</asp:ListItem>
                                        <asp:ListItem>Arizona</asp:ListItem>
                                        <asp:ListItem>Arkansas</asp:ListItem>
                                        <asp:ListItem>California</asp:ListItem>
                                        <asp:ListItem>Colorado</asp:ListItem>
                                        <asp:ListItem>Connecticut</asp:ListItem>
                                        <asp:ListItem>Delaware</asp:ListItem>
                                        <asp:ListItem>Florida</asp:ListItem>
                                        <asp:ListItem>Georgia</asp:ListItem>
                                        <asp:ListItem>Hawaii</asp:ListItem>
                                        <asp:ListItem>Idaho</asp:ListItem>
                                        <asp:ListItem>Illinois</asp:ListItem>
                                        <asp:ListItem>Indiana</asp:ListItem>
                                        <asp:ListItem>Iowa</asp:ListItem>
                                        <asp:ListItem>Kansas</asp:ListItem>
                                        <asp:ListItem>Kentucky</asp:ListItem>
                                        <asp:ListItem>Louisiana</asp:ListItem>
                                        <asp:ListItem>Maine</asp:ListItem>
                                        <asp:ListItem>Maryland</asp:ListItem>
                                        <asp:ListItem>Massachusetts</asp:ListItem>
                                        <asp:ListItem>Michigan</asp:ListItem>
                                        <asp:ListItem>Minnesota</asp:ListItem>
                                        <asp:ListItem>Mississippi</asp:ListItem>
                                        <asp:ListItem>Missouri</asp:ListItem>
                                        <asp:ListItem>Montana</asp:ListItem>
                                        <asp:ListItem>Nebraska</asp:ListItem>
                                        <asp:ListItem>Nevada</asp:ListItem>
                                        <asp:ListItem>New Hampshire</asp:ListItem>
                                        <asp:ListItem>New Jersey</asp:ListItem>
                                        <asp:ListItem>New Mexico</asp:ListItem>
                                        <asp:ListItem>New York</asp:ListItem>
                                        <asp:ListItem>North Carolina</asp:ListItem>
                                        <asp:ListItem>North Dakota</asp:ListItem>
                                        <asp:ListItem>Ohio</asp:ListItem>
                                        <asp:ListItem>Oklahoma</asp:ListItem>
                                        <asp:ListItem>Oregon</asp:ListItem>
                                        <asp:ListItem>Pennsylvania</asp:ListItem>
                                        <asp:ListItem>Rhode Island</asp:ListItem>
                                        <asp:ListItem>South Carolina</asp:ListItem>
                                        <asp:ListItem>South Dakota</asp:ListItem>
                                        <asp:ListItem>Tennessee</asp:ListItem>
                                        <asp:ListItem>Texas</asp:ListItem>
                                        <asp:ListItem>Utah</asp:ListItem>
                                        <asp:ListItem>Vermont</asp:ListItem>
                                        <asp:ListItem>Virginia</asp:ListItem>
                                        <asp:ListItem>Washington</asp:ListItem>
                                        <asp:ListItem>West Virginia</asp:ListItem>
                                        <asp:ListItem>Wisconsin</asp:ListItem>
                                        <asp:ListItem>Wyoming</asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:TextBox ID="txtState" runat="server" Font-Size="Medium" Visible="false"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:Label ID="lblStateError" runat="server" Text="State error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblZip" runat="server" Text="Zip code" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtZip" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblZipError" runat="server" Text="Zip error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblAddress" runat="server" Text="Address" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtAddress" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblAddressError" runat="server" Text="Address error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPhone" runat="server" Text="Phone#" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtPhone" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblPhoneError" runat="server" Text="Phone error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblRole" runat="server" Text="Role" Font-Size="Medium"></asp:Label></td>
                                <td>
                                    <asp:DropDownList ID="drpRole" runat="server" Font-Size="Medium" AutoPostBack="true" OnSelectedIndexChanged="drpRole_SelectedIndexChanged">
                                        <asp:ListItem>Select a role</asp:ListItem>
                                        <asp:ListItem>Admin</asp:ListItem>
                                        <asp:ListItem>Physician</asp:ListItem>
                                        <asp:ListItem>Patient</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                                <td>
                                    <asp:Label ID="lblRoleError" runat="server" Text="Role error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblPatientId" runat="server" Text="Patient ID" Font-Size="Medium" Visible="False"></asp:Label></td>
                                <td>
                                    <asp:TextBox ID="txtPatientId" runat="server" Font-Size="Medium" Visible="False"></asp:TextBox></td>
                                <td>
                                    <asp:Label ID="lblPatientIdError" runat="server" Text="Patient ID error" ForeColor="Red" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>


                            <tr>
                                <td>
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit" BackColor="Green" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnSubmit_Click" /></td>
                                <td>
                                    <asp:Button ID="btnCancel" runat="server" Text="Go back" BackColor="red" Font-Bold="True" Font-Size="Medium" Height="34px" Width="140px" OnClick="btnCancel_Click" /></td>

                            </tr>
                            <tr>
                                <td></td>
                                <td><asp:Label ID="lblResult" runat="server" Text="Result" Visible="False" Font-Size="Medium"></asp:Label></td>
                            </tr>
                        </table>
                            </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="drpRole" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <br />

            </div>
        </div>
    </div>
</asp:Content>
