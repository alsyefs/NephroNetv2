<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="NephroNet.Accounts.Patient.MyProfile" EnableEventValidation="false" %>

<%--<asp:Content ID="Content1" ContentPlaceHolderID="default" runat="server">
</asp:Content>--%>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <%--<h2><%: Title %></h2>--%>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>

                <asp:UpdatePanel ID="upContent" UpdateMode="Conditional" runat="server">
                    <ContentTemplate>
                        <%--Table start--%>
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
                                            <asp:Button ID="btnShortProfile" runat="server" Text="Edit Short Profile" BackColor="orange" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnShortProfile_Click" /></td>
                                        <td>
                                            <asp:Button ID="btnCompleteProfile" runat="server" Text="Edit Complete Profile" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCompleteProfile_Click" /></td>
                                    </tr>
                                </table>
                            </div>
                        </div>
                        <%--Table end--%>

                        <%--Table start--%>
                        <div runat="server" id="EditShortProfile">
                            <div>
                                <table style="width: 100%;">
                                    <tr>
                                        <td>Edit Short Profile Information</td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblFirstname" runat="server" Text="First name" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:TextBox ID="txtFirstname" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblFirstnameError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblLastname" runat="server" Text="Last name" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:TextBox ID="txtLastname" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblLastnameError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblRace" runat="server" Text="Race" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpRace" runat="server" Font-Size="Medium">
                                                <asp:ListItem>Select your race</asp:ListItem>
                                                <asp:ListItem>Asian</asp:ListItem>
                                                <asp:ListItem>African/Black</asp:ListItem>
                                                <asp:ListItem>Hawaiian/Pacific Islander</asp:ListItem>
                                                <asp:ListItem>Indian/Alaska Native</asp:ListItem>
                                                <asp:ListItem>Hispanic/Latino</asp:ListItem>
                                                <asp:ListItem>Middle-Eastern</asp:ListItem>
                                                <asp:ListItem>Caucasian</asp:ListItem>
                                                <asp:ListItem>Some other</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblRaceError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblGender" runat="server" Text="Gender" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpGender" runat="server" Font-Size="Medium">
                                                <asp:ListItem>Select gender</asp:ListItem>
                                                <asp:ListItem>Female</asp:ListItem>
                                                <asp:ListItem>Male</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblGenderError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblBirthdateYear" runat="server" Text="Birthdate Year" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpYearList" runat="server" AutoPostBack="true" OnSelectedIndexChanged="drpYearList_SelectedIndexChanged"></asp:DropDownList>
                                        </td>

                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblBirthdate" runat="server" Text="Birthdate" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Calendar ID="calBirthdate" runat="server"></asp:Calendar>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblBirthdateError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblNationality" runat="server" Text="Nationality" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpNationality" runat="server" Font-Size="Medium"></asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblNationalityError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblRoleNameView" runat="server" Text="Role" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblRoleNameViewDisplay" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblIsPrivate" runat="server" Text="View permission" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpIsPrivate" runat="server" Font-Size="Medium">
                                                <asp:ListItem>Select View Permission</asp:ListItem>
                                                <asp:ListItem>Public</asp:ListItem>
                                                <asp:ListItem>Private</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblIsPrivateError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblBlockedUsersView" runat="server" Text="Blocked users" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblBlockedUsersViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>
                                        <td class="thirdCell">
                                            <asp:Button ID="btnEditBlockedUsersView" runat="server" Text="Edit Blocked Users" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnEditBlockedUsersView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblCurrentHealthConditionsView" runat="server" Text="Current health conditions" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblCurrentHealthConditionsViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>
                                        <td class="thirdCell">
                                            <asp:Button ID="btnCurrentHealthConditionsView" runat="server" Text="Edit Current Conditions" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnCurrentHealthConditionsView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblCurrentTreatmentsView" runat="server" Text="Current treatments" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblCurrentTreatmentsViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>
                                        <td class="thirdCell">
                                            <asp:Button ID="btnCurrentTreatmentsView" runat="server" Text="Edit Current Treatments" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnCurrentTreatmentsView_Click" /></td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnSaveEditShortProfile" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveEditShortProfile_Click"/></td>
                                        <td>
                                            <asp:Button ID="btnCancelEditShortProfile" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelEditShortProfile_Click"/></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblSaveShortProfileMessage" runat="server" Text="You have successfully updated your short profile!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <%--Table end--%>
                        <div runat="server" id="EditBlockedUsers">
                            <table style="width: 100%;">
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblSearchUsersToBlock" runat="server" Text="Search by name" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:TextBox ID="txtSearchBlockedUsers" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnSearchBlockUsers" runat="server" Text="Search" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSearchBlockUsers_Click" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="secondCell">
                                        <asp:Label ID="lblSearchUsersToBlockError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblSearchResults" runat="server" Text="Search results" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:ListBox ID="drpBlockUsersSearchResult" Font-Size="Medium" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnAddSelectedUsersToBlock" runat="server" Text="Add" BackColor="yellow" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnAddSelectedUsersToBlock_Click" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="secondCell">
                                        <asp:Label ID="lblAddToBlockListError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblBlockedUsersListToSave" runat="server" Text="List to be saved" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:ListBox ID="drpBlockUsersOldResult" Font-Size="Medium" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnRemoveSelectedUsersToBlock" runat="server" Text="Remove" BackColor="orange" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnRemoveSelectedUsersToBlock_Click" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="secondCell">
                                        <asp:Label ID="lblRemoveFromBlockListError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveBlockedUsers" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveBlockedUsers_Click" /></td>
                                    <td>
                                        <asp:Button ID="btnCancelBlockedUsers" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelBlockedUsers_Click" /></td>
                                </tr>
                            </table>
                            <asp:Label ID="lblSaveBlockedUsersMessage" runat="server" Text="You have successfully updated your list of blocked users!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                        </div>


                        <div runat="server" id="EditCurrentHealthConditions">
                            <table style="width: 100%;">
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblCurrentConditionToList" runat="server" Text="Type a condition" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:TextBox ID="txtCurrentConditionToList" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnAddConditionToList" runat="server" Text="Add" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnAddConditionToList_Click" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="secondCell">
                                        <asp:Label ID="lblAddConditionToListError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblConditionsToBeSaved" runat="server" Text="Conditions to be saved" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:ListBox ID="drpConditionsToBeSaved" Font-Size="Medium" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnConditionToBeRemoved" runat="server" Text="Remove" BackColor="orange" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnConditionToBeRemoved_Click" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblRemoveConditionToListError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveCurrentConditions" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveCurrentConditions_Click" /></td>
                                    <td>
                                        <asp:Button ID="btnCancelCurrentConditions" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelCurrentConditions_Click" /></td>
                                </tr>
                            </table>
                            <asp:Label ID="lblCurrentConditionsMessage" runat="server" Text="You have successfully updated your list of current health conditions!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                        </div>
                        <div runat="server" id="EditCurrentTreatments">
                            <table style="width: 100%;">
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblTypeTreatment" runat="server" Text="Type a treatment" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:TextBox ID="txtTypeTreatment" runat="server" Font-Size="Medium"></asp:TextBox></td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnAddTreatmentToList" runat="server" Text="Add" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnAddTreatmentToList_Click" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                    <td class="secondCell">
                                        <asp:Label ID="lblAddTreatmentToListError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                </tr>
                                <tr>
                                    <td class="firstCell">
                                        <asp:Label ID="lblTreatmentsToBeSaved" runat="server" Text="Treatments to be saved" Font-Size="Medium"></asp:Label></td>
                                    <td class="secondCell">
                                        <asp:ListBox ID="drpTreatmentsToBeSaved" Font-Size="Medium" runat="server" SelectionMode="Multiple"></asp:ListBox>
                                    </td>
                                    <td class="thirdCell">
                                        <asp:Button ID="btnTreatmentsToBeRemoved" runat="server" Text="Remove" BackColor="orange" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnTreatmentsToBeRemoved_Click" /></td>
                                </tr>
                                <tr>
                                    <td>
                                        <td class="secondCell">
                                            <asp:Label ID="lblRemoveTreatmentError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </td>
                                </tr>
                            </table>
                            <br />
                            <br />
                            <table style="width: 100%;">
                                <tr>
                                    <td>
                                        <asp:Button ID="btnSaveTreatment" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveTreatment_Click" /></td>
                                    <td>
                                        <asp:Button ID="btnCancelTreatment" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelTreatment_Click" /></td>
                                </tr>
                            </table>
                            <asp:Label ID="lblTreatmentsSavedSuccessfully" runat="server" Text="You have successfully updated your list of current treatments!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                        </div>
                        <%--Table Edit Complete Profile start--%>
                        <div runat="server" id="EditCompleteProfile">
                            <div>
                                        <h3>Edit Complete Profile Information</h3>
                                <table class="tableEdit" style="width: 100%;">
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblOnDialysis" runat="server" Text="On dialysis?" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpOnDialysis" runat="server" Font-Size="Medium" Width="100%">
                                                <asp:ListItem>Are you on dialysis?</asp:ListItem>
                                                <asp:ListItem>Yes</asp:ListItem>
                                                <asp:ListItem>No</asp:ListItem>
                                            </asp:DropDownList>

                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblOnDialysisError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblKidneyDisease" runat="server" Text="Kidney disease stage" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpKidneyDisease" runat="server" Font-Size="Medium" Width="100%">
                                                <asp:ListItem>Select your stage</asp:ListItem>
                                                <asp:ListItem>Stage 1</asp:ListItem>
                                                <asp:ListItem>Stage 2</asp:ListItem>
                                                <asp:ListItem>Stage 3</asp:ListItem>
                                                <asp:ListItem>Stage 4</asp:ListItem>
                                                <asp:ListItem>Stage 5</asp:ListItem>
                                                <asp:ListItem>I am not sure</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblKidneyDiseaseError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblIssueStartedYear" runat="server" Text="Issue started year" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpIssueStarted" runat="server" AutoPostBack="true" Width="100%" OnSelectedIndexChanged="drpIssueStarted_SelectedIndexChanged"></asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblIssueStarted" runat="server" Text="Issue started" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:Calendar ID="calIssueStarted" runat="server" Width="100%"></asp:Calendar>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblIssueStartedError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblBloodType" runat="server" Text="Blood type" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpBloodType" runat="server" AutoPostBack="true" Width="100%">
                                                <asp:ListItem>Select your blood type</asp:ListItem>
                                                <asp:ListItem>A+</asp:ListItem>
                                                <asp:ListItem>A-</asp:ListItem>
                                                <asp:ListItem>B+</asp:ListItem>
                                                <asp:ListItem>B-</asp:ListItem>
                                                <asp:ListItem>O+</asp:ListItem>
                                                <asp:ListItem>O-</asp:ListItem>
                                                <asp:ListItem>AB+</asp:ListItem>
                                                <asp:ListItem>AB-</asp:ListItem>
                                                <asp:ListItem>I am not sure</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblBloodTypeError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblCountry" runat="server" Text="Country" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpCountry" runat="server" AutoPostBack="true" Width="100%" OnSelectedIndexChanged="drpCountry_SelectedIndexChanged"></asp:DropDownList>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblCountryError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblCity" runat="server" Text="City" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:TextBox ID="txtCity" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblCityError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblState" runat="server" Text="State" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:DropDownList ID="drpState" runat="server" AutoPostBack="true" Visible="false" Width="100%">
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
                                            <asp:TextBox ID="txtState" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblStateError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblAddress" runat="server" Text="Address" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:TextBox ID="txtAddress" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblAddressError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblZip" runat="server" Text="Zip code" Font-Size="Medium"></asp:Label></td>
                                        <td class="secondCell">
                                            <asp:TextBox ID="txtZip" runat="server" Font-Size="Medium" Width="100%"></asp:TextBox>
                                        </td>
                                        <td class="thirdCell">
                                            <asp:Label ID="lblZipError" runat="server" Text=" " Visible="false" ForeColor="Red" Font-Size="Medium"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblMajorDiagnosesView" runat="server" Text="Major diagnoses" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblMajorDiagnosesViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnMajorDiagnosesView" runat="server" Text="Edit Major Diagnoses" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnMajorDiagnosesView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblPhoneNumbersView" runat="server" Text="Phone numbers" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblPhoneNumbersViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnPhoneNumbersView" runat="server" Text="Edit Phone Numbers" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnPhoneNumbersView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblEmailsView" runat="server" Text="Emails" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblEmailsViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnEmailsView" runat="server" Text="Edit Emails" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnEmailsView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblPastHealthConditionsView" runat="server" Text="Past health conditions" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblPastHealthConditionsViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnPastHealthConditionsView" runat="server" Text="Edit Past Health Conditions" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnPastHealthConditionsView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblAllergiesView" runat="server" Text="Allergies" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblAllergiesViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnAllergiesView" runat="server" Text="Edit Allergies" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnAllergiesView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblInsurancesView" runat="server" Text="Insurances" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblInsurancesViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnInsurancesView" runat="server" Text="Edit Insurances" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnInsurancesView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblPastPatientIDsView" runat="server" Text="Past patient IDs" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblPastPatientIDsViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnPastPatientIDsView" runat="server" Text="Edit Past Patient IDs" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnPastPatientIDsView_Click" /></td>
                                    </tr>
                                    <tr>
                                        <td class="firstCell">
                                            <asp:Label ID="lblTreatmentsHistoryView" runat="server" Text="Treatments history" Font-Size="Medium"></asp:Label></td>
                                        <%--<td class="secondCell">
                                            <asp:Label ID="lblTreatmentsHistoryViewList" runat="server" Text=" " Font-Size="Medium"></asp:Label></td>--%>
                                        <td class="secondCell">
                                            <asp:Button ID="btnTreatmentsHistoryView" runat="server" Text="Edit Treatments History" Font-Size="Medium" Width="100%" BackColor="orange" Font-Bold="True" OnClick="btnTreatmentsHistoryView_Click" /></td>
                                    </tr>
                                </table>
                                <br />
                                <br />
                                <table style="width: 100%;">
                                    <tr>
                                        <td>
                                            <asp:Button ID="btnSaveEditCompleteProfile" runat="server" Text="Save" BackColor="green" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnSaveEditCompleteProfile_Click" /></td>
                                        <td>
                                            <asp:Button ID="btnCancelEditCompleteProfile" runat="server" Text="Go Back" BackColor="red" Font-Bold="True" Font-Size="Medium" Width="50%" OnClick="btnCancelEditCompleteProfile_Click" /></td>
                                    </tr>
                                </table>
                                <asp:Label ID="lblSaveCompleteProfileMessage" runat="server" Text="You have successfully updated your complete profile!" Font-Size="Medium" ForeColor="green" Visible="false"></asp:Label>
                            </div>
                        </div>
                        <%--Table Edit Complete Profile end--%>
                        <div runat="server" id="EditMajorDiagnoses"></div>
                        <div runat="server" id="EditPhoneNumbers"></div>
                        <div runat="server" id="EditEmails"></div>
                        <div runat="server" id="EditPastHealthConditions"></div>
                        <div runat="server" id="EditAllergies"></div>
                        <div runat="server" id="EditInsurances"></div>
                        <div runat="server" id="EditPastPatientIDs"></div>
                        <div runat="server" id="EditTreatmentsHistory"></div>
                    </ContentTemplate>
                    <Triggers>
                        <%--Short Profile Controls:--%>
                        <asp:AsyncPostBackTrigger ControlID="btnShortProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveEditShortProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelEditShortProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnEditBlockedUsersView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCurrentHealthConditionsView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCurrentTreatmentsView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveBlockedUsers" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelBlockedUsers" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSearchBlockUsers" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddSelectedUsersToBlock" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnRemoveSelectedUsersToBlock" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddConditionToList" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnConditionToBeRemoved" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveCurrentConditions" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelCurrentConditions" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAddTreatmentToList" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnTreatmentsToBeRemoved" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnSaveTreatment" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelTreatment" EventName="Click" />
                        <%--Complete Profile Controls:--%>
                        <asp:AsyncPostBackTrigger ControlID="btnSaveEditCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancelEditCompleteProfile" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnMajorDiagnosesView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnPhoneNumbersView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnEmailsView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnPastHealthConditionsView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnAllergiesView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnInsurancesView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnPastPatientIDsView" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnTreatmentsHistoryView" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
