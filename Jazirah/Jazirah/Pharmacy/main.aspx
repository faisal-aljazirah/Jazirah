<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Pharmacy/PH.Master" CodeBehind="main.aspx.vb" Inherits="Jazirah.main" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder" runat="server">
    <link rel="stylesheet" type="text/css" href="../app-assets/css/plugins/forms/wizard.min.css">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder" runat="server">
<!--project Total Earning, visit & post-->
<div class="row">
    <div class="col-xl-4 col-lg-12">
        <div class="card">
            <div class="card-body" id="chartPlace1">
                <div class="earning-chart position-relative">
                    <div class="chart-title position-absolute mt-2 ml-2">
                        <h1 class="display-4">$9865</h1>
                        <span class="text-muted">Total Earning</span>
                    </div>
                    <canvas id="earning-chart" class="height-450 block"></canvas>
                    <div class="chart-stats position-absolute position-bottom-0 position-right-0 mb-2 mr-3">
                        <a href="#" class="btn bg-cyan mr-1 white">Statistics <i class="icon-stats-bars"></i></a> <span class="text-muted">for the <a href="#" class="primary darken-2">last year.</a></span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div id="earning-chart1-script"></div>
    <div class="col-xl-8 col-lg-12">
        <div class="card">
            <div class="card-body">
                <div class="card-block">
                    <canvas id="posts-visits" class="height-400"></canvas>
                </div>
            </div>
        </div>
    </div>
    <div id="earning-chart2-script"></div>
</div>
<section id="number-tabs">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Form wizard with number tabs</h4>
                    <a class="heading-elements-toggle"><i class="ft-ellipsis-h font-medium-3"></i></a>
                                <div class="heading-elements">
                        <ul class="list-inline mb-0">
                            <li><a data-action="collapse"><i class="ft-minus"></i></a></li>
                            <li><a data-action="reload"><i class="ft-rotate-cw"></i></a></li>
                            <li><a data-action="expand"><i class="ft-maximize"></i></a></li>
                            <li><a data-action="close"><i class="ft-x"></i></a></li>
                        </ul>
                    </div>
                </div>
                <div class="card-content collapse show">
                    <div class="card-body">
                        <form action="#" class="number-tab-steps wizard-circle">

                            <!-- Step 1 -->
                            <h6>Step 1</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="firstName1">First Name :</label>
                                            <input type="text" class="form-control" id="firstName1" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="lastName1">Last Name :</label>
                                            <input type="text" class="form-control" id="lastName1" >
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="emailAddress1">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress1" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="location1">Select City :</label>
                                            <select class="custom-select form-control" id="location1" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="phoneNumber1">Phone Number :</label>
                                            <input type="tel" class="form-control" id="phoneNumber1" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="date1">Date of Birth :</label>
                                            <input type="date" class="form-control" id="date1" >
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 2 -->
                            <h6>Step 2</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="proposalTitle1">Proposal Title :</label>
                                            <input type="text" class="form-control" id="proposalTitle1" >
                                        </div>
                                        <div class="form-group">
                                            <label for="emailAddress2">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress2" >
                                        </div>
                                        <div class="form-group">
                                            <label for="videoUrl1">Video URL :</label>
                                            <input type="url" class="form-control" id="videoUrl1" >
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="jobTitle1">Job Title :</label>
                                            <input type="text" class="form-control" id="jobTitle1" >
                                        </div>
                                        <div class="form-group">
                                            <label for="shortDescription1">Short Description :</label>
                                            <textarea name="shortDescription" id="shortDescription1" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 3 -->
                            <h6>Step 3</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="eventName1">Event Name :</label>
                                            <input type="text" class="form-control" id="eventName1" >
                                        </div>
                                        <div class="form-group">
                                            <label for="eventType1">Event Type :</label>
                                            <select class="custom-select form-control" id="eventType1" data-placeholder="Type to search cities" name="eventType1">
                                                <option value="Banquet">Banquet</option>
                                                <option value="Fund Raiser">Fund Raiser</option>
                                                <option value="Dinner Party">Dinner Party</option>
                                                <option value="Wedding">Wedding</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventLocation1">Event Location :</label>
                                            <select class="custom-select form-control" id="eventLocation1" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="jobTitle2">Event Date - Time :</label>
                                            <div class='input-group'>
                                                <input type='text' class="form-control datetime" id="jobTitle2" />
                                                <span class="input-group-addon">
                                                    <span class="ft-calendar"></span>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventStatus1">Event Status :</label>
                                            <select class="custom-select form-control" id="eventStatus1" name="eventStatus">
                                                <option value="Planning">Planning</option>
                                                <option value="In Progress">In Progress</option>
                                                <option value="Finished">Finished</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label>Requirements :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status1" class="custom-control-input" id="staffing1">
                                                    <label class="custom-control-label" for="staffing1">Staffing</label>
                                                </div>
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status1" class="custom-control-input" id="catering1">
                                                    <label class="custom-control-label" for="catering1">Catering</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 4 -->
                            <h6>Step 4</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="meetingName1">Name of Meeting :</label>
                                            <input type="text" class="form-control" id="meetingName1" >
                                        </div>

                                        <div class="form-group">
                                            <label for="meetingLocation1">Location :</label>
                                            <input type="text" class="form-control" id="meetingLocation1" >
                                        </div>

                                        <div class="form-group">
                                            <label for="participants1">Names of Participants</label>
                                            <textarea name="participants" id="participants1" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="decisions1">Decisions Reached</label>
                                            <textarea name="decisions" id="decisions1" rows="4" class="form-control"></textarea>
                                        </div>
                                        <div class="form-group">
                                            <label>Agenda Items :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda1" class="custom-control-input" id="item11">
                                                    <label class="custom-control-label" for="item11">1st item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda1" class="custom-control-input" id="item12">
                                                    <label class="custom-control-label" for="item12">2nd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda1" class="custom-control-input" id="item13">
                                                    <label class="custom-control-label" for="item13">3rd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda1" class="custom-control-input" id="item14">
                                                    <label class="custom-control-label" for="item14">4th item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda1" class="custom-control-input" id="item15">
                                                    <label class="custom-control-label" for="item15">5th item</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
