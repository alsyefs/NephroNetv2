<%@ Page Title="" Language="C#" MasterPageFile="~/Patient.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="NephroNet.Accounts.Patient.Profile" %>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%--Body start--%>
    <div class="container">
        <br />
        <h2><%: Title %></h2>
        <div class="panel panel-default">
            <div class="panel-body">
                <%--Content start--%>
                 <%--Table start--%>
                        <div runat="server" id="View">
                            <div >
                                <table border="1" style="width: 100%;">
                                    <asp:Label ID="lblRow" runat="server" Text=" "></asp:Label>
                                </table>
                                <br />
                                <br />
                            </div>
                        </div>
                        <%--Table end--%>
                <asp:Label ID="lblAdminCommands" runat="server" Text=" "></asp:Label>
                <script type="text/javascript">
                    function OpenPopup(site) { popup(site); }
                    // copied from http://www.dotnetfunda.com/codes/code419-code-to-open-popup-window-in-center-position-.aspx
                    function popup(url) {
                        var width = 500;
                        var height = 300;
                        var left = (screen.width - width) / 2;
                        var top = (screen.height - height) / 2;
                        var params = 'width=' + width + ', height=' + height;
                        params += ', top=' + top + ', left=' + left;
                        params += ', directories=no';
                        params += ', location=no';
                        params += ', menubar=no';
                        params += ', resizable=no';
                        params += ', scrollbars=no';
                        params += ', status=no';
                        params += ', toolbar=no';
                        newwin = window.open(url, 'windowname5', params);
                        if (window.focus) { newwin.focus() }
                        return false;
                    }
                </script>
                <%--Content end--%>
            </div>
        </div>
    </div>
    <%--Body end--%>
</asp:Content>
