(function ($) {
    function Report() {
        var $this = this;     

        function initializeForm() {

            $(document).off("click", ".exportToExcel").on("click", ".exportToExcel", function () {
                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');
                
                $('.dynamicLink').remove();              
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSV?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelActiveCustomer").on("click", ".exportToExcelActiveCustomer", function () {
                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVActiveCustomer?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelInactiveCustomer").on("click", ".exportToExcelInactiveCustomer", function () {

                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVInactiveCustomer?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelNewOrderOrder").on("click", ".exportToExcelNewOrderOrder", function () {

                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVNewOrderOrder?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelCancelledOrder").on("click", ".exportToExcelCancelledOrder", function () {

                /*var date = $('#reservation').val();*/
                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVTotalCancelledOrder?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelCategory").on("click", ".exportToExcelCategory", function () {

                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVCategory?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelBrand").on("click", ".exportToExcelBrand", function () {

                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVBrand?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
            $(document).off("click", ".exportToExcelReturnedOrder").on("click", ".exportToExcelReturnedOrder", function () {

                var Getdate = $('#reservation1').val();
                var date = Getdate.replace(' - ', '-');

                $('.dynamicLink').remove();
                $('body').append('<a id="link" class="dynamicLink" href=' + domain + 'Reports/ExportCSVReturnedOrder?start_date=' + date + '>& nbsp;</a >');
                $('#link')[0].click();
            });
        }
        $this.init = function () {
            initializeForm();
        }
    }
    $(function () {
        var self = new Report();
        self.init();
    });
})(jQuery)