﻿<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/CP/CP.Master" CodeBehind="s_pharmacy.aspx.vb" Inherits="Jazirah.s_pharmacy" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
<link rel="stylesheet" type="text/css" href="../app-assets/vendors/css/datatables/dataTables.bootstrap4.css">

<style>
.autocomplete-suggestions {
  border: 1px solid #e4e4e4;
  background: #F4F4F4;
  cursor: default;
  overflow: auto; }

.autocomplete-suggestion {
  padding: 2px 5px;
  font-size: 1.2em;
  white-space: nowrap;
  overflow: hidden; }

.autocomplete-selected {
  background: #f0f0f0; }

.autocomplete-suggestions strong {
  font-weight: normal;
  color: #3399ff;
  font-weight: bolder; }

label{
    padding:20px ;
     width:25%;
}
.radio{
    width:340px
}
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<div class="row">
                    <fieldset class="form-group position-relative has-icon-left">
                            <input type="text" class="form-control form-control-sm input-sm" id="iconLeft2" placeholder="Icon Left, Large Input">
                            <div class="form-control-position">
                            <i class="icon-bulb success font-small-4"></i>
                            </div>
                        <small>Icon Left class <code>.has-icon-left</code>, Large Input class <code>.input-sm</code>&amp; <code>.font-medium-4</code> class for Large Icon </small>
                        </fieldset>
<div class="input-group input-group-sm mb-3">
    <input type="text" class="form-control" placeholder="Search">
    <div class="input-group-append">
        <button class="btn btn-success" type="button">Go</button>
    </div>
</div>
						<fieldset>
							<div class="input-group">
								<input type="text" class="touchspin" value="50" data-bts-min="1" data-bts-max="100"/>
							</div>
						</fieldset>

                        <fieldset class="form-group">
                            <input type="text" class="form-control" id="helpInput1" placeholder="Help Text">
                            <p class="text-left"><small class="text-muted">Helper aligned to left</small></p>
                        </fieldset>
</div>
<div class="row">
    <div class="col-xs-4" >
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">General Settings</h4>
            </div>
	        <div class="card-body collapse in">
                <div class="card-block">
                    <div class="row">
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblintYear%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control form-control-sm input-sm text-sm-center" id="intYear" value="<%=intYear %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblintStartupFY%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="intStartupFY" value="<%=intStartupFY %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblbyteLocalCurrency%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="byteLocalCurrency" value="<%=byteLocalCurrency %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblbyteCurrencyRound%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="byteCurrencyRound" value="<%=byteCurrencyRound%>" /></div>
                        </div>
                    </div>
                    <div class="form-actions right">
                        <button type="button" class="btn btn-primary" onclick="javascript:saveGeneralSettings();"><i class="icon-check2"></i>  <%=Save%></button>
					</div>
                </div>
            </div> 
        </div>
    </div>

    <div class="col-xs-4" >
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">Default Cash Information</h4>
            </div>
	        <div class="card-body collapse in">
                <div class="card-block">
                    <div class="row">
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblbyteDepartment_Cash%> </h5></div>
                            <div class="col-md-7"><input type="hidden" id="idDept" value="<%=byteDepartment_Cash%>" /><input type="text" class="form-control" id="txtDept" value="<%=strDepartment_Cash%>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lbllngContact_Cash%> </h5></div>
                            <div class="col-md-7"><input type="hidden" id="idContactComp" value="<%=lngContact_Cash %>" /><input type="text" class="form-control" id="txtContactComp" value="<%=strContact_Cash%>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lbllngSalesman_Cash%> </h5></div>
                            <div class="col-md-7"><input type="hidden" id="idContactDoct" value="<%=lngSalesman_Cash %>" /><input type="text" class="form-control" id="txtContactDoct" value="<%=strSalesman_Cash%>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lbllngPatient_Cash%> </h5></div>
                            <div class="col-md-7"><input type="hidden" id="idPatient" value="<%=lngPatient_Cash %>" /><input type="text" class="form-control" id="txtPatient" value="<%=strPatient_Cash %>" /></div>
                        </div>
                    </div>
                    <div class="form-actions right">
                        <button type="button" class="btn btn-primary" onclick="javascript:saveDefaultCash();"><i class="icon-check2"></i>  <%=Save%></button>
					</div>
                </div>
            </div> 
        </div>
    </div>

        <div class="col-xs-4" >
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">Limitations</h4>
            </div>
	        <div class="card-body collapse in">
                <div class="card-block">
                    <div class="row">
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblbyteInvoicesLimitDay%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="byteInvoicesLimitDay" value="<%=byteInvoicesLimitDay %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblOrdersLimitDays%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="OrdersLimitDays" value="<%=OrdersLimitDays%>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblCancelLimitDays%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="CancelLimitDays" value="<%=CancelLimitDays %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblDaysToCalculateMedicalInvoices%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="DaysToCalculateMedicalInvoices" value="<%=DaysToCalculateMedicalInvoices %>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblDaysToCalculateMedicineInvoices%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="DaysToCalculateMedicineInvoices" value="<%=DaysToCalculateMedicineInvoices %>" /></div>
                        </div>
                    </div>
                    <div class="form-actions right">
                        <button type="button" class="btn btn-primary" onclick="javascript:saveLimitations();"><i class="icon-check2"></i>  <%=Save%></button>
					</div>
                </div>
            </div> 
        </div>
    </div>
</div>  

<div class="row">
    <div class="col-xs-6" >
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">Restrictions</h4>
            </div>
	        <div class="card-body collapse in">
                <div class="card-block">
                    <div class="row">
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblTaxEnabled%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If TaxEnabled = "True" Then Response.Write("active")%>"><input type="radio" id="TaxEnabled1" name="TaxEnabled" <%If TaxEnabled = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If TaxEnabled = "False" Then Response.Write("active")%>"><input type="radio" id="TaxEnabled0" name="TaxEnabled" <%If TaxEnabled = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblChangeQuantity_Cash%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If ChangeQuantity_Cash = "True" Then Response.Write("active")%>"><input type="radio" id="ChangeQuantity_Cash1" name="ChangeQuantity_Cash" <%If ChangeQuantity_Cash = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If ChangeQuantity_Cash = "False" Then Response.Write("active")%>"><input type="radio" id="ChangeQuantity_Cash0" name="ChangeQuantity_Cash" <%If ChangeQuantity_Cash = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblChangeQuantity_Insurance%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If ChangeQuantity_Insurance = "True" Then Response.Write("active")%>"><input type="radio" id="ChangeQuantity_Insurance1" name="ChangeQuantity_Insurance" <%If ChangeQuantity_Insurance = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If ChangeQuantity_Insurance = "False" Then Response.Write("active")%>"><input type="radio" id="ChangeQuantity_Insurance0" name="ChangeQuantity_Insurance" <%If ChangeQuantity_Insurance = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAddDiscount_Cash%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If AddDiscount_Cash = "True" Then Response.Write("active")%>"><input type="radio" id="AddDiscount_Cash1" name="AddDiscount_Cash" <%If AddDiscount_Cash = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If AddDiscount_Cash = "False" Then Response.Write("active")%>"><input type="radio" id="AddDiscount_Cash0" name="AddDiscount_Cash" <%If AddDiscount_Cash = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAddDiscount_Insurance%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If AddDiscount_Insurance = "True" Then Response.Write("active")%>"><input type="radio" id="AddDiscount_Insurance1" name="AddDiscount_Insurance" <%If AddDiscount_Insurance = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If AddDiscount_Insurance = "False" Then Response.Write("active")%>"><input type="radio" id="AddDiscount_Insurance0" name="AddDiscount_Insurance" <%If AddDiscount_Insurance = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblOneQuantityPerItem%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If OneQuantityPerItem = "True" Then Response.Write("active")%>"><input type="radio" id="OneQuantityPerItem1" name="OneQuantityPerItem" <%If OneQuantityPerItem = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If OneQuantityPerItem = "False" Then Response.Write("active")%>"><input type="radio" id="OneQuantityPerItem0" name="OneQuantityPerItem" <%If OneQuantityPerItem = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAllowExtraItem_Insurance%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If AllowExtraItem_Insurance = "True" Then Response.Write("active")%>"><input type="radio" id="AllowExtraItem_Insurance1" name="AllowExtraItem_Insurance" <%If AllowExtraItem_Insurance = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If AllowExtraItem_Insurance = "False" Then Response.Write("active")%>"><input type="radio" id="AllowExtraItem_Insurance0" name="AllowExtraItem_Insurance" <%If AllowExtraItem_Insurance = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAuto_MoveRejectedToCash_Insurance%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If Auto_MoveRejectedToCash_Insurance = "True" Then Response.Write("active")%>"><input type="radio" id="Auto_MoveRejectedToCash_Insurance1" name="Auto_MoveRejectedToCash_Insurance" <%If Auto_MoveRejectedToCash_Insurance = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If Auto_MoveRejectedToCash_Insurance = "False" Then Response.Write("active")%>"><input type="radio" id="Auto_MoveRejectedToCash_Insurance0" name="Auto_MoveRejectedToCash_Insurance" <%If Auto_MoveRejectedToCash_Insurance = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblSalesmanType%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If SalesmanType = "0" Then Response.Write("active")%>"><input type="radio" id="SalesmanType0" name="SalesmanType" <%If SalesmanType = "0" Then Response.Write("checked")%>> <%=Sales%> </label>
                                    <label class="btn btn-secondary btn-sm <%If SalesmanType = "1" Then Response.Write("active")%>"><input type="radio" id="SalesmanType1" name="SalesmanType" <%If SalesmanType = "1" Then Response.Write("checked")%>> <%=Cashier%> </label>
                                    <label class="btn btn-secondary btn-sm <%If SalesmanType = "2" Then Response.Write("active")%>"><input type="radio" id="SalesmanType2" name="SalesmanType" <%If SalesmanType = "2" Then Response.Write("checked")%>> <%=Both%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAskBeforeSend%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If AskBeforeSend = "True" Then Response.Write("active")%>"><input type="radio" id="AskBeforeSend1" name="AskBeforeSend" <%If AskBeforeSend = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If AskBeforeSend = "False" Then Response.Write("active")%>"><input type="radio" id="AskBeforeSend0" name="AskBeforeSend" <%If AskBeforeSend = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblAskBeforeReturn%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If AskBeforeReturn = "True" Then Response.Write("active")%>"><input type="radio" id="AskBeforeReturn1" name="AskBeforeReturn" <%If AskBeforeReturn = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If AskBeforeReturn = "False" Then Response.Write("active")%>"><input type="radio" id="AskBeforeReturn0" name="AskBeforeReturn" <%If AskBeforeReturn = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblOnePaymentForCashier%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If OnePaymentForCashier = "True" Then Response.Write("active")%>"><input type="radio" id="OnePaymentForCashier1" name="OnePaymentForCashier" <%If OnePaymentForCashier = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If OnePaymentForCashier = "False" Then Response.Write("active")%>"><input type="radio" id="OnePaymentForCashier0" name="OnePaymentForCashier" <%If OnePaymentForCashier = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblForcePaymentOnCloseInvoice%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If ForcePaymentOnCloseInvoice = "True" Then Response.Write("active")%>"><input type="radio" id="ForcePaymentOnCloseInvoice1" name="ForcePaymentOnCloseInvoice" <%If ForcePaymentOnCloseInvoice = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If ForcePaymentOnCloseInvoice = "False" Then Response.Write("active")%>"><input type="radio" id="ForcePaymentOnCloseInvoice0" name="ForcePaymentOnCloseInvoice" <%If ForcePaymentOnCloseInvoice = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblDirectCancelInvoic%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If DirectCancelInvoic = "True" Then Response.Write("active")%>"><input type="radio" id="DirectCancelInvoic1" name="DirectCancelInvoic" <%If DirectCancelInvoic = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If DirectCancelInvoic = "False" Then Response.Write("active")%>"><input type="radio" id="DirectCancelInvoic0" name="DirectCancelInvoic" <%If DirectCancelInvoic = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblSusbendMax%> </h5></div>
                            <div class="col-md-7"><input type="number" class="form-control" id="SusbendMax" value="<%=SusbendMax %>" /></div>
                        </div>
                    </div>
                    <div class="form-actions right">
                        <button type="button" class="btn btn-primary" onclick="javascript:saveRestrictions();"><i class="icon-check2"></i>  <%=Save%></button>
					</div>
                </div>
            </div> 
        </div>
    </div>


</div>
<div class="row">
    <div class="col-xs-6" >
        <div class="card">
            <div class="card-header">
                <h4 class="card-title">Printing</h4>
            </div>
	        <div class="card-body collapse in">
                <div class="card-block">
                    <div class="row">
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblPopupToPrint%> </h5></div>
                            <div class="col-md-7">
                                <div class="btn-group btn-group-toggle radio" data-toggle="buttons">
                                    <label class="btn btn-secondary btn-sm <%If PopupToPrint = "True" Then Response.Write("active")%>"><input type="radio" id="PopupToPrint1" name="PopupToPrint" <%If PopupToPrint = "True" Then Response.Write("checked")%>> <%=Yes%> </label>
                                    <label class="btn btn-secondary btn-sm <%If PopupToPrint = "False" Then Response.Write("active")%>"><input type="radio" id="PopupToPrint0" name="PopupToPrint" <%If PopupToPrint = "False" Then Response.Write("checked")%>> <%=No%> </label>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblPrintDose%> </h5></div>
                            <div class="col-md-7">
                                <select id="PrintDose" class="form-control">
                                    <option value="0" <%If PrintDose = 0 Then Response.Write("selected")%>><%=NoPrint%></option>
                                    <option value="1" <%If PrintDose = 1 Then Response.Write("selected")%>><%=AutoPrint%></option>
                                    <option value="2" <%If PrintDose = 2 Then Response.Write("selected")%>><%=ManualPrint%></option>
                                    <option value="3" <%If PrintDose = 3 Then Response.Write("selected")%>><%=Both%></option>
                                </select>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblPrintInvoice%> </h5></div>
                            <div class="col-md-7">
                                <select id="PrintInvoice" class="form-control">
                                    <option value="0" <%If PrintInvoice = 0 Then Response.Write("selected")%>><%=NoPrint%></option>
                                    <option value="1" <%If PrintInvoice = 1 Then Response.Write("selected")%>><%=AutoPrint%></option>
                                    <option value="2" <%If PrintInvoice = 2 Then Response.Write("selected")%>><%=AskToPrint%></option>
                                    <option value="3" <%If PrintInvoice = 3 Then Response.Write("selected")%>><%=UserDefined%></option>
                                </select>
                            </div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblDosePrinter%> </h5></div>
                            <div class="col-md-7"><input type="text" class="form-control" id="DosePrinter" value="<%=DosePrinter%>" /></div>
                        </div>
                        <div class="col-md-12 form-group">
                            <div class="col-md-5"><h5> <%=lblInvoicePrinter%> </h5></div>
                            <div class="col-md-7"><input type="text" class="form-control" id="InvoicePrinter" value="<%=InvoicePrinter%>" /></div>
                        </div>
                    </div>
                    <div class="form-actions right">
                        <button type="button" class="btn btn-primary" onclick="javascript:savePrinting();"><i class="icon-check2"></i>  <%=Save%></button>
					</div>
                </div>
            </div> 
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
     <script src="../app-assets/vendors/js/datatables/jquery.dataTables.min.js" type="text/javascript"></script>
  <script src="../app-assets/vendors/js/datatables/dataTables.bootstrap4.min.js" type="text/javascript"></script>
  <script type="text/javascript">

  </script>
    <script src="../app-assets/vendors/js/autocomplete/jquery.autocomplete.min.js"></script>
    <script>

        $(document).ready(function () {
            $('#txtDept').autocomplete(
                {
                    triggerSelectOnValidInput: true, onInvalidateSelection: function () {
                        $('#txtDept').val('');
                    }, lookup: function (query, done) {
                        if ($('#txtDept').val().length > 0) {
                            $.ajax({
                                type: 'POST',
                                url: 's_pharmacy.aspx/findDepartment',
                                data: '{query: "' + query + '"}',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    done(jQuery.parseJSON(response.d));
                                },
                                failure: function (msg) {
                                    alert(msg);
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);
                                }
                            });
                        } else {
                            done(jQuery.parseJSON(''));
                        }
                    }, onSelect: function (suggestion) {
                        $('#txtDept').val(suggestion.value);
                        $('#idDept').val(suggestion.id);


                    }
                });
        });




        $(document).ready(function () {
            $('#txtContactDoct').autocomplete({
                    triggerSelectOnValidInput: true, onInvalidateSelection: function () {
                        $('#txtContactDoct').val('');
                    }, lookup: function (query, done) {
                        if ($('#txtContactDoct').val().length > 0) {
                            $.ajax({
                                type: 'POST',
                                url: 's_pharmacy.aspx/findContactDoct',
                                data: '{query: "' + query + '"}',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    done(jQuery.parseJSON(response.d));
                                },
                                failure: function (msg) {
                                    alert(msg);
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);
                                }
                            });
                        } else {
                            done(jQuery.parseJSON(''));
                        }
                    }, onSelect: function (suggestion) {
                        $('#txtContactDoct').val(suggestion.value);
                        $('#idContactDoct').val(suggestion.id);
                    }
            });
        });




        $(document).ready(function () {
            $('#txtContactComp').autocomplete(
                {
                    triggerSelectOnValidInput: true, onInvalidateSelection: function () {
                        $('#txtContactComp').val('');
                    }, lookup: function (query, done) {
                        if ($('#txtContactComp').val().length > 0) {
                            $.ajax({
                                type: 'POST',
                                url: 's_pharmacy.aspx/findContactComp',
                                data: '{query: "' + query + '"}',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    done(jQuery.parseJSON(response.d));
                                },
                                failure: function (msg) {
                                    alert(msg);
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);
                                }
                            });
                        } else {
                            done(jQuery.parseJSON(''));
                        }
                    }, onSelect: function (suggestion) {
                        $('#txtContactComp').val(suggestion.value);
                        $('#idContactComp').val(suggestion.id);


                    }
                });
        });
        $(document).ready(function () {
            $('#txtPatient').autocomplete(
                {
                    triggerSelectOnValidInput: true, onInvalidateSelection: function () {
                        $('#txtPatient').val('');
                    }, lookup: function (query, done) {
                        if ($('#txtPatient').val().length > 0) {
                            $.ajax({
                                type: 'POST',
                                url: 's_pharmacy.aspx/findPatient',
                                data: '{query: "' + query + '"}',
                                contentType: 'application/json; charset=utf-8',
                                dataType: 'json',
                                success: function (response) {
                                    done(jQuery.parseJSON(response.d));
                                },
                                failure: function (msg) {
                                    alert(msg);
                                },
                                error: function (xhr, ajaxOptions, thrownError) {
                                    alert('Load Form, update form error! ' + xhr.status + ' error =' + thrownError + ' xhr.responseText = ' + xhr.responseText);
                                }
                            });
                        } else {
                            done(jQuery.parseJSON(''));
                        }
                    }, onSelect: function (suggestion) {
                        $('#txtPatient').val(suggestion.value);
                        $('#idPatient').val(suggestion.id);


                    }
                });
        });

        function saveGeneralSettings() {
            $.ajax({
                type: 'POST',
                url: 's_pharmacy.aspx/saveGeneralSettings',
                data: '{intYear: "' + $('#intYear').val() + '",intStartupFY: "' + $('#intStartupFY').val() + '",byteLocalCurrency: "' + $('#byteLocalCurrency').val() + '",byteCurrencyRound: "' + $('#byteCurrencyRound').val() + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        msg('', 'Data has saved successfully!', 'success');
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

        function saveDefaultCash() {
            $.ajax({
                type: 'POST',
                url: 's_pharmacy.aspx/saveDefaultCash',
                data: '{byteDepartment_Cash: "' + $('#idDept').val() + '",lngContact_Cash: "' + $('#idContactComp').val() + '",lngSalesman_Cash: "' + $('#idContactDoct').val() + '",lngPatient_Cash: "' + $('#idPatient').val() + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        msg('', 'Data has saved successfully!', 'success');
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
        
        function saveRestrictions() {
            salesmanType = 0;
            if ($('#SalesmanType1').prop('checked')) salesmanType = 1;
            if ($('#SalesmanType2').prop('checked')) salesmanType = 2;
            $.ajax({
                type: 'POST',
                url: 's_pharmacy.aspx/saveRestrictions',
                data: '{TaxEnabled: "' + $('#TaxEnabled1').prop('checked') + '",ChangeQuantity_Cash: "' + $('#ChangeQuantity_Cash1').prop('checked') + '",ChangeQuantity_Insurance: "' + $('#ChangeQuantity_Insurance1').prop('checked') + '",AddDiscount_Cash: "' + $('#AddDiscount_Cash1').prop('checked') + '",AddDiscount_Insurance: "' + $('#AddDiscount_Insurance1').prop('checked') + '",OneQuantityPerItem: "' + $('#OneQuantityPerItem1').prop('checked') + '",AllowExtraItem_Insurance: "' + $('#AllowExtraItem_Insurance1').prop('checked') + '",Auto_MoveRejectedToCash_Insurance: "' + $('#Auto_MoveRejectedToCash_Insurance1').prop('checked') + '",SalesmanType: "' + salesmanType + '",AskBeforeSend: "' + $('#AskBeforeSend1').prop('checked') + '",AskBeforeReturn: "' + $('#AskBeforeReturn1').prop('checked') + '",OnePaymentForCashier: "' + $('#OnePaymentForCashier1').prop('checked') + '",ForcePaymentOnCloseInvoice: "' + $('#ForcePaymentOnCloseInvoice1').prop('checked') + '",DirectCancelInvoic: "' + $('#DirectCancelInvoic1').prop('checked') + '",SusbendMax: "' + $('#SusbendMax').val() + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        msg('', 'Data has saved successfully!', 'success');
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
        
        function saveLimitations() {
            $.ajax({
                type: 'POST',
                url: 's_pharmacy.aspx/saveLimitations',
                data: '{byteInvoicesLimitDay: "' + $('#byteInvoicesLimitDay').val() + '",OrdersLimitDays: "' + $('#OrdersLimitDays').val() + '",CancelLimitDays: "' + $('#CancelLimitDays').val() + '",DaysToCalculateMedicalInvoices: "' + $('#DaysToCalculateMedicalInvoices').val() + '",DaysToCalculateMedicineInvoices: "' + $('#DaysToCalculateMedicineInvoices').val() + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        msg('', 'Data has saved successfully!', 'success');
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

        function savePrinting() {
            $.ajax({
                type: 'POST',
                url: 's_pharmacy.aspx/savePrinting',
                data: '{PopupToPrint: "' + $('#PopupToPrint1').prop('checked') + '",PrintDose: "' + $('#PrintDose').val() + '",PrintInvoice: "' + $('#PrintInvoice').val() + '",DosePrinter: "' + $('#DosePrinter').val() + '",InvoicePrinter: "' + $('#InvoicePrinter').val() + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        msg('', 'Data has saved successfully!', 'success');
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
