<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Main/MN.Master" CodeBehind="profile.aspx.vb" Inherits="Jazirah.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
	<div class="row match-height">
		<div class="col-xl-12 col-lg-12">
			<div class="card">
				<div class="card-body collapse in">
					<div class="card-block">
                        <div class="row">
                            <div class="col-md-3 col-sm-12">
                                <a class="media-left" href="#">
									<img class="media-object" src="../../app-assets/images/portrait/small/avatar-s-3.png" alt="Generic placeholder image" style="width: 256px;height: 256px;" />
								</a>
                            </div>
                            <div class="col-md-5 col-sm-12">
                                <form id="frmProfile">
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblUserName%>:</label><div class="col-md-8"><input type="text" id="txtUserName" class="form-control" readonly="readonly" value="<%=UserName%>" /></div></div>
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblFullName%>:</label><div class="col-md-8"><input type="text" id="txtFullName" class="form-control" value="<%=FullName%>" /></div></div>
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblPosition%>:</label><div class="col-md-8"><input type="text" id="txtPosition" class="form-control" value="<%=Position%>" /></div></div>
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblMobile%>:</label><div class="col-md-8"><input type="text" id="txtMobile" class="form-control" value="<%=Mobile%>" /></div></div>
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblEmail%>:</label><div class="col-md-8"><input type="text" id="txtEmail" class="form-control" value="<%=Email%>" /></div></div>
                                <div class="col-md-12 pb-1"><label class="col-md-4 text-md-right"><%=lblExtension%>:</label><div class="col-md-8"><input type="text" id="txtExtension" class="form-control" value="<%=Extension%>" /></div></div>
                                </form>
                            </div>
                            <div class="col-md-4 col-sm-12">

                            </div>
                        </div>
                        <div class="row">
                            <hr />
                            <div class="offset-sm-3">
                                <button type="button" class="btn btn-success" onclick="javascript:updateProfile()"><%=btnUpdateProfile%></button>
                                <button type="button" class="btn btn-success" onclick="javascript:showGlobalModal('../Main/ajax.aspx/viewChangePassword','{}','#mdlConfirm');"><%=btnChangePassword%></button>
                                <button type="button" class="btn btn-success" disabled="disabled"><%=btnUploadPicture%></button>
                            </div>
						</div>
					</div>
                </div>
			</div>
		</div>
	</div>
<div id="prtJS"></div>
<div class="modal fade" id="mdlAlpha" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlBeta" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlGamma" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="static"></div>
<div class="modal fade" id="mdlInfo" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlMessage" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
<div class="modal fade" id="mdlConfirm" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true" data-backdrop="false"></div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
    <script type="text/javascript">
        function showGlobalModal(path, param, modal) {
            $.ajax({
                type: "POST",
                url: path,
                data: param,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        $(modal).html(response.d);
                        $(modal).modal('show');
                    }
                },
                failure: function (msg) {
                    alert(msg);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
                }
            });
        }

        function changePassword(userName, oldPassword, newPassword) {
            $.ajax({
                type: 'POST',
                url: '../Main/ajax.aspx/ChangePassword',
                data: '{UserName: "' + userName + '",OldPassword: "' + oldPassword + '", NewPassword:"' + newPassword + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        $('#prtJS').html(response.d);
                    }
                },
                failure: function (msg) {
                    alert(msg);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
                }
            });
        }

        function updateProfile() {
            var userName = $('#txtUserName').val();
            var fullName = $('#txtFullName').val();
            var position = $('#txtPosition').val();
            var mobile = $('#txtMobile').val();
            var email = $('#txtEmail').val();
            var extension = $('#txtExtension').val();
            $.ajax({
                type: 'POST',
                url: '../Main/ajax.aspx/UpdateProfile',
                data: '{strUserName: "' + userName + '",strFullName: "' + fullName + '", strPosition:"' + position + '", strMobile: "' + mobile + '", strEmail: "' + email + '", strExtension: "' + extension + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        $('#prtJS').html(response.d);
                    }
                },
                failure: function (msg) {
                    alert(msg);
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Load Form, update form error! " + xhr.status + " error =" + thrownError + " xhr.responseText = " + xhr.responseText);
                }
            });
        }
    </script>
</asp:Content>
