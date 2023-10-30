(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            /*ckEditor();*/
            var gridCms = new Global.GridHelper('#grid-vendor', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": false, "searchable": false },
                    { "targets": [3], "sortable": false, "searchable": false },
                    { "targets": [4], "sortable": false, "searchable": false },
                    { "targets": [5], "sortable": true, "searchable": false },
                    { "targets": [6], "sortable": false, "searchable": false },
                    {
                        "targets": [7], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Vendor/Vendersave/" + row[0],
                                id: "addModel",
                                title: "Edit",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#modal-add-Coupons",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[0] != 8) {
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Vendor/Vedorproductdata/" + row[0],
                                    id: "deleteType",
                                    title: "View Business Product",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "form",
                                    'data-target': "#modal-delete-Coupons1",
                                    html: $("<i/>", {
                                        class: "fa fa-fw fa-shopping-cart"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;"
                                actionLink += $("<a/>", {
                                    href: domain + "/admin/Vendor/ViewVendor/" + row[0],
                                    id: "deleteType",
                                    title: "View",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "form",
                                    'data-target': "#viewModel",
                                    html: $("<i/>", {
                                        class: "fa fa-eye"
                                    }),
                                }).append("").get(0).outerHTML;
                            }
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
                "sAjaxSource": "/admin/Vendor/Index",
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
                    /*ckEditor();*/
                    //initGridControlsWithEvents();
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
                    "searchPlaceholder": "Search by Business Name"
                }
            });
            table = gridCms.DataTable();
            table.search("").draw();
        }

        function initilizeModel() {
            $("#modal-add-Coupons").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-Coupons .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelperWithFiles($("#modal-add-Coupons1").find("#frm-add-edit-Tax form"), { updateTargetId: "validation-summary" }, function (data) {

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
            $("#modal-delete-Coupons").on('hidden.bs.modal', function (e) {
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