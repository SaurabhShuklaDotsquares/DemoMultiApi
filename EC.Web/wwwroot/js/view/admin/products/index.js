(function ($) {
    function Cms() {
        var $this = this;
        function initializeGrid() {
            var gridCms = new Global.GridHelper('#grid-products', {
                "columnDefs": [
                    { "targets": [0], "visible": false },
                    { "targets": [1], "visible": true, "sortable": false, "searchable": true },
                    { "targets": [2], "sortable": true, "searchable": true },
                    { "targets": [3], "sortable": true, "searchable": true },
                    { "targets": [4], "sortable": true, "searchable": true },
                    {
                        "targets": [5], "sortable": false, "searchable": false,
                        "render": function (data, type, row, meta) {

                            return '<img src="/Uploads/' + row[5] + '" width="50px" height="50px">';
                        }
                    },
                    { "targets": [6], "sortable": true, "searchable": true },
                    {
                        "targets": [7], "sortable": true, "searchable": false, "data": "7",
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
                        "targets": [8], "data": "0", "searchable": false, "sortable": false,
                        "render": function (data, type, row, meta) {
                            var actionLink = $("<a/>", {
                                href: "/admin/category/createedit/" + row[0],
                                id: "editTypeModal",
                                class: "btn btn-primary btn-sm",
                                oncontextmenu: 'return false',
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-category",
                                html: $("<i/>", {
                                    class: "fas fa-edit"
                                }),
                            }).append("").get(0).outerHTML + "&nbsp;"
                            if (row[9] != "True") {
                                actionLink += $("<a/>", {
                                    href: "/admin/category/delete/" + row[0],
                                    id: "deleteType",
                                    class: "btn btn-danger btn-sm",
                                    oncontextmenu: 'return false',
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-category",
                                    html: $("<i/>", {
                                        class: "fas fa-trash-alt"
                                    }),
                                }).append("").get(0).outerHTML + "&nbsp;";
                                actionLink += $("<a/>", {
                                    href: "/admin/category/view/" + row[0],
                                    id: "viewTypeModal",
                                    class: "btn btn-primary btn-sm",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-view-category",
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
                "sAjaxSource": "/admin/category/index",
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
                    "searchPlaceholder": "search by title"
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

                    $.get('/admin/category/activecategorystatus', { id: this.value }, function (result) {
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