<!-- Form wizard with number tabs section end -->

<!-- Form wizard with icon tabs section start -->
<section id="icon-tabs">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Form wizard with icon tabs</h4>
                    <a class="heading-elements-toggle"><i class="ft-ellipsis-h font-medium-3"></i></a>
                    <div class="heading-elements">
                        <ul class="list-inline mb-0">
                            <li><a data-action="collapse"><i class="ft-minus"></i></a></li>
                            <li><a data-action="reload"><i class="ft-rotate-cw"></i></a></li>
                            <li><a data-action="expand"><i class="ft-maximize"></i></a></li>
                            <li><a data-action="close"><i class="ft-x"></i></a></li>
                        </ul>
                    </div>
                </div>
                <div class="card-content collapse show">
                    <div class="card-body">
                        <form action="#" class="icons-tab-steps wizard-circle">

                            <!-- Step 1 -->
                            <h6><i class="step-icon fa fa-home"></i> Step 1</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="firstName2">First Name :</label>
                                            <input type="text" class="form-control" id="firstName2" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="lastName2">Last Name :</label>
                                            <input type="text" class="form-control" id="lastName2" >
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="emailAddress3">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress3" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="location2">Select City :</label>
                                            <select class="custom-select form-control" id="location2" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="phoneNumber2">Phone Number :</label>
                                            <input type="tel" class="form-control" id="phoneNumber2" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="date2">Date of Birth :</label>
                                            <input type="date" class="form-control" id="date2" >
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 2 -->
                            <h6><i class="step-icon fa fa-pencil"></i>Step 2</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="proposalTitle2">Proposal Title :</label>
                                            <input type="text" class="form-control" id="proposalTitle2" >
                                        </div>
                                        <div class="form-group">
                                            <label for="emailAddress4">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress4" >
                                        </div>
                                        <div class="form-group">
                                            <label for="videoUrl2">Video URL :</label>
                                            <input type="url" class="form-control" id="videoUrl2" >
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="jobTitle3">Job Title :</label>
                                            <input type="text" class="form-control" id="jobTitle3" >
                                        </div>
                                        <div class="form-group">
                                            <label for="shortDescription2">Short Description :</label>
                                            <textarea name="shortDescription" id="shortDescription2" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 3 -->
                            <h6><i class="step-icon fa fa-tv"></i>Step 3</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="eventName2">Event Name :</label>
                                            <input type="text" class="form-control" id="eventName2" >
                                        </div>
                                        <div class="form-group">
                                            <label for="eventType2">Event Type :</label>
                                            <select class="custom-select form-control" id="eventType2" data-placeholder="Type to search cities" name="eventType2">
                                                <option value="Banquet">Banquet</option>
                                                <option value="Fund Raiser">Fund Raiser</option>
                                                <option value="Dinner Party">Dinner Party</option>
                                                <option value="Wedding">Wedding</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventLocation2">Event Location :</label>
                                            <select class="custom-select form-control" id="eventLocation2" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label>Event Date - Time :</label>
                                            <div class='input-group'>
                                                <input type='text' class="form-control datetime" />
                                                <span class="input-group-addon">
                                                    <span class="ft-calendar"></span>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventStatus2">Event Status :</label>
                                            <select class="custom-select form-control" id="eventStatus2" name="eventStatus">
                                                <option value="Planning">Planning</option>
                                                <option value="In Progress">In Progress</option>
                                                <option value="Finished">Finished</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label>Requirements :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status2" class="custom-control-input" id="staffing2">
                                                    <label class="custom-control-label" for="staffing2">Staffing</label>
                                                </div>
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status2" class="custom-control-input" id="catering2">
                                                    <label class="custom-control-label" for="catering2">Catering</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 4 -->
                            <h6><i class="step-icon fa fa-image"></i>Step 4</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="meetingName2">Name of Meeting :</label>
                                            <input type="text" class="form-control" id="meetingName2" >
                                        </div>

                                        <div class="form-group">
                                            <label for="meetingLocation2">Location :</label>
                                            <input type="text" class="form-control" id="meetingLocation2" >
                                        </div>

                                        <div class="form-group">
                                            <label for="participants2">Names of Participants</label>
                                            <textarea name="participants" id="participants2" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="decisions2">Decisions Reached</label>
                                            <textarea name="decisions" id="decisions2" rows="4" class="form-control"></textarea>
                                        </div>
                                        <div class="form-group">
                                            <label>Agenda Items :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda2" class="custom-control-input" id="item21">
                                                    <label class="custom-control-label" for="item21">1st item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda2" class="custom-control-input" id="item22">
                                                    <label class="custom-control-label" for="item22">2nd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda2" class="custom-control-input" id="item23">
                                                    <label class="custom-control-label" for="item23">3rd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda2" class="custom-control-input" id="item24">
                                                    <label class="custom-control-label" for="item24">4th item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda2" class="custom-control-input" id="item25">
                                                    <label class="custom-control-label" for="item25">5th item</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
