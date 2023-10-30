(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#example', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true, "width": "2%" },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": false },
                    { "targets": [4], "sortable": false, "searchable": false },

                    { "targets": [5], "sortable": true, "searchable": false },
                    { "targets": [6], "sortable": true, "searchable": false },
                    { "targets": [7], "sortable": true, "searchable": false },
                    { "targets": [8], "sortable": true, "searchable": true },
                    {
                        "targets": [9], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/Campaigns/IndexUsers/",
                                id: "editTypeModal",
                                class: "btn btn-primary btn-sm",
                                title: "Edit",
                                oncontextmenu: 'return false',
                                'data-toggle': "form",
                                'data-target': "#frmCampaign",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[7] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/Campaigns/delete/" + row[0],
                                    id: "deleteType",
                                    class: "btn btn-danger btn-sm",
                                    title: "Delete",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-Campaign",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/Campaigns/View/" + row[0],
                                    id: "viewTypeModal",
                                    class: "btn btn-primary btn-sm",
                                    title: "View",
                                    'data-toggle': "form",
                                    'data-target': "#modal-add-view-attribute",
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
                "sAjaxSource": "/admin/Campaigns/IndexUsers",
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
                    "searchPlaceholder": "Search by Tittle"
                }
            });
            table = gridCms.DataTable();
            table.search("").draw();
        }
        //function initGridControlsWithEvents() {
        //    debugger;
        //    //Check all the check box on tick of header checkbox
        //    $("#select_all").onchange(function () {
        //        if ($(this).is(":checked")) {
        //            $('.check-row').attr("checked", true);
        //        } else {
        //            $('.check-row').attr("checked", false);
        //        }
        //    });
        //}

        function initilizeModel() {
            $("#modal-add-Shipping").off().on('shown.bs.modal', function (event) {
                var button = $(event.relatedTarget);
                var recipient1 = $(button).attr("href");
                $('#modal-add-Shipping .modal-content').load(recipient1, function () {
                    formAddEditCategory = new Global.FormHelperWithFiles($("#modal-add-Shipping").find("#frm-add-edit-Tax form"), { updateTargetId: "validation-summary" }, function (data) {

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
            $("#modal-delete-Shipping").on('hidden.bs.modal', function (e) {
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