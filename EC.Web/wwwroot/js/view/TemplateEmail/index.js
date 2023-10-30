(function ($) {
    function Cms() {
        var $this = this;
       /* var actionurl = "/admin/Email/CreateEdit/";*/
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-Email', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": false },
                    { "targets": [4], "sortable": true, "searchable": false },
                    { "targets": [5], "sortable": true, "searchable": false },
                    { "targets": [6], "sortable": true, "searchable": false },
                    {
                        "targets": [7], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            
                            var hideClass = JSON.parse(row[7]) ? 'hidden' : '';
                            var actionLink = $("<a/>", {
                                href: "/admin/Email/CreateEdit/" + row[0],
                                id: "editModel" + row[0],
                                title: "Edit",
                                class: "btn btn-primary btn-sm ",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#frmproductmanager",
                                html: $("<i/>", {
                                    class: "fas fa-edit",
                                    
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            actionLink += $("<a/>", {
                                href: "/admin/Email/Delete/" + row[0],
                                id: "deleteType",
                                title:"Delete",
                                class: "btn btn-danger btn-sm " + hideClass,
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-delete-email",
                                html: $("<i/>", {
                                    class: "fas fa-trash-alt"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;";
                            actionLink += $("<a/>", {
                                href: "/admin/Email/View/" + row[0],
                                id: "viewModel",
                                title: "View",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#frmproductmanager",
                                html: $("<i/>", {
                                    class: "fa fa-eye"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"

                            return actionLink;
                        }
                    }
                   
                ],
                "direction": "rtl",
                "bPaginate": true,
                "sPaginationType": "simple_numbers",
                "bProcessing": true,
                "bServerSide": true,
                "bAutoWidth": false,
                "stateSave": false,
                "sAjaxSource": "/admin/Email/index",
                "fnServerData": function (url, data, callback) {
                    $.ajax({
                        "url": url,
                        "data": data,
                        "success": callback,
                        "contentType": "application/x-www-form-urlencoded; charset=utf-8",
                        "dataType": "json",
                        "type": "POST",
                        "cache": false,
                        "error": function () {

                        }
                    });
                },
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull)
                {
                   
                },
                "fnDrawCallback": function (oSettings) {
                   /* initGridControlsWithEvents();*/
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').hide();
                    }
                    else {
                        $(oSettings.nTableWrapper).find('.dataTables_paginate').show();
                    }
                },

                "stateSaveCallback": function (settings, data) {
                    localStorage.setItem('DataTables_' + settings.sInstance, JSON.stringify(data));
                },
                "stateLoadCallback": function (settings) {
                    return JSON.parse(localStorage.getItem('DataTables_' + settings.sInstance));
                },
                "language": {
                    "searchPlaceholder": "Search by title"
                }
            });
            table = gridCms.DataTable();
            table.search("").draw();
        }

        //function initGridControlsWithEvents() {
        //    if ($('.switchBox').data('bootstrapSwitch')) {
        //        $('.switchBox').off('switchChange.bootstrapSwitch');
        //        $('.switchBox').bootstrapSwitch('destroy');
        //    }

        //    $('.switchBox').bootstrapSwitch()
        //        .on('switchChange.bootstrapSwitch', function () {
        //            var switchElement = this;

        //            $.get('/admin/Banner/ActiveBannerStatus', { id: this.value }, function (result) {
        //                if (!result.isSuccess) {
        //                    $(switchElement).bootstrapSwitch('toggleState', true);
        //                }
        //                else {
        //                    Global.ShowMessage("Status Updated Successfully.", Global.MessageType.Success);
        //                }
        //            });
        //        });
        //}

        $("#emailTemplateForm").submit(function () {
            debugger;
            var returnValue = true;
            var descriptionText = '';
            var Description = CKEDITOR.instances['Description'].getData();
            if (Description != undefined && Description != null && Description != '') {
                descriptionText = Description.replace(/(<([^>]+)>)/ig, '').replace(/&nbsp;/g, '').replace('\n', '').trim();
            }
            if (!$("#emailTemplateForm").valid()) {
                returnValue = false;
                $("#errorMessageDescription").text('*required')
            }
            if (Description == "" || Description == undefined || Description == null || Description == '' || descriptionText == '') {
                returnValue = false;
                $("#errorMessageDescription").text('*required')
            }

        function initilizeModel() {
            $("#modal-add-edit-banner").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-banner .modal-content').load(recipient1, function () {
                    formAddEditBrands = new Global.FormHelperWithFiles($("#modal-add-edit-banner").find("#frm-add-edit-banner form"), { updateTargetId: "validation-summary" }, function (data) {

                        //console.log(data.isSuccess);
                        if (data.isSuccess == true) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            // $("#validation-summary").show();
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    $('.form-checkbox').bootstrapSwitch();
                    $(".custom-file-input").on("change", function () {
                        var fileName = $(this).val().split("\\").pop();
                        $(this).siblings(".custom-file-label").addClass("selected").html(fileName);
                    });

                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            $("#modal-delete-email").on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initializeGrid();
            initilizeModel();
        }

    }
    $(function () {
        var self = new Cms();
        self.init();
    });
})(jQuery)