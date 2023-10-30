(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-ContactUs', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": false },
                    { "targets": [2], "sortable": false, "searchable": false },
                    { "targets": [3], "sortable": false, "searchable": false },
                    { "targets": [4], "sortable": true, "searchable": false },
                    
                    {
                        "targets": [5], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/ContactUs/View/" + row[0],
                                id: "editModel",
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
                "sAjaxSource": "/admin/ContactUs/index",
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
                "fnDrawCallback": function (oSettings) {
                    /*initGridControlsWithEvents();*/
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
                    "searchPlaceholder": "Search by Email"
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

        //            $.get('/admin/Payments/ActivePaymentStatus', { id: this.value }, function (result) {
        //                if (!result.isSuccess) {
        //                    $(switchElement).bootstrapSwitch('toggleState', true);
        //                }
        //                else {
        //                    Global.ShowMessage("Status Updated Successfully.", Global.MessageType.Success);
        //                }
        //            });
        //        });
        //}
        function initilizeModel() {
            $("#modal-add-edit-category").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-category .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelperWithFiles($("#modal-add-edit-category").find("#frm-add-edit-category form"), { updateTargetId: "validation-summary" }, function (data) {

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
            $("#modal-delete-category").on('hidden.bs.modal', function (e) {
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