<!-- Form wizard with icon tabs section end -->

<!-- Form wizard with step validation section start -->
<section id="validation">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Validation Example</h4>
                    <a class="heading-elements-toggle"><i class="ft-ellipsis-h font-medium-3"></i></a>
                    <div class="heading-elements">
                        <ul class="list-inline mb-0">
                            <li><a data-action="collapse"><i class="ft-minus"></i></a></li>
                            <li><a data-action="reload"><i class="ft-rotate-cw"></i></a></li>
                            <li><a data-action="expand"><i class="ft-maximize"></i></a></li>
                            <li><a data-action="close"><i class="ft-x"></i></a></li>
                        </ul>
                    </div>
                </div>
                <div class="card-content collapse show">
                    <div class="card-body">
                        <form action="#" class="steps-validation wizard-circle">

                            <!-- Step 1 -->
                            <h6>Step 1</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="firstName3">
                                                First Name :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="firstName3" name="firstName" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="lastName3">
                                                Last Name :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="lastName3" name="lastName" >
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="emailAddress5">
                                                Email Address :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="email" class="form-control required" id="emailAddress5" name="emailAddress">
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="location">
                                                Select City :
                                                <span class="danger">*</span>
                                            </label>
                                            <select class="custom-select form-control required" id="location" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="phoneNumber3">Phone Number :</label>
                                            <input type="tel" class="form-control" id="phoneNumber3" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="date3">Date of Birth :</label>
                                            <input type="date" class="form-control" id="date3" >
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 2 -->
                            <h6>Step 2</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="proposalTitle3">
                                                Proposal Title :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="proposalTitle3" name="proposalTitle">
                                        </div>
                                        <div class="form-group">
                                            <label for="emailAddress6">
                                                Email Address :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="email" class="form-control required" id="emailAddress6" name="emailAddress">
                                        </div>
                                        <div class="form-group">
                                            <label for="videoUrl3">Video URL :</label>
                                            <input type="url" class="form-control" id="videoUrl3" name="videoUrl" >
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="jobTitle5">
                                                Job Title :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="jobTitle5" name="jobTitle" >
                                        </div>
                                        <div class="form-group">
                                            <label for="shortDescription3">Short Description :</label>
                                            <textarea name="shortDescription" id="shortDescription3" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 3 -->
                            <h6>Step 3</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="eventName3">
                                                Event Name :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="eventName3" name="eventName" >
                                        </div>
                                        <div class="form-group">
                                            <label for="eventType3">
                                                Event Type :
                                                <span class="danger">*</span>
                                            </label>
                                            <select class="custom-select form-control required" id="eventType3" name="eventType">
                                                <option value="Banquet">Banquet</option>
                                                <option value="Fund Raiser">Fund Raiser</option>
                                                <option value="Dinner Party">Dinner Party</option>
                                                <option value="Wedding">Wedding</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventLocation3">Event Location :</label>
                                            <select class="custom-select form-control" id="eventLocation3" name="eventLocation">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="eventDate">
                                                Event Date - Time :
                                                <span class="danger">*</span>
                                            </label>
                                            <div class='input-group'>
                                                <input type='text' class="form-control datetime required" id="eventDate" name="eventDate" />
                                                <span class="input-group-addon">
                                                    <span class="ft-calendar"></span>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventStatus3">
                                                Event Status :
                                                <span class="danger">*</span>
                                            </label>
                                            <select class="custom-select form-control required" id="eventStatus3" name="eventStatus">
                                                <option value="Planning">Planning</option>
                                                <option value="In Progress">In Progress</option>
                                                <option value="Finished">Finished</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label>Requirements :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status3" class="custom-control-input" id="staffing3">
                                                    <label class="custom-control-label" for="staffing3">Staffing</label>
                                                </div>
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status3" class="custom-control-input" id="catering3">
                                                    <label class="custom-control-label" for="catering3">Catering</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 4 -->
                            <h6>Step 4</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="meetingName3">
                                                Name of Meeting :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="meetingName3" name="meetingName" >
                                        </div>

                                        <div class="form-group">
                                            <label for="meetingLocation3">
                                                Location :
                                                <span class="danger">*</span>
                                            </label>
                                            <input type="text" class="form-control required" id="meetingLocation3" name="meetingLocation" >
                                        </div>

                                        <div class="form-group">
                                            <label for="participants3">Names of Participants</label>
                                            <textarea name="participants" id="participants3" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="decisions3">Decisions Reached</label>
                                            <textarea name="decisions" id="decisions3" rows="4" class="form-control"></textarea>
                                        </div>
                                        <div class="form-group">
                                            <label>Agenda Items :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda3" class="custom-control-input" id="item31">
                                                    <label class="custom-control-label" for="item31">1st item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda3" class="custom-control-input" id="item32">
                                                    <label class="custom-control-label" for="item32">2nd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda3" class="custom-control-input" id="item33">
                                                    <label class="custom-control-label" for="item33">3rd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda3" class="custom-control-input" id="item34">
                                                    <label class="custom-control-label" for="item34">4th item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda3" class="custom-control-input" id="item35">
                                                    <label class="custom-control-label" for="item35">5th item</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
<!-- Form wizard with step validation section end -->

<!-- Form wizard with vertical tabs section start -->
<section id="vertical-tabs">
    <div class="row">
        <div class="col-12">
            <div class="card">
                <div class="card-header">
                    <h4 class="card-title">Form wizard with vertical tabs</h4>
                    <a class="heading-elements-toggle"><i class="ft-ellipsis-h font-medium-3"></i></a>
                    <div class="heading-elements">
                        <ul class="list-inline mb-0">
                            <li><a data-action="collapse"><i class="ft-minus"></i></a></li>
                            <li><a data-action="reload"><i class="ft-rotate-cw"></i></a></li>
                            <li><a data-action="expand"><i class="ft-maximize"></i></a></li>
                            <li><a data-action="close"><i class="ft-x"></i></a></li>
                        </ul>
                    </div>
                </div>
                <div class="card-content collapse show">
                    <div class="card-body">
                        <form action="#" class="vertical-tab-steps wizard-circle">

                            <!-- Step 1 -->
                            <h6>Step 1</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="firstName4">First Name :</label>
                                            <input type="text" class="form-control" id="firstName4" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="lastName4">Last Name :</label>
                                            <input type="text" class="form-control" id="lastName4" >
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="emailAddress7">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress7" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="location3">Select City :</label>
                                            <select class="custom-select form-control" id="location3" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="phoneNumber4">Phone Number :</label>
                                            <input type="tel" class="form-control" id="phoneNumber4" >
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="date4">Date of Birth :</label>
                                            <input type="date" class="form-control" id="date4" >
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 2 -->
                            <h6>Step 2</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="proposalTitle4">Proposal Title :</label>
                                            <input type="text" class="form-control" id="proposalTitle4" >
                                        </div>
                                        <div class="form-group">
                                            <label for="emailAddress8">Email Address :</label>
                                            <input type="email" class="form-control" id="emailAddress8" >
                                        </div>
                                        <div class="form-group">
                                            <label for="videoUrl4">Video URL :</label>
                                            <input type="url" class="form-control" id="videoUrl4" >
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="jobTitle6">Job Title :</label>
                                            <input type="text" class="form-control" id="jobTitle6" >
                                        </div>
                                        <div class="form-group">
                                            <label for="shortDescription4">Short Description :</label>
                                            <textarea name="shortDescription" id="shortDescription4" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 3 -->
                            <h6>Step 3</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="eventName4">Event Name :</label>
                                            <input type="text" class="form-control" id="eventName4" >
                                        </div>
                                        <div class="form-group">
                                            <label for="eventType4">Event Type :</label>
                                            <select class="custom-select form-control" id="eventType4" data-placeholder="Type to search cities" name="eventType4">
                                                <option value="Banquet">Banquet</option>
                                                <option value="Fund Raiser">Fund Raiser</option>
                                                <option value="Dinner Party">Dinner Party</option>
                                                <option value="Wedding">Wedding</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventLocation4">Event Location :</label>
                                            <select class="custom-select form-control" id="eventLocation4" name="location">
                                                <option value="">Select City</option>
                                                <option value="Amsterdam">Amsterdam</option>
                                                <option value="Berlin">Berlin</option>
                                                <option value="Frankfurt">Frankfurt</option>
                                            </select>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label>Event Date - Time :</label>
                                            <div class='input-group'>
                                                <input type='text' class="form-control datetime" />
                                                <span class="input-group-addon">
                                                    <span class="ft-calendar"></span>
                                                </span>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <label for="eventStatus4">Event Status :</label>
                                            <select class="custom-select form-control" id="eventStatus4" name="eventStatus">
                                                <option value="Planning">Planning</option>
                                                <option value="In Progress">In Progress</option>
                                                <option value="Finished">Finished</option>
                                            </select>
                                        </div>
                                        <div class="form-group">
                                            <label>Requirements :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status4" class="custom-control-input" id="staffing4">
                                                    <label class="custom-control-label" for="staffing4">Staffing</label>
                                                </div>
                                                <div class="d-inline-block custom-control custom-checkbox">
                                                    <input type="checkbox" name="status4" class="custom-control-input" id="catering4">
                                                    <label class="custom-control-label" for="catering4">Catering</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>

                            <!-- Step 4 -->
                            <h6>Step 4</h6>
                            <fieldset>
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="meetingName4">Name of Meeting :</label>
                                            <input type="text" class="form-control" id="meetingName4" >
                                        </div>

                                        <div class="form-group">
                                            <label for="meetingLocation4">Location :</label>
                                            <input type="text" class="form-control" id="meetingLocation4" >
                                        </div>

                                        <div class="form-group">
                                            <label for="participants4">Names of Participants</label>
                                            <textarea name="participants" id="participants4" rows="4" class="form-control"></textarea>
                                        </div>
                                    </div>

                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <label for="decisions4">Decisions Reached</label>
                                            <textarea name="decisions" id="decisions4" rows="4" class="form-control"></textarea>
                                        </div>
                                        <div class="form-group">
                                            <label>Agenda Items :</label>
                                            <div class="c-inputs-stacked">
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda4" class="custom-control-input" id="item41">
                                                    <label class="custom-control-label" for="item41">1st item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda4" class="custom-control-input" id="item42">
                                                    <label class="custom-control-label" for="item42">2nd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda4" class="custom-control-input" id="item43">
                                                    <label class="custom-control-label" for="item43">3rd item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda4" class="custom-control-input" id="item44">
                                                    <label class="custom-control-label" for="item44">4th item</label>
                                                </div>
                                                <div class="custom-control custom-checkbox">
                                                    <input type="checkbox" name="agenda4" class="custom-control-input" id="item45">
                                                    <label class="custom-control-label" for="item45">5th item</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </fieldset>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</section>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterPlaceHolder" runat="server">
    <script src="../app-assets/vendors/js/charts/chart.min.js" type="text/javascript"></script>
    <script src="https://pixinvent.com/bootstrap-admin-template/robust/app-assets/vendors/js/vendors.min.js" type="text/javascript"></script>
    <script src="../app-assets/vendors/js/extensions/jquery.steps.min.js" type="text/javascript"></script>
    <script src="../app-assets/vendors/js/forms/validation/jquery.validate.min.js"></script>
    <script type="text/javascript">
        function getChart(dstElement, scriptElement, func, type) {
            $.ajax({
                type: 'POST',
                url: 'ajax.aspx/' + func,
                data: '{ElementID: "' + dstElement + '", Type: "' + type + '"}',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d.substr(0, 4) == 'Err:') {
                        msg('', response.d.substr(4, response.d.length), 'error');
                    } else {
                        $(scriptElement).html(response.d);
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

        $(document).ready(function () {
            getChart('earning-chart', '#earning-chart1-script', 'drowChart', 'line');
            getChart('posts-visits', '#earning-chart2-script', 'drowChart', 'line');
        });
        
        
        function drawChart(eId, eType, eOptions, eData) {
            var eContext = document.getElementById(eId).getContext("2d");

            var eConfig = {
                type: eType,

                // Chart Options
                options: eOptions,

                // Chart Data
                data: eData
            };

            // Create the chart
            var eChart = new Chart(eContext, eConfig);
        }

        $(".number-tab-steps").steps({ headerTag: "h6", bodyTag: "fieldset", transitionEffect: "fade", titleTemplate: '<span class="step">#index#</span> #title#', labels: { finish: "Submit" }, onFinished: function (e, t) { alert("Form submitted.") } }), $(".icons-tab-steps").steps({ headerTag: "h6", bodyTag: "fieldset", transitionEffect: "fade", titleTemplate: '<span class="step">#index#</span> #title#', labels: { finish: "Submit" }, onFinished: function (e, t) { alert("Form submitted.") } }), $(".vertical-tab-steps").steps({ headerTag: "h6", bodyTag: "fieldset", transitionEffect: "fade", stepsOrientation: "vertical", titleTemplate: '<span class="step">#index#</span> #title#', labels: { finish: "Submit" }, onFinished: function (e, t) { alert("Form submitted.") } }); var form = $(".steps-validation").show(); $(".steps-validation").steps({ headerTag: "h6", bodyTag: "fieldset", transitionEffect: "fade", titleTemplate: '<span class="step">#index#</span> #title#', labels: { finish: "Submit" }, onStepChanging: function (e, t, i) { return i < t || !(3 === i && Number($("#age-2").val()) < 18) && (t < i && (form.find(".body:eq(" + i + ") label.error").remove(), form.find(".body:eq(" + i + ") .error").removeClass("error")), form.validate().settings.ignore = ":disabled,:hidden", form.valid()) }, onFinishing: function (e, t) { return form.validate().settings.ignore = ":disabled", form.valid() }, onFinished: function (e, t) { alert("Submitted!") } }), $(".steps-validation").validate({ ignore: "input[type=hidden]", errorClass: "danger", successClass: "success", highlight: function (e, t) { $(e).removeClass(t) }, unhighlight: function (e, t) { $(e).removeClass(t) }, errorPlacement: function (e, t) { e.insertAfter(t) }, rules: { email: { email: !0 } } }), $(".datetime").daterangepicker({ timePicker: !0, timePickerIncrement: 30, locale: { format: "MM/DD/YYYY h:mm A" } });
    </script>
</asp:Content>
