<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Main/MN.Master" CodeBehind="profile.aspx.vb" Inherits="Jazirah.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
	<div class="row match-height">
		<div class="col-xl-12 col-lg-12">
			<div class="card">
				<div class="card-header">
					<h4 class="card-title">Your Profile</h4>
					<a class="heading-elements-toggle"><i class="icon-ellipsis font-medium-3"></i></a>
        			<div class="heading-elements">
						<ul class="list-inline mb-0">
							<li><a data-action="collapse"><i class="icon-minus4"></i></a></li>
							<li><a data-action="reload"><i class="icon-reload"></i></a></li>
							<li><a data-action="expand"><i class="icon-expand2"></i></a></li>
							<li><a data-action="close"><i class="icon-cross2"></i></a></li>
						</ul>
					</div>
				</div>
				<div class="card-body collapse in">
					<div class="card-block">
						<div class="media-list">
							<div class="media">
								<a class="media-left" href="#">
									<img class="media-object" src="../../app-assets/images/portrait/small/avatar-s-3.png" alt="Generic placeholder image" style="width: 256px;height: 256px;" />
								</a>
								<div class="media-body h4">
									<div class="row">
                                        <div class="col-sm-6">
                                            <p class="col-sm-5 text-xs-right">You Name:</p><p class="col-sm-7">Soft</p>
                                            <p class="col-sm-5 text-xs-right">You Name:</p><p class="col-sm-7">Soft</p>
                                            <p class="col-sm-5 text-xs-right">You Name:</p><p class="col-sm-7">Soft</p>
                                        </div>
                                        <div class="col-sm-6">
                                        </div>
									</div>
								</div>
							</div>
                            <hr />
                            <div class="offset-sm-3">
                                <button type="button" class="btn btn-success">Update Profile</button>
                                <button type="button" class="btn btn-success">Change Password</button>
                                <button type="button" class="btn btn-success">Upload Picture</button>
                            </div>
						</div>
					</div>
					</div>
			</div>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
</asp:Content>
