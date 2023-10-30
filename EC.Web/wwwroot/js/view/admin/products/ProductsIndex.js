(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-products', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": false, "searchable": false },
                    {
                        "targets": [4], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {

                            return row[4];
                        }
                    },
                    {
                        "targets": [5], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                
                            }).append("").get(0).outerHTML + "&nbsp;"
                            var url = "/admin/Products/ApprovedUnApproved?id=" + row[0] + "&status=" +0;
                            var showtext = "Approved";
                            if (row[5] == 1) {
                                /*url = "/admin/Products/Approve/" + row[0];*/
                                showtext = "Approved";
                            }
                            else if (row[5] == 0) {
                                url = "/admin/Products/ApprovedUnApproved?id=" + row[0] + "&status=" + 1;
                                showtext = "UnApproved"
                            }
                            actionLink += $("<a/>", {
                                href: url,
                                id: "editModel",
                                class: "btn btn-primary btn-sm approve",
                                text: showtext,
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-approveunapprove-Products",
                            }).append("").get(0).outerHTML + "&nbsp;"
                            return actionLink;
                        }

                    },

                    {
                        "targets": [6], "sortable": false, "searchable": false, "data": "6",
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
                    { "targets": [7], "sortable": true, "searchable": true },
                    { "targets": [8], "sortable": true, "searchable": true },
                    {
                        "targets": [9], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Products/Create/" + row[0],
                                id: "editModel",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#frmproductmanager",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[10] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/Products/Delete/" + row[0],
                                    id: "deleteType",
                                    class: "btn btn-danger btn-sm",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-Products",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/Products/view/" + row[0],
                                    id: "viewTypeModal",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-view-Products",
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
                "sAjaxSource": "/admin/Products/Index",
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
                    "searchPlaceholder": "Search by product name"
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

                    $.get('/admin/Products/ActiveProductsStatus', { id: this.value }, function (result) {
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