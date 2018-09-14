<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="NephroNet.Accounts.Admin.Profile" %>

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
                    function terminateAccount(userId) {
                        console.log('started terminateAccount');
                        if (confirm('Are you sure you want to terminate the selected account?'))
                            terminate(userId);
                    }
                    function terminate(userId) {
                        console.log('terminating...');
                        var topicID = parseInt(userId);
                        var obj = {
                            in_profileId: userId,
                            terminateOrUnlock: 1 //1 = terminate
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("Profile.aspx/terminateOrUnlockAccount") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (msg) {
                                location.reload(true);
                                console.log('Successfully terminated the account!');
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                        console.log('terminating...DONE!');
                    }
                    function unlockAccount(userId) {
                        if (confirm('Are you sure you want to unlock the selected account?'))
                            unlock(userId);
                    }
                    function unlock(userId) {
                        console.log('unlocking...');
                        var topicID = parseInt(userId);
                        var obj = {
                            in_profileId: userId,
                            terminateOrUnlock: 2 //2 = unlocking
                        };
                        var param = JSON.stringify(obj);  // stringify the parameter
                        $.ajax({
                            method: "POST",
                            url: '<%= ResolveUrl("Profile.aspx/terminateOrUnlockAccount") %>',
                            data: param,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            async: true,
                            cache: false,
                            success: function (msg) {
                                location.reload(true);
                                console.log('Successfully unlocked the account!');
                            },
                            error: function (xhr, status, error) {
                                console.log(xhr.responseText);
                            }
                        });
                        console.log('unlocking...DONE!');
                    }
                </script>
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
