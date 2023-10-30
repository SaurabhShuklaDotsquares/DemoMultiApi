(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-users', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": true },
                    { "targets": [4], "sortable": true, "searchable": true },
                    { "targets": [5], "sortable": true, "searchable": true },
                    { "targets": [6], "sortable": false, "searchable": false },
                    //{
                    //    "targets": [5], "sortable": false, "searchable": false,
                    //    "render": function (data, type, row, meta) {

                    //        return '<img src="/Uploads/' + row[5] + '" width="50px" height="50px">';
                    //    }
                    //},
                    { "targets": [7], "sortable": true, "searchable": true },
                    {
                        "targets": [8], "sortable": true, "searchable": false, "data": "8",
                        "render": function (data, type, row, meta) {
                            var json = {
                                type: "checkbox",
                                class: "switchBox switch-small",
                                value: row[0],
                                'data-on': "success",
                                'data-off': "danger"
                            };
                            if (data === "True") {
                                json.checked = true;
                            }
                            return $('<input/>', json).get(0).outerHTML;
                        }
                    },
                    {
                        "targets": [9], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/usermanager/CreateEdit/" + row[0],
                                id: "editTypeModal",
                                class: "btn btn-primary btn-sm",
                                title: "Edit",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-users",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[9] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/usermanager/delete/" + row[0],
                                    id: "deleteType",
                                    class: "btn btn-danger btn-sm",
                                    title: "Delete",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-users",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/usermanager/resetpassword/" + row[0],
                                    id: "resetPasswordModel",
                                    class: "btn btn-danger btn-sm",
                                    title: "Reset password",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-resetpassword-users",
                                    html: $("<i/>", {
                                        class: "fa fa-key"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/usermanager/view/" + row[0],
                                    id: "ViewModel",
                                    title: "View",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-View-users",
                                    html: $("<i/>", {
                                        class: "fa fa-eye"
                                    }),
                                }).append("").get(0).outerHTML;
                                return actionLink;
                            }
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
                "sAjaxSource": "/admin/usermanager/index",
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
                    initGridControlsWithEvents();
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
                    "searchPlaceholder": "Search by email"
                }
            });
            table = gridCms.DataTable();
            table.search("").draw();
        }
        function initGridControlsWithEvents() {
            if ($('.switchBox').data('bootstrapSwitch')) {
                $('.switchBox').off('switchChange.bootstrapSwitch');
                $('.switchBox').bootstrapSwitch('destroy');
            }
            $('.switchBox').bootstrapSwitch()
                .on('switchChange.bootstrapSwitch', function () {
                    var switchElement = this;

                    $.get('/admin/usermanager/activeusersstatus', { id: this.value }, function (result) {
                        if (!result.isSuccess) {
                            $(switchElement).bootstrapSwitch('toggleState', true);
                        }
                        else {
                            Global.ShowMessage("Status Updated Successfully.", Global.MessageType.Success);
                        }
                    });
                });
        }
        function initilizeModel() {
            $("#modal-add-edit-users").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-edit-users .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelper($("#modal-add-edit-users").find("#frm-add-edit-users form"), { updateTargetId: "validation-summary" }, function (data) {
                        if (data.isSuccess) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    $('.form-checkbox').bootstrapSwitch();
                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });


            $("#modal-delete-users").on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });

            $("#modal-resetpassword-users").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-resetpassword-users .modal-content').load(recipient1, function () {
                    formResetPasswordUserManager = new Global.FormHelper($("#modal-resetpassword-users").find("#frm-resetpassword-users form"), { updateTargetId: "validation-summary" }, function (data) {
                        if (data.isSuccess) {
                            $("#validation-summary").html("");
                            $("#validation-summary").hide();
                            window.location.href = data.redirectUrl;
                        }
                        else {
                            $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
                        }
                    });
                    $('.form-checkbox').bootstrapSwitch();
                });
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
           
            //$("#modal-resetpassword-users").off().on('shown.bs.modal', function (event) {
            //    var button = $(event.relatedTarget);
            //    var recipient2 = $(button).attr("href");
            //    $('#modal-resetpassword-users .modal-content').load(recipient2, function () {
            //        formAddEditCategory1 = new Global.FormHelper($("#modal-resetpassword-users").find("#frm-resetpassword-users form"), { updateTargetId: "validation-summary" }, function (data) {
            //            if (data.isSuccess == true) {
            //                $("#validation-summary").html("");
            //                $("#validation-summary").hide();
            //                window.location.href = data.redirectUrl;
            //            }
            //            else {
            //                $("#validation-summary").text(data.data).show().delay(5000).fadeOut(2000);
            //            }
            //        });
            //    });
            //}).on('hidden.bs.modal', function (e) {
            //    $(this).removeData('bs.modal');
            //});